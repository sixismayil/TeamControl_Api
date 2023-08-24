using TeamControlV2.Domain.Models;
using AutoMapper;
using System.Linq;
using TeamControlV2.DTO.ResponseModels;
using TeamControlV2.DTO.ResponseModels.Inner;
using TeamControlV2.DTO.RequestModels;
using TeamControlV2.DTO.RequestModels.Employee;

namespace TeamControlV2.Extensions
{
    public class MappingEntity : Profile
    {
        public MappingEntity()
        {

            CreateMap<CUSTOMER_VIEW_MODEL, CUSTOMER>().ReverseMap();
            CreateMap<CUSTOMER, CustomerPayload>().ReverseMap();
            CreateMap<EMPLOYEE, NewEmployeePayload>().ReverseMap();
            CreateMap<EMPLOYEE_VIEW_MODEL, EMPLOYEE>().ReverseMap();
            CreateMap<POSITION, PositionPayload>().ReverseMap();
            CreateMap<VACATION_REASON, VacationReasonPayload>().ReverseMap();
            CreateMap<VACATION, VacationPayload>().ReverseMap();
            CreateMap<PROJECT_STATUS, ProjectStatusPayload>().ReverseMap();
            CreateMap<PROJECT_VIEW_MODEL, PROJECT>().ReverseMap();
            CreateMap<PROJECT, ProjectPayload>().ReverseMap();
            CreateMap<EMPLOYEE, PROFILE_VIEW_MODEL>().ReverseMap();
            CreateMap<EMPLOYEE, ProfilePayload>().ReverseMap();
            CreateMap<SALARY, SalaryPayload>().ReverseMap();
            CreateMap<BONUS_AND_PRIZE, BonusAndPrizePayload>().ReverseMap();
            CreateMap<BONUS_AND_PRIZE_VIEW_MODEL, BONUS_AND_PRIZE>().ReverseMap();

        }
    }
}
