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
                //Identity ilk olarak kurulurken bu ayarlara g�re kurulur.
                opts.User.RequireUniqueEmail = true;
                opts.User.AllowedUserNameCharacters = "abc�defg�h�ijklmno�pqrs�tu�vwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
                                                   // UserName alan�nda yukar�da izin verdi�im karakterlerin olabilec�ini girdim. (+,@ gibi i�aretler yukar�da bulunmad���ndan UserName alan�na girilemezler.) 
                                                   // https://docs.microsoft.com/tr-tr/dotnet/api/microsoft.aspnetcore.identity.useroptions.allowedusernamecharacters?view=aspnetcore-6.0

                opts.Password.RequiredLength = 4;  // �ifre do�rulama alan� i�in �ifrenin ka� karakterli olmas� gerekti�ini belirttil. (Ger�ek hayat senaryolar�nda 8 karakterli olmas� daha do�ru olacakt�r.)
                opts.Password.RequireNonAlphanumeric = false; //*,! gibi Alphanumeric karakter girilmesini istemiyorum. 
                opts.Password.RequireUppercase = false; // B�y�k harf gereklili�i girilmesini istemiyorum.
                opts.Password.RequireLowercase = false; // K���k harf gereklili�i girilmesini istemiyorum.
                opts.Password.RequireDigit = false;     // Say�sal karakter olmas� gereklili�ini istemiyorum
            }).AddPasswordValidator<CustomPasswordValidator>() //Kendi yazd���m�z custom password validation s�n�f�m�z� Identity mimarisine ekledik. Gerekli kurallar� da CustomPasswordValidator i�erisinde belirttik.
              .AddUserValidator<CustomUserValidator>()  //UserName ile ilgili custom kurallar yazd�k.
              .AddErrorDescriber<CustomIdentityErrorDescriber>() //Error mesajlar�m�z� t�rk�e olarak �zelle�tirdik.
              .AddEntityFrameworkStores<AppIdentityDbContext>();


            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages(); // Herhangi bir i�erik d�nmeyen sayfalar�m�zda bize bilgilendirici yaz� d�nmesi i�in eklendi. 
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseStaticFiles(); //js, css gibi dosyalar�m�z�n y�klenebilmesi i�in. 

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
