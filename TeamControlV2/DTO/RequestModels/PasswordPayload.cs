using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.DTO.RequestModels
{
    public class PasswordPayload
    {
        public string Password { get; set; }

        public string CurrentPassword { get; set; }
    }
}
