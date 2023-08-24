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
    public class ProjectStatusService : IProjectStatusService
    {
        AppConfiguration config = new AppConfiguration();
        private readonly IRepository<PROJECT_STATUS> _projectStatuses;
        private readonly IRepository<PROJECT> _projects;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly ISqlService _sqlService;

        public ProjectStatusService(
            IRepository<PROJECT_STATUS> projectStatuses,
            IRepository<PROJECT> projects,
            IMapper mapper,
            ILoggerManager logger,
            ISqlService sqlService
            )
        {
            _projects = projects;
            _projectStatuses = projectStatuses;
            _logger = logger;
            _mapper = mapper;
            _sqlService = sqlService;
        }

        public void CreateProjectStatus(ProjectStatusPayload projectStatus, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                PROJECT_STATUS status = _mapper.Map<PROJECT_STATUS>(projectStatus);
                status.IsActive = true;
                _projectStatuses.Insert(status);
                _projectStatuses.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB create project status error";
                _logger.LogError($"ProjectStatusService CreateProjectStatus : {traceId}" + $"{ex}");
            }
        }

        public void DeleteProjectStatus(int id, ref int errorCode, ref bool statusExists, ref string message, string traceId)
        {
            try
            {
                PROJECT_STATUS status = _projectStatuses.AllQuery.Where(x=> x.IsActive == true).FirstOrDefault(x => x.Id == id);
                PROJECT project = _projects.AllQuery.Where(x => x.IsActive == true).FirstOrDefault(x=> x.StatusId == status.Id);
                if(project == null)
                {
                    status.IsActive = false;
                    _projectStatuses.Update(status);
                    _projectStatuses.Save();

                }
                else
                {
                       errorCode = ErrorCode.OPERATION;
                       statusExists = true;
                       message = "Bu status hal-hazırda istifadədə olduğu üçün silinə bilməz.";
                }
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB delete project status error";
                _logger.LogError($"ProjectStatusService DeleteProjectStatus : {traceId}" + $"{ex}");
            }
        }

        public ProjectStatusPayload GetProjectStatus(int id, ref int errorCode, ref string message, string traceId)
        {
            ProjectStatusPayload result = new ProjectStatusPayload();
            try
            {
                PROJECT_STATUS status = _projectStatuses.AllQuery.Where(x=>x.IsActive == true).FirstOrDefault(x => x.Id == id);
                result = _mapper.Map<ProjectStatusPayload>(status);
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB get project status error";
                _logger.LogError($"ProjectStatusService GetProjectStatus : {traceId}" + $"{ex}");
            }
            return result;
        }

        public List<PROJECT_STATUS_VIEW_MODEL> GetProjectStatuses(int skip, int limit, ref decimal totalCount, bool isExport, ref int errorCode, ref string message, string traceId)
        {
            List<PROJECT_STATUS_VIEW_MODEL> response = new List<PROJECT_STATUS_VIEW_MODEL>();
            try
            {
                using (SqlConnection con = new SqlConnection(config.ConnectionString))
                {
                    SqlCommand cmd;
                    using (cmd = con.CreateCommand())
                    {
                        con.Open();

                        cmd.CommandText = _sqlService.ProjectStatuses(false, isExport, limit, skip);
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            PROJECT_STATUS_VIEW_MODEL project_status_view_model = new PROJECT_STATUS_VIEW_MODEL()
                            {
                                Id = (int)rdr["Id"],
                                Name = rdr["Name"].ToString(),
                                Key = rdr["Key"].ToString(),
                                Color = rdr["Color"].ToString()
                            };
                            response.Add(project_status_view_model);
                        }
                        rdr.Close();
                        cmd = con.CreateCommand();
                        if (!isExport)
                        {
                            cmd.CommandText = _sqlService.ProjectStatuses(true, false, limit, skip);

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
                message = "DB get project statuses error";
                _logger.LogError($"ProjectStatusService GetProjectStatuses : {traceId}" + $"{ex}");
            }
            return response;
        }

        public void UpdateProjectStatus(ProjectStatusPayload projectStatus, int id, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                PROJECT_STATUS oldData = _projectStatuses.AllQuery.AsNoTracking().FirstOrDefault(x => x.Id == id);
                PROJECT_STATUS newData = _mapper.Map<PROJECT_STATUS>(projectStatus);
                newData.Id = id;
                newData.IsActive = true;
                oldData = newData;
                _projectStatuses.Update(oldData);
                _projectStatuses.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB update project status error";
                _logger.LogError($"ProjectStatusService UpdateProjectStatus : {traceId}" + $"{ex}");
            }
        }
    }
}
