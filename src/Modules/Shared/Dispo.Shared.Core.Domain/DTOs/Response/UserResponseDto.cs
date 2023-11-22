﻿namespace Dispo.Shared.Core.Domain.DTOs.Response
{
    public class UserResponseDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CpfCnpj { get; set; }
        public string Phone { get; set; }
        public DateTime BirthDate { get; set; }
        public long AccountId { get; set; }
        public long BranchId { get; set; }
    }
}