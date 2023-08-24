using System;

namespace TeamControlV2.DTO.ResponseModels.Inner
{
    public class VACATION_FILTER_VIEW_MODEL
    {
        public string Employee { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int VacationReasonId { get; set; }
    }
}
