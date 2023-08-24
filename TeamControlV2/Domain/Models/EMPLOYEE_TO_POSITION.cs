using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.Domain.Models
{
    public class EMPLOYEE_TO_POSITION
    {
        [Column("ID")]
        public int Id { get; set; }

        [Column("POS_ID")]
        public int PositionId { get; set; }

        [Column("EMP_ID")]
        public int EmployeeId { get; set; }

        public virtual POSITION Position { get; set; }

        public virtual EMPLOYEE Employee { get; set; }
    }
}
