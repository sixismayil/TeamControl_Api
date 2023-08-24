using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using TeamControlV2.Infrastructure;
using TeamControlV2.Infrastructure.Repository;
using TeamControlV2.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.Services.Interface;
using TeamControlV2.Services.Implementation;
using TeamControlV2.Logging;
using AutoMapper;
using TeamControlV2.Extensions;

namespace TeamControlV2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {





            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TeamControlV2", Version = "v1" });
            });

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            //Added services
            
            services.AddAutoMapper(x => x.AddProfile(new MappingEntity())); 
            services.ConfigureCors();
            services.ConfigureJWTService();
            services.ConfigureLoggerService();

            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            services.AddTransient<IJwtHandler, JwtHandler>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IValidation, Validation>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<ISqlService, SqlService>();
            services.AddTransient<ICmdService, CmdService>();
            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddTransient<ILookupService, LookupService>();
            services.AddTransient<IPositionService, PositionService>();
            services.AddTransient<IVacationReasonService, VacationReasonService>();
            services.AddTransient<IProjectStatusService, ProjectStatusService>();
            services.AddTransient<IProjectService, ProjectService>();
            services.AddTransient<IHomeService, HomeService>();
            services.AddTransient<IVacationService, VacationService>();
            services.AddTransient<IProfileService, ProfileService>();
            services.AddTransient<ISalaryService, SalaryService>();
            services.AddTransient<IBonusAndPrizeService, BonusAndPrizeService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TeamControlV2 v1"));
            }


            app.ConfigureCustomExceptionMiddleware();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
