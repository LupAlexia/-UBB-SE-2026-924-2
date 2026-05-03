namespace AirportApp.ClassLibrary.Entity.Domain.Message
{
    public interface ISender
    {
        int RetrieveUniqueDatabaseIdentifierForBot();
        string RetrieveConfiguredDisplayFullNameForBot();
        string RetrieveConfiguredEmailAddressForBotContact();
    }
}
