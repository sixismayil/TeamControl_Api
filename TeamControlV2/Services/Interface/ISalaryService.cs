using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.DTO.RequestModels;
using TeamControlV2.DTO.ResponseModels.Inner;

namespace TeamControlV2.Services.Interface
{
    public interface ISalaryService
    {
        void CreateSalary(SalaryPayload salary,int currentUserId, ref int errorCode, ref string message, string traceId);

        List<SALARY_VIEW_MODEL> GetSalaries(SALARY_FILTER_VIEW_MODEL model, int skip, int limit, ref decimal totalCount, bool isExport, ref int errorCode, ref string message, string traceId);

        SalaryPayload GetSalary(int id, ref int errorCode, ref string message, string traceId);

        void UpdateSalary(SalaryPayload project, int currentUserId, int id, ref int errorCode, ref string message, string traceId);

        void DeleteSalary(int id, int currentUserId, ref int errorCode, ref string message, string traceId);

        int GetSalaryByEmployee (int id, ref int errorCode, ref string message, string traceId);

    }
}
