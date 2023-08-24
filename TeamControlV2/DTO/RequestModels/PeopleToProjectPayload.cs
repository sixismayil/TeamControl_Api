using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.DTO.RequestModels
{
    public class PeopleToProjectPayload
    {
        public int PersonType { get; set; }

        public int Role { get; set; }

        public int?[] Position { get; set; }

        public int? EmployeeId { get; set; }

        public int? CustomerId { get; set; }

    }
}
