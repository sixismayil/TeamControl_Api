using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.DTO.HelperModels;
using TeamControlV2.DTO.HelperModels.Const;
using TeamControlV2.DTO.ResponseModels.Inner;
using TeamControlV2.DTO.ResponseModels.Main;
using TeamControlV2.Infrastructure;
using TeamControlV2.Logging;
using TeamControlV2.Services.Interface;
using TeamControlV2.Validations;

namespace TeamControlV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HomeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHomeService _homeService;
        private readonly IValidation _validation;
        private readonly ILoggerManager _logger;

        public int Errcodes { get; private set; }

        public HomeController(
            AppDbContext context,
            IHomeService homeService,
            IValidation validation,
            ILoggerManager logger)
        {
            _context = context;
            _homeService = homeService;
            _validation = validation;
            _logger = logger;
        }

        [HttpGet, Route("get-projects"), Authorize]
        public IActionResult GetProjects(decimal count)
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);

            if (!currentUserRole)
            {
                return Unauthorized();
            }

            ResponseListTotal<PROJECT_VIEW_MODEL> responseList = new ResponseListTotal<PROJECT_VIEW_MODEL>();
            ResponseTotal<PROJECT_VIEW_MODEL> response = new ResponseTotal<PROJECT_VIEW_MODEL>();
            responseList.Response = response;
            responseList.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;
            responseList.Status = new Status();
            int errorCode = 0;
            decimal totalCount = 0;
            string message = null;

            try
            {
                responseList.Response.Data = _homeService.GetProjects(count, ref errorCode, ref message, responseList.TraceID);
                responseList.Response.Total = totalCount;
                if (errorCode != 0)
                {
                    responseList.Status.ErrCode = errorCode;
                    responseList.Status.Message = message;
                    return StatusCode(_validation.CheckErrorCode(errorCode), responseList);
                }
                else
                {
                    return Ok(responseList);
                    //return Ok(currentUserId);
                }
            }
            catch (Exception ex)
            {
                responseList.Status.ErrCode = ErrorCode.SYSTEM;
                responseList.Status.Message = message;
                _logger.LogError($"HomeController GetProjects : {responseList.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, responseList);
            }
        }

        [HttpGet, Route("get-statistics"), Authorize]
        public IActionResult GetStatistics()
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);

            if (!currentUserRole)
            {
                return Unauthorized();
            }

            ResponseObject<STATISTICS_VIEW_MODEL> response = new ResponseObject<STATISTICS_VIEW_MODEL>();
            response.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;
            response.Status = new Status();
            response.Response = new STATISTICS_VIEW_MODEL();

            int errorCode = 0;
            string message = null;

            try
            {
                response.Response = _homeService.GetStatistics(ref errorCode, ref message, response.TraceID);
                if (errorCode != 0)
                {
                    response.Status.ErrCode = errorCode;
                    response.Status.Message = message;
                    return StatusCode(_validation.CheckErrorCode(errorCode), response);
                }
                else
                {
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.Status.ErrCode = ErrorCode.SYSTEM;
                response.Status.Message = message;
                _logger.LogError($"HomeController GetStatistics : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet, Route("get-vacations"), Authorize]
        public IActionResult GetVacations(decimal count)
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);

            if (!currentUserRole)
            {
                return Unauthorized();
            }

            ResponseListTotal<VACATION_VIEW_MODEL> responseList = new ResponseListTotal<VACATION_VIEW_MODEL>();
            ResponseTotal<VACATION_VIEW_MODEL> response = new ResponseTotal<VACATION_VIEW_MODEL>();
            responseList.Response = response;
            responseList.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;
            responseList.Status = new Status();
            int errorCode = 0;
            decimal totalCount = 0;
            string message = null;

            try
            {
                responseList.Response.Data = _homeService.GetVacations(count, ref errorCode, ref message, responseList.TraceID);
                responseList.Response.Total = totalCount;
                if (errorCode != 0)
                {
                    responseList.Status.ErrCode = errorCode;
                    responseList.Status.Message = message;
                    return StatusCode(_validation.CheckErrorCode(errorCode), responseList);
                }
                else
                {
                    return Ok(responseList);
                }

            }
            catch (Exception ex)
            {
                responseList.Status.ErrCode = ErrorCode.SYSTEM;
                responseList.Status.Message = message;
                _logger.LogError($"HomeController GetVacations : {responseList.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, responseList);
            }
        }
    }
}
