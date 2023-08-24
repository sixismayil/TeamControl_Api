using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.DTO.ResponseModels.Inner
{
    public class EMPLOYEE_FILTER_VIEW_MODEL
    {
        public string Fullname { get; set; }

        public int ProjectId { get; set; }

        public int ProjectStatusId { get; set; }
    }
}
