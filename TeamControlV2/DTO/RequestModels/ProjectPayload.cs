using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.DTO.RequestModels
{
    public class ProjectPayload
    {
        public string Name { get; set; }

        public int StatusId { get; set; }

        public int TeamLeaderId { get; set; }
    }
}
