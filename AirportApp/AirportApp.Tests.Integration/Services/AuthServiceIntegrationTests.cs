using FluentAssertions;
using TicketManager.Domain;
using TicketManager.Repository;
using TicketManager.Service;

namespace TicketManager.Tests.Integration.Services;

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
    private readonly IUserRepository userRepository;
    private readonly AuthService authentificationService;

    public AuthServiceIntegrationTests()
    {
        var databaseConnectionFactory = new DatabaseConnectionFactory(GetTestConnectionString());
        var membershipRepository = new MembershipRepository(databaseConnectionFactory);
        userRepository = new UserRepository(databaseConnectionFactory, membershipRepository);
        authentificationService = new AuthService(userRepository);
    }

    [Fact]
    public void RegisterAndLogin_ValidData_Succeeds()
    {
        string uniqueCode = Guid.NewGuid().ToString().Substring(UniqueCodeStartIndex, UniqueCodeLength);
        string email = $"{AndreiEmail}_{uniqueCode}{DomainGmail}";
        string phone = AndreiPhone;
        string username = $"{AndreiUsername}_{uniqueCode}";
        string password = AndreiPassword;

        authentificationService.Register(email, phone, username, password);
        var loginResult = authentificationService.Login(email, password);

        loginResult.Should().NotBeNull();
        loginResult.Email.Should().Be(email);
    }

    [Fact]
    public void Register_DuplicateEmailAddress_ThrowsException()
    {
        string uniqueCode = Guid.NewGuid().ToString().Substring(UniqueCodeStartIndex, UniqueCodeLength);
        string email = $"{ClaudiaEmail}_{uniqueCode}{DomainYahoo}";
        authentificationService.Register(email, DefaultPhone, $"ClaudiaR_{uniqueCode}", ClaudiaPassword);

        Action registerAction = () => authentificationService.Register(email, DefaultPhone, $"{AlternateUsername}_{uniqueCode}", AlternatePassword);
        registerAction.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Register_ValidUser_HashesPasswordInDatabase()
    {
        string uniqueCode = Guid.NewGuid().ToString().Substring(UniqueCodeStartIndex, UniqueCodeLength);
        string email = $"{SorinEmail}_{uniqueCode}{DomainGmail}";
        string password = SorinPassword;

        authentificationService.Register(email, SorinPhone, $"{SorinUsername}_{uniqueCode}", password);
        var user = userRepository.GetByEmail(email);

        user!.PasswordHash.Should().NotBe(password);
    }
}


