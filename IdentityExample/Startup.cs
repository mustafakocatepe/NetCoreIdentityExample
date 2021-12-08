using IdentityExample.CustomValidation;
using IdentityExample.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample
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
            services.AddDbContext<AppIdentityDbContext>(opts =>
            {
                opts.UseSqlServer(Configuration["ConnectionStrings:DefaultConnectionString"]);
            });

            services.AddIdentity<AppUser, AppRole>(opts =>
            {
                //Identity ilk olarak kurulurken bu ayarlara göre kurulur.
                opts.User.RequireUniqueEmail = true;
                opts.User.AllowedUserNameCharacters = "abcçdefgðhýijklmnoçpqrsþtuüvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
                                                   // UserName alanýnda yukarýda izin verdiðim karakterlerin olabilecðini girdim. (+,@ gibi iþaretler yukarýda bulunmadýðýndan UserName alanýna girilemezler.) 
                                                   // https://docs.microsoft.com/tr-tr/dotnet/api/microsoft.aspnetcore.identity.useroptions.allowedusernamecharacters?view=aspnetcore-6.0

                opts.Password.RequiredLength = 4;  // Þifre doðrulama alaný için þifrenin kaç karakterli olmasý gerektiðini belirttil. (Gerçek hayat senaryolarýnda 8 karakterli olmasý daha doðru olacaktýr.)
                opts.Password.RequireNonAlphanumeric = false; //*,! gibi Alphanumeric karakter girilmesini istemiyorum. 
                opts.Password.RequireUppercase = false; // Büyük harf gerekliliði girilmesini istemiyorum.
                opts.Password.RequireLowercase = false; // Küçük harf gerekliliði girilmesini istemiyorum.
                opts.Password.RequireDigit = false;     // Sayýsal karakter olmasý gerekliliðini istemiyorum
            }).AddPasswordValidator<CustomPasswordValidator>() //Kendi yazdýðýmýz custom password validation sýnýfýmýzý Identity mimarisine ekledik. Gerekli kurallarý da CustomPasswordValidator içerisinde belirttik.
              .AddUserValidator<CustomUserValidator>()  //UserName ile ilgili custom kurallar yazdýk.
              .AddErrorDescriber<CustomIdentityErrorDescriber>() //Error mesajlarýmýzý türkçe olarak özelleþtirdik.
              .AddEntityFrameworkStores<AppIdentityDbContext>();


            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages(); // Herhangi bir içerik dönmeyen sayfalarýmýzda bize bilgilendirici yazý dönmesi için eklendi. 
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseStaticFiles(); //js, css gibi dosyalarýmýzýn yüklenebilmesi için. 

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
