using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.Domain.Models;
using TeamControlV2.DTO.HelperModels;
using TeamControlV2.DTO.HelperModels.Const;
using TeamControlV2.DTO.RequestModels;
using TeamControlV2.DTO.ResponseModels.Inner;
using TeamControlV2.DTO.ResponseModels.Main;
using TeamControlV2.Extensions;
using TeamControlV2.Infrastructure.Repository;
using TeamControlV2.Logging;
using TeamControlV2.Services.Interface;

namespace TeamControlV2.Services.Implementation
{
    public class CustomerService : ICustomerService
    {
        AppConfiguration config = new AppConfiguration();
        private readonly IRepository<CUSTOMER> _customers;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly ISqlService _sqlService;
        public CustomerService(
            IRepository<CUSTOMER> customers,
            IMapper mapper,
            ILoggerManager logger,
            ISqlService sqlService
            )
        {
            _customers = customers;
            _mapper = mapper;
            _logger = logger;
            _sqlService = sqlService;
        }

        public void CreateCustomer(CustomerPayload customer, int currentUserId, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                CUSTOMER cus = _mapper.Map<CUSTOMER>(customer);
                cus.CreatedBy = currentUserId; 
                cus.CreatedOn = DateTime.Now;
                cus.UpdatedBy = null;
                cus.UpdatedOn = null;
                cus.IsActive = true;
                _customers.Insert(cus);
                _customers.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB create error";
                _logger.LogError($"CustomerService Create : {traceId}" + $"{ex}");
            }
        }

