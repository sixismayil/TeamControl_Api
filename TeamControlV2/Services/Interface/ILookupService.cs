using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.Domain.Models;
using TeamControlV2.DTO.ResponseModels.Inner;

namespace TeamControlV2.Services.Interface
{
    public interface ILookupService
    {
        public IEnumerable<PROJECT_STATUS> GetStatus();

        public IEnumerable<PROJECT> GetProjects();

        public IEnumerable<POSITION> GetPositions();

        public IEnumerable<VACATION_REASON> GetVacationReasons();

        public IEnumerable<WORKER_VIEW_MODEL> GetEmployees();

        public IEnumerable<WORKER_VIEW_MODEL> GetCustomers();
    }
}
