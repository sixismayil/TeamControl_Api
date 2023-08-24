using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.Domain.Models
{
    public class VACATION_REASON
    {
        [Column("ID")]
        public int Id { get; set; }

        [Column("NAME")]
        public string Name { get; set; }

        [Column("KEY")]
        public string Key { get; set; }

        [Column("IS_ACTIVE")]
        public bool IsActive { get; set; }
    }
}