        public void DeleteCustomer(int id, int currentUserId, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                CUSTOMER customer = _customers.AllQuery.FirstOrDefault(x => x.Id == id && x.IsActive == true);
                customer.IsActive = false;
                customer.UpdatedBy = currentUserId;
                customer.UpdatedOn = DateTime.Now;
                _customers.Update(customer);
                _customers.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB delete customer error";
                _logger.LogError($"CustomerService DeleteCustomer : {traceId}" + $"{ex}");
            }
        }

        public CustomerPayload GetCustomer(int id, ref int errorCode, ref string message, string traceId)
        {
            CustomerPayload result = new CustomerPayload();
            
            try
            {
                CUSTOMER customer = _customers.AllQuery.FirstOrDefault(x => x.Id == id && x.IsActive == true);
                result = _mapper.Map<CustomerPayload>(customer);
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB get customer error";
                _logger.LogError($"CustomerService GetCustomer : {traceId}" + $"{ex}");
            }
            return result;
            //return customer;
        }

        public List<CUSTOMER_VIEW_MODEL> GetCustomers(CUSTOMER_FILTER_VIEW_MODEL model, int skip, int limit, ref decimal totalCount, bool isExport, ref int errorCode, ref string message, string traceId)
        {
            List<CUSTOMER_VIEW_MODEL> response = new List<CUSTOMER_VIEW_MODEL>();

            try
            {
                using (SqlConnection con = new SqlConnection(config.ConnectionString))
                {
                    SqlCommand cmd;
                    using (cmd = con.CreateCommand())
                    {
                        con.Open();

                        cmd.CommandText = _sqlService.Customers(false, isExport, limit, skip, model);
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            CUSTOMER_VIEW_MODEL a = new CUSTOMER_VIEW_MODEL()
                            {
                                Id = (int)rdr["Id"],
                                Firstname = rdr["Firstname"].ToString(),
                                Lastname = rdr["Lastname"].ToString(),
                                PhoneNumber = rdr["PhoneNumber"].ToString(),
                                Projects = rdr["Projects"].ToString()
                            };
                            response.Add(a);

                        }
                        rdr.Close();
                        cmd = con.CreateCommand();
                        if (!isExport)
                        {
                            cmd.CommandText = _sqlService.Customers(true, false, limit, skip, model);

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
                message = "DB get customers error";
                _logger.LogError($"CustomerService GetCustomers : {traceId}" + $"{ex}");
            }
            return response;
        }

        public void UpdateCustomer(CustomerPayload customer, int id, int currentUserId, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                CUSTOMER oldData = _customers.AllQuery.AsNoTracking().FirstOrDefault(x => x.Id == id && x.IsActive == true);
                CUSTOMER newData = _mapper.Map<CUSTOMER>(customer);
                newData.Id = id;
                newData.CreatedOn = oldData.CreatedOn;
                newData.CreatedBy = oldData.CreatedBy;
                newData.UpdatedBy = currentUserId;
                newData.UpdatedOn = DateTime.Now;
                newData.IsActive = true;
                
                _customers.Update(newData);
                _customers.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB update customer error";
                _logger.LogError($"CustomerService UpdateCustomer : {traceId}" + $"{ex}");
            }
        }

        public List<string> GetProjectsParticipated(int id, ref int errorCode, ref string message, string traceId)
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

                        cmd.CommandText = String.Format(@"DECLARE @CustomerId INT 
                                SET @CustomerId = {0} 
                                SELECT
                                CONCAT(proj.NAME, '?', pstatus.NAME, '?', pstatus.COLOR) 'PROJECT' 
                                FROM
                                	CUSTOMER_TO_PROJECT AS cus2p
                                	LEFT JOIN PROJECT AS proj ON cus2p.PROJECT_ID = proj.ID
                                	LEFT JOIN PROJECT_STATUS AS pstatus ON proj.STATUS_ID = pstatus.ID 
                                WHERE
                                	cus2p.CUSTOMER_ID = @CustomerId 
                                AND proj.IS_ACTIVE = 1
                                ORDER BY
                                	cus2p.PROJECT_ID DESC", id);
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
                message = "DB create error";
                _logger.LogError($"CustomerService GetProjectsParticipated : {traceId}" + $"{ex}");
            }
            return response;
        }

        public List<CUSTOMER_DETAIL_PROJECT_VIEW_MODEL> GetProjectsDetailed(int id, ref int errorCode, ref string message, string traceId)
        {
            List<CUSTOMER_DETAIL_PROJECT_VIEW_MODEL> response = new List<CUSTOMER_DETAIL_PROJECT_VIEW_MODEL>();
            try
            {
                using (SqlConnection con = new SqlConnection(config.ConnectionString))
                {
                    SqlCommand cmd;
                    using (cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = String.Format(@"DECLARE @CustomerId INT 
                                SET @CustomerId = {0} SELECT
                                proj.NAME 'PROJECT',
                                CONCAT(pstatus.NAME,'?',pstatus.COLOR) 'STATUS',
                                CONCAT( emp.NAME, ' ', emp.SURNAME ) 'TEAM_LEADER',
                                cus2p.IS_MAIN 'ROLE' 
                                FROM
                                	CUSTOMER_TO_PROJECT AS cus2p
                                	LEFT JOIN PROJECT AS proj ON cus2p.PROJECT_ID = proj.ID
                                	LEFT JOIN PROJECT_STATUS AS pstatus ON proj.STATUS_ID = pstatus.ID
                                	LEFT JOIN EMPLOYEE AS emp ON emp.ID = proj.TEAM_LEADER_ID 
                                WHERE
                                	cus2p.CUSTOMER_ID = @CustomerId  
								AND 
                                    proj.IS_ACTIVE = 1
                                ORDER BY
                                	cus2p.PROJECT_ID DESC", id);
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            CUSTOMER_DETAIL_PROJECT_VIEW_MODEL a = new CUSTOMER_DETAIL_PROJECT_VIEW_MODEL
                            {
                                Project = rdr["PROJECT"].ToString(),
                                Status = rdr["STATUS"].ToString(),
                                TeamLeader = rdr["TEAM_LEADER"].ToString(),
                                Role = (bool)rdr["ROLE"] == true ? "Əsas" : "Köməkçi"
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
    }
}
