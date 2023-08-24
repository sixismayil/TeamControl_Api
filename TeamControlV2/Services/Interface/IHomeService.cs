using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.DTO.ResponseModels.Inner;

namespace TeamControlV2.Services.Interface
{
    public interface IHomeService
    {
        List<PROJECT_VIEW_MODEL> GetProjects(decimal count, ref int errorCode, ref string message, string traceId);

        STATISTICS_VIEW_MODEL GetStatistics(ref int errorCode, ref string message, string traceId);

        List<VACATION_VIEW_MODEL> GetVacations(decimal count, ref int errorCode, ref string message, string traceId);
    }
}
