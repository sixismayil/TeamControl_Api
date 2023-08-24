using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.DTO.RequestModels;
using TeamControlV2.DTO.ResponseModels.Inner;

namespace TeamControlV2.Services.Interface
{
    public interface IVacationReasonService
    {
        void CreateVacationReason(VacationReasonPayload vacationReason, ref int errorCode, ref string message, string traceId);
        List<VACATION_REASON_VIEW_MODEL> GetVacationReasons(int skip, int limit, ref decimal totalCount, bool isExport, ref int errorCode, ref string message, string traceId);
        VacationReasonPayload GetVacationReason(int id, ref int errorCode, ref string message, string traceId);
        void UpdateVacationReason(VacationReasonPayload vacationReason, int id, ref int errorCode, ref string message, string traceId);
        void DeleteVacationReason(int id, ref int errorCode, ref bool reasonExists, ref string message, string traceId);
    }
}
