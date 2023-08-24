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
    public class ProjectService : IProjectService
    {
        AppConfiguration config = new AppConfiguration();
        private readonly IRepository<PROJECT> _projects;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly ISqlService _sqlService;
        private readonly IRepository<PROJECT_TO_EMPLOYEE> _projectToEmployees;
        private readonly IRepository<CUSTOMER_TO_PROJECT> _customersToProject;
        public ProjectService(
            IRepository<PROJECT> projects,
            IMapper mapper,
            ILoggerManager logger,
            ISqlService sqlService,
            IRepository<PROJECT_TO_EMPLOYEE> projectToEmployees,
            IRepository<CUSTOMER_TO_PROJECT> customersToProject
            )
        {
            _projects = projects;
            _mapper = mapper;
            _logger = logger;
            _sqlService = sqlService;
            _projectToEmployees = projectToEmployees;
            _customersToProject = customersToProject;
        }

        public void CreateProject(ProjectPayload project, int currentUserId, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                PROJECT proj = _mapper.Map<PROJECT>(project);
                proj.CreatedBy = currentUserId;
                proj.CreatedAt = DateTime.Now;
                proj.UpdatedBy = null;
                proj.UpdatedBy = null;
                proj.IsActive = true;

                _projects.Insert(proj);
                _projects.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB create project error";
                _logger.LogError($"ProjectService CreateProject : {traceId}" + $"{ex}");
            }
        }

        public void DeleteProject(int id, int currentUserId, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                PROJECT project = _projects.AllQuery.FirstOrDefault(x => x.Id == id && x.IsActive == true);
                project.IsActive = false;
                project.UpdatedBy = currentUserId;
                project.UpdatedAt = DateTime.Now;
                _projects.Update(project);
                _projects.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB delete project error";
                _logger.LogError($"ProjectService DeleteProject : {traceId}" + $"{ex}");
            }
        }

        public ProjectPayload GetProject(int id, ref int errorCode, ref string message, string traceId)
        {
            ProjectPayload result = new ProjectPayload();
            try
            {
                PROJECT project = _projects.AllQuery.FirstOrDefault(x => x.Id == id);
                result = _mapper.Map<ProjectPayload>(project);
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB get project error";
                _logger.LogError($"ProjectService GetProject : {traceId}" + $"{ex}");
            }
            return result;
        }

        public List<PROJECT_VIEW_MODEL> GetProjects(PROJECT_FILTER_VIEW_MODEL model, int skip, int limit, ref decimal totalCount, bool isExport, ref int errorCode, ref string message, string traceId)
        {
            List<PROJECT_VIEW_MODEL> response = new List<PROJECT_VIEW_MODEL>();
            try
            {
                using (SqlConnection con = new SqlConnection(config.ConnectionString))
                {
                    SqlCommand cmd;
                    using (cmd = con.CreateCommand())
                    {
                        con.Open();

                        cmd.CommandText = _sqlService.Projects(false, isExport, limit, skip, model);
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            PROJECT_VIEW_MODEL a = new PROJECT_VIEW_MODEL()
                            {
                                Id = (int)rdr["ID"],
                                Name = rdr["NAME"].ToString(),
                                Status = rdr["STATUS"].ToString(),
                                TeamLeader = rdr["TEAM_LEADER"].ToString()
                            };
                            response.Add(a);
                        }
                        rdr.Close();
                        cmd = con.CreateCommand();
                        if (!isExport)
                        {
                            cmd.CommandText = _sqlService.Projects(true, false, limit, skip, model);

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
                message = "DB get projects error";
                _logger.LogError($"ProjectService GetProjects : {traceId}" + $"{ex}");
            }

            return response;
        }

        public List<WORKER_VIEW_MODEL> GetTeamleaders(ref int errorCode, ref string message, string traceId)
        {
            List<WORKER_VIEW_MODEL> response = new List<WORKER_VIEW_MODEL>();

            try
            {
                using (SqlConnection con = new SqlConnection(config.ConnectionString))
                {
                    SqlCommand cmd;
                    using (cmd = con.CreateCommand())
                    {
                        con.Open();

                        cmd.CommandText = _sqlService.TeamLeadersLookup();
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            WORKER_VIEW_MODEL a = new WORKER_VIEW_MODEL()
                            {
                                Id = (int)rdr["ID"],
                                Fullname = rdr["NAME"].ToString()
                            };

                            response.Add(a);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB get teamleaders error";
                _logger.LogError($"ProjectService GetTeamleaders : {traceId}" + $"{ex}");
            }

            return response;

        }

        public void UpdateProject(ProjectPayload project, int id, int currentUserId, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                PROJECT oldData = _projects.AllQuery.AsNoTracking().FirstOrDefault(x => x.Id == id);
                PROJECT newData = _mapper.Map<PROJECT>(project);
                newData.Id = id;
                newData.UpdatedAt = DateTime.Now;
                newData.UpdatedBy = currentUserId;
                newData.CreatedAt = oldData.CreatedAt;
                newData.CreatedBy = oldData.CreatedBy;
                newData.IsActive = true;

                _projects.Update(newData);
                _projects.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB update project error";
                _logger.LogError($"ProjectService UpdateProject : {traceId}" + $"{ex}");
            }
        }

        public void UpdatePeopleInProjects(PeopleToProjectPayload[] peopleToProjectPayload, int projectId, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                foreach (var element in _customersToProject.AllQuery.Where(x => x.ProjectId == projectId))
                {
                    _customersToProject.Remove(element);
                }

                foreach (var element in _projectToEmployees.AllQuery.Where(x => x.ProjectId == projectId))
                {
                    _projectToEmployees.Remove(element);
                }

                foreach (PeopleToProjectPayload data in peopleToProjectPayload)
                {
                    if(data.PersonType == 2) //it's a customer
                    {
                        CUSTOMER_TO_PROJECT newData = new CUSTOMER_TO_PROJECT()
                        {
                            CustomerId = (int)data.CustomerId,
                            ProjectId = projectId,
                            IsMain = (data.Role == 1) ? true : false,
                        };
                        _customersToProject.Insert(newData);
                    }
                    else // it's an employee
                    {
                        foreach(int positionId in data.Position)
                        {
                            PROJECT_TO_EMPLOYEE newData = new PROJECT_TO_EMPLOYEE()
                            {
                                EmployeeId = (int)data.EmployeeId,
                                ProjectId = projectId,
                                PositionId = positionId,
                                IsMain = (data.Role == 1) ? true : false
                            };
                            _projectToEmployees.Insert(newData);
                        }
                    }
                }
                _customersToProject.Save();
                _projectToEmployees.Save();
            }
            catch (Exception ex) 
            {
                errorCode = ErrorCode.DB;
                message = "DB update people in projects error";
                _logger.LogError($"ProjectService UpdatePeopleInProjects : {traceId}" + $"{ex}");
            }

        }

        public void AddPeopleToProject(PeopleToProjectPayload[] peopleToProjectPayload, int projectId, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                foreach(PeopleToProjectPayload data in peopleToProjectPayload)
                {
                    if (data.PersonType == 1) //it's an employee
                    {
                        foreach(int positionId in data.Position)
                        {
                            PROJECT_TO_EMPLOYEE emp = new PROJECT_TO_EMPLOYEE();
                            emp.EmployeeId = (int)data.EmployeeId;
                            emp.ProjectId = projectId;
                            emp.IsMain = data.Role == 1 ? true : false;
                            emp.PositionId = positionId;

                            if (_projectToEmployees.AllQuery.FirstOrDefault(x => x.EmployeeId == data.EmployeeId && x.ProjectId == projectId && x.PositionId == positionId) == null)
                            {
                                _projectToEmployees.Insert(emp);
                            }
                        }
                    }
                    else //it's a customer
                    {
                        CUSTOMER_TO_PROJECT cus = new CUSTOMER_TO_PROJECT();
                        cus.CustomerId = (int)data.CustomerId;
                        cus.ProjectId = projectId;
                        cus.IsMain= data.Role == 1 ? true : false;

                        if(_customersToProject.AllQuery.FirstOrDefault(x=>x.CustomerId == data.CustomerId && x.ProjectId == projectId) == null){
                            _customersToProject.Insert(cus);
                        }
                    }
                }
                _projectToEmployees.Save();
                _customersToProject.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB add people to project error";
                _logger.LogError($"ProjectService AddPeopleToProject : {traceId}" + $"{ex}");
            }
        }

        public List<PROJECT_PARTICIPANT_VIEW_MODEL> GetProjectParticipants(int projectId, ref int errorCode, ref string message, string traceId)
        {
            List<PROJECT_PARTICIPANT_VIEW_MODEL> response = new List<PROJECT_PARTICIPANT_VIEW_MODEL>();

            try
            {
                using (SqlConnection con = new SqlConnection(config.ConnectionString))
                {
                    SqlCommand cmd;
                    using (cmd = con.CreateCommand())
                    {
                        con.Open();

                        cmd.CommandText = String.Format(@"SELECT
                                                        	cus2p.CUSTOMER_ID,
                                                        	cus2p.IS_MAIN 
                                                        FROM
                                                        	CUSTOMER_TO_PROJECT AS cus2p
                                                        	LEFT JOIN CUSTOMER AS CUS ON cus2p.CUSTOMER_ID = cus.ID 
                                                        WHERE
                                                        	cus2p.PROJECT_ID = {0}
                                                        	AND cus.IS_ACTIVE = 1", projectId);
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            PROJECT_PARTICIPANT_VIEW_MODEL a = new PROJECT_PARTICIPANT_VIEW_MODEL()
                            {
                                PersonType=2,
                                Role=((bool)rdr["IS_MAIN"]) ? 1 : 0,
                                CustomerId=(int)rdr["CUSTOMER_ID"]
                            };

                            response.Add(a);
                        }
                        rdr.Close();
                        cmd = con.CreateCommand();
                        cmd.CommandText = String.Format(@"SELECT
                                                        	proj2emp.EMP_ID,
                                                        	STRING_AGG ( ISNULL( CONVERT ( NVARCHAR ( max ), proj2emp.POS_ID ), ' ' ), ',' ) AS POSITIONS,
                                                        	proj2emp.IS_MAIN 
                                                        FROM
                                                        	PROJECT_TO_EMPLOYEE AS proj2emp
                                                        	LEFT JOIN EMPLOYEE AS emp ON emp.ID = proj2emp.EMP_ID 
                                                        WHERE
                                                        	proj2emp.PROJECT_ID = {0} 
                                                        	AND emp.IS_ACTIVE = 1 
                                                        GROUP BY
                                                        	proj2emp.EMP_ID,
                                                        	proj2emp.IS_MAIN 
                                                        ORDER BY
                                                        	EMP_ID", projectId);

                        rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            PROJECT_PARTICIPANT_VIEW_MODEL b = new PROJECT_PARTICIPANT_VIEW_MODEL()
                            {
                                PersonType = 1,
                                Role = ((bool)rdr["IS_MAIN"]) ? 1 : 0,
                                EmployeeId = (int)rdr["EMP_ID"],
                                Position = rdr["POSITIONS"].ToString().Split(',').Select(n => Convert.ToInt32(n)).ToList()
                            };

                            response.Add(b);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB get project participants error";
                _logger.LogError($"ProjectService GetProjectParticipants : {traceId}" + $"{ex}");
            }

            return response;
        }

        public PROJECT_VIEW_MODEL GetGeneralInfo(int projectId, ref int errorCode, ref string message, string traceId)
        {
            PROJECT_VIEW_MODEL response = new PROJECT_VIEW_MODEL();
            try
            {
                using (SqlConnection con = new SqlConnection(config.ConnectionString))
                {
                    SqlCommand cmd;
                    using (cmd = con.CreateCommand())
                    {
                        con.Open();

                        cmd.CommandText = String.Format(@"SELECT
                          	proj.ID AS ID,
                          	proj.NAME AS NAME,
                          	CONCAT(pstatus.NAME, '?', pstatus.COLOR) AS STATUS,
                          	CONCAT(emp.NAME, ' ', emp.SURNAME) AS TEAM_LEADER 
                          FROM
                          	PROJECT AS proj
                          	LEFT JOIN EMPLOYEE AS emp ON proj.TEAM_LEADER_ID = emp.ID
                          	LEFT JOIN PROJECT_STATUS AS pstatus ON pstatus.ID = proj.STATUS_ID 
                          WHERE
                          	proj.ID = {0}", projectId);
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            response.Id = (int)rdr["ID"];
                            response.Name = rdr["NAME"].ToString();
                            response.Status = rdr["STATUS"].ToString();
                            response.TeamLeader = rdr["TEAM_LEADER"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB get general info error";
                _logger.LogError($"ProjectService GetGeneralInfo : {traceId}" + $"{ex}");
            }
            return response;
        }

        public List<CUSTOMER_TO_PROJECT_VIEW_MODEL> GetProjectCustomers(int projectId, ref int errorCode, ref string message, string traceId)
        {
            List<CUSTOMER_TO_PROJECT_VIEW_MODEL> result = new List<CUSTOMER_TO_PROJECT_VIEW_MODEL>();
            try
            {
                using (SqlConnection con = new SqlConnection(config.ConnectionString))
                {
                    SqlCommand cmd;
                    using (cmd = con.CreateCommand())
                    {
                        con.Open();

                        cmd.CommandText = String.Format(@"SELECT
	                            CONCAT(CUS.NAME, ' ', CUS.SURNAME) AS CUSTOMER,
	                            cus2p.IS_MAIN 
                            FROM
                            	CUSTOMER_TO_PROJECT AS CUS2P
                            	LEFT JOIN CUSTOMER AS CUS ON CUS2P.CUSTOMER_ID = CUS.ID 
                            WHERE
                            	CUS2P.PROJECT_ID = {0}", projectId);
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            CUSTOMER_TO_PROJECT_VIEW_MODEL a = new CUSTOMER_TO_PROJECT_VIEW_MODEL()
                            {
                                Fullname = rdr["CUSTOMER"].ToString(),
                                Role = (bool)rdr["IS_MAIN"]? "Əsas" : "Köməkçi"
                            };
                            result.Add(a);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = ex.Message;
                _logger.LogError($"ProjectService GetProjectCustomers : {traceId}" + $"{ex}");
            }
            return result;
        }

        public List<PROJECT_TO_EMPLOYEE_VIEW_MODEL> GetProjectEmployees(int projectId, ref int errorCode, ref string message, string traceId)
        {
            List<PROJECT_TO_EMPLOYEE_VIEW_MODEL> result = new List<PROJECT_TO_EMPLOYEE_VIEW_MODEL>();
            try
            {
                using (SqlConnection con = new SqlConnection(config.ConnectionString))
                {
                    SqlCommand cmd;
                    using (cmd = con.CreateCommand())
                    {
                        con.Open();

                        //cmd.CommandText = String.Format(@"SELECT
                        //        emp.ID,
                        //        CONCAT(emp.NAME,' ',emp.SURNAME) as EMP,
                        //        STRING_AGG( ISNULL(CONVERT(NVARCHAR(max),pos.NAME), ' '), ',') as POS,
                        //        p2emp.IS_MAIN
                        //        FROM PROJECT_TO_EMPLOYEE AS p2emp
                        //        LEFT JOIN EMPLOYEE AS emp
                        //        ON p2emp.EMP_ID = emp.ID
                        //        LEFT JOIN POSITION AS pos
                        //        ON pos.ID = p2emp.POS_ID
                        //        WHERE p2emp.PROJECT_ID = {0}
                        //        GROUP BY CONCAT(emp.NAME,' ',emp.SURNAME), emp.ID, p2emp.IS_MAIN", projectId);
                        cmd.CommandText = String.Format(@"SELECT
                                                        	emp.ID,
                                                        	CONCAT( emp.NAME, ' ', emp.SURNAME ) AS EMP,
                                                        	pos.NAME AS POS,
                                                        	p2emp.IS_MAIN 
                                                        FROM
                                                        	PROJECT_TO_EMPLOYEE AS p2emp
                                                        	LEFT JOIN EMPLOYEE AS emp ON p2emp.EMP_ID = emp.ID
                                                        	LEFT JOIN POSITION AS pos ON pos.ID = p2emp.POS_ID 
                                                        WHERE
                                                        	p2emp.PROJECT_ID = {0} AND emp.IS_ACTIVE = 1", projectId);
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            PROJECT_TO_EMPLOYEE_VIEW_MODEL a = new PROJECT_TO_EMPLOYEE_VIEW_MODEL()
                            {
                                Name = rdr["EMP"].ToString(),
                                Position = rdr["POS"].ToString(),
                                IsMain = (bool)rdr["IS_MAIN"]? 1 : 0
                            };
                            result.Add(a);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = ex.Message;
                _logger.LogError($"ProjectService GetProjectEmployees : {traceId}" + $"{ex}");
            }
            return result;
        }
    
        public bool GetActiveStatus(int projectId, ref int errorCode, ref string message, string traceId)
        {
            bool result = new bool();
            try
            {
                result = _projects.AllQuery.Where(x => x.Id == projectId).FirstOrDefault().IsActive;
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB get project active status error";
                _logger.LogError($"ProjectService GetActiveStatus : {traceId}" + $"{ex}");
            }
            return result;
        }
    }
}
