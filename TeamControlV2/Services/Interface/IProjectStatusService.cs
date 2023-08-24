using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.DTO.RequestModels;
using TeamControlV2.DTO.ResponseModels.Inner;

namespace TeamControlV2.Services.Interface
{
    public interface IProjectStatusService
    {
        void CreateProjectStatus(ProjectStatusPayload projectStatus, ref int errorCode, ref string message, string traceId);
        List<PROJECT_STATUS_VIEW_MODEL> GetProjectStatuses(int skip, int limit, ref decimal totalCount, bool isExport, ref int errorCode, ref string message, string traceId);
        ProjectStatusPayload GetProjectStatus(int id, ref int errorCode, ref string message, string traceId);
        void UpdateProjectStatus(ProjectStatusPayload projectStatus, int id, ref int errorCode, ref string message, string traceId);
        void DeleteProjectStatus(int id, ref int errorCode, ref bool statusExists, ref string message, string traceId);
    }
}
