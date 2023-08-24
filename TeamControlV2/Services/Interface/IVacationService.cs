using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.DTO.RequestModels;
using TeamControlV2.DTO.ResponseModels.Inner;

namespace TeamControlV2.Services.Interface
{
    public interface IVacationService
    {
        void CreateVacation(VacationPayload vacation, int currentUserId, ref int errorCode, ref string message, string traceId);

        List<VACATION_VIEW_MODEL> GetVacations(VACATION_FILTER_VIEW_MODEL model, int skip, int limit, ref decimal totalCount, bool isExport, ref int errorCode, ref string message, string traceId);

        VacationPayload GetVacation(int id, ref int errorCode, ref string message, string traceId);

        void UpdateVacation(VacationPayload project, int currentUserId, int id, ref int errorCode, ref string message, string traceId);

        void DeleteVacation(int id, int currentUserId, ref int errorCode, ref string message, string traceId);
    }
}
