using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using P3.Models;
using P3.Repositories;

using P3.Models;

using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.AspNetCore.Builder;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

using Microsoft.IdentityModel.Tokens;
 
using System.Text;

using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Hosting;

using P3.Repositories;

using Microsoft.OpenApi.Models;
 
namespace Book

{

    public class Program

    {
        public static object JwtBearerDefaults { get; private set; }

        public static void Main(string[] args)

        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var configuration = new ConfigurationBuilder().SetBasePath(builder.Environment.ContentRootPath)

                .AddJsonFile("appsettings.json").Build();

            builder.Services.AddCors();

            builder.Services.AddDbContext<MyDbContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("myconnection")));

            builder.Services.AddScoped<IUsersRepo, UsersRepo>();

            builder.Services.AddAuthentication(opt =>

            {

               // opt.DefaultAuthenticateScheme = JwtBearerDefaults.authen

               // opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(o =>

            {

                var key = Encoding.UTF8.GetBytes(configuration["JWT:Key"]);

                o.SaveToken = true;

                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters

                {

                    ValidateIssuer = false,

                    ValidateAudience = false,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,

                    ValidAudience = configuration["JWT:Audience"],

                    ValidIssuer = configuration["JWT:Issuer"],

                    IssuerSigningKey = new SymmetricSecurityKey(key),

                };

            });

            //.AddGoogle(options =>

            //{

            //    options.SaveTokens = true;

            //    options.ClientId = "485947985520-2929tagfo657ogj3ghljgq3e51rle2em.apps.googleusercontent.com";

            //    options.ClientSecret = "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";

            //});

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            builder.Services.AddEndpointsApiExplorer();

           // builder.Services.AddSwaggerGen();

            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type=ReferenceType.SecurityScheme,
                Id="Bearer"
            }
        },
        new string[]{}
    }
});
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())

            {

                app.UseSwagger();

                app.UseSwaggerUI();

            }

            app.UseHttpsRedirection();

            app.UseCors(builder => builder

            .AllowAnyMethod()

            .AllowAnyHeader()

            .SetIsOriginAllowed(origin => true)  // You can keep this line if it's necessary for your use case

            .AllowCredentials()

            );

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();

        }

    }

}
