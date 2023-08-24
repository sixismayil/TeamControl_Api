using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.Domain.Models;
using TeamControlV2.Infrastructure.Repository;
using TeamControlV2.Logging;
using TeamControlV2.Services.Interface;

namespace TeamControlV2.Services.Implementation
{
    public class AuthService : IAuthService
    {
        //AppConfiguration config = new AppConfiguration();
        private readonly IRepository<EMPLOYEE> _employee;
        private readonly ILoggerManager _logger;

        public AuthService(
            IRepository<EMPLOYEE> employee,
            ILoggerManager logger
            )
        {
            _employee = employee;
            _logger = logger;
        }

        public EMPLOYEE GetEmployeeWithEmail(string email)
        {
            return _employee.AllQuery.FirstOrDefault(x => x.Email == email);
        }
    }
}
