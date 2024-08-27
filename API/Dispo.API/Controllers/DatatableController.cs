using Dispo.API.ResponseBuilder;
using Dispo.Shared.Core.Domain.Interfaces;
using Dispo.Shared.Filter.Model;
using Dispo.Shared.Filter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.API.Controllers
{
    [Route("api/v1/datatable")]
    [ApiController]
    [Authorize]
    public class DatatableController : ControllerBase
    {
        private readonly IDatatableRepository _datatableRepository;
        private readonly IFilterService _filterService;

        public DatatableController(IDatatableRepository datatableRepository, IFilterService filterService)
        {
            _datatableRepository = datatableRepository;
            _filterService = filterService;
        }

        [HttpGet("get-count")]
        public IActionResult GetCount([FromQuery] string entity)
        {
            var type = Type.GetType($"Dispo.Shared.Core.Domain.Entities.{entity}, Dispo.Shared.Core.Domain");
            if (type is null)
            {
                return BadRequest("Entidade inválida.");
            }

            var method = _datatableRepository.GetType().GetMethod("GetTotalRecords");
            if (method is null)
            {
                return BadRequest($"Método 'GetTotalRecords' não implementado para a entidade '{entity}'");
            }

            var genericMethod = method.MakeGenericMethod(type);
            var result = genericMethod.Invoke(_datatableRepository, new object[] {});

            return Ok(new ResponseModelBuilder().WithData(result)
                        .WithSuccess(true)
                        .WithAlert(AlertType.Success)
                        .Build());
        }

        [HttpGet("get-all")]
        public IActionResult Get([FromQuery] PaginationModel paginationModel)
        {
            dynamic datatableData = null;

            if (paginationModel.Entity == "Manufacturer")
            {
                datatableData = _datatableRepository.GetToDatatableManufacturer(paginationModel.PageNumber, paginationModel.PageSize).ToList();
            }
            else if (paginationModel.Entity == "Product")
            {
                datatableData = _datatableRepository.GetToDatatableProduct(paginationModel.PageNumber, paginationModel.PageSize).ToList();
            }
            else if (paginationModel.Entity == "Supplier")
            {
                datatableData = _datatableRepository.GetToDatatableSupplier(paginationModel.PageNumber, paginationModel.PageSize).ToList();
            }


            return Ok(new ResponseModelBuilder().WithData(datatableData)
                                                .WithSuccess(true)
                                                .WithAlert(AlertType.Success)
                                                .Build());
        }

        [HttpPost("get-by-filter")]
        public IActionResult GetByFilter([FromBody] FilterModel filter)
        {
            try
            {
                var type = Type.GetType($"Dispo.Shared.Core.Domain.Entities.{filter.Entity}, Dispo.Shared.Core.Domain");
                if (type is null)
                {
                    return BadRequest("Entidade inválida.");
                }

                var method = _filterService.GetType().GetMethod("Get");
                if (method is null)
                {
                    return BadRequest($"Método 'Get' não implementado para a entidade '{filter.Entity}'");
                }

                var genericMethod = method.MakeGenericMethod(type);
                var result = genericMethod.Invoke(_filterService, new object[] { filter });

                return Ok(new ResponseModelBuilder().WithData(result)
                                                    .WithSuccess(true)
                                                    .Build());
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModelBuilder().WithMessage(ex.Message)
                                                            .WithSuccess(false)
                                                            .Build()); ;
            }
        }
    }
}
