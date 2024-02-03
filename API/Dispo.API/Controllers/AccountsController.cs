﻿using Dispo.API.ResponseBuilder;
using Dispo.Infra.Core.Application.Interfaces;
using Dispo.Shared.Core.Domain;
using Dispo.Shared.Core.Domain.Entities;
using Dispo.Shared.Core.Domain.Exceptions;
using Dispo.Shared.Core.Domain.Interfaces;
using Dispo.Shared.Filter.Model;
using Dispo.Shared.Filter.Services;
using Dispo.Shared.Utils;
using EscNet.Cryptography.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.API.Controllers
{
    [Route("/api/v1/accounts")]
    [ApiController]
    [Authorize(Roles = RolesManager.AllRoles)]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRijndaelCryptography _rijndaelCryptography;
        private readonly IAccountResolverService _accountResolverService;
        private readonly IAccountService _accountService;
        private readonly IFilterService _filterService;

        public AccountsController(IAccountRepository accountRepository, IRijndaelCryptography rijndaelCryptography, IAccountResolverService userResolverService, IAccountService accountService, IFilterService filterService)
        {
            _accountRepository = accountRepository;
            _rijndaelCryptography = rijndaelCryptography;
            _accountResolverService = userResolverService;
            _accountService = accountService;
            _filterService = filterService;
        }

        [HttpGet("get-id")]
        public IActionResult GetAccountIdByEmail([FromRoute] string email)
        {
            var accountId = _accountRepository.GetAccountIdByEmail(_rijndaelCryptography.Encrypt(email));

            if (accountId.IsIdValid())
            {
                return Ok(new ResponseModelBuilder().WithMessage($"Account Id: {accountId}")
                                                    .WithSuccess(true)
                                                    .WithData(accountId)
                                                    .Build());
            }

            return BadRequest(new ResponseModelBuilder().WithMessage("Account Id not found")
                                                        .Build());
        }

        [HttpPost("change-warehouse")]
        public IActionResult ChangeWarehouse([FromBody] long warehouseId)
        {
            try
            {
                var accountId = _accountResolverService.GetLoggedAccountId() ?? throw new NotFoundException("Faça o login no sistema.");
                _accountService.ChangeWarehouse(accountId, warehouseId);

                return Ok(new ResponseModelBuilder().WithMessage("O depósito foi vinculado ao usuário.")
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

        [AllowAnonymous]
        [HttpPost("test-entity")]
        public IActionResult TestEntity([FromBody] FilterModel filter)
        {
            try
            {
                var result = _filterService.Get<Account>(filter);
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