using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.DTO.ResponseModels.Inner;

namespace TeamControlV2.Services.Interface
{
    public interface ICmdService
    {
        SqlCommand CustomersCmd(SqlCommand cmd, bool isSkip, CUSTOMER_FILTER_VIEW_MODEL model, int skip, int limit);
        SqlCommand EmployeesCmd(SqlCommand cmd, bool isSkip, EMPLOYEE_FILTER_VIEW_MODEL model, int skip, int limit);

    }
}
