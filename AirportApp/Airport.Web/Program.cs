using System.Text.Json.Serialization;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Repository.Database;
using AirportApp.ClassLibrary.Repository;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Airport.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls("http://localhost:5253");

            builder.Services.AddDbContext<AirportDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IAddOnRepository, AddOnRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IFAQRepository, FAQRepository>();
            builder.Services.AddScoped<IFlightRepository, FlightRepository>();
            builder.Services.AddScoped<IFlightTicketRepository, FlightTicketRepository>();
            builder.Services.AddScoped<IMembershipRepository, MembershipRepository>();
            builder.Services.AddScoped<ITicketRepository, ComplaintTicketRepository>();
            builder.Services.AddScoped<ITicketCategoryRepository, ComplaintTicketCategoryRepository>();
            builder.Services.AddScoped<ITicketSubcategoryRepository, ComplaintTicketSubcategoryRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddScoped<IRepository<int, Chat>, ChatDatabaseRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageDatabaseRepository>();
            builder.Services.AddScoped<IRepository<int, Review>, ReviewRepository>();
            builder.Services.AddScoped<IRepository<int, FAQNode>, DecisionTreeRepository>();

            builder.Services.AddControllers(options =>
                {
                    options.SuppressAsyncSuffixInActionNames = false;
                })

                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
