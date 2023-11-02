﻿using System.Text.Json.Serialization;

namespace Dispo.Shared.Core.Domain.DTOs.Response
{
    public class UserAccountResponseDto
    {
        [JsonIgnore]
        public long Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CpfCnpj { get; set; }
        public string Phone { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public long? CurrentWarehouseId { get; set; }
    }
}