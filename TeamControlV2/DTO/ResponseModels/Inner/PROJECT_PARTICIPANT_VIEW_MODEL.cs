using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.DTO.ResponseModels.Inner
{
    public class PROJECT_PARTICIPANT_VIEW_MODEL
    {
        public int PersonType { get; set; }

        public int Role { get; set; }

        public List<int> Position { get; set; }

        public int? CustomerId { get; set; }

        public int? EmployeeId { get; set; }
    }
}
