using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.Domain.Models
{
    public class BONUS_AND_PRIZE
    {
        [Column("ID")]
        public int Id { get; set; }

        [Column("AMOUNT")]
        public int Amount { get; set; }

        [Column("DATE")]
        public DateTime Date { get; set; }

        [Column("PROJECT_ID")]
        public int? ProjectId { get; set; }

        [Column("IS_PRIZE")]
        public bool IsPrize { get; set; }

        [Column("REASON")]
        public string Reason { get; set; }

        [Column("EMP_ID")]
        public int EmployeeId { get; set; }

        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; }

        [Column("UPDATED_AT")]
        public DateTime? UpdatedAt { get; set; }

        [Column("CREATED_BY")]
        public int CreatedBy { get; set; }

        [Column("UPDATED_BY")]
        public int? UpdatedBy { get; set; }

        [Column("IS_ACTIVE")]
        public bool IsActive { get; set; }

    }
}
