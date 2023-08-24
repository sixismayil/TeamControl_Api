using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
    public class ProfileService : IProfileService
    {

        private readonly IRepository<EMPLOYEE> _employees;
        private readonly IRepository<EMPLOYEE_TO_POSITION> _employee_positions;

        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly ISqlService _sqlService;

        public ProfileService(
            IRepository<EMPLOYEE> employees,
            IRepository<EMPLOYEE_TO_POSITION> employee_positions,
            IMapper mapper,
            ILoggerManager logger,
            ISqlService sqlService)
        {
            _employees = employees;
            _employee_positions = employee_positions;
            _mapper = mapper;
            _logger = logger;
            _sqlService = sqlService;
        }

        public PROFILE_VIEW_MODEL GetData(int currentUserId, ref int errorCode, ref string message, string traceId)
        {
            PROFILE_VIEW_MODEL result = new PROFILE_VIEW_MODEL();

            try
            {
                EMPLOYEE employee = _employees.AllQuery.FirstOrDefault(x => x.Id == currentUserId && x.IsActive == true);
                result = _mapper.Map<PROFILE_VIEW_MODEL>(employee);
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB get data error";
                _logger.LogError($"ProfileService GetData : {traceId}" + $"{ex}");
            }
            return result;
        }

        public void UpdateData(ProfilePayload profile, int currentUserId, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                EMPLOYEE oldData = _employees.AllQuery.AsNoTracking().FirstOrDefault(x => x.Id == currentUserId && x.IsActive == true);
                EMPLOYEE employeeTest = _employees.AllQuery.AsNoTracking().FirstOrDefault(x => x.Email == profile.Email && x.Id != currentUserId);

                if(employeeTest != null)
                {
                    errorCode = 1;
                    message = "Daxil etdiyiniz email başqa bir istifadəçiyə məxsusdur.";
                    return;
                }
                EMPLOYEE newData = _mapper.Map<EMPLOYEE>(profile);
                newData.Id = currentUserId;
                newData.CreatedAt = oldData.CreatedAt;
                newData.UpdatedAt = DateTime.Now;
                newData.CreatedBy = oldData.CreatedBy;
                newData.UpdatedBy = currentUserId;
                newData.Password = oldData.Password;
                newData.IsActive = true;
                _employees.Update(newData);
                _employees.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = ex.Message;
                _logger.LogError($"ProfileService UpdateData : {traceId}" + $"{ex}");
            }
        }
    }
}
