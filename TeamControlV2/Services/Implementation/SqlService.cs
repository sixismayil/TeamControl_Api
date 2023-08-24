using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.DTO.ResponseModels.Inner;
using TeamControlV2.Services.Interface;

namespace TeamControlV2.Services.Implementation
{
    public class SqlService : ISqlService
    {
        public string Customers(bool isCount, bool isExport, int limit, int skip, CUSTOMER_FILTER_VIEW_MODEL model)
        {
            string result = "";

            string count = @"SELECT COUNT(*) as 'totalCount' ";
            string mainStart = String.Format(
                            @"DECLARE @Skip_val int
							SET @Skip_val = {0}
							DECLARE @Limit_val int
							SET @Limit_val = {1}
                            DECLARE @Fullname varchar(100)
							SET @Fullname = '{2}'
                            DECLARE @ProjectStatus int
							SET @ProjectStatus = '{3}'
                            DECLARE @ProjectId int
							SET @ProjectId = '{4}'
                            ",
                            skip,
                            limit,
                            model.Fullname,
                            model.ProjectStatusId,
                            model.ProjectId);

            string variables = @"SELECT
							cus.ID 'Id',
							cus.NAME 'Firstname',
							cus.SURNAME 'Lastname',
							cus.PHONE_NUMBER 'PhoneNumber',
							proj.PROJECTS 'Projects' ";
            string cases = "";
            string cases_2 = "";
            filterCustomers(ref cases, ref cases_2, model);

            string mainPart = String.Format(@"FROM
							CUSTOMER as cus
                            {0}
							LEFT JOIN
							(SELECT 
							cus.ID as 'ID',
							STRING_AGG( ISNULL(CONVERT(NVARCHAR(max),ps.ID), ' ') + '-' + p.NAME + '-' + ps.COLOR, ',') as 'PROJECTS'
							FROM
							CUSTOMER as cus
							LEFT JOIN
							CUSTOMER_TO_PROJECT as c2p
							on
							cus.ID = c2p.CUSTOMER_ID
							LEFT JOIN
							PROJECT as p
							on
							c2p.PROJECT_ID = p.ID
							LEFT JOIN
							PROJECT_STATUS as ps
							on
							p.STATUS_ID = ps.ID 
					        WHERE p.IS_ACTIVE = 1
							GROUP BY cus.ID) as proj
							on cus.ID = proj.ID  WHERE cus.IS_ACTIVE = 1 ", cases_2);
            string order = " ORDER BY cus.NAME ASC  ";
            //
            string end = @" OFFSET @skip_val ROWS FETCH NEXT @Limit_val ROWS ONLY";

            if (!isCount && !isExport)
            {
                result = mainStart + variables + mainPart + cases + order + end;
            }
            else if (isCount && !isExport)
            {
                result = mainStart + count + mainPart + cases;
            }
            else if (!isCount && isExport)
            {
                result = mainStart + variables + mainPart + cases + order;
            }

            return result;
        }

        public string Employees(bool isCount, bool isExport, int limit, int skip, EMPLOYEE_FILTER_VIEW_MODEL model)
        {
            string result = "WHERE emp.IS_ACTIVE = 1 ";
            string mainStart = String.Format(
                            @"DECLARE @Skip_val int
							SET @Skip_val = {0}
							DECLARE @Limit_val int
							SET @Limit_val = {1}
                            DECLARE @Fullname varchar(100)
							SET @Fullname = '{2}'
                            DECLARE @ProjectStatus int
							SET @ProjectStatus = '{3}'
                            DECLARE @ProjectId int
							SET @ProjectId = '{4}'
                            ",
                            skip,
                            limit,
                            model.Fullname,
                            model.ProjectStatusId,
                            model.ProjectId);

            string count = @"SELECT COUNT(*) as 'totalCount' ";
            string variables = @"SELECT 
							emp.ID,
							emp.NAME 'Firstname',
							emp.SURNAME 'Lastname',
							proj.PROJECTS 'Projects' ";
                            
            string cases = "";
            string cases_2 = "";
            filterEmployees(ref cases, ref cases_2, model);

            string mainPart = String.Format(@"FROM
                                EMPLOYEE as emp
                                {0}
                                LEFT JOIN
                                (	SELECT
                            emp.ID as 'ID',
 							STRING_AGG( ISNULL(CONVERT(NVARCHAR(max),ps.ID), ' ') + '-' + p.NAME + '-' + ps.COLOR, ',') as 'PROJECTS'
                            FROM
                                EMPLOYEE as emp
                            LEFT JOIN
                                PROJECT_TO_EMPLOYEE as e2p
                            on
                                emp.ID = e2p.EMP_ID
                            LEFT JOIN
                                PROJECT as p
                            on
                                e2p.PROJECT_ID = p.ID
                            LEFT JOIN
 							    PROJECT_STATUS as ps
 							on
 							p.STATUS_ID = ps.ID 
                             WHERE p.IS_ACTIVE = 1
                            GROUP BY emp.ID) as proj
                            on	emp.ID = proj.ID 
                            WHERE emp.IS_ACTIVE = 1", cases_2);

            string order = " ORDER BY emp.NAME ASC  ";

            string end = @"OFFSET @skip_val ROWS FETCH NEXT @Limit_val ROWS ONLY";

            if (!isCount && !isExport)
            {
                result = mainStart + variables + mainPart + cases + order + end;
            }
            else if (isCount && !isExport)
            {
                result = mainStart + count + mainPart + cases;
            }
            else if (!isCount && isExport)
            {
                result = mainStart + variables + mainPart + cases + order;
            }
            return result;
        }

        public string Projects(bool isCount, bool isExport, int limit, int skip, PROJECT_FILTER_VIEW_MODEL model)
        {
            string result = "";
            string mainStart = String.Format(
                            @" DECLARE @Skip_val int
							SET @Skip_val = {0}
							DECLARE @Limit_val int
							SET @Limit_val = {1}
                            DECLARE @ProjectId int
							SET @ProjectId = '{2}'
                            DECLARE @ProjectStatusId int
							SET @ProjectStatusId = '{3}'
                            DECLARE @TeamLeaderId int
							SET @TeamLeaderId = '{4}'
                            ",
                            skip,
                            limit,
                            model.ProjectId,
                            model.ProjectStatusId,
                            model.TeamLeaderId);
            string count = @" SELECT COUNT(*) as 'totalCount' ";
            string variables = @" SELECT 
							proj.ID 'ID',
							proj.NAME 'NAME',
							CONCAT(pstatus.NAME,'?', pstatus.COLOR) 'STATUS',
							CONCAT(emp.NAME,' ', emp.SURNAME) 'TEAM_LEADER' ";

            string cases = " WHERE proj.IS_ACTIVE = 1 ";

            filterProjects(ref cases, model);

            string mainPart = String.Format(@" FROM PROJECT as proj
							LEFT JOIN 
							PROJECT_STATUS as pstatus
							ON proj.STATUS_ID = pstatus.ID
							LEFT JOIN 
							EMPLOYEE as emp 
							ON proj.TEAM_LEADER_ID = emp.ID 
                            ");

            string order = " ORDER BY proj.ID DESC ";

            string end = @" OFFSET @skip_val ROWS FETCH NEXT @Limit_val ROWS ONLY";

            if (!isCount && !isExport)
            {
                result = mainStart + variables + mainPart + cases + order + end;
            }
            else if (isCount && !isExport)
            {
                result = mainStart + count + mainPart + cases;
            }
            else if (!isCount && isExport)
            {
                result = mainStart + variables + mainPart + cases + order;
            }
            return result;
        }

        public string Positions(bool isCount, bool isExport, int limit, int skip)
        {
            string result = "";

            string count = @" SELECT COUNT(*) as 'totalCount' ";

            string mainStart = String.Format(
                            @"DECLARE @Skip_val int
							SET @Skip_val = {0}
							DECLARE @Limit_val int
							SET @Limit_val = {1}
                            ",
                            skip,
                            limit);

            string variables = @" SELECT
                            pos.ID as 'Id',
                            pos.NAME as 'Name',
                            pos.[Key] as 'Key' ";

            string mainPart = @" FROM
							POSITION as pos 
                            WHERE pos.IS_ACTIVE = 1";

            string order = " ORDER BY pos.NAME ASC  ";
            string end = @" OFFSET @skip_val ROWS FETCH NEXT @Limit_val ROWS ONLY ";

            if (!isCount && !isExport)
            {
                result = mainStart + variables + mainPart +  order + end;
            }
            else if (isCount && !isExport)
            {
                result = mainStart + count + mainPart;
            }
            else if (!isCount && isExport)
            {
                result = mainStart + variables + mainPart + order;
            }

            return result;
        }

        public string ProjectsHome(bool isCount, bool isExport, int limit, int skip)
        {
            string result = "";

            string count = @" SELECT COUNT(*) as 'totalCount' ";

            string mainStart = String.Format(
                            @"DECLARE @Skip_val int
							SET @Skip_val = {0}
							DECLARE @Limit_val int
							SET @Limit_val = {1}
                            ",
                            skip,
                            limit);

            string variables = @" SELECT
                            pos.ID as 'Id',
                            pos.NAME as 'Name',
                            pos.[Key] as 'Key' ";

            string mainPart = @" FROM
							PROJECT as proj ";

            string order = " ORDER BY pos.NAME ASC  ";
            string end = @" OFFSET @skip_val ROWS FETCH NEXT @Limit_val ROWS ONLY ";

            if (!isCount && !isExport)
            {
                result = mainStart + variables + mainPart + order + end;
            }
            else if (isCount && !isExport)
            {
                result = mainStart + count + mainPart;
            }
            else if (!isCount && isExport)
            {
                result = mainStart + variables + mainPart + order;
            }

            return result;
        }

        public string TeamLeadersLookup()
        {
            string mainPart = @"SELECT DISTINCT
                            TEAM_LEADER_ID AS 'ID',
                            CONCAT(EMPLOYEE.NAME,' ', EMPLOYEE.SURNAME) AS 'NAME'
                            FROM
                            PROJECT
                            JOIN EMPLOYEE
                            ON TEAM_LEADER_ID = EMPLOYEE.ID 
                            WHERE EMPLOYEE.IS_ACTIVE = 1 AND PROJECT.IS_ACTIVE = 1
                            ORDER BY 'NAME' ASC ";
            return mainPart;
        }

        public string VacationReasons(bool isCount, bool isExport, int limit, int skip)
        {
            string result = "";

            string count = @" SELECT COUNT(*) as 'totalCount' ";

            string mainStart = String.Format(
                            @"DECLARE @Skip_val int
							SET @Skip_val = {0}
							DECLARE @Limit_val int
							SET @Limit_val = {1}
                            ",
                            skip,
                            limit);

            string variables = @" SELECT
                            vac_res.ID as 'Id',
                            vac_res.NAME as 'Name',
                            vac_res.[Key] as 'Key' ";

            string mainPart = @" FROM
							VACATION_REASON as vac_res 
                            WHERE vac_res.IS_ACTIVE = 1";

            string order = " ORDER BY vac_res.NAME ASC  ";
            string end = @" OFFSET @skip_val ROWS FETCH NEXT @Limit_val ROWS ONLY ";

            if (!isCount && !isExport)
            {
                result = mainStart + variables + mainPart + order + end;
            }
            else if (isCount && !isExport)
            {
                result = mainStart + count + mainPart;
            }
            else if (!isCount && isExport)
            {
                result = mainStart + variables + mainPart + order;
            }

            return result;
        }

        public string ProjectStatuses(bool isCount, bool isExport, int limit, int skip)
        {
            string result = "";

            string count = @" SELECT COUNT(*) as 'totalCount' ";

            string mainStart = String.Format(
                            @"DECLARE @Skip_val int
							SET @Skip_val = {0}
							DECLARE @Limit_val int
							SET @Limit_val = {1}
                            ",
                            skip,
                            limit);

            string variables = @" SELECT
                            proj_status.ID as 'Id',
                            proj_status.NAME as 'Name',
                            proj_status.[KEY] as 'Key',
                            proj_status.COLOR as 'Color' ";

            string mainPart = @" FROM
							PROJECT_STATUS as proj_status 
                            WHERE proj_status.IS_ACTIVE = 1";

            string order = " ORDER BY proj_status.NAME ASC  ";
            string end = @" OFFSET @skip_val ROWS FETCH NEXT @Limit_val ROWS ONLY ";

            if (!isCount && !isExport)
            {
                result = mainStart + variables + mainPart + order + end;
            }
            else if (isCount && !isExport)
            {
                result = mainStart + count + mainPart;
            }
            else if (!isCount && isExport)
            {
                result = mainStart + variables + mainPart + order;
            }

            return result;
        }
        
        public void filterEmployees(ref string cases,ref string cases_2,  EMPLOYEE_FILTER_VIEW_MODEL model)
        {
             if (model.Fullname != null && model.Fullname.Trim() != "")
            {
                cases += " AND ";
                cases += " UPPER(CONCAT_WS(' ', emp.NAME , emp.SURNAME )) LIKE '%' + UPPER(@Fullname) + '%' ";
            }

            if (model.ProjectStatusId != 0 || model.ProjectId != 0)
            {
                cases_2 += @" LEFT JOIN
							(SELECT 
							emp.ID as 'ID',
							p.STATUS_ID as 'PSD'
							FROM
							EMPLOYEE as emp
							LEFT JOIN
             						PROJECT_TO_EMPLOYEE as e2p
							on
							emp.ID = e2p.EMP_ID
							LEFT JOIN
							PROJECT as p
							on
							e2p.PROJECT_ID = p.ID
							LEFT JOIN
							PROJECT_STATUS as ps
							on
							p.STATUS_ID = ps.ID ";
                    cases += " AND ";


                if(model.ProjectStatusId != 0 && model.ProjectId != 0)
                {
                    cases_2 += @" where 
							p.STATUS_ID = @ProjectStatus
							AND 
							e2p.PROJECT_ID = @ProjectId 
							) as proj_st
							on emp.ID = proj_st.ID  ";
                }else if(model.ProjectStatusId != 0 && model.ProjectId == 0){
                    cases_2 += @" where 
							p.STATUS_ID = @ProjectStatus
							) as proj_st
							on emp.ID = proj_st.ID ";
                }
                else 
                {
                    cases_2 += @" where 
							e2p.PROJECT_ID = @ProjectId 
							) as proj_st
							on emp.ID = proj_st.ID ";
                }


                cases += " proj_st.PSD is not null ";
         }
        }

        public void filterCustomers(ref string cases, ref string cases_2, CUSTOMER_FILTER_VIEW_MODEL model)
        {
             if (model.Fullname != null && model.Fullname.Trim() != "")
            {
                cases += " AND ";
                cases += " UPPER(CONCAT_WS(' ', cus.NAME , cus.SURNAME )) LIKE '%' + UPPER(@Fullname) + '%' ";
            }

            if (model.ProjectStatusId != 0 || model.ProjectId != 0)
            {
                cases_2 += @" LEFT JOIN
							(SELECT 
							cus.ID as 'ID',
							p.STATUS_ID as 'PSD'
							FROM
							CUSTOMER as cus
							LEFT JOIN
							CUSTOMER_TO_PROJECT as c2p
							on
							cus.ID = c2p.CUSTOMER_ID
							LEFT JOIN
							PROJECT as p
							on
							c2p.PROJECT_ID = p.ID
							LEFT JOIN
							PROJECT_STATUS as ps
							on
							p.STATUS_ID = ps.ID ";
                    cases += " AND ";

                if(model.ProjectStatusId != 0 && model.ProjectId != 0)
                {
                    cases_2 += @" where 
							p.STATUS_ID = @ProjectStatus
							AND 
							c2p.PROJECT_ID = @ProjectId 
							) as proj_st
							on cus.ID = proj_st.ID ";
                }else if(model.ProjectStatusId != 0 && model.ProjectId == 0){
                    cases_2 += @" where 
							p.STATUS_ID = @ProjectStatus
							) as proj_st
							on cus.ID = proj_st.ID ";
                }
                else 
                {
                    cases_2 += @" where 
							c2p.PROJECT_ID = @ProjectId 
							) as proj_st
							on cus.ID = proj_st.ID ";
                }


                cases += "  proj_st.PSD is not null ";
              
            }
           
        }

