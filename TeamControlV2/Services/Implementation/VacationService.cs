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
    public class VacationService : IVacationService
    {
        AppConfiguration config = new AppConfiguration();
        private readonly IRepository<VACATION> _vacations;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly ISqlService _sqlService;
        public VacationService(
            IRepository<VACATION> vacations,
            IMapper mapper,
            ILoggerManager logger,
            ISqlService sqlService
            )
        {
            _vacations = vacations;
            _mapper = mapper;
            _logger = logger;
            _sqlService = sqlService;
        }

        public void CreateVacation(VacationPayload vacation, int currentUserId, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                VACATION vac = _mapper.Map<VACATION>(vacation);
                vac.CreatedBy = currentUserId;
                vac.CreatedAt = DateTime.Now;
                vac.UpdatedBy = null;
                vac.UpdatedAt = null;
                vac.IsActive = true;
                _vacations.Insert(vac);
                _vacations.Save();

            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB create vacation error";
                _logger.LogError($"VacationController CreateVacation : {traceId}" + $"{ex}");
            }
        }


        public void DeleteVacation(int id, int currentUserId, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                VACATION vacation = _vacations.AllQuery.FirstOrDefault(x => x.Id == id && x.IsActive == true);
                vacation.IsActive = false;
                vacation.UpdatedBy = currentUserId;
                vacation.UpdatedAt = DateTime.Now;
                _vacations.Update(vacation);
                _vacations.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB delete vacation error";
                _logger.LogError($"VacationController DeleteVacation : {traceId}" + $"{ex}");
            }
        }



        public VacationPayload GetVacation(int id, ref int errorCode, ref string message, string traceId)
        {
            VacationPayload result = new VacationPayload();
            try
            {
                // VACATION vacation = _vacations.AllQuery.FirstOrDefault(x => x.Id == id);
                VACATION vacation = _vacations.AllQuery.FirstOrDefault(x => x.Id == id && x.IsActive == true);
                result = _mapper.Map<VacationPayload>(vacation);
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB get vacation error";
                _logger.LogError($"VacationService GetVacation : {traceId}" + $"{ex}");
            }
            return result;
        }



        public List<VACATION_VIEW_MODEL> GetVacations(VACATION_FILTER_VIEW_MODEL model, int skip, int limit, ref decimal totalCount, bool isExport, ref int errorCode, ref string message, string traceId)
        {
            List<VACATION_VIEW_MODEL> response = new List<VACATION_VIEW_MODEL>();
            try
            {
                using (SqlConnection con = new SqlConnection(config.ConnectionString))
                {
                    SqlCommand cmd;
                    using (cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = _sqlService.Vacations(false, isExport, limit, skip, model);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            VACATION_VIEW_MODEL a = new VACATION_VIEW_MODEL()
                            {
                                Id = (int)rdr["ID"],
                                Employee = rdr["EMPLOYEE"].ToString(),
                                StartDate = rdr["START_DATE"].ToString(),
                                VacationReason = rdr["VACATION_REASON"].ToString(),
                                EndDate = rdr["END_DATE"].ToString(),
                                Period = ((DateTime)rdr["END_DATE"] - (DateTime)rdr["START_DATE"]).ToString(),
                            };
                            response.Add(a);
                        }
                        rdr.Close();
                        cmd = con.CreateCommand();
                        if (!isExport)
                        {
                            cmd.CommandText = _sqlService.Vacations(true, false, limit, skip, model);

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
                _logger.LogError($"VacationService GetVacations : {traceId}" + $"{ex}");
            }

            return response;
        }


        public void UpdateVacation(VacationPayload vacation, int id, int currentUserId, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                VACATION oldData = _vacations.AllQuery.AsNoTracking().FirstOrDefault(x => x.Id == id && x.IsActive == true);
                VACATION newData = _mapper.Map<VACATION>(vacation);
                newData.Id = id;
                newData.CreatedAt = oldData.CreatedAt;
                newData.UpdatedAt = DateTime.Now;
                newData.CreatedBy = oldData.CreatedBy;
                newData.UpdatedBy = currentUserId;
                newData.IsActive = true;
                _vacations.Update(newData);
                _vacations.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB update vacation error";
                _logger.LogError($"VacationController CreateVacation : {traceId}" + $"{ex}");
            }
        }

    }
}
