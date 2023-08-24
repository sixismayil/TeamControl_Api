using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.DTO.ResponseModels.Inner;

namespace TeamControlV2.Services.Interface
{
    public interface ISqlService
    {
        string Customers(bool isCount, bool isExport, int limit, int skip, CUSTOMER_FILTER_VIEW_MODEL model);

        string Employees(bool isCount, bool isExport, int limit, int skip, EMPLOYEE_FILTER_VIEW_MODEL model);

        string Projects(bool isCount, bool isExport, int limit, int skip, PROJECT_FILTER_VIEW_MODEL model);

        string ProjectsHome(bool isCount, bool isExport, int limit, int skip);
        
        string Vacations(bool isCount, bool isExport, int limit, int skip, VACATION_FILTER_VIEW_MODEL model);

        string Salaries(bool isCount, bool isExport, int limit, int skip, SALARY_FILTER_VIEW_MODEL model);

        string TeamLeadersLookup();

        string Positions(bool isCount, bool isExport, int limit, int skip);

        string VacationReasons(bool isCount, bool isExport, int limit, int skip);

        string ProjectStatuses(bool isCount, bool isExport, int limit, int skip);
   
        string BonusesAndPrizes(bool isCount, bool isExport, int limit, int skip, BONUS_AND_PRIZE_FILTER_VIEW_MODEL model);

    }
}
