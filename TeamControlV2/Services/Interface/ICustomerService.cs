using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.Domain.Models;
using TeamControlV2.DTO.HelperModels;
using TeamControlV2.DTO.RequestModels;
using TeamControlV2.DTO.ResponseModels.Inner;
using TeamControlV2.DTO.ResponseModels.Main;

namespace TeamControlV2.Services.Interface
{
    public interface ICustomerService
    {
        void CreateCustomer(CustomerPayload customer, int currentUserId, ref int errorCode, ref string message, string traceId);

        List<CUSTOMER_VIEW_MODEL> GetCustomers(CUSTOMER_FILTER_VIEW_MODEL model, int skip, int limit, ref decimal totalCount, bool isExport, ref int errorCode, ref string message, string traceId);

        CustomerPayload GetCustomer(int id, ref int errorCode, ref string message, string traceId);

        List<string> GetProjectsParticipated(int id, ref int errorCode, ref string message, string traceId);

        List<CUSTOMER_DETAIL_PROJECT_VIEW_MODEL> GetProjectsDetailed(int id, ref int errorCode, ref string message, string traceId);

        void UpdateCustomer(CustomerPayload customer, int id, int currentUserId, ref int errorCode, ref string message, string traceId);

        void DeleteCustomer(int id, int currentUserId, ref int errorCode, ref string message, string traceId);
    }
}
