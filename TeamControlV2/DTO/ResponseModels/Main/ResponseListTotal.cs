using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.DTO.HelperModels;

namespace TeamControlV2.DTO.ResponseModels.Main
{
    public class ResponseListTotal<T>
    {
        public Status Status { get; set; }

        public ResponseTotal<T> Response { get; set; }

        public string TraceID { get; set; }
    }
}
