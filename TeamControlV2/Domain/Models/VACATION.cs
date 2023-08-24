using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.Domain.Models
{
    public class VACATION
    {
        [Column("ID")]
        public int Id { get; set; }

        [Column("START_DATE")]
        public DateTime StartDate { get; set; }

        [Column("END_DATE")]
        public DateTime EndDate { get; set; }

        [Column("REASON_ID")]
        public int VacationReasonId { get; set; }

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

        public virtual VACATION_REASON VacationReason { get; set; }

        public virtual EMPLOYEE Employee { get; set; }
    }
}
