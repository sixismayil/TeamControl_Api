using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.DTO.RequestModels;
using TeamControlV2.DTO.ResponseModels.Inner;

namespace TeamControlV2.Services.Interface
{
    public interface IProjectService
    {
        void CreateProject(ProjectPayload project, int currentUserId, ref int errorCode, ref string message, string traceId);

        List<PROJECT_VIEW_MODEL> GetProjects(PROJECT_FILTER_VIEW_MODEL model, int skip, int limit, ref decimal totalCount, bool isExport, ref int errorCode, ref string message, string traceId);

        List<WORKER_VIEW_MODEL> GetTeamleaders(ref int errorCode, ref string message, string traceId);

        ProjectPayload GetProject(int id, ref int errorCode, ref string message, string traceId);

        void UpdateProject(ProjectPayload project, int id, int currentUserId, ref int errorCode, ref string message, string traceId);

        public void UpdatePeopleInProjects(PeopleToProjectPayload[] peopleToProjectPayload, int projectId, ref int errorCode, ref string message, string traceId);

        void AddPeopleToProject(PeopleToProjectPayload[] peopleToProjectPayload, int projectId, ref int errorCode, ref string message, string traceId);

        List<PROJECT_PARTICIPANT_VIEW_MODEL> GetProjectParticipants(int projectId, ref int errorCode, ref string message, string traceId);

        void DeleteProject(int id, int currentUserId, ref int errorCode, ref string message, string traceId);

        PROJECT_VIEW_MODEL GetGeneralInfo(int projectId, ref int errorCode, ref string message, string traceId);

        public List<CUSTOMER_TO_PROJECT_VIEW_MODEL> GetProjectCustomers(int projectId, ref int errorCode, ref string message, string traceId);

        public List<PROJECT_TO_EMPLOYEE_VIEW_MODEL> GetProjectEmployees(int projectId, ref int errorCode, ref string message, string traceId);

        public bool GetActiveStatus(int projectId, ref int errorCode, ref string message, string traceId);
    }
}
