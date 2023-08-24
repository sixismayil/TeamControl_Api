using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.Domain.Models
{
    public class CUSTOMER_TO_PROJECT
    {
        [Column("ID")]
        public int Id { get; set; }

        [Column("CUSTOMER_ID")]
        public int CustomerId { get; set; }

        [Column("PROJECT_ID")]
        public int ProjectId { get; set; }

        [Column("IS_MAIN")]
        public bool IsMain { get; set; }
        //[ForeignKey("CUSTOMER_ID")]
        //public virtual CUSTOMER Customer { get; set; }
        //[ForeignKey("PROJECT_ID")]
        //public virtual PROJECT Project { get; set; }
    }
}
