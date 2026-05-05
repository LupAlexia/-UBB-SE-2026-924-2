using FluentAssertions;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service;
using AirportApp.ClassLibrary.DataAccess;

namespace AirportApp.Tests.Integration.Services;

[TestClass]
public class AuthServiceIntegrationTests : BaseIntegrationTest
{
    private const int UniqueCodeStartIndex = 0;
    private const int UniqueCodeLength = 4;
    private const string AndreiEmail = "andrei.codreanu";
    private const string AndreiUsername = "AndreiC";
    private const string AndreiPassword = "Parola@Andrei123";
    private const string AndreiPhone = "0733887766";
    private const string ClaudiaEmail = "claudia.radu";
    private const string ClaudiaPassword = "ParolaClaudia1";
    private const string SorinEmail = "sorin.mihai";
    private const string SorinUsername = "SorinM";
    private const string SorinPassword = "MihaiSecret99!";
    private const string SorinPhone = "0766112233";
    private const string DomainYahoo = "@yahoo.ro";
    private const string DomainGmail = "@gmail.com";
    private const string DefaultPhone = "0744112233";
    private const string AlternateUsername = "AltUtilizator";
    private const string AlternatePassword = "AltaParola2";
    private readonly ICustomerRepository userRepository;
    private readonly AuthService authentificationService;

    public AuthServiceIntegrationTests()
    {
        var dbContext = CreateDbContext();
        var membershipRepository = new MembershipRepository(dbContext);
        userRepository = new CustomerRepository(dbContext, membershipRepository);
        authentificationService = new AuthService(userRepository);
    }

    [TestMethod]
    public async Task RegisterAndLogin_ValidData_Succeeds()
    {
        string uniqueCode = Guid.NewGuid().ToString().Substring(UniqueCodeStartIndex, UniqueCodeLength);
        string email = $"{AndreiEmail}_{uniqueCode}{DomainGmail}";
        string phone = AndreiPhone;
        string username = $"{AndreiUsername}_{uniqueCode}";
        string password = AndreiPassword;

        await authentificationService.RegisterAsync(email, phone, username, password);
        var loginResult = await authentificationService.LoginAsync(email, password);

        loginResult.Should().NotBeNull();
        loginResult.Email.Should().Be(email);
    }

    [TestMethod]
    public async Task Register_DuplicateEmailAddress_ThrowsException()
    {
        string uniqueCode = Guid.NewGuid().ToString().Substring(UniqueCodeStartIndex, UniqueCodeLength);
        string email = $"{ClaudiaEmail}_{uniqueCode}{DomainYahoo}";
        await authentificationService.RegisterAsync(email, DefaultPhone, $"ClaudiaR_{uniqueCode}", ClaudiaPassword);

        Func<Task> registerAction = async () => await authentificationService.RegisterAsync(email, DefaultPhone, $"{AlternateUsername}_{uniqueCode}", AlternatePassword);
        await registerAction.Should().ThrowAsync<InvalidOperationException>();
    }

    [TestMethod]
    public async Task Register_ValidUser_HashesPasswordInDatabase()
    {
        string uniqueCode = Guid.NewGuid().ToString().Substring(UniqueCodeStartIndex, UniqueCodeLength);
        string email = $"{SorinEmail}_{uniqueCode}{DomainGmail}";
        string password = SorinPassword;

        await authentificationService.RegisterAsync(email, SorinPhone, $"{SorinUsername}_{uniqueCode}", password);
        var user = await userRepository.GetByEmailAsync(email);

        user!.PasswordHash.Should().NotBe(password);
    }
}
