using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.DTO.RequestModels
{
    public class AddPeopleToProjectPayload
    {
        public int PersonType { get; set; }

        public int? EmployeeId { get; set; }

        public int? CustomerId { get; set; }

        public int RoleId { get; set; }

        public int?[] Position { get; set; }
    }
}
