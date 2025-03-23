using Microsoft.Extensions.Logging;
using WebServiceForAdvertisingPlatforms.Service;
using WebServiceForAdvertisingPlatforms.Logging;

namespace WebServiceForAdvertisingPlatforms
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

             builder.Logging.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logger.txt"));

            // Add services to the container.
            builder.Services.AddSingleton<RegionTreeService>();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            //app.UseStaticFiles();

            //app.MapGet("/", () => Results.Redirect("/index.html"));

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            //app.Run(async (context) =>
            //{
            //    app.Logger.LogInformation($"Path: {context.Request.Path}  Time:{DateTime.Now.ToLongTimeString()}");
            //    await context.Response.WriteAsync("Hello World!");
            //});

            app.Run();
        }
    }
}
