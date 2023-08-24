using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace TeamControlV2.DTO.RequestModels
{
    public class VacationPayload
    {
        public int EmployeeId { get; set; }

        public int VacationReasonId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

    }
}
