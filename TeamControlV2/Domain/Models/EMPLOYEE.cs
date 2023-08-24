using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.Domain.Models
{
    public class EMPLOYEE
    {
        [Column("ID")]
        public int Id { get; set; }

        [Column("NAME")]
        public string Name { get; set; }

        [Column("SURNAME")]
        public string Surname { get; set; }

        [Column("EMAIL")]
        public string Email { get; set; }

        [Column("PHONE_NUMBER")]
        public string PhoneNumber { get; set; }

        [Column("RELATIVES_PHONE_NUMBER")]
        public string RelativesPhoneNumber { get; set; }

        [Column("ADDRESS")]
        public string Address { get; set; }

        [Column("BIOGRAPHY")]
        public string Biography { get; set; }

        [Column("BIRTH_DATE")]
        public DateTime Birthdate { get; set; }

        [Column("RECRUITMENT_DATE")]
        public DateTime RecruitmentDate { get; set; }

        [Column("SALARY")]
        public double Salary { get; set; }

        [Column("STATUS")]
        public bool Status { get; set; }

        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; }

        [Column("UPDATED_AT")]
        public DateTime? UpdatedAt { get; set; }

        [Column("CREATED_BY")]
        public int CreatedBy { get; set; }

        [Column("UPDATED_BY")]
        public int? UpdatedBy { get; set; }

        [Column("PASSWORD")]
        public string Password { get; set; }

        [Column("IS_ACTIVE")]
        public bool IsActive { get; set; }

        [Column("IS_ADMIN")]
        public bool IsAdmin { get; set; }
    }
}