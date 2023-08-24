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
using TeamControlV2.Logging;
using TeamControlV2.Services.Interface;
using TeamControlV2.Validations;

namespace TeamControlV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profiles;
        private readonly IValidation _validation;
        private readonly ILoggerManager _logger;

        public int Errcodes { get; private set; }

        public ProfileController(
            IProfileService profiles,
            IValidation validation,
            ILoggerManager logger
            )
        {
            _profiles = profiles;
            _validation = validation;
            _logger = logger;
        }
        [HttpGet, Route("get-data"), Authorize]
        public IActionResult GetData()
        {
            var currentUser = HttpContext.User;
            int currentUserId = Convert.ToInt32(currentUser.FindFirst("UserId").Value);

            ResponseObject<PROFILE_VIEW_MODEL> response = new ResponseObject<PROFILE_VIEW_MODEL>();
            response.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;
            response.Status = new Status();
            response.Response = new PROFILE_VIEW_MODEL();

            int errorCode = 0;
            string message = null;

            try
            {
                response.Response = _profiles.GetData(currentUserId, ref errorCode, ref message, response.TraceID);

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
                _logger.LogError($"ProfileController GetData : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPost, Route("update-data"), Authorize]
        public IActionResult UpdateData([FromBody] ProfilePayload data)
        {

            var currentUser = HttpContext.User;
            int currentUserId = Convert.ToInt32(currentUser.FindFirst("UserId").Value);

            ResponseSimple response = new ResponseSimple();
            response.Status = new Status();
            response.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;

            int errorCode = 0;
            string message = null;
            try
            {
                _profiles.UpdateData(data, currentUserId, ref errorCode, ref message, response.TraceID);
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
                _logger.LogError($"ProfileController UpdateData : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }
    }
}
