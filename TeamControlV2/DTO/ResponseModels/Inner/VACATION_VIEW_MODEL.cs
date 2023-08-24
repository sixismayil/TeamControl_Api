using System;

namespace TeamControlV2.DTO.ResponseModels.Inner
{
    public class VACATION_VIEW_MODEL
    {
        public int Id { get; set; }

        public string Employee { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string VacationReason { get; set; }

        public string Period { get; set; }

    }
}
