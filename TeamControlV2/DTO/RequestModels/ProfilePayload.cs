using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.DTO.RequestModels
{
    public class ProfilePayload
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string RelativesPhoneNumber { get; set; }

        public string Address { get; set; }

        public string Biography { get; set; }

        public string Birthdate { get; set; }
    }
}
