using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.Domain.Models;
using TeamControlV2.DTO.ResponseModels.Inner;
using TeamControlV2.Infrastructure.Repository;
using TeamControlV2.Services.Interface;

namespace TeamControlV2.Services.Implementation
{
    public class LookupService : ILookupService
    {
        private readonly IRepository<PROJECT_STATUS> _statuses;
        private readonly IRepository<PROJECT> _projects;
        private readonly IRepository<POSITION> _positions;
        private readonly IRepository<VACATION_REASON> _vacationReasons;
        private readonly IRepository<EMPLOYEE> _employees;
        private readonly IRepository<CUSTOMER> _customers;

        public LookupService(
            IRepository<PROJECT_STATUS> statuses,
            IRepository<PROJECT> projects,
            IRepository<POSITION> positions,
            IRepository<VACATION_REASON> vacationReasons,
            IRepository<EMPLOYEE> employees,
            IRepository<CUSTOMER> customers
            )
        {
            _statuses = statuses;
            _projects = projects;
            _positions = positions;
            _vacationReasons = vacationReasons;
            _employees = employees;
            _customers = customers;
    }

        public IEnumerable<POSITION> GetPositions()
        {
            return _positions.AllQuery.Where(x=>x.IsActive == true);
        }
        
        public IEnumerable<PROJECT_STATUS> GetStatus()
        {
            return _statuses.AllQuery.Where(x=> x.IsActive == true).OrderBy(x=>x.Name);
        }
        
        public IEnumerable<PROJECT> GetProjects()
        {
            return _projects.AllQuery.Where(x=>x.IsActive == true).OrderBy(x=>x.Name);
        }

        public IEnumerable<VACATION_REASON> GetVacationReasons()
        {
            return _vacationReasons.AllQuery.Where(x => x.IsActive == true);
        }

        public IEnumerable<WORKER_VIEW_MODEL> GetEmployees()
        {
            List<WORKER_VIEW_MODEL> workerList = new List<WORKER_VIEW_MODEL>();
            foreach(var employee in _employees.AllQuery.Where(x=> x.IsActive == true ).OrderBy(x => x.Name)
           .ToList())
            {
                WORKER_VIEW_MODEL worker = new WORKER_VIEW_MODEL();
                worker.Id = employee.Id;
                worker.Fullname = $"{employee.Name} {employee.Surname}";
                workerList.Add(worker);
            }
            return workerList;
        }

        public IEnumerable<WORKER_VIEW_MODEL> GetCustomers()
        {
            List<WORKER_VIEW_MODEL> workerList = new List<WORKER_VIEW_MODEL>();
            foreach (var customer in _customers.AllQuery.Where(x=>x.IsActive==true).OrderBy(x => x.Name)
           .ToList())
            {
                WORKER_VIEW_MODEL worker = new WORKER_VIEW_MODEL();
                worker.Id = customer.Id;
                worker.Fullname = $"{customer.Name} {customer.Surname}";
                workerList.Add(worker);
            }
            return workerList;
        }
    }
}
