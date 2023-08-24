using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.DTO.ResponseModels.Inner
{
    public class PROJECT_VIEW_MODEL
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }

        public string TeamLeader { get; set; }
    }
}
