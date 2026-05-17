using System.Text.Json.Serialization;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Repository.Database;
using AirportApp.ClassLibrary.Repository;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Service;
using AirportApp.ClassLibrary.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Airport.Web
{
    public class Program
    {
        public static void Main(string[] arguments)
        {
            var builder = WebApplication.CreateBuilder(arguments);
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
            builder.Services.AddScoped<IRepository<int, User>, UserRepository>();

            builder.Services.AddScoped<IRepository<int, Chat>, ChatDatabaseRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageDatabaseRepository>();
            builder.Services.AddScoped<IRepository<int, Review>, ReviewRepository>();
            builder.Services.AddScoped<IRepository<int, Sender>, SenderRepository>();
            builder.Services.AddScoped<IRepository<int, FAQNode>, DecisionTreeRepository>();

            builder.Services.AddScoped<IBotStrategy, DecisionTreeStrategy>();
            builder.Services.AddScoped<BotEngineIdentity>();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<ICancellationService, CancellationService>();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<IComplaintTicketService, ComplaintTicketService>();
            builder.Services.AddScoped<IComplaintTicketCategoryService, ComplaintTicketCategoryService>();
            builder.Services.AddScoped<IComplaintTicketSubcategoryService, ComplaintTicketSubcategoryService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<IDecisionTreeService, DecisionTreeService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IFAQService, FAQService>();
            builder.Services.AddScoped<IFlightSearchService, FlightSearchService>();
            builder.Services.AddScoped<IMembershipService, MembershipService>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<IPricingService, PricingService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IUserService, UserService>();

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

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
