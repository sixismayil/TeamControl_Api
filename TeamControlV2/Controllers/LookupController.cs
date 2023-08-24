using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.Services.Interface;

namespace TeamControlV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LookupController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILookupService _lookupService;

        public LookupController(IMapper mapper, ILookupService lookupService)
        {
            _mapper = mapper;
            _lookupService = lookupService;
        }

        [HttpGet, Route("project-status"), Authorize]
        public IActionResult Status()
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);

            if (!currentUserRole)
            {
                return Unauthorized();
            }

            return Ok(new { Result = _lookupService.GetStatus() });
        }

        [HttpGet, Route("projects"), Authorize]
        public IActionResult Projects()
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);

            if (!currentUserRole)
            {
                return Unauthorized();
            }
            return Ok(new { Result = _lookupService.GetProjects() });
        }

        [HttpGet, Route("positions"), Authorize]
        public IActionResult Positions()
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);

            if (!currentUserRole)
            {
                return Unauthorized();
            }
            return Ok(new { Result = _lookupService.GetPositions() });
        }

        [HttpGet, Route("vacation-reasons"), Authorize]
        public IActionResult VacationReasons()
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);

            if (!currentUserRole)
            {
                return Unauthorized();
            }
            return Ok(new { Result = _lookupService.GetVacationReasons() });
        }

        [HttpGet, Route("employees"), Authorize]
        public IActionResult Employees()
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);

            if (!currentUserRole)
            {
                return Unauthorized();
            }
            return Ok(new { Result = _lookupService.GetEmployees() });
        }

        [HttpGet, Route("customers"), Authorize]
        public IActionResult Customers()
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);

            if (!currentUserRole)
            {
                return Unauthorized();
            }
            return Ok(new { Result = _lookupService.GetCustomers() });
        }
    }
}
