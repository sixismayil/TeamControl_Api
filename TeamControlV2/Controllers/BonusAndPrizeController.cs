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

    public class BonusAndPrizeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IBonusAndPrizeService _bonusesAndPrizes;

        private readonly IValidation _validation;
        private readonly ILoggerManager _logger;

        public int Errcodes { get; private set; }

        public BonusAndPrizeController(
            AppDbContext context,
            IBonusAndPrizeService bonusesAndPrizes,
            IValidation validation,
            ILoggerManager logger
            )
        {
            _context = context;
            _bonusesAndPrizes = bonusesAndPrizes;
            _validation = validation;
            _logger = logger;
        }

        [HttpPost, Route("create-bonus-and-prize")]
        public IActionResult CreateBonusAndPrize([FromBody] BonusAndPrizePayload bonusAndPrize)
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
                _bonusesAndPrizes.CreateBonusAndPrize(bonusAndPrize, currentUserId, ref errorCode, ref message, response.TraceID);
                if (errorCode != 0)
                {
                    response.Status.ErrCode = errorCode;
                    response.Status.Message = message;
                    return StatusCode(_validation.CheckErrorCode(errorCode), response);
                }
                else
                {
                    if (bonusAndPrize.IsPrize == 1)
                    {
                        response.Status.Message = "Yeni premiya yaradıldı.";
                    }
                    else {
                        response.Status.Message = "Yeni bonus yaradıldı.";
                    }

                }
            }
            catch (Exception ex)
            {
                response.Status.ErrCode = ErrorCode.SYSTEM;
                response.Status.Message = message;
                _logger.LogError($"BonusAndPrizeController CreateBonusAndPrize : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }


        [HttpDelete, Route("delete-bonus-and-prize")]
        public IActionResult DeleteBonusAndPrize(int id)
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
                _bonusesAndPrizes.DeleteBonusAndPrize(id, currentUserId, ref errorCode, ref message, response.TraceID);
                if (errorCode != 0)
                {
                    response.Status.ErrCode = errorCode;
                    response.Status.Message = message;
                    return StatusCode(_validation.CheckErrorCode(errorCode), response);
                }
                else
                {
                  response.Status.Message = "Deaktiv olundu.";
                }
            }
            catch (Exception ex)
            {
                response.Status.ErrCode = ErrorCode.SYSTEM;
                response.Status.Message = message;
                _logger.LogError($"BonusAndPrizeController DeleteBonusAndPrize : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }

        [HttpGet, Route("get-bonus-and-prize")]
        public IActionResult GetBonusAndPrize(int id)
        {
            ResponseObject<BonusAndPrizePayload> response = new ResponseObject<BonusAndPrizePayload>();
            response.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;
            response.Status = new Status();
            response.Response = new BonusAndPrizePayload();

            int errorCode = 0;
            string message = null;

            try
            {
                response.Response = _bonusesAndPrizes.GetBonusAndPrize(id, ref errorCode, ref message, response.TraceID);

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
                _logger.LogError($"BonusAndPrizeController GetBonusAndPrize : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPost, Route("get-bonuses-and-prizes")]
        public IActionResult GetBonusesAndPrizes([FromBody] BONUS_AND_PRIZE_FILTER_VIEW_MODEL model, int limit, int skip, bool isExport)
        {
            ResponseListTotal<BONUS_AND_PRIZE_VIEW_MODEL> responseList = new ResponseListTotal<BONUS_AND_PRIZE_VIEW_MODEL>();
            ResponseTotal<BONUS_AND_PRIZE_VIEW_MODEL> response = new ResponseTotal<BONUS_AND_PRIZE_VIEW_MODEL>();
            responseList.Response = response;
            responseList.TraceID = Activity.Current.Id ?? HttpContext.TraceIdentifier;
            responseList.Status = new Status();
            int errorCode = 0;
            decimal totalCount = 0;
            string message = null;

            try
            {
                responseList.Response.Data = _bonusesAndPrizes.GetBonusesAndPrizes(model, skip, limit, ref totalCount, isExport, ref errorCode, ref message, responseList.TraceID);
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
                _logger.LogError($"BonusAndPrizeController GetBonusesAndPrizes : {responseList.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, responseList);
            }
        }


        
        [HttpPost, Route("update-bonus-and-prize"), Authorize]
        public IActionResult UpdateBonusAndPrize(BonusAndPrizePayload bonusAndPrize, int id)
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
                _bonusesAndPrizes.UpdateBonusAndPrize(bonusAndPrize, id, currentUserId, ref errorCode, ref message, response.TraceID);
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
                _logger.LogError($"BonusAndPrizeController UpdateBonusAndPrize : {response.TraceID}" + $"{ex}");
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, response);
            }
            return Ok(response);
        }

        


    }
}
