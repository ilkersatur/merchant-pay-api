
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using VposApi.Models;
using VposApi.Config;
using VposApi.Services;

namespace VposApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.Configure<VposSettings>(builder.Configuration.GetSection("VposSettings"));
            builder.Services.AddHttpClient(); // HttpClient injection için


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<ISecureService, SecureService>();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            builder.Services.AddScoped<IValidator<PaymentRequest>, PaymentValidator>();
            builder.Services.AddScoped<IValidator<SecureRequest>, SecureValidator>();
            builder.Services.AddScoped<IValidator<Auth3DSModel>, Auth3DSValidator>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCors("AllowAll");

            app.UseAuthorization();

            app.MapControllers();


            app.Run();
        }
    }
}
