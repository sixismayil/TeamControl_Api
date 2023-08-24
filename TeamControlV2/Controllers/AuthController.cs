using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.DTO.HelperModels.Jwt;
using TeamControlV2.DTO.RequestModels;
using TeamControlV2.Logging;
using TeamControlV2.Services.Interface;

namespace TeamControlV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IJwtHandler _jwthandler;
        private readonly IAuthService _authService;
        private readonly ILoggerManager _logger;

        public AuthController(
            IJwtHandler jwthandler,
            IAuthService authService,
            ILoggerManager logger)
        {
            _jwthandler = jwthandler;
            _authService = authService;
            _logger = logger;
        }
        [HttpPost, Route("normal-login"), AllowAnonymous]
        public IActionResult NormalLogin([FromBody] NormalLoginModel employee)
        {
            if (employee == null)
            {
                return BadRequest("Invalid client request");
            }
            var db_employee = _authService.GetEmployeeWithEmail(employee.Email);

            if (db_employee != null && db_employee.IsActive == true)
            {
                bool verified = BCrypt.Net.BCrypt.Verify(employee.Password, db_employee.Password);
                if (verified)
                {
                    var claims = new JwtCustomClaims
                    {
                        UserId = db_employee.Id.ToString(),
                        UserName = $"{db_employee.Name} {db_employee.Surname}",
                        UserRole = db_employee.IsAdmin.ToString()
                    };
                    var jwt = _jwthandler.CreateToken(claims);
                    return Ok(new { Result = jwt, ErrorCode = 0 });
                }
                else
                {
                    return Ok(new { Result = "İstifadəçi adı və ya şifrə yalnışdır.", ErrorCode = 1 });
                }
            }
            else
            {
                return Ok(new { Result = "İstifadəçi adı və ya şifrə yalnışdır.", ErrorCode = 1 });
            }
        }
    }
}
