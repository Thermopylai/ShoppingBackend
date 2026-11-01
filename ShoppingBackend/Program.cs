
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using ShoppingBackend.Models;
using System.Globalization;

namespace ShoppingBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ShoplistDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ShoplistConnection")));
            
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ShoplistDbRestApi",
                    Version = "v1"
                });
                
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var fi = new CultureInfo("fi-FI");

                options.DefaultRequestCulture = new RequestCulture(fi);
                options.SupportedCultures = new[] { fi };
                options.SupportedUICultures = new[] { fi };

                options.CultureInfoUseUserOverride = false;
            });

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("All", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            app.UseRequestLocalization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShoplistDbRestApi v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();
            app.UseAuthorization();
            
            app.UseCors("All");

            app.MapControllers();

            app.Run();
        }
    }
}
