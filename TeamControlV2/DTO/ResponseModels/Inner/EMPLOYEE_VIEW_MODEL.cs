using System.Collections.Generic;

namespace TeamControlV2.DTO.ResponseModels.Inner
{
    public class EMPLOYEE_VIEW_MODEL
    {
        public int Id { get; set; }
        
        public string Firstname { get; set; }

        public string Lastname { get; set; }
           
        public string Projects { get; set; }
        
        public bool IsAdmin { get; set; }

    }
}
