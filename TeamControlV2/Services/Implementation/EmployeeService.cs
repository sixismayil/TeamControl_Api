using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.Domain.Models;
using TeamControlV2.DTO.HelperModels;
using TeamControlV2.DTO.HelperModels.Const;
using TeamControlV2.DTO.RequestModels;
using TeamControlV2.DTO.RequestModels.Employee;
using TeamControlV2.DTO.ResponseModels.Inner;
using TeamControlV2.Infrastructure.Repository;
using TeamControlV2.Logging;
using TeamControlV2.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace TeamControlV2.Services.Implementation
{
    public class EmployeeService : IEmployeeService
    {
        AppConfiguration config = new AppConfiguration();
        private readonly IRepository<EMPLOYEE> _employees;
        private readonly IRepository<EMPLOYEE_TO_POSITION> _employee_positions;
        private readonly IRepository<PROJECT_TO_EMPLOYEE> _employee_projects;

        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly ISqlService _sqlService;

        public EmployeeService(
            IRepository<EMPLOYEE> employees,
            IRepository<EMPLOYEE_TO_POSITION> employee_positions,
            IRepository<PROJECT_TO_EMPLOYEE> employee_projects,
            IMapper mapper,
            ILoggerManager logger,
            ISqlService sqlService
            )
        {
            _employees = employees;
            _employee_positions = employee_positions;
            _employee_projects = employee_projects;
            _mapper = mapper;
            _logger = logger;
            _sqlService = sqlService;

        }

        public void CreateEmployee(NewEmployeePayload employee, int currentUserId, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                EMPLOYEE emp = _mapper.Map<EMPLOYEE>(employee);
                emp.CreatedBy = currentUserId;
                emp.CreatedAt = DateTime.Now;
                emp.UpdatedAt = null;
                emp.UpdatedBy = null;
                emp.IsActive = true;
                emp.Password = BCrypt.Net.BCrypt.HashPassword("Aa123456");
                _employees.Insert(emp);
                _employees.Save();

                foreach (var i in employee.Position)
                {
                    EMPLOYEE_TO_POSITION ep = new EMPLOYEE_TO_POSITION();
                    ep.PositionId = i;
                    ep.EmployeeId = emp.Id;
                    _employee_positions.Insert(ep);
                }
                _employee_positions.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB create error";
                _logger.LogError($"EmployeeController Create : {traceId}" + $"{ex}");
            }
        }


        public void ResetPassword(int id, int currentUserId, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                EMPLOYEE emp = _employees.AllQuery.FirstOrDefault(x => x.Id == id && x.IsActive == true);
                emp.Password = BCrypt.Net.BCrypt.HashPassword("Aa123456");
                _employees.Update(emp);
                _employees.Save();

            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB reset password error";
                _logger.LogError($"EmployeeController ResetPassword : {traceId}" + $"{ex}");
            }
        }


        public void DeleteEmployee(int id, int currentUserId, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                // var emp_pos = _employee_positions.AllQuery.Where(x => x.EmployeeId == id).ToList();
                // foreach (var i in emp_pos)
                // {
                //     _employee_positions.Remove(i);
                // }
                // _employee_positions.Save();


                // var emp_proj = _employee_projects.AllQuery.Where(x => x.EmployeeId == id).ToList();
                // foreach (var i in emp_proj)
                // {
                //     _employee_projects.Remove(i);
                // }
                // _employee_projects.Save();


                EMPLOYEE employee = _employees.AllQuery.FirstOrDefault(x => x.Id == id && x.IsActive == true);
                employee.IsActive = false;
                employee.UpdatedBy = currentUserId;
                employee.UpdatedAt = DateTime.Now;
                _employees.Update(employee);
                _employees.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB delete employee error";
                _logger.LogError($"EmployeeController DeleteEmployee : {traceId}" + $"{ex}");
            }
        }

        public NewEmployeePayload GetEmployee(int id, ref int errorCode, ref string message, string traceId)
        {
            NewEmployeePayload result = new NewEmployeePayload();

            try
            {
                EMPLOYEE employee = _employees.AllQuery.FirstOrDefault(x => x.Id == id && x.IsActive == true);
                result = _mapper.Map<NewEmployeePayload>(employee);
                var emp_pos = _employee_positions.AllQuery.Where(x => x.EmployeeId == id).ToList();
                result.Position = new List<int>();
                foreach (var i in emp_pos)
                {
                    result.Position.Add(i.PositionId);
                }

            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB get employee error";
                _logger.LogError($"EmployeeController GetEmployee : {traceId}" + $"{ex}");

            }
            return result;
        }

        public List<EMPLOYEE_VIEW_MODEL> GetEmployees(EMPLOYEE_FILTER_VIEW_MODEL model, int skip, int limit, ref decimal totalCount, bool isExport, ref int errorCode, ref string message, string traceId)
        {

            List<EMPLOYEE_VIEW_MODEL> response = new List<EMPLOYEE_VIEW_MODEL>();

            try
            {
                using (SqlConnection con = new SqlConnection(config.ConnectionString))
                {
                    SqlCommand cmd;
                    using (cmd = con.CreateCommand())
                    {
                        con.Open();

                        cmd.CommandText = _sqlService.Employees(false, isExport, limit, skip, model);
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            EMPLOYEE_VIEW_MODEL a = new EMPLOYEE_VIEW_MODEL()
                            {
                                Id = (int)rdr["Id"],
                                Firstname = rdr["Firstname"].ToString(),
                                Lastname = rdr["Lastname"].ToString(),
                                Projects = rdr["Projects"].ToString()
                            };
                            response.Add(a);

                        }
                        rdr.Close();
                        cmd = con.CreateCommand();
                        if (!isExport)
                        {
                            cmd.CommandText = _sqlService.Employees(true, false, limit, skip, model);

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
                message = "DB get employees error";
                _logger.LogError($"EmployeeService GetEmployeess : {traceId}" + $"{ex}");
            }
            return response;
        }

        public void UpdateEmployee(NewEmployeePayload employee, int id, int currentUserId, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                EMPLOYEE oldData = _employees.AllQuery.AsNoTracking().FirstOrDefault(x => x.Id == id && x.IsActive == true);
                EMPLOYEE newData = _mapper.Map<EMPLOYEE>(employee);
                newData.Id = id;
                newData.CreatedAt = oldData.CreatedAt;
                newData.UpdatedAt = DateTime.Now;
                newData.CreatedBy = oldData.CreatedBy;
                newData.UpdatedBy = currentUserId;
                newData.Password = oldData.Password;
                newData.IsActive = true;
                _employees.Update(newData);
                _employees.Save();

                var emp_pos = _employee_positions.AllQuery.Where(x => x.EmployeeId == id).ToList();
                foreach (var i in emp_pos)
                {
                    _employee_positions.Remove(i);
                }
                _employee_positions.Save();

                foreach (var i in employee.Position)
                {
                    EMPLOYEE_TO_POSITION ep = new EMPLOYEE_TO_POSITION();
                    ep.PositionId = i;
                    ep.EmployeeId = id;
                    _employee_positions.Insert(ep);
                }
                _employee_positions.Save();

            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = ex.Message;
                _logger.LogError($"EmployeeController UpdateEmployee : {traceId}" + $"{ex}");
            }
        }

        public void ChangePassword(int id, PasswordPayload passwordPayload, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                EMPLOYEE employee = _employees.AllQuery.FirstOrDefault(x => x.Id == id && x.IsActive == true);
                bool verified = BCrypt.Net.BCrypt.Verify(passwordPayload.CurrentPassword, employee.Password);
                if (verified)
                {
                    employee.Password = BCrypt.Net.BCrypt.HashPassword(passwordPayload.Password);
                    _employees.Update(employee);
                    _employees.Save();
                }
                else
                {
                    errorCode = 1;
                    message = "Daxil edilmiş şifrə yanlışdır.";
                    return;
                }

            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB change password error";
                _logger.LogError($"EmployeeController Create : {traceId}" + $"{ex}");
            }
        }


        public List<string> getProjectsEmployeeParticipated(int id, ref int errorCode, ref string message, string traceId)
        {
            List<string> response = new List<string>();
            try
            {
                using (SqlConnection con = new SqlConnection(config.ConnectionString))
                {
                    SqlCommand cmd;
                    using (cmd = con.CreateCommand())
                    {
                        con.Open();

                        cmd.CommandText = String.Format(@"DECLARE @EmployeeId INT 
                                SET @EmployeeId = {0} 
                                SELECT
                                CONCAT(proj.NAME, '?', pstatus.NAME, '?', pstatus.COLOR,'?', proj.ID) 'PROJECT' 
                                FROM
                                	PROJECT_TO_EMPLOYEE AS e2p
                                	LEFT JOIN PROJECT AS proj ON e2p.PROJECT_ID = proj.ID
                                	LEFT JOIN PROJECT_STATUS AS pstatus ON proj.STATUS_ID = pstatus.ID 
                                WHERE
                                	e2p.EMP_ID = @EmployeeId 
                                AND proj.IS_ACTIVE = 1
                                ORDER BY
                                	e2p.PROJECT_ID DESC", id);
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            string a = rdr["PROJECT"].ToString();
                            response.Add(a);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB EmployeeParticipated error";
                _logger.LogError($"EmployeeService GetProjectsEmployeeParticipated : {traceId}" + $"{ex}");
            }
            return response;
        }


        public List<EMPLOYEE_DETAIL_SALARY_VIEW_MODEL> GetSalariesDetailed(int id, ref int errorCode, ref string message, string traceId)
        {
            List<EMPLOYEE_DETAIL_SALARY_VIEW_MODEL> response = new List<EMPLOYEE_DETAIL_SALARY_VIEW_MODEL>();
            try
            {
                using (SqlConnection con = new SqlConnection(config.ConnectionString))
                {
                    SqlCommand cmd;
                    using (cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = String.Format(@" DECLARE @EmployeeId INT 
                                SET @EmployeeId = {0} SELECT
                                sal.DATE 'DATE',
								sal.AMOUNT 'AMOUNT',
								sal.END_SALARY 'END_SALARY'
                                FROM
                                SALARY AS sal
                                LEFT JOIN EMPLOYEE AS emp ON sal.EMP_ID = emp.ID
                                WHERE
                                sal.EMP_ID = @EmployeeId  
								AND 
                                emp.IS_ACTIVE = 1
                                AND
                                sal.IS_ACTIVE = 1
                                ORDER BY
                                sal.CREATED_AT DESC ", id);
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            EMPLOYEE_DETAIL_SALARY_VIEW_MODEL a = new EMPLOYEE_DETAIL_SALARY_VIEW_MODEL
                            {
                                Date = rdr["DATE"].ToString(),
                                Amount = rdr["AMOUNT"].ToString(),
                                Salary = rdr["END_SALARY"].ToString(),
                            };
                            response.Add(a);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB create error";
                _logger.LogError($"CustomerService GetProjectsParticipated : {traceId}" + $"{ex}");
            }
            return response;
        }



        public List<EMPLOYEE_DETAIL_BONUS_VIEW_MODEL> GetBonusesDetailed(int id, ref int errorCode, ref string message, string traceId)
        {
            List<EMPLOYEE_DETAIL_BONUS_VIEW_MODEL> response = new List<EMPLOYEE_DETAIL_BONUS_VIEW_MODEL>();
            try
            {
                using (SqlConnection con = new SqlConnection(config.ConnectionString))
                {
                    SqlCommand cmd;
                    using (cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = String.Format(@"
                                    DECLARE @EmployeeId INT 
                                    SET @EmployeeId = {0} SELECT
                                    bon.DATE 'DATE',
                                    bon.AMOUNT 'AMOUNT',
							        CONCAT(proj.NAME,'?', pstatus.COLOR) 'PROJECT'
                                    FROM 
                                    BONUS_AND_PRIZE as bon
                                    LEFT JOIN EMPLOYEE as emp ON bon.EMP_ID = emp.ID
									left join PROJECT as proj
                                    on proj.ID = bon.PROJECT_ID
							        left join 
							        PROJECT_STATUS as pstatus
							        ON proj.STATUS_ID = pstatus.ID
                                    WHERE 
                                    bon.EMP_ID = @EmployeeId
                                    AND 
                                    emp.IS_ACTIVE = 1 
                                    AND bon.IS_PRIZE = 0
                                    AND bon.IS_ACTIVE = 1
                                    
                                    ORDER BY 
                                    bon.CREATED_AT DESC ", id);
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            EMPLOYEE_DETAIL_BONUS_VIEW_MODEL a = new EMPLOYEE_DETAIL_BONUS_VIEW_MODEL
                            {
                                Date = rdr["DATE"].ToString(),
                                Amount = rdr["AMOUNT"].ToString(),
                                Project = rdr["PROJECT"].ToString(),
                            };
                            response.Add(a);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB bonuses lookup error";
                _logger.LogError($"EmployeeService GetBonusesDetailed : {traceId}" + $"{ex}");
            }
            return response;
        }


        public List<EMPLOYEE_DETAIL_PREMIUM_VIEW_MODEL> GetPremiumsDetailed(int id, ref int errorCode, ref string message, string traceId)
        {
            List<EMPLOYEE_DETAIL_PREMIUM_VIEW_MODEL> response = new List<EMPLOYEE_DETAIL_PREMIUM_VIEW_MODEL>();
            try
            {
                using (SqlConnection con = new SqlConnection(config.ConnectionString))
                {
                    SqlCommand cmd;
                    using(cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = String.Format(
                             @" DECLARE @EmployeeId INT 
                                SET @EmployeeId = {0} SELECT
                                bon.DATE 'DATE',
                                bon.AMOUNT 'AMOUNT',
                                bon.REASON 'REASON'
                                FROM 
                                BONUS_AND_PRIZE as bon
                                LEFT JOIN EMPLOYEE as emp ON bon.EMP_ID = emp.ID
                                WHERE 
                                bon.EMP_ID = @EmployeeId
                                AND 
                                emp.IS_ACTIVE = 1 
                                AND bon.IS_ACTIVE = 1
                                AND bon.IS_PRIZE = 1

                                ORDER BY 
                                bon.CREATED_AT DESC ", id);
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            EMPLOYEE_DETAIL_PREMIUM_VIEW_MODEL a = new EMPLOYEE_DETAIL_PREMIUM_VIEW_MODEL
                            {
                                Date = rdr["DATE"].ToString(),
                                Amount = rdr["AMOUNT"].ToString(),
                                Reason = rdr["REASON"].ToString(),
                            };
                            response.Add(a);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB; ;
                message = "DB premiums lookup error";
                _logger.LogError($"EmployeeService GetPremiumsDetailed : {traceId}" + $"{ex}");
            }
            return response;
        }



        public List<EMPLOYEE_DETAIL_VACATION_VIEW_MODEL> GetVacationsDetailed(int id, ref int errorCode, ref string message, string traceId)
        {
            List<EMPLOYEE_DETAIL_VACATION_VIEW_MODEL> response = new List<EMPLOYEE_DETAIL_VACATION_VIEW_MODEL>();
            try
            {
                using (SqlConnection con = new SqlConnection(config.ConnectionString))
                {
                    SqlCommand cmd;
                    using(cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = String.Format(
                             @" DECLARE @EmployeeId INT
                                SET @EmployeeId = {0} SELECT
                                CONCAT(vac.START_DATE, ' - ', vac.END_DATE) as DATE,
                                 vac.START_DATE as START,
                                 vac.END_DATE as [END],
                                reason.NAME as REASON
                                from VACATION as vac
                                left join EMPLOYEE as emp
                                on emp.ID = vac.EMP_ID
                                left join VACATION_REASON as reason
                                on reason.ID = vac.REASON_ID

                                WHERE 
                                vac.EMP_ID = @EmployeeId
                                AND 
                                emp.IS_ACTIVE = 1 
                                AND vac.IS_ACTIVE = 1

                                ORDER BY 
                                vac.CREATED_AT DESC", id);
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            EMPLOYEE_DETAIL_VACATION_VIEW_MODEL a = new EMPLOYEE_DETAIL_VACATION_VIEW_MODEL
                            {
                                Reason = rdr["REASON"].ToString(),
                                Date = rdr["DATE"].ToString(),
                                StartDate = rdr["START"].ToString(),
                                EndDate = rdr["END"].ToString(),
                                Period = ((DateTime)rdr["END"] - (DateTime)rdr["START"]).ToString()
                            };
                            response.Add(a);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB; ;
                message = "DB vacations lookup error";
                _logger.LogError($"EmployeeService GetVacationsDetailed : {traceId}" + $"{ex}");
            }
            return response;
        }






    }
}
