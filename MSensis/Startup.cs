using DinkToPdf;
using DinkToPdf.Contracts;
 using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MSensis.Email;
using MSensis.LocalizationResources;
using MSensis.Models;
using MSensis.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Wkhtmltopdf.NetCore;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;

namespace MSensis
{
    public class Startup
    {
        
        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.ConfigureApplicationCookie(opts =>
            {
                opts.Cookie.Expiration = TimeSpan.FromDays(14);

                opts.Cookie.Name = "SecurityLogin";
            });

            
            services.AddTransient<FileManager>(); 
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            //services.Configure<SmtpSettings>(Configuration.GetSection("SmtpSettings"));
            services.AddSingleton<IMailer, Mailer>();
            services.AddAuthorization();
            services.AddCors();

            //code to try make app bilingual---------------------------------------------------------------------
            services.AddLocalization(opts =>
            {
                opts.ResourcesPath = "Resources";
            });

            //------------------------------------------------------------------------------------------------------
            services.AddMvc().AddRazorPagesOptions(options =>
            {
                options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "/Account/Login");
            })
            .AddViewLocalization(
                opts => { opts.ResourcesPath = "Resources"; })
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //this code is also for localization----------------------------------------------------------------------
            services.Configure<RequestLocalizationOptions>(opts =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en"),
                    new CultureInfo("en-US"),
                    new CultureInfo("el"),
                    new CultureInfo("el-GR")
                };
                opts.DefaultRequestCulture = new RequestCulture("en-US");
                //formatting numbers, dates
                opts.SupportedCultures = supportedCultures;
                //UI string  that we have localized
                opts.SupportedUICultures = supportedCultures;
            });
            //-------------------------------------------------------------------------------------------------------------
             
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            //{
            //    var context = serviceScope.ServiceProvider.GetRequiredService<MSensisContext>();
            //    context.Database.Migrate();
            //}

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            //another piece of code for localization---------------------------------------------
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);
            //------------------------------------------------------------------------------------

            app.UseCookiePolicy();
            
            app.UseFileServer();
            app.UseAuthentication();
            

            loggerFactory.AddFile("Logs/myapp-{Date}.txt");


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
  
}
      