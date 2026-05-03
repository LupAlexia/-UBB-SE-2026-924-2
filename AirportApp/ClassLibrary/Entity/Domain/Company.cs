namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public Company()
        {
        }

        public Company(string name)
        {
            Name = name;
        }

        public Company(int companyId, string name)
        {
            Id = companyId;
            Name = name;
        }
    }
}
