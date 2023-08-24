using System;

namespace TeamControlV2.DTO.RequestModels
{
    public class SalaryPayload
    {
        public int EmployeeId { get; set; }

        public DateTime Date { get; set; }

        public int Amount { get; set; }

        public double Salary { get; set; } 

    }
}
