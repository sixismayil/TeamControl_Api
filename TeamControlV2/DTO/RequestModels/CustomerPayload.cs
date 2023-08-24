using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.DTO.RequestModels
{
    public class CustomerPayload
    {
       
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Position { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
       
    }
}
