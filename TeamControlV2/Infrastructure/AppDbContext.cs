using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.Domain.Models;

namespace TeamControlV2.Infrastructure
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ADMIN> ADMIN { get; set; }
        public DbSet<EMPLOYEE> EMPLOYEE { get; set; }
        public DbSet<CUSTOMER> CUSTOMER { get; set; }
        public DbSet<POSITION> POSITION { get; set; }
        public DbSet<VACATION_REASON> VACATION_REASON { get; set; }
        public DbSet<PROJECT_STATUS> PROJECT_STATUS { get; set; }
        public DbSet<PROJECT> PROJECT { get; set; }
        public DbSet<BONUS_AND_PRIZE> BONUS_AND_PRIZE { get; set; }
        public DbSet<CUSTOMER_TO_PROJECT> CUSTOMER_TO_PROJECT { get; set; }
        public DbSet<EMPLOYEE_TO_POSITION> EMPLOYEE_TO_POSITION { get; set; }
        public DbSet<PROJECT_TO_EMPLOYEE> PROJECT_TO_EMPLOYEE { get; set; }
        public DbSet<SALARY> SALARY { get; set; }
        public DbSet<VACATION> VACATION { get; set; }
    }
}
