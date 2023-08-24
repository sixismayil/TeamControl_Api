using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.DTO.ResponseModels.Inner
{
    public class CUSTOMER_VIEW_MODEL
    {
        public int Id { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string PhoneNumber { get; set; }

        public string Projects { get; set; }
    }
}
