using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.Domain.Models
{
    public class PROJECT_STATUS
    {
        [Column("ID")]
        public int Id { get; set; }

        [Column("NAME")]
        public string Name { get; set; }

        [Column("KEY")]
        public string Key { get; set; }

        [Column("COLOR")]
        public string Color { get; set; }

        [Column("IS_ACTIVE")]
        public bool IsActive { get; set; }

    }
}
