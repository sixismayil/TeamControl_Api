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
    public class PositionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPositionService _positions;
        private readonly IValidation _validation;
        private readonly ILoggerManager _logger;

        public int Errcodes { get; private set; }

        public PositionController(

            AppDbContext context,
            IPositionService positions,
            IValidation validation,
            ILoggerManager logger
            )
        {
            _context = context;
            _positions = positions;
            _validation = validation;
            _logger = logger;
        }

        [HttpPost, Route("create-position"), Authorize]
        public IActionResult CreatePosition([FromBody] PositionPayload position)
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value); 

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
                _positions.CreatePosition(position, ref errorCode, ref message, response.TraceID);
                if (errorCode != 0)
                {
                    response.Status.ErrCode = errorCode;
                    response.Status.Message = message;
                    return StatusCode(_validation.CheckErrorCode(errorCode), response);
                }
                else
                {
                    response.Status.Message = "Yeni vəzifə yaradıldı.";
                }
            }
            catch (Exception ex)
            {
                response.Status.ErrCode = ErrorCode.SYSTEM;
                response.Status.Message = message;
                _logger.LogError($"PositionController CreatePosition : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }

        [HttpGet, Route("get-positions"), Authorize]
        public IActionResult GetPositions(int limit, int skip, bool isExport)
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);

            if (!currentUserRole)
            {
                return Unauthorized();
            }

            ResponseListTotal<POSITION_VIEW_MODEL> responseList = new ResponseListTotal<POSITION_VIEW_MODEL>();
            ResponseTotal<POSITION_VIEW_MODEL> response = new ResponseTotal<POSITION_VIEW_MODEL>();

            responseList.Response = response;
            responseList.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;
            responseList.Status = new Status();
            int errorCode = 0;
            decimal totalCount = 0;
            string message = null;

            try
            {
                responseList.Response.Data = _positions.GetPositions(skip, limit, ref totalCount, isExport, ref errorCode, ref message, responseList.TraceID);
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
                _logger.LogError($"PositionController GetPositions : {responseList.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, responseList);
            }
        }

        [HttpGet, Route("get-position"), Authorize]
        public IActionResult GetPosition(int id)
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);

            if (!currentUserRole)
            {
                return Unauthorized();
            }

            ResponseObject<PositionPayload> response = new ResponseObject<PositionPayload>();
            response.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;
            response.Status = new Status();
            response.Response = new PositionPayload();

            int errorCode = 0;
            string message = null;

            try
            {
                response.Response = _positions.GetPosition(id, ref errorCode, ref message, response.TraceID);

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
                _logger.LogError($"PositionController GetPosition : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPost, Route("update-position"), Authorize]
        public IActionResult UpdatePosition(PositionPayload position, int id)
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);

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
                _positions.UpdatePosition(position, id, ref errorCode, ref message, response.TraceID);
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
                _logger.LogError($"PositionController UpdatePosition : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }

        [HttpDelete, Route("delete-position"), Authorize]
        public IActionResult DeletePosition(int id)
        {
            var currentUser = HttpContext.User;
            bool currentUserRole = Convert.ToBoolean(currentUser.FindFirst("UserRole").Value);

            if (!currentUserRole)
            {
                return Unauthorized();
            }

            ResponseSimple response = new ResponseSimple();
            response.Status = new Status();
            response.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;

            int errorCode = 0;
            string message = null;
            bool positionExists = false;

            try
            {
                _positions.DeletePosition(id, ref errorCode, ref positionExists, ref message, response.TraceID);
                if (errorCode != 0 || errorCode == 46)
                {
                    response.Status.ErrCode = errorCode;
                    response.Status.Message = message;
                    return StatusCode(_validation.CheckErrorCode(errorCode), response);
                }
                else
                {
                    if (positionExists == true)
                    {
                        response.Status.Message = message;
                    }
                    else
                    {
                        response.Status.Message = "Vəzifə silindi.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status.ErrCode = ErrorCode.SYSTEM;
                response.Status.Message = message;
                _logger.LogError($"PositionController DeletePosition : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }
    }
}
