﻿using Dispo.Commom;
using Dispo.Domain.DTOs;
using Dispo.Domain.Entities;
using Dispo.Infrastructure.Context;
using Dispo.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dispo.Infrastructure.Repositories
{
    public class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        private readonly DispoContext _dispoContext;

        public AccountRepository(DispoContext dispoContext)
            : base(dispoContext)
        {
            _dispoContext = dispoContext;
        }

        #region Expressions

        private Expression<Func<Account, bool>> ExpBySignInModel(string email, string password)
            => exp => exp.Email.Equals(email) && exp.Password.Equals(password);

        private Expression<Func<Account, bool>> ExpById(long accountId)
            => exp => exp.Id.Equals(accountId);

        private Expression<Func<Account, bool>> ExpExistsByEmail(string email)
            => exp => exp.Email.Equals(email);

        #endregion Expressions

        public bool ExistsByEmail(string email)
        {
            return _dispoContext.Accounts
                                .Any(ExpExistsByEmail(email));
        }

        public bool ExistsByEmailAndPassword(string email, string password)
        {
            return _dispoContext.Accounts
                                .Any(ExpBySignInModel(email, password));
        }

        public Account? GetAccountByEmailAndPassword(string email, string password)
            => _dispoContext.Accounts
                            .Include(x => x.Role)
                            .FirstOrDefault(ExpBySignInModel(email, password));

        public void ResetPassword(Account account, string newPassword)
        {
            _dispoContext.Entry(account).State = EntityState.Modified;
            account.Password = newPassword;
            _dispoContext.SaveChanges();
        }

        public long GetAccountIdByEmail(string email)
            => (_dispoContext.Accounts.Where(x => x.Email == email)
                                      .Select(s => s.Id)
                                      .SingleOrDefault())
                                      .ToLong();

        public UserInfoResponseDto GetUserInfoResponseDto(long id)
            => _dispoContext.Accounts.Where(x => x.Id == id)
                                     .Include(x => x.User)
                                     .Select(s => new UserInfoResponseDto()
                                     {
                                         Email = s.Email,
                                         FirstName = s.User.FirstName,
                                         LastName = s.User.LastName,
                                         CpfCnpj = s.User.Cpf,
                                         Phone = s.User.Phone,
                                         BirthDate = s.User.BirthDate
                                     })
                                     .SingleOrDefault() ?? new UserInfoResponseDto();

        public string GetUserNameByAccountId(long id)
            => _dispoContext.Accounts.Where(x => x.Id == id)
                                     .Select(s => s.User.FirstName)
                                     .FirstOrDefault() ?? string.Empty;

        public string GetRoleKeyByAccountId(long id)
            => _dispoContext.Accounts.Where(x => x.Id == id)
                                     .Select(s => s.Role.Key)
                                     .FirstOrDefault() ?? string.Empty;

        public IList<AccountUserInfoDto> GetAccountsUserInfo()
            => _dispoContext.Accounts.Where(x => x.Ativo == true)
                                                 //.Include(x => x.User)
                                                 .Select(x => new AccountUserInfoDto()
                                                 {
                                                     AccountId = x.Id,
                                                     Email = x.Email,
                                                     RoleName = x.RoleId.ToString(),
                                                     //FullName = x.UserId.IsIdValid() ? $"{x.User.FirstName} {x.User.LastName}" : string.Empty,
                                                     //Phone = x.UserId.IsIdValid() ? x.User.Phone : string.Empty,

                                                 })
                                                 .ToList();
    }
}