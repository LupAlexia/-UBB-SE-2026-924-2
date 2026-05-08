namespace AirportApp.ClassLibrary.Entity.Domain
{
    public interface ISender
    {
        int RetrieveUniqueDatabaseIdentifierForBot();
        string RetrieveConfiguredDisplayFullNameForBot();
        string RetrieveConfiguredEmailAddressForBotContact();
    }
}
