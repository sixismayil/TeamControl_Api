using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.Domain.Models
{
    public class PROJECT_TO_EMPLOYEE
    {
        [Column("ID")]
        public int Id { get; set; }

        [Column("PROJECT_ID")]
        public int ProjectId { get; set; }

        [Column("POS_ID")]
        public int PositionId { get; set; }

        [Column("EMP_ID")]
        public int EmployeeId { get; set; }

        [Column("IS_MAIN")]
        public bool IsMain { get; set; }

        public virtual PROJECT Project { get; set; }

        public virtual POSITION Position { get; set; }

        public virtual EMPLOYEE Employee { get; set; }
    }
}
