using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.DTO.HelperModels;
using TeamControlV2.DTO.HelperModels.Const;
using TeamControlV2.DTO.RequestModels;
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
    public class VacationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IVacationService _vacations;
        private readonly IValidation _validation;
        private readonly ILoggerManager _logger;

        public int Errcodes { get; private set; }

        public VacationController(
            AppDbContext context,
            IVacationService vacations,
            IValidation validation,
            ILoggerManager logger
            )
        {
            _context = context;
            _vacations = vacations;
            _validation = validation;
            _logger = logger;
        }

        [HttpPost, Route("create-vacation"), Authorize]
        public IActionResult CreateVacation([FromBody] VacationPayload vacation)
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);
            int currentUserId = Convert.ToInt32(currentUser.FindFirst("UserId").Value);

            if (!currentUserRole)
            {
                return Unauthorized();
            }

            ResponseSimple response = new ResponseSimple();
            response.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;
            response.Status = new Status();

            int errorCode = 0;
            string message = null;


            try
            {
                _vacations.CreateVacation(vacation, currentUserId, ref errorCode, ref message, response.TraceID);
                if (errorCode != 0)
                {
                    response.Status.ErrCode = errorCode;
                    response.Status.Message = message;
                    return StatusCode(_validation.CheckErrorCode(errorCode), response);
                }
                else
                {
                    response.Status.Message = "Yeni  məzuniyyət yaradıldı.";
                }
            }
            catch (Exception ex)
            {
                response.Status.ErrCode = ErrorCode.SYSTEM;
                response.Status.Message = message;
                _logger.LogError($"VacationController CreateVacation : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }

        [HttpPost, Route("get-vacations"), Authorize]
        public IActionResult GetVacations([FromBody] VACATION_FILTER_VIEW_MODEL model, int limit, int skip, bool isExport)
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
                responseList.Response.Data = _vacations.GetVacations(model, skip, limit, ref totalCount, isExport, ref errorCode, ref message, responseList.TraceID);
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
                _logger.LogError($"VacationController GetVacations : {responseList.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, responseList);
            }
        }


        [HttpGet, Route("get-vacation"), Authorize]
        public IActionResult GetVacation(int id)
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);

            if (!currentUserRole)
            {
                return Unauthorized();
            }

            ResponseObject<VacationPayload> response = new ResponseObject<VacationPayload>();
            response.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;
            response.Status = new Status();
            response.Response = new VacationPayload();

            int errorCode = 0;
            string message = null;

            try
            {
                response.Response = _vacations.GetVacation(id, ref errorCode, ref message, response.TraceID);

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
                _logger.LogError($"VacationController GetVacation : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPost, Route("update-vacation"), Authorize]
        public IActionResult UpdateVacation(VacationPayload vacation, int id)
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);
            int currentUserId = Convert.ToInt32(currentUser.FindFirst("UserId").Value);

            if (!currentUserRole)
            {
                return Unauthorized();
            }


            ResponseSimple response = new ResponseSimple();
            response.Status = new Status();
            response.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;

            int errorCode = 0;
            string message = null;

            try
            {
                _vacations.UpdateVacation(vacation, id, currentUserId, ref errorCode, ref message, response.TraceID);
                if (errorCode != 0)
                {
                    response.Status.ErrCode = errorCode;
                    response.Status.Message = message;
                    return StatusCode(_validation.CheckErrorCode(errorCode), response);
                }
                else
                {
                    response.Status.Message = "Dəyişikliklər uğurla həyata keçirildi.";
                }

            }
            catch (Exception ex)
            {
                response.Status.ErrCode = ErrorCode.SYSTEM;
                response.Status.Message = message;
                _logger.LogError($"VacationController UpdateVacation : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }


        [HttpDelete, Route("delete-vacation"), Authorize]
        public IActionResult DeleteVacation(int id)
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);
            int currentUserId = Convert.ToInt32(currentUser.FindFirst("UserId").Value);

            if (!currentUserRole)
            {
                return Unauthorized();
            }

            ResponseSimple response = new ResponseSimple();
            response.Status = new Status();
            response.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;

            int errorCode = 0;
            string message = null;

            try
            {
                _vacations.DeleteVacation(id, currentUserId, ref errorCode, ref message, response.TraceID);
                if (errorCode != 0)
                {
                    response.Status.ErrCode = errorCode;
                    response.Status.Message = message;
                    return StatusCode(_validation.CheckErrorCode(errorCode), response);
                }
                else
                {
                    response.Status.Message = "Məzuniyyət deaktiv olundu.";
                }
            }
            catch (Exception ex)
            {
                response.Status.ErrCode = ErrorCode.SYSTEM;
                response.Status.Message = message;
                _logger.LogError($"VacationController DeleteVacation : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }


    }
}
