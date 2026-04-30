namespace AirportApp.Src.Domain
{
    public class Company
    {
        public int CompanyId { get; set; }
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
            CompanyId = companyId;
            Name = name;
        }
    }
}
