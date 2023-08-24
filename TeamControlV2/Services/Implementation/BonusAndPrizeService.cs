using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.Domain.Models;
using TeamControlV2.DTO.HelperModels.Const;
using TeamControlV2.DTO.RequestModels;
using TeamControlV2.DTO.ResponseModels.Inner;
using TeamControlV2.Infrastructure.Repository;
using TeamControlV2.Logging;
using TeamControlV2.Services.Interface;

namespace TeamControlV2.Services.Implementation
{
    public class BonusAndPrizeService : IBonusAndPrizeService
    {
        AppConfiguration config = new AppConfiguration();
        private readonly IRepository<BONUS_AND_PRIZE> _bonusesAndPrizes;
        private readonly IRepository<EMPLOYEE> _employees;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly ISqlService _sqlService;
        public BonusAndPrizeService(
            IRepository<BONUS_AND_PRIZE> bonusesAndPrizes,
            IRepository<EMPLOYEE> employees,
            IMapper mapper,
            ILoggerManager logger,
            ISqlService sqlService
            )
        {
            _bonusesAndPrizes = bonusesAndPrizes;
            _employees = employees;
            _mapper = mapper;
            _logger = logger;
            _sqlService = sqlService;
        }

        public void CreateBonusAndPrize(BonusAndPrizePayload bonusAndPrize, int currentUserId, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                BONUS_AND_PRIZE bap = _mapper.Map<BONUS_AND_PRIZE>(bonusAndPrize);
                bap.CreatedBy = currentUserId;
                bap.CreatedAt = DateTime.Now;
                bap.UpdatedBy = null;
                bap.UpdatedAt = null;
                bap.IsActive = true;
                bap.Reason = (bonusAndPrize.IsPrize == 1) ? bonusAndPrize.Reason : null;
                bap.ProjectId = (bonusAndPrize.IsPrize == 0) ? bonusAndPrize.ProjectId : null;
                _bonusesAndPrizes.Insert(bap);
                _bonusesAndPrizes.Save();

            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                //message = "DB create bonus and prize error";
                message = ex.Message;

                _logger.LogError($"BonusAndPrizeController CreateBonusAndPrize : {traceId}" + $"{ex}");
            }
        }


        public void DeleteBonusAndPrize(int id, int currentUserId, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                BONUS_AND_PRIZE bonusAndPrize = _bonusesAndPrizes.AllQuery.FirstOrDefault(x => x.Id == id && x.IsActive == true);
                bonusAndPrize.IsActive = false;
                bonusAndPrize.UpdatedBy = currentUserId;
                bonusAndPrize.UpdatedAt = DateTime.Now;
                _bonusesAndPrizes.Update(bonusAndPrize);
                _bonusesAndPrizes.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB delete BonusAndPrize error";
                _logger.LogError($"BonusAndPrizeController DeleteBonusAndPrize : {traceId}" + $"{ex}");
            }
        }


        public BonusAndPrizePayload GetBonusAndPrize(int id, ref int errorCode, ref string message, string traceId)
        {
            BonusAndPrizePayload result = new BonusAndPrizePayload();
            try
            {
                BONUS_AND_PRIZE bonusAndPrize = _bonusesAndPrizes.AllQuery.FirstOrDefault(x => x.Id == id && x.IsActive == true);
                result = _mapper.Map<BonusAndPrizePayload>(bonusAndPrize);
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB get bonus error";
                _logger.LogError($"BonusAndPrizeService GetBonusAndPrize : {traceId}" + $"{ex}");
            }
            return result;
        }


        public List<BONUS_AND_PRIZE_VIEW_MODEL> GetBonusesAndPrizes(BONUS_AND_PRIZE_FILTER_VIEW_MODEL model, int skip, int limit, ref decimal totalCount, bool isExport, ref int errorCode, ref string message, string traceId)
        {
            List<BONUS_AND_PRIZE_VIEW_MODEL> response = new List<BONUS_AND_PRIZE_VIEW_MODEL>();
            try
            {
                using (SqlConnection con = new SqlConnection(config.ConnectionString))
                {
                    SqlCommand cmd;
                    using (cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = _sqlService.BonusesAndPrizes(false, isExport, limit, skip, model);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            BONUS_AND_PRIZE_VIEW_MODEL a = new BONUS_AND_PRIZE_VIEW_MODEL()
                            {
                                Id = (int)rdr["ID"],
                                Employee = rdr["EMPLOYEE"].ToString(),
                                Date = rdr["DATE"].ToString(),
                                Amount = rdr["AMOUNT"].ToString(),
                                Reason = rdr["REASON"].ToString(),
                                Project = rdr["PROJECT"].ToString(),
                                IsPrize = (bool)rdr["IsPrize"]
                            };
                            response.Add(a);
                        }
                        rdr.Close();
                        cmd = con.CreateCommand();
                        if (!isExport)
                        {
                             cmd.CommandText = _sqlService.BonusesAndPrizes(true, false, limit, skip, model);

                            rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {
                                totalCount = (int)rdr["totalCount"];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = ex.Message;
                _logger.LogError($"BonusAndPrizeService GetBonusesAndPrizes : {traceId}" + $"{ex}");
            }

            return response;
        }



        public void UpdateBonusAndPrize(BonusAndPrizePayload bonusAndPrize, int id, int currentUserId, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                BONUS_AND_PRIZE oldData = _bonusesAndPrizes.AllQuery.AsNoTracking().FirstOrDefault(x => x.Id == id && x.IsActive == true);
                BONUS_AND_PRIZE newData = _mapper.Map<BONUS_AND_PRIZE>(bonusAndPrize);
                newData.Id = id;
                newData.CreatedAt = oldData.CreatedAt;
                newData.UpdatedAt = DateTime.Now;
                newData.CreatedBy = oldData.CreatedBy;
                newData.UpdatedBy = currentUserId;
                newData.IsActive = true;
                _bonusesAndPrizes.Update(newData);
                _bonusesAndPrizes.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB update BonusAndPrize error";
                _logger.LogError($"BonusAndPrizeController CreateBonusAndPrize : {traceId}" + $"{ex}");
            }
        }

    }
}