        public void filterProjects(ref string cases, PROJECT_FILTER_VIEW_MODEL model)
        {
            if (model.ProjectId != 0)
            {
                cases += " AND ";
                cases += " proj.ID = @ProjectId  ";
            }

            if (model.ProjectStatusId != 0)
            {
                cases += " AND ";
                cases += " proj.STATUS_ID = @ProjectStatusId ";
            }

            if (model.TeamLeaderId != 0)
            {
                cases += " AND ";
                cases += " emp.ID = @TeamLeaderId ";
            }
        }

        public string Vacations(bool isCount, bool isExport, int limit, int skip, VACATION_FILTER_VIEW_MODEL model)
        {
            string result = "";
            string count = @"select COUNT(*) as totalCount";
            string mainStart = String.Format(
                            @" 	DECLARE @Skip_val INT 
	                            SET @Skip_val = {0}
	                            DECLARE @Limit_val INT 
	                            SET @Limit_val = {1}
	                            DECLARE @Employee VARCHAR ( 100 ) 
	                            SET @Employee = '{2}'
	                            DECLARE @Start_Date datetime2 
	                            SET @Start_Date = '{3}'
	                            DECLARE @End_Date datetime2 
	                            SET @End_Date = '{4}' 
	                            DECLARE @VacationReasonId INT 
	                            SET @VacationReasonId = {5} ",
                            skip,
                            limit,
                            model.Employee,
                            model.StartDate,
                            model.EndDate,
                            model.VacationReasonId
            );

            string variables = @" SELECT 
							vac.ID,
							CONCAT(emp.NAME, ' ', emp.SURNAME) 'EMPLOYEE',
							vac.START_DATE 'START_DATE',
							vac.END_DATE 'END_DATE',
							reason.NAME 'VACATION_REASON' ";

            string cases = " WHERE vac.IS_ACTIVE = 1 AND emp.IS_ACTIVE = 1 ";
            filterVacations(ref cases, model);


            string mainPart = @"
	                from VACATION as vac
                    left join EMPLOYEE as emp
                    on emp.ID = vac.EMP_ID
                    left join VACATION_REASON as reason
                    on reason.ID = vac.REASON_ID ";

            string order = " ORDER BY vac.ID DESC  ";

            string end = @"OFFSET @skip_val ROWS FETCH NEXT @Limit_val ROWS ONLY";

            if (!isCount && !isExport)
            {
                result = mainStart + variables + mainPart + cases + order + end;
            }
            else if (isCount && !isExport)
            {
                result = mainStart + count + mainPart + cases;
            }
            else if (!isCount && isExport)
            {
                result = mainStart + variables + mainPart + cases + order;
            }
            return result;
        }


