namespace Repository.Logic.Models
{
    public class Person : IdentityModel
    {
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string FullName => $"{Lastname} {Firstname}";
    }
}
