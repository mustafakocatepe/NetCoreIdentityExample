using IdentityExample.CustomValidation;
using IdentityExample.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IdentityExample.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, AppRole>(opts =>
            {
                //Identity ilk olarak kurulurken bu ayarlara göre kurulur.
                opts.User.RequireUniqueEmail = true;
                opts.User.AllowedUserNameCharacters = "abcçdefgğhıijklmnoçpqrsştuüvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
                // UserName alanında yukarıda izin verdiğim karakterlerin olabilecğini girdim. (+,@ gibi işaretler yukarıda bulunmadığından UserName alanına girilemezler.) 
                // https://docs.microsoft.com/tr-tr/dotnet/api/microsoft.aspnetcore.identity.useroptions.allowedusernamecharacters?view=aspnetcore-6.0

                opts.Password.RequiredLength = 4;  // Şifre doğrulama alanı için şifrenin kaç karakterli olması gerektiğini belirttil. (Gerçek hayat senaryolarında 8 karakterli olması daha doğru olacaktır.)
                opts.Password.RequireNonAlphanumeric = false; //*,! gibi Alphanumeric karakter girilmesini istemiyorum. 
                opts.Password.RequireUppercase = false; // Büyük harf gerekliliği girilmesini istemiyorum.
                opts.Password.RequireLowercase = false; // Küçük harf gerekliliği girilmesini istemiyorum.
                opts.Password.RequireDigit = false;     // Sayısal karakter olması gerekliliğini istemiyorum
            }).AddPasswordValidator<CustomPasswordValidator>() //Kendi yazdığımız custom password validation sınıfımızı Identity mimarisine ekledik. Gerekli kuralları da CustomPasswordValidator içerisinde belirttik.
             .AddUserValidator<CustomUserValidator>()  //UserName ile ilgili custom kurallar yazdık.
             .AddErrorDescriber<CustomIdentityErrorDescriber>() //Error mesajlarımızı türkçe olarak özelleştirdik.
             .AddEntityFrameworkStores<AppIdentityDbContext>();

            return services;
        }

        public static IServiceCollection ConfigureCookie(this IServiceCollection services) 
        {
            CookieBuilder cookieBuilder = new();
            cookieBuilder.Name = "MyIdentityExample";
            cookieBuilder.HttpOnly = true; //Güvenlik için ClientSide tarafta Cookieme erişimi engellemek için bu ayarı True olarak set ediyoruz. Sadece bir http isteği üzerinden Cookie bilgisini almak istiyorum.Client Side tarafında Cookie bilgisini okumak istemiyorum.
            cookieBuilder.Expiration = TimeSpan.FromDays(60); //60 gün boyunca kullanıcı Login olmadan sistemimde işlem yapabilir. 
            cookieBuilder.SameSite = SameSiteMode.Lax; //Bir cookie'yi kaydettikten sonra sadece bu site üzerinden cookie'ye ulaşabilirim. (Lax bu ayarı kapatır, Strict ise bu ayarı açar.)
            cookieBuilder.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            //Always : Kullanıcının Cookie'sini sadece HTTPS üzerinden bir istek geldiyse gönderiyor.
            //SameAsRequest : Eğer istek HTTP üzerinden geldiyse HTPP, eğer istek HTTPS üzerinden geldiyse HTTPS üzerinden gönderir.
            //None : İstek ne üzerinden gelirse gelsin HTTP üzerinden gönderir.
            //Eğer HTTPS üzerine bir protokol var ise mutlaka Alwats olarak tutmak mantıklıdır.  

            services.ConfigureApplicationCookie(opts =>
            {
                opts.LoginPath = new PathString("/Home/Login"); //Kullanıcı eğer Login olmadan login olan kullanıcıların erişmeye çalıştığı bir sayfaya erişmeye çalışırsa burada belirttiğimiz sayfaya yönlendirilir.
                //opts.LogoutPath = new PathString();
                opts.Cookie = cookieBuilder;
                opts.SlidingExpiration = true; //Eğer true set edilirse Kullanıcının Cookie Expiration gününün yarısına geldiğinde Cookie yenilenerek Expiration günü kadar tekrar Cookie'si aktif olur.
                                               //Kullanıcın Cookie süresinin ömrünü uzatmak için mantıklı bir yaklaşım. Kullanıcıyı tekrar tekrar Login olmaya zorlamamış oluyoruz.
            });
            return services;

        }
    }
}
