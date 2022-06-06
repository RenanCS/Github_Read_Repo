using FluentValidation.AspNetCore;
using GitRepositoryRead.Api.Filter;
using GitRepositoryRead.Application.Service;
using GitRepositoryRead.Core.Service;
using GitRepositoryRead.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Linq;
using System.Net.Http.Headers;

namespace GitRepositoryRead.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers(
                   options => options.Filters.Add(typeof(ValidationFilter))
                   )
                   .AddFluentValidation(fluentValidation =>
               fluentValidation.RegisterValidatorsFromAssemblyContaining<RepositoriesInputModelValidator>());

            services.AddScoped<IGithubService, GithubService>();

            services.AddHttpClient("GithubApi", client =>
            {
                string baseURL = Configuration.GetSection("GitRepository:BaseURL").Value;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("githubteste", "1"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                client.BaseAddress = new Uri(baseURL);
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GitRepositoryRead.Api", Version = "v1" });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GitRepositoryRead.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
