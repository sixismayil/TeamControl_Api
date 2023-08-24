using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.Domain.Models
{
    public class CUSTOMER
    {
        [Column("ID")]
        public int Id { get; set; }

        [Column("NAME")]
        public string Name { get; set; }

        [Column("SURNAME")]
        public string Surname { get; set; }

        [Column("POSITION")]
        public string Position { get; set; }

        [Column("EMAIL")]
        public string Email { get; set; }

        [Column("PHONE_NUMBER")]
        public string PhoneNumber { get; set; }

        [Column("CREATED_AT")]
        public DateTime CreatedOn { get; set; }

        [Column("UPDATED_AT")]
        public DateTime? UpdatedOn { get; set; }

        [Column("CREATED_BY")]
        public int CreatedBy { get; set; }

        [Column("UPDATED_BY")]
        public int? UpdatedBy { get; set; }

        [Column("IS_ACTIVE")]
        public bool IsActive { get; set; }
    }
}