        public void filterVacations(ref string cases, VACATION_FILTER_VIEW_MODEL model)
        {

            if (model.Employee != null && model.Employee.Trim() != "")
            {
                cases += " and ";
                cases += " UPPER(CONCAT_WS(' ', emp.NAME, emp.SURNAME)) LIKE '%' + UPPER(@Employee) + '%' ";
            }

            if (model.StartDate != null && model.EndDate != null)
            {
                cases += " and ";
                cases += " vac.START_DATE >= @Start_Date and vac.END_DATE <= @End_Date ";
            }

            if (model.VacationReasonId != 0)
            {
                cases += " and ";
                cases += " vac.REASON_ID = @VacationReasonId ";
            }

        }


        public string Salaries(bool isCount, bool isExport, int limit, int skip, SALARY_FILTER_VIEW_MODEL model)
        {
            string result = "";
            string count = @"select COUNT(*) as totalCount";
            string mainStart = String.Format(
                            @" 	DECLARE @Skip_val INT
                                SET @Skip_val = {0}
                                DECLARE @Limit_val INT
                                SET @Limit_val = {1}
                                DECLARE @Employee VARCHAR ( 100 )
                                SET @Employee = '{2}'
                                DECLARE @Date datetime2
                                SET @Date = '{3}' 
                                DECLARE @Salary INT
                                SET @Salary = '{4}'",
                            skip,
                            limit,
                            model.Employee,
                            model.Date,
                            model.Salary
            );

            string variables = @" SELECT 
            				sal.ID,
							CONCAT(emp.NAME, ' ', emp.SURNAME) as Employee,
							sal.EMP_ID 'EMP_ID',
							sal.DATE 'DATE',
							sal.AMOUNT 'AMOUNT',
                            sal.END_SALARY 'END_SALARY' ";

            string cases = " WHERE sal.IS_ACTIVE = 1 and  emp.IS_ACTIVE = 1 ";
            filterSalaries(ref cases, model);


            string mainPart = @"
	                from SALARY as sal
                    left join EMPLOYEE as emp
                    on emp.ID = sal.EMP_ID ";

            string order = " ORDER BY sal.ID DESC  ";

            string end = @"OFFSET @skip_val ROWS FETCH NEXT @Limit_val ROWS ONLY";

            if (!isCount && !isExport)
            {
                result = mainStart + variables + mainPart + cases + order + end;
            }
            else if (isCount && !isExport)
            {
                result = mainStart + count + mainPart + cases;
            }
            else if (!isCount && isExport)
            {
                result = mainStart + variables + mainPart + cases + order;
            }
            return result;
        }

        public void filterSalaries(ref string cases, SALARY_FILTER_VIEW_MODEL model)
        {

            if (model.Employee != null && model.Employee.Trim() != "")
            {
                cases += " and ";
                cases += " UPPER(CONCAT_WS(' ', emp.NAME, emp.SURNAME)) LIKE '%' + UPPER(@Employee) + '%' ";
            }

            if (model.Date != null && model.Date != null)
            {
                cases += " and ";
                cases += " sal.DATE <= @Date ";
            }

            if (model.Salary != null && model.Salary.Trim() != "")
            {
                cases += " and ";
                cases += " sal.END_SALARY =  @Salary";
            }

        }


        public string BonusesAndPrizes(bool isCount, bool isExport, int limit, int skip, BONUS_AND_PRIZE_FILTER_VIEW_MODEL model)
        {
            string result = "";
            string count = @"select COUNT(*) as totalCount";
            string mainStart = String.Format(
                            @" 	DECLARE @Skip_val INT
	                            SET @Skip_val = {0}
	                            DECLARE @Limit_val INT
	                            SET @Limit_val = {1}
	                            DECLARE @Employee VARCHAR ( 100 )
	                            SET @Employee = '{2}'
	                            DECLARE @Date datetime2
	                            SET @Date = '{3}'
	                            DECLARE @Amount int
	                            SET @Amount = '{4}' ",
                            skip,
                            limit,
                            model.Employee,
                            model.Date,
                            model.Amount
            );

            string variables = @" SELECT 
							bap.ID,
							CONCAT(emp.NAME, ' ', emp.SURNAME) as Employee,
							bap.DATE 'Date',
							bap.AMOUNT 'Amount',
							bap.REASON 'Reason',
							CONCAT(proj.NAME,'?', pstatus.COLOR) 'Project',
							bap.IS_PRIZE 'IsPrize'
						 ";

            string cases = " WHERE bap.IS_ACTIVE = 1 and  emp.IS_ACTIVE = 1 ";

            filterBonusesAndPrizes(ref cases, model);


            string mainPart = @"
	                from BONUS_AND_PRIZE as bap
                    left join EMPLOYEE as emp
                    on emp.ID = bap.EMP_ID
					left join PROJECT as proj
                    on proj.ID = bap.PROJECT_ID
					left join 
					PROJECT_STATUS as pstatus
					ON proj.STATUS_ID = pstatus.ID ";

            string order = " ORDER BY bap.ID DESC  ";

            string end = @"OFFSET @skip_val ROWS FETCH NEXT @Limit_val ROWS ONLY";

            if (!isCount && !isExport)
            {
                result = mainStart + variables + mainPart + cases + order + end;
            }
            else if (isCount && !isExport)
            {
                result = mainStart + count + mainPart + cases;
            }
            else if (!isCount && isExport)
            {
                result = mainStart + variables + mainPart + cases + order;
            }
            return result;
        }



        public void filterBonusesAndPrizes(ref string cases, BONUS_AND_PRIZE_FILTER_VIEW_MODEL model)
        {
            if (model.Employee != null && model.Employee.Trim() != "")
            { cases += " and ";
              cases += " UPPER(CONCAT_WS(' ', emp.NAME, emp.SURNAME)) LIKE '%' + UPPER(@Employee) + '%'  ";
            }

            if (model.Date != null && model.Date != null)
            {
                cases += " and ";
                cases += " bap.DATE <= @Date ";
            }

            if (model.Amount != null && model.Amount.Trim() != "")
            {
                cases += " and ";
                cases += " bap.AMOUNT =  @Amount";
            }
        }
   
    
    
    }

}
