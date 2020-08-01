using System;
using AutoMapper;
using Commander.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;

namespace Commander
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        readonly string MyAllowSpecificOrigins = "_devEnvCorsPolicy";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(
                    name: MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
                    });
            });

            services.AddDbContext<CommanderContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("CommanderConnection")));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Commander API", Version = "v1" });
            });

            services.AddControllers().AddNewtonsoftJson(s => s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //services.AddScoped<ICommanderRepo, MockCommanderRepo>(); //Mockeado
            services.AddScoped<ICommanderRepo, SqlCommanderRepo>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Commander API V1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
