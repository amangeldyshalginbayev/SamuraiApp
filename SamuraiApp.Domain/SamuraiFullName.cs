
namespace SamuraiApp.Domain
{
    public class SamuraiFullName
    {
        public SamuraiFullName(string surName, string givenName)
        {
            SurName = surName;
            GivenName = givenName;
        }
        public string SurName { get; set; }
        public string GivenName { get; set; }
        public string FullName => $"{GivenName} {SurName}";
        public string FullNameReverse => $"{SurName} {GivenName}";
    }
}