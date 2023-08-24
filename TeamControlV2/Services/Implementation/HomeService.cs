using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.Domain.Models;
using TeamControlV2.DTO.HelperModels.Const;
using TeamControlV2.DTO.ResponseModels.Inner;
using TeamControlV2.Infrastructure.Repository;
using TeamControlV2.Logging;
using TeamControlV2.Services.Interface;

namespace TeamControlV2.Services.Implementation
{
    public class HomeService:IHomeService
    {
        AppConfiguration config = new AppConfiguration();
        private readonly IRepository<PROJECT> _projects;
        private readonly IRepository<CUSTOMER> _customers;
        private readonly IRepository<EMPLOYEE> _employees;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly ISqlService _sqlService;

        public HomeService(
            IRepository<PROJECT> projects,
            IRepository<CUSTOMER> customers,
            IRepository<EMPLOYEE> employees,
            IMapper mapper,
            ILoggerManager logger,
            ISqlService sqlService
            )
        {
            _projects = projects;
            _logger = logger;
            _mapper = mapper;
            _sqlService = sqlService;
            _customers = customers;
            _employees = employees;
        }

        public List<PROJECT_VIEW_MODEL> GetProjects(decimal count, ref int errorCode, ref string message, string traceId)
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
                        cmd.CommandText = String.Format(@"SELECT
                            	TOP {0} proj.NAME,
                            	CONCAT( emp.NAME, ' ', emp.SURNAME ) AS TEAM_LEADER,
                            	CONCAT( pstatus.NAME, '?', pstatus.COLOR ) AS STATUS,
                            	pstatus.COLOR 'COLOR',
                            	proj.CREATED_AT 
                            FROM
                            	PROJECT AS proj
                            	LEFT JOIN EMPLOYEE AS emp ON proj.TEAM_LEADER_ID = emp.ID
                            	LEFT JOIN PROJECT_STATUS AS pstatus ON pstatus.ID = proj.STATUS_ID 
                                WHERE proj.IS_ACTIVE = 1
                            ORDER BY
                            	proj.CREATED_AT DESC", count);
                            
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            PROJECT_VIEW_MODEL a = new PROJECT_VIEW_MODEL()
                            {
                                Name = rdr["NAME"].ToString(),
                                TeamLeader = rdr["TEAM_LEADER"].ToString(),
                                Status = rdr["STATUS"].ToString()
                            };
                            response.Add(a);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB get projects error";
                _logger.LogError($"HomeService GetProjects : {traceId}" + $"{ex}");
            }
            return response;
        }

        public STATISTICS_VIEW_MODEL GetStatistics(ref int errorCode, ref string message, string traceId)
        {
            STATISTICS_VIEW_MODEL result = new STATISTICS_VIEW_MODEL();
            try
            {
                result.CustomerCount = _customers.AllQuery.Where(x=> x.IsActive == true).Count();
                result.EmployeeCount = _employees.AllQuery.Where(x=>x.IsActive == true).Count();
                result.ProjectCount = _projects.AllQuery.Where(x => x.IsActive == true).Count();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB get statistics error";
                _logger.LogError($"HomeService GetStatistics : {traceId}" + $"{ex}");
            }

            return result;
        }

        public List<VACATION_VIEW_MODEL> GetVacations(decimal count, ref int errorCode, ref string message, string traceId)
        {
            List<VACATION_VIEW_MODEL> response = new List<VACATION_VIEW_MODEL>();
            try
            {
                using (SqlConnection con = new SqlConnection(config.ConnectionString))
                {
                    SqlCommand cmd;
                    using (cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = String.Format(@"SELECT
                            	TOP {0} CONCAT( emp.NAME, ' ', emp.SURNAME ) AS EMPLOYEE,
                            	vac_res.NAME AS VAC_TYPE,
                            	vac.[END_DATE] AS D_END,
                            	vac.[START_DATE] AS START 
                            FROM
                            	VACATION AS vac
                            	LEFT JOIN EMPLOYEE AS emp ON vac.EMP_ID = emp.ID
                            	LEFT JOIN VACATION_REASON AS vac_res ON vac.REASON_ID = vac_res.ID 
                           WHERE vac.IS_ACTIVE = 1
                            ORDER BY
                            START DESC", count);
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            VACATION_VIEW_MODEL a = new VACATION_VIEW_MODEL()
                            {
                                Employee = rdr["EMPLOYEE"].ToString(),
                                VacationReason = rdr["VAC_TYPE"].ToString(),
                                EndDate = rdr["D_END"].ToString(),
                                StartDate = rdr["START"].ToString(),
                               

                            };
                            response.Add(a);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB get vacations error";
                _logger.LogError($"HomeService GetVacations : {traceId}" + $"{ex}");
            }
            return response;
        }
    }
}
