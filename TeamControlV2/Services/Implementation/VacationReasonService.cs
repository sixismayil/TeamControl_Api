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
    public class VacationReasonService : IVacationReasonService
    {
        AppConfiguration config = new AppConfiguration();
        private readonly IRepository<VACATION_REASON> _vacationReasons;
        private readonly IRepository<VACATION> _vacations;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly ISqlService _sqlService;
        public VacationReasonService(
            IRepository<VACATION_REASON> vacationReasons,
            IRepository<VACATION> vacations,
            IMapper mapper,
            ILoggerManager logger,
            ISqlService sqlService)
        {
            _vacationReasons = vacationReasons;
            _vacations = vacations;
            _logger = logger;
            _mapper = mapper;
            _sqlService = sqlService;
        }

        public void CreateVacationReason(VacationReasonPayload vacationReason, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                VACATION_REASON vac_reason = _mapper.Map<VACATION_REASON>(vacationReason);
                vac_reason.IsActive = true;
                _vacationReasons.Insert(vac_reason);
                _vacationReasons.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB create vacation reason error";
                _logger.LogError($"VacationReasonService CreateVacationReason : {traceId}" + $"{ex}");
            }
        }

        public void DeleteVacationReason(int id, ref int errorCode,ref bool reasonExists, ref string message, string traceId)
        {
            try
            {
                VACATION_REASON vac_reason = _vacationReasons.AllQuery.FirstOrDefault(x => x.Id == id);
                VACATION vacation = _vacations.AllQuery
                    .Where(x => x.IsActive == true)
                    .FirstOrDefault(x => x.VacationReasonId == vac_reason.Id);
                if(vacation == null)
                {
                    vac_reason.IsActive = false;
                    _vacationReasons.Update(vac_reason);
                    _vacationReasons.Save();
                }
                else
                {
                    errorCode = ErrorCode.OPERATION;
                    reasonExists = true;
                    message = "Bu məzuniyyət səbəbi hal-hazırda istifadədə olduğu üçün silinə bilməz.";
                }
                
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB delete vacation reason error";
                _logger.LogError($"VacationReasonService DeleteVacationReason : {traceId}" + $"{ex}");
            }
        }

        public VacationReasonPayload GetVacationReason(int id, ref int errorCode, ref string message, string traceId)
        {
            VacationReasonPayload result = new VacationReasonPayload();
            try
            {
                VACATION_REASON vac_reason = _vacationReasons.AllQuery.FirstOrDefault(x => x.Id == id);
                result = _mapper.Map<VacationReasonPayload>(vac_reason);
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB get vacation reason error";
                _logger.LogError($"VacationReasonService GetVacationReason : {traceId}" + $"{ex}");
            }
            return result;
        }

        public List<VACATION_REASON_VIEW_MODEL> GetVacationReasons(int skip, int limit, ref decimal totalCount, bool isExport, ref int errorCode, ref string message, string traceId)
        {
            List<VACATION_REASON_VIEW_MODEL> response = new List<VACATION_REASON_VIEW_MODEL>();

            try
            {
                using (SqlConnection con = new SqlConnection(config.ConnectionString))
                {
                    SqlCommand cmd;
                    using (cmd = con.CreateCommand())
                    {
                        con.Open();

                        cmd.CommandText = _sqlService.VacationReasons(false, isExport, limit, skip);
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            VACATION_REASON_VIEW_MODEL vac_res_view_model = new VACATION_REASON_VIEW_MODEL()
                            {
                                Id = (int)rdr["Id"],
                                Name = rdr["Name"].ToString(),
                                Key = rdr["Key"].ToString()
                            };
                            response.Add(vac_res_view_model);
                        }
                        rdr.Close();
                        cmd = con.CreateCommand();
                        if (!isExport)
                        {
                            cmd.CommandText = _sqlService.VacationReasons(true, false, limit, skip);

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
                message = "DB get position error";
                _logger.LogError($"PositionService GetPositions : {traceId}" + $"{ex}");
            }
            return response;
        }

        public void UpdateVacationReason(VacationReasonPayload vacationReason, int id, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                VACATION_REASON oldData = _vacationReasons.AllQuery.AsNoTracking().FirstOrDefault(x => x.Id == id);
                VACATION_REASON newData = _mapper.Map<VACATION_REASON>(vacationReason);
                newData.Id = id;
                newData.IsActive = true;
                oldData = newData;
                _vacationReasons.Update(oldData);
                _vacationReasons.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB update position error";
                _logger.LogError($"VacationReasonService UpdateVacationReason : {traceId}" + $"{ex}");
            }
        }
    }
}
