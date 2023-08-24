using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.DTO.ResponseModels.Inner;
using TeamControlV2.Services.Interface;

namespace TeamControlV2.Services.Implementation
{
    public class CmdService : ICmdService
    {
        public SqlCommand CustomersCmd(SqlCommand cmd, bool isSkip, CUSTOMER_FILTER_VIEW_MODEL model, int skip, int limit)
        {
            //cmd.Parameters.Add(new SqlParameter("Fullname", model.Fullname));
            //cmd.Parameters.Add(new SqlParameter("ProjectStatus", model.ProjectStatus));
            //cmd.Parameters.Add(new SqlParameter("ProjectName", model.ProjectName));

            //if (!isSkip)
            //{
            //    cmd.Parameters.Add(new SqlParameter("skip", skip));
            //    cmd.Parameters.Add(new SqlParameter("limit", limit));
            //}
            return cmd;
        }

        public SqlCommand EmployeesCmd(SqlCommand cmd, bool isSkip, EMPLOYEE_FILTER_VIEW_MODEL model, int skip, int limit)
        {
            cmd.Parameters.Add(new SqlParameter("Fullname", model.Fullname));
            cmd.Parameters.Add(new SqlParameter("ProjectId", model.ProjectId));
            cmd.Parameters.Add(new SqlParameter("ProjectStatusId", model.ProjectStatusId));

            if (!isSkip)
            {
            cmd.Parameters.Add(new SqlParameter("skip", skip));
            cmd.Parameters.Add(new SqlParameter("limit", limit));
            }
            
            return cmd;
        }

    }
}
