using System;

namespace TeamControlV2.DTO.RequestModels
{
    public class BonusAndPrizePayload
    {
        public int IsPrize { get; set; }

        public int EmployeeId { get; set; }

        public int Amount { get; set; }

        public DateTime Date { get; set; }

        public string Reason { get; set; }
        
        public int? ProjectId { get; set; }

    }
}
