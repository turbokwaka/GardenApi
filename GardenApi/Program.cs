using Microsoft.EntityFrameworkCore;

namespace GardenApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        Configure(app);

        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Add authorization service
        services.AddAuthorization();

        // Add Swagger services
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // Add controllers support
        services.AddControllers();

        // Add HttpClient service
        services.AddHttpClient();

        // Add DbContext with PostgreSQL connection
        services.AddDbContext<ApplicationContext>(options => 
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Add CORS policy
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });
    }

    private static void Configure(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            // Use Swagger in development environment
            app.UseSwagger();
            app.UseSwaggerUI();

            // Use developer exception page
            app.UseDeveloperExceptionPage();
        }

        // Enable HTTPS redirection
        app.UseHttpsRedirection();

        // Use routing
        app.UseRouting();

        // Use CORS
        app.UseCors("AllowAllOrigins");

        // Use authorization
        app.UseAuthorization();

        // Map controller routes
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
