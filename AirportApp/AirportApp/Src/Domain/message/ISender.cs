namespace AirportApp.Src.Model.Message
{
    public interface ISender
    {
        int RetrieveUniqueDatabaseIdentifierForBot();
        string RetrieveConfiguredDisplayFullNameForBot();
        string RetrieveConfiguredEmailAddressForBotContact();
    }
}
