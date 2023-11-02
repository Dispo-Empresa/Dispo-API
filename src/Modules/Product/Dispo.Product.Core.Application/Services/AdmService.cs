﻿using Dispo.Shared.Core.Domain.Entities;
using Dispo.Shared.Core.Domain.Interfaces;
using Dispo.Product.Core.Application.Services.Interfaces;
using Dispo.Shared.Core.Domain.DTOs.Request;

namespace Dispo.Product.Core.Application.Services
{
    public class AdmService : IAdmService
    {
        public readonly IAccountRepository _accountRepository;
        public readonly IWarehouseAccountRepository _houseAccountRepository;

        public AdmService(IAccountRepository accountRepository, IWarehouseAccountRepository houseAccountRepository)
        {
            _accountRepository = accountRepository;
            _houseAccountRepository = houseAccountRepository;
        }

        public void CreateEmployee(CreateEmployeeRequestDto createEmployeeRequestDto)
        {
            try
            {
                var newAccount = new Account();
                newAccount.Email = createEmployeeRequestDto.Email;
                newAccount.Ativo = true;
                newAccount.Password = "alterarsenha";
                newAccount.RoleId = createEmployeeRequestDto.RoleId;

                _accountRepository.Create(newAccount);

                foreach (var warehouseId in createEmployeeRequestDto.WarehousesId)
                {
                    var newWarehouseAccount = new WarehouseAccount();
                    newWarehouseAccount.WarehouseId = warehouseId;
                    newWarehouseAccount.AccountId = newAccount.Id;

                    _houseAccountRepository.Create(newWarehouseAccount);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}