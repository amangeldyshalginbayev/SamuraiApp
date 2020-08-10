using System.Collections.Generic;

namespace SamuraiApp.Domain
{
    public class Samurai
    {
        public Samurai()
        {
            Quotes = new List<Quote>();
            SamuraiBattles = new List<SamuraiBattle>();
        }

        public Samurai(string publicName, string secretName) : this()
        {
            Name = publicName;
            SecretIdentity = new SecretIdentity {RealName = secretName};
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public SamuraiFullName ElegantName { get; set; }
        public List<Quote> Quotes { get; set; }
        public List<SamuraiBattle> SamuraiBattles { get; set; }
        public SecretIdentity SecretIdentity { get; set; }

        public List<Battle> Battles()
        {
            var battles = new List<Battle>();
            foreach (var join in SamuraiBattles)
            {
                battles.Add(join.Battle);
            }

            return battles;
        }
    }
}