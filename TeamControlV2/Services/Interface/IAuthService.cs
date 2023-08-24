using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.Domain.Models;

namespace TeamControlV2.Services.Interface
{
    public interface IAuthService
    {
        EMPLOYEE GetEmployeeWithEmail(string email);
    }
}
