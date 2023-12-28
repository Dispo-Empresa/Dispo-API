﻿using Dispo.API.ResponseBuilder;
using Dispo.Movement.Core.Application.Interfaces;
using Dispo.Shared.Core.Domain;
using Dispo.Shared.Core.Domain.DTOs;
using Dispo.Shared.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.API.Controllers
{
    [Route("/api/v1/movements")]
    [ApiController]
    [Authorize(Roles = RolesManager.WarehouseOperatorAssociated)]
    public class MovementsController : ControllerBase
    {
        private readonly IMovementService _movementService;
        private readonly IMovementRepository _movementRepository;

        public MovementsController(IMovementService movementService, IMovementRepository movementRepository)
        {
            _movementService = movementService;
            _movementRepository = movementRepository;
        }

        /// <summary>
        /// Realiza a movimentação de um produto.
        /// </summary>
        [HttpPost("move")]
        public async Task<IActionResult> MoveProduct([FromBody] ProductMovimentationDto productMovimentationDto)
        {
            try
            {
                productMovimentationDto.Validate();
                await _movementService.MoveProductAsync(productMovimentationDto);

                return Ok(new ResponseModelBuilder().WithMessage("Movimentação de produto realizada com sucesso.")
                                                    .WithSuccess(true)
                                                    .Build());
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModelBuilder().WithMessage(ex.Message)
                                                            .WithSuccess(false)
                                                            .Build());
            }
        }

        /// <summary>
        /// Realiza a movimentação de entrada em lote.
        /// </summary>
        [HttpPost("move/batch")]
        public async Task<IActionResult> BatchInputMovement([FromBody] BatchMovimentationDto batchMovimentationDto)
        {
            try
            {
                await _movementService.MoveBatchAsync(batchMovimentationDto);
                return Ok(new ResponseModelBuilder().WithMessage("Movimentação de produto realizada com sucesso.")
                                                    .WithSuccess(true)
                                                    .Build());
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModelBuilder().WithMessage(ex.Message)
                                                            .WithSuccess(false)
                                                            .Build());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var movimentationDetails = _movementRepository.GetDetails();
                return Ok(new ResponseModelBuilder().WithData(movimentationDetails)
                                                    .WithSuccess(true)
                                                    .Build());
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModelBuilder().WithMessage(ex.Message)
                                                            .WithSuccess(false)
                                                            .Build());
            }
        }
    }
}