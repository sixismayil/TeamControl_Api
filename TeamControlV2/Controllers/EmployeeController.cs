using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.Domain.Models;
using TeamControlV2.DTO.HelperModels;
using TeamControlV2.DTO.HelperModels.Const;
using TeamControlV2.DTO.RequestModels;
using TeamControlV2.DTO.RequestModels.Employee;
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
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employees;
        private readonly IValidation _validation;
        private readonly ILoggerManager _logger;

        public int Errcodes { get; private set; }

        public EmployeeController(
            IEmployeeService employees,
            IValidation validation,
            ILoggerManager logger)
        {
            _employees = employees;
            _validation = validation;
            _logger = logger;
        }

        [HttpPost, Route("create-employee"), Authorize]
        public IActionResult Create([FromBody] NewEmployeePayload employee)
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
                _employees.CreateEmployee(employee, currentUserId, ref errorCode, ref message, response.TraceID);
                if (errorCode != 0)
                {
                    response.Status.ErrCode = errorCode;
                    response.Status.Message = message;
                    return StatusCode(_validation.CheckErrorCode(errorCode), response);

                }
                else
                {
                    response.Status.Message = "Yeni işçi yaradıldı.";
                }
            }
            catch (Exception ex)
            {
                response.Status.ErrCode = ErrorCode.SYSTEM;
                response.Status.Message = message;
                _logger.LogError($"EmployeeController Create : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }

        [HttpPost, Route("get-employees"), Authorize]
        public IActionResult GetEmployees([FromBody] EMPLOYEE_FILTER_VIEW_MODEL model, int limit, int skip, bool isExport)
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);
            int currentUserId = Convert.ToInt32(currentUser.FindFirst("UserId").Value);

            if (!currentUserRole)
            {
                return Unauthorized();
            }

            ResponseListTotal<EMPLOYEE_VIEW_MODEL> responseList = new ResponseListTotal<EMPLOYEE_VIEW_MODEL>();
            ResponseTotal<EMPLOYEE_VIEW_MODEL> response = new ResponseTotal<EMPLOYEE_VIEW_MODEL>();
            responseList.Response = response;
            responseList.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;
            responseList.Status = new Status();
            int errorCode = 0;
            decimal totalCount = 0;
            string message = null;

            try
            {
                responseList.Response.Data = _employees.GetEmployees(model, skip, limit, ref totalCount, isExport, ref errorCode, ref message, responseList.TraceID);
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
                _logger.LogError($"EmployeeController GetEmployees : {responseList.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, responseList);
            }
        }

        [HttpGet, Route("get-employee"), Authorize]
        public IActionResult GetEmployee(int id)
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);
            ResponseObject<NewEmployeePayload> response = new ResponseObject<NewEmployeePayload>();
            response.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;
            response.Status = new Status();
            response.Response = new NewEmployeePayload();

            int errorCode = 0;
            string message = null;

            if (!currentUserRole)
            {
                return Unauthorized();
            }

            try
            {
                response.Response = _employees.GetEmployee(id, ref errorCode, ref message, response.TraceID);

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
                _logger.LogError($"EmployeeController GetEmployee : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPost, Route("update-employee"), Authorize]
        public IActionResult UpdateEmployee([FromBody] NewEmployeePayload employee, int id)
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
                _employees.UpdateEmployee(employee, id, currentUserId, ref errorCode, ref message, response.TraceID);
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
                _logger.LogError($"EmployeeController UpdateEmployee : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }

        [HttpDelete, Route("delete-employee"), Authorize]
        public IActionResult DeleteEmployee(int id)
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
                _employees.DeleteEmployee(id, currentUserId, ref errorCode, ref message, response.TraceID);
                if (errorCode != 0)
                {
                    response.Status.ErrCode = errorCode;
                    response.Status.Message = message;
                    return StatusCode(_validation.CheckErrorCode(errorCode), response);
                }
                else
                {
                    response.Status.Message = "İşçi deaktiv olundu.";
                }
            }
            catch (Exception ex)
            {
                response.Status.ErrCode = ErrorCode.SYSTEM;
                response.Status.Message = message;
                _logger.LogError($"EmployeeController DeleteEmployee : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }

        [HttpGet, Route("get-projects-employee-participated")]
        public IActionResult getProjectsEmployeeParticipated(int employeeId)
        {
            ResponseListTotal<string> responseList = new ResponseListTotal<string>();
            ResponseTotal<string> response = new ResponseTotal<string>();
            responseList.Response = response;
            responseList.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;
            responseList.Status = new Status();
            int errorCode = 0;
            string message = null;
            try
            {
                responseList.Response.Data = _employees.getProjectsEmployeeParticipated(employeeId, ref errorCode, ref message, responseList.TraceID);
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
            catch (Exception ex) {
                responseList.Status.ErrCode = ErrorCode.SYSTEM;
                responseList.Status.Message = message;
                _logger.LogError($"EmployeeController GetProjectsEmployeeParticipated : {responseList.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, responseList);
            }
        }


        [HttpPost, Route("change-password"), Authorize]
        public IActionResult ChangePassword([FromBody] PasswordPayload passwordPayload)
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);
            int currentUserId = Convert.ToInt32(currentUser.FindFirst("UserId").Value);

            ResponseSimple response = new ResponseSimple();
            response.Status = new Status();
            response.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;

            int errorCode = 0;
            string message = null;

            try
            {
                _employees.ChangePassword(currentUserId, passwordPayload, ref errorCode, ref message, response.TraceID);
                if (errorCode != 0)
                {
                    response.Status.ErrCode = errorCode;
                    response.Status.Message = message;
                    return StatusCode(_validation.CheckErrorCode(errorCode), response);
                }
                else
                {
                    response.Status.Message = "Şifrə dəyişdirildi.";
                }
            }
            catch (Exception ex)
            {
                response.Status.ErrCode = ErrorCode.SYSTEM;
                response.Status.Message = message;
                _logger.LogError($"CustomerController ChangePassword : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }



        [HttpGet, Route("get-salaries-detailed"), Authorize]
        public IActionResult GetSalariesDetailed(int employeeId)
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);
            ResponseListTotal<EMPLOYEE_DETAIL_SALARY_VIEW_MODEL> responseList = new ResponseListTotal<EMPLOYEE_DETAIL_SALARY_VIEW_MODEL>();
            ResponseTotal<EMPLOYEE_DETAIL_SALARY_VIEW_MODEL> response = new ResponseTotal<EMPLOYEE_DETAIL_SALARY_VIEW_MODEL>();
            responseList.Response = response;
            responseList.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;
            responseList.Status = new Status();
            int errorCode = 0;
            string message = null;

            if (!currentUserRole)
            {
                return Unauthorized();
            }

            try
            {
                responseList.Response.Data = _employees.GetSalariesDetailed(employeeId, ref errorCode, ref message, responseList.TraceID);
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
                _logger.LogError($"EmployeeController GetSalariesDetailed : {responseList.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, responseList);
            }
        }


        [HttpGet, Route("get-bonuses-detailed"), Authorize]
        public IActionResult GetBonusesDetailed(int employeeId)
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);
            ResponseListTotal<EMPLOYEE_DETAIL_BONUS_VIEW_MODEL> responseList = new ResponseListTotal<EMPLOYEE_DETAIL_BONUS_VIEW_MODEL>();
            ResponseTotal<EMPLOYEE_DETAIL_BONUS_VIEW_MODEL> response = new ResponseTotal<EMPLOYEE_DETAIL_BONUS_VIEW_MODEL>();
            responseList.Response = response;
            responseList.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;
            responseList.Status = new Status();
            int errorCode = 0;
            string message = null;

            if (!currentUserRole)
            {
                return Unauthorized();
            }

            try
            {
                responseList.Response.Data = _employees.GetBonusesDetailed(employeeId, ref errorCode, ref message, responseList.TraceID);
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
                _logger.LogError($"EmployeeController GetBonusesDetailed : {responseList.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, responseList);
            }

        }

        [HttpGet, Route("get-premiums-detailed"), Authorize]
        public IActionResult GetPremiumsDetailed(int employeeId)
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);
            ResponseListTotal<EMPLOYEE_DETAIL_PREMIUM_VIEW_MODEL> responseList = new ResponseListTotal<EMPLOYEE_DETAIL_PREMIUM_VIEW_MODEL>();
            ResponseTotal<EMPLOYEE_DETAIL_PREMIUM_VIEW_MODEL> response = new ResponseTotal<EMPLOYEE_DETAIL_PREMIUM_VIEW_MODEL>();
            responseList.Response = response;
            responseList.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;
            responseList.Status = new Status();
            int errorCode = 0;
            string message = null;

            if (!currentUserRole)
            {
                return Unauthorized();
            }

            try
            {
                responseList.Response.Data = _employees.GetPremiumsDetailed(employeeId, ref errorCode, ref message, responseList.TraceID);
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
                _logger.LogError($"EmployeeController GetPremiumsDetailed : {responseList.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, responseList);
            }





        }
   

   
        [HttpGet, Route("get-vacations-detailed"), Authorize]
        public IActionResult GetVacationsDetailed(int employeeId)
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);
            ResponseListTotal<EMPLOYEE_DETAIL_VACATION_VIEW_MODEL> responseList = new ResponseListTotal<EMPLOYEE_DETAIL_VACATION_VIEW_MODEL>();
            ResponseTotal<EMPLOYEE_DETAIL_VACATION_VIEW_MODEL> response = new ResponseTotal<EMPLOYEE_DETAIL_VACATION_VIEW_MODEL>();
            responseList.Response = response;
            responseList.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;
            responseList.Status = new Status();
            int errorCode = 0;
            string message = null;

            if (!currentUserRole)
            {
                return Unauthorized();
            }

            try
            {
                responseList.Response.Data = _employees.GetVacationsDetailed(employeeId, ref errorCode, ref message, responseList.TraceID);
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
                _logger.LogError($"EmployeeController GetVacationsDetailed : {responseList.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, responseList);
            }

        }






        [HttpGet, Route("reset-password"), Authorize]
        public IActionResult ResetPassword(int id)
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
                _employees.ResetPassword( id, currentUserId, ref errorCode, ref message, response.TraceID);
                if (errorCode != 0)
                {
                    response.Status.ErrCode = errorCode;
                    response.Status.Message = message;
                    return StatusCode(_validation.CheckErrorCode(errorCode), response);

                }
                else
                {
                    response.Status.Message = "Şifrə uğurla sıfırlandı.";
                }
            }
            catch (Exception ex)
            {
                response.Status.ErrCode = ErrorCode.SYSTEM;
                response.Status.Message = message;
                _logger.LogError($"EmployeeController ResetPassword : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }



    }
}

