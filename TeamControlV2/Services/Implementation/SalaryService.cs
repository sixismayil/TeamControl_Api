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
    public class SalaryService : ISalaryService
    {
        
        AppConfiguration config = new AppConfiguration();
        private readonly IRepository<SALARY> _salaries;
        private readonly IRepository<EMPLOYEE> _employees;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly ISqlService _sqlService;
        public SalaryService(
            IRepository<SALARY> salaries,
            IRepository<EMPLOYEE> employees,
            IMapper mapper,
            ILoggerManager logger,
            ISqlService sqlService
            )
        {
            _salaries = salaries;
            _employees = employees;
            _mapper = mapper;
            _logger = logger;
            _sqlService = sqlService;
        }


        public void CreateSalary(SalaryPayload salary, int currentUserId, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                SALARY sal = _mapper.Map<SALARY>(salary);
                EMPLOYEE employee = _employees.AllQuery.FirstOrDefault(x => x.Id == salary.EmployeeId);
                sal.CreatedBy = currentUserId;
                sal.CreatedAt = DateTime.Now;
                sal.UpdatedBy = null;
                sal.UpdatedAt = null;
                sal.IsActive = true;
                sal.EndSalary = employee.Salary + sal.Amount;
                employee.Salary = employee.Salary + sal.Amount;
                _salaries.Insert(sal);
                _salaries.Save();
                _employees.Update(employee);
                _employees.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB create salary error";
                _logger.LogError($"SalaryController CreateSalary : {traceId}" + $"{ex}");
            }
        }


        public void DeleteSalary(int id, int currentUserId, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                SALARY salary = _salaries.AllQuery.FirstOrDefault(x => x.Id == id && x.IsActive == true);
                EMPLOYEE employee = _employees.AllQuery.FirstOrDefault(x=>x.Id == salary.EmployeeId);
                employee.Salary = salary.EndSalary - salary.Amount;
                salary.IsActive = false;
                salary.UpdatedBy = currentUserId;
                salary.UpdatedAt = DateTime.Now;
                _salaries.Update(salary);
                _salaries.Save();
                _employees.Update(employee);
                _employees.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB delete salary error";
                _logger.LogError($"SalaryController DeleteSalary : {traceId}" + $"{ex}");
            }
        }

        public SalaryPayload GetSalary(int id, ref int errorCode, ref string message, string traceId)
        {
            SalaryPayload result = new SalaryPayload();
            try
            {
                SALARY salary = _salaries.AllQuery.FirstOrDefault(x => x.Id == id && x.IsActive == true);
                result = _mapper.Map<SalaryPayload>(salary);
                result.Salary = _salaries.AllQuery.FirstOrDefault(x => x.Id == id && x.IsActive == true).EndSalary;

            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB get salary error";
                _logger.LogError($"SalaryService GetSalary : {traceId}" + $"{ex}");
            }
            return result;
        }


        public List<SALARY_VIEW_MODEL> GetSalaries(SALARY_FILTER_VIEW_MODEL model, int skip, int limit, ref decimal totalCount, bool isExport, ref int errorCode, ref string message, string traceId)
        {
            List<SALARY_VIEW_MODEL> response = new List<SALARY_VIEW_MODEL>();
            try
            {
                using (SqlConnection con = new SqlConnection(config.ConnectionString))
                {
                    SqlCommand cmd;
                    using (cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = _sqlService.Salaries(false, isExport, limit, skip, model);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            SALARY_VIEW_MODEL a = new SALARY_VIEW_MODEL()
                            {
                                Id = (int)rdr["ID"],
                                Employee = rdr["EMPLOYEE"].ToString(),
                                Date = rdr["DATE"].ToString(),
                                Amount = rdr["AMOUNT"].ToString(),
                                Salary = rdr["END_SALARY"].ToString(),
                                IsEdittable = ((int)rdr["ID"]== _salaries.AllQuery.Where(x => x.EmployeeId == (int)rdr["EMP_ID"] && x.IsActive==true).Max(x => x.Id))?true:false
                            };
                            response.Add(a);
                        }
                        rdr.Close();
                        cmd = con.CreateCommand();
                        if (!isExport)
                        {
                            cmd.CommandText = _sqlService.Salaries(true, false, limit, skip, model);

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
                message = ex.Message;
                _logger.LogError($"SalaryService GetSalaries : {traceId}" + $"{ex}");
            }

            return response;
        }

        public void UpdateSalary(SalaryPayload salary, int id, int currentUserId,  ref int errorCode, ref string message, string traceId)
        {
            try
            {
                SALARY oldData = _salaries.AllQuery.AsNoTracking().FirstOrDefault(x => x.Id == id && x.IsActive == true);
                EMPLOYEE employee = _employees.AllQuery.FirstOrDefault(x => x.Id == salary.EmployeeId);
                SALARY newData = _mapper.Map<SALARY>(salary);
                newData.Id = id;
                newData.CreatedAt = oldData.CreatedAt;
                newData.UpdatedAt = DateTime.Now;
                newData.CreatedBy = oldData.CreatedBy;
                newData.UpdatedBy = currentUserId;
                newData.IsActive = true;
                newData.EndSalary = oldData.EndSalary - oldData.Amount + salary.Amount;
                employee.Salary = newData.EndSalary;
                _employees.Update(employee);
                _employees.Save();
                _salaries.Update(newData);
                _salaries.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB update salary error";
                _logger.LogError($"SalaryController CreateSalary : {traceId}" + $"{ex}");
            }
        }

        public int GetSalaryByEmployee(int id, ref int errorCode, ref string message, string traceID) 
        {
            int result = new int();
            try
            {
                EMPLOYEE employee = _employees.AllQuery.FirstOrDefault(x => x.Id == id);
                result = (int)employee.Salary;
            }
            catch (Exception ex)
            {

                errorCode = ErrorCode.DB;
                message = "DB get salary by employee error";
                _logger.LogError($"SalaryService GetSalaryByEmployee: {traceID}" + $"{ex}");

            }
            return result;
        }

    }
}
