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
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ICustomerService _customers;
        private readonly IValidation _validation;
        private readonly ILoggerManager _logger;

        public int Errcodes { get; private set; }

        public CustomerController(

            AppDbContext context,
            ICustomerService customers,
            IValidation validation,
            ILoggerManager logger)
        {
            _context = context;
            _customers = customers;
            _validation = validation;
            _logger = logger;
        }

        [HttpPost, Route("create-customer"), Authorize]
        public IActionResult Create([FromBody] CustomerPayload customer)
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
                _customers.CreateCustomer(customer, currentUserId, ref errorCode, ref message, response.TraceID);
                if (errorCode != 0)
                {
                    response.Status.ErrCode = errorCode;
                    response.Status.Message = message;
                    return StatusCode(_validation.CheckErrorCode(errorCode), response);
                }
                else
                {
                    response.Status.Message = "Yeni müştəri yaradıldı.";
                }
            }
            catch (Exception ex)
            {
                response.Status.ErrCode = ErrorCode.SYSTEM;
                response.Status.Message = message;
                _logger.LogError($"CustomerController Create : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }

        [HttpPost, Route("get-customers"), Authorize]
        public IActionResult GetCustomers([FromBody] CUSTOMER_FILTER_VIEW_MODEL model, int limit, int skip, bool isExport)
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);
            ResponseListTotal<CUSTOMER_VIEW_MODEL> responseList = new ResponseListTotal<CUSTOMER_VIEW_MODEL>();
            ResponseTotal<CUSTOMER_VIEW_MODEL> response = new ResponseTotal<CUSTOMER_VIEW_MODEL>();
            responseList.Response = response;
            responseList.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;
            responseList.Status = new Status();
            int errorCode = 0;
            decimal totalCount = 0;
            string message = null;

            if (!currentUserRole)
            {
                return Unauthorized();
            }

            try
            {
                responseList.Response.Data = _customers.GetCustomers(model, skip, limit, ref totalCount, isExport,ref errorCode, ref message, responseList.TraceID);
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
                _logger.LogError($"CustomerController GetCustomers : {responseList.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, responseList);
            }
        }

        [HttpGet, Route("get-customer"), Authorize]
        public IActionResult GetCustomer(int id)
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);
            ResponseObject<CustomerPayload> response = new ResponseObject<CustomerPayload>();
            response.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;
            response.Status = new Status();
            response.Response = new CustomerPayload();

            int errorCode = 0;
            string message = null;

            if (!currentUserRole)
            {
                return Unauthorized();
            }

            try
            {
                response.Response = _customers.GetCustomer(id, ref errorCode, ref message, response.TraceID);

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
                _logger.LogError($"CustomerController GetCustomer : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet, Route("get-projects-participated"), Authorize]
        public IActionResult GetProjectsParticipated(int customerId)
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);
            ResponseListTotal<string> responseList = new ResponseListTotal<string>();
            ResponseTotal<string> response = new ResponseTotal<string>();
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
                responseList.Response.Data = _customers.GetProjectsParticipated(customerId, ref errorCode, ref message, responseList.TraceID);
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
                _logger.LogError($"CustomerController GetProjectsParticipated : {responseList.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, responseList);
            }
        }

        [HttpGet, Route("get-projects-detailed"), Authorize]
        public IActionResult GetProjectsDetailed(int customerId)
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);
            ResponseListTotal<CUSTOMER_DETAIL_PROJECT_VIEW_MODEL> responseList = new ResponseListTotal<CUSTOMER_DETAIL_PROJECT_VIEW_MODEL>();
            ResponseTotal<CUSTOMER_DETAIL_PROJECT_VIEW_MODEL> response = new ResponseTotal<CUSTOMER_DETAIL_PROJECT_VIEW_MODEL>();
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
                responseList.Response.Data = _customers.GetProjectsDetailed(customerId, ref errorCode, ref message, responseList.TraceID);
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
                _logger.LogError($"CustomerController GetProjectsParticipated : {responseList.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, responseList);
            }
        }

        [HttpPost, Route("update-customer"), Authorize]
        public IActionResult UpdateCustomer(CustomerPayload customer,int id)
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
                _customers.UpdateCustomer(customer, id, currentUserId, ref errorCode, ref message, response.TraceID);
                if(errorCode != 0)
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
                _logger.LogError($"CustomerController UpdateCustomer : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
           
            return Ok(response);
        }
        
        [HttpDelete, Route("delete-customer"), Authorize]
        public IActionResult DeleteCustomer(int id)
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
                _customers.DeleteCustomer(id, currentUserId, ref errorCode, ref message, response.TraceID);
                if (errorCode != 0)
                {
                    response.Status.ErrCode = errorCode;
                    response.Status.Message = message;
                    return StatusCode(_validation.CheckErrorCode(errorCode), response);
                }
                else
                {
                    response.Status.Message = "Müştəri deaktiv edildi.";
                }
            }
            catch (Exception ex)
            {
                response.Status.ErrCode = ErrorCode.SYSTEM;
                response.Status.Message = message;
                _logger.LogError($"CustomerController DeleteCustomer : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }
    }
}
