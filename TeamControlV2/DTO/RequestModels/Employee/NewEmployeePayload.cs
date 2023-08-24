using System.Collections.Generic;

namespace TeamControlV2.DTO.RequestModels.Employee
{
    public class NewEmployeePayload
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string RelativesPhoneNumber { get; set; }

        public string Address { get; set; }

        public string Biography { get; set; }

        public string Birthdate { get; set; }

        public string RecruitmentDate { get; set; }

        public bool IsAdmin { get; set;}

        public string Salary { get; set; }
        public List<int> Position { get; set; }

    }
}
