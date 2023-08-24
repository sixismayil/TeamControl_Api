using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.Domain.Models;
using TeamControlV2.DTO.HelperModels;
using TeamControlV2.DTO.RequestModels;
using TeamControlV2.DTO.RequestModels.Employee;
using TeamControlV2.DTO.ResponseModels.Inner;


namespace TeamControlV2.Services.Interface
{
    public interface IEmployeeService
    {
        void CreateEmployee(NewEmployeePayload employee, int currentUserId, ref int errorCode, ref string message, string traceId);

        List<EMPLOYEE_VIEW_MODEL> GetEmployees(EMPLOYEE_FILTER_VIEW_MODEL model, int skip, int limit, ref decimal totalCount, bool isExport, ref int errorCode, ref string message, string traceId);

        NewEmployeePayload GetEmployee(int id, ref int errorCode, ref string message, string traceId);

        void UpdateEmployee(NewEmployeePayload employee, int id, int currentUserId, ref int errorCode, ref string message, string traceId);

        void ChangePassword(int id, PasswordPayload passwordPayload, ref int errorCode, ref string message, string traceId);
        void DeleteEmployee(int id, int currentUserId, ref int errorCode, ref string message, string traceId);

        List<string> getProjectsEmployeeParticipated(int id, ref int errorCode, ref string message, string traceId);

        List<EMPLOYEE_DETAIL_SALARY_VIEW_MODEL> GetSalariesDetailed(int id, ref int errorCode, ref string message, string traceId);

        List<EMPLOYEE_DETAIL_BONUS_VIEW_MODEL> GetBonusesDetailed(int id, ref int errorCode, ref string message, string traceId);

        List<EMPLOYEE_DETAIL_PREMIUM_VIEW_MODEL> GetPremiumsDetailed(int id, ref int errorCode, ref string message, string traceId);
        
        List<EMPLOYEE_DETAIL_VACATION_VIEW_MODEL> GetVacationsDetailed(int id, ref int errorCode, ref string message, string traceId);

        void ResetPassword(int id, int currentUserId, ref int errorCode, ref string message, string traceId);
    }
}
