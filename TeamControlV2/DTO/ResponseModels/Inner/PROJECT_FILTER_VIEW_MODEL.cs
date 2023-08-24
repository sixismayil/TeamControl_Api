using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.DTO.ResponseModels.Inner
{
    public class PROJECT_FILTER_VIEW_MODEL
    {
        public int ProjectId { get; set; }

        public int ProjectStatusId { get; set; }

        public int TeamLeaderId { get; set; }
    }
}
