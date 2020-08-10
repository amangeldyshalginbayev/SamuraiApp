using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;

namespace ConsoleApp
{
    internal class Program
    {
        private static SamuraiContext _context = new SamuraiContext();

        private static void Main(string[] args)
        {
            // PrePopulateSamuraisAndBattles();
            // JoinBattleAndSamurai();
            // EnlistSamuraiIntoBattle();
            // EnlistSamuraiIntoABattleUntracked();
            // AddNewSamuraiViaDisconnectedBattleObject();

            // RemoveJoinBetweenSamuraiAndBattleSimple();
            // RemoveBattleFromSamurai();

            // AddNewSamuraiWithSecretIdentity();
            // AddSecretIdentityUsingSamuraiId();
            // AddSecretIdentityToExistingSamurai();
            // ReplaceASecretIdentity();
            // ReplaceASecretIdentityNotTracked();


            // CreateSamurai();
            // RetrieveSamuraisCreatedInPastWeek();
            // CreateThenEditSamuraiWithQuote();
            
            
        }

        private static void CreateThenEditSamuraiWithQuote()
        {
            var samurai = new Samurai {Name = "Ulken tasak"};
            var quote = new Quote {Text = "Aren't I MARVELous with my huge dick?"};
            samurai.Quotes.Add(quote);
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
            quote.Text += " See what I did there?";
            _context.SaveChanges();
        }

        private static void RetrieveSamuraisCreatedInPastWeek()
        {
            var oneWeekAgo = DateTime.Now.AddDays(-7);
            // var newSamurais = _context.Samurais
            //     .Where(s => EF.Property<DateTime>(s, "Created") >= oneWeekAgo)
            //     .ToList();

            var samuraisCreated = _context.Samurais
                .Where(s => EF.Property<DateTime>(s, "Created") >= oneWeekAgo)
                .Select(s => new {s.Id, s.Name, Created = EF.Property<DateTime>(s, "Created")})
                .ToList();
            Console.WriteLine(samuraisCreated[0]);
        }

        private static void CreateSamurai()
        {
            var samurai = new Samurai {Name = "Ronin"};
            _context.Samurais.Add(samurai);
            var timestamp = DateTime.Now;
            _context.Entry(samurai).Property("Created").CurrentValue = timestamp;
            _context.Entry(samurai).Property("LastModified").CurrentValue = timestamp;
            _context.SaveChanges();
        }

        private static void ReplaceASecretIdentityNotTracked()
        {
            Samurai samurai;
            using (var separateOperation = new SamuraiContext())
            {
                samurai = separateOperation.Samurais.Include(s => s.SecretIdentity)
                    .FirstOrDefault(s => s.Id == 1);
            }

            samurai.SecretIdentity = new SecretIdentity {RealName = "Zhappar"};
            _context.Samurais.Attach(samurai);
            _context.SaveChanges();
        }

        private static void ReplaceASecretIdentity()
        {
            var samurai = _context.Samurais.Include(s => s.SecretIdentity)
                .FirstOrDefault(s => s.Id == 1);
            samurai.SecretIdentity = new SecretIdentity {RealName = "Sampson"};
            _context.SaveChanges();
        }

        private static void AddSecretIdentityToExistingSamurai()
        {
            Samurai samurai;
            using (var separateOperation = new SamuraiContext())
            {
                samurai = _context.Samurais.Find(2);
            }

            samurai.SecretIdentity = new SecretIdentity {RealName = "Hulia"};
            _context.Samurais.Attach(samurai);
            _context.SaveChanges();
        }

        private static void AddSecretIdentityUsingSamuraiId()
        {
            // Note: SamuraiId 1 does not have a secret identity yet!
            var identity = new SecretIdentity {SamuraiId = 1};
            _context.Add(identity);
            _context.SaveChanges();
        }

        private static void AddNewSamuraiWithSecretIdentity()
        {
            var samurai = new Samurai {Name = "Jina Ujichika"};
            samurai.SecretIdentity = new SecretIdentity {RealName = "Julie"};
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void RemoveBattleFromSamurai()
        {
            // Goal:Remove join between Samurai Id = 3 and Battle Id = 1
            var samurai = _context.Samurais.Include(s => s.SamuraiBattles)
                .ThenInclude(sb => sb.Battle)
                .SingleOrDefault(s => s.Id == 3);

            var sbToRemove = samurai.SamuraiBattles.SingleOrDefault(sb => sb.BattleId == 1);
            samurai.SamuraiBattles.Remove(sbToRemove); //remove via List<T>
            // _context.Remove(sbToRemove); //remove using DbContext
            _context.ChangeTracker.DetectChanges();
            _context.SaveChanges();
        }

        private static void RemoveJoinBetweenSamuraiAndBattleSimple()
        {
            // This join object actually needs to be pulled from database, not created here!!!
            var join = new SamuraiBattle {BattleId = 1, SamuraiId = 8};
            _context.Remove(join);
            _context.SaveChanges();
        }

        private static void GetSamuraiWithBattles_ElegantWay()
        {
            var samurai = _context.Samurais.Find(1);
            var samuraisBattles = samurai.Battles();
        }

        private static void GetSamuraiWithBattles()
        {
            var samuraiWithBattles = _context.Samurais
                .Include(s => s.SamuraiBattles)
                .ThenInclude(sb => sb.Battle).FirstOrDefault(s => s.Id == 1);
            var battle = samuraiWithBattles.SamuraiBattles.First().Battle;
            var allTheBattles = new List<Battle>();
            foreach (var samuraiBattle in samuraiWithBattles.SamuraiBattles)
            {
                allTheBattles.Add(samuraiBattle.Battle);
            }
        }

        private static void AddNewSamuraiViaDisconnectedBattleObject()
        {
            Battle battle;
            using (var separateOperation = new SamuraiContext())
            {
                battle = separateOperation.Battles.Find(1);
            }

            var newSamurai = new Samurai {Name = "SampsonSan"};
            battle.SamuraiBattles.Add(new SamuraiBattle {Samurai = newSamurai});
            _context.Battles.Attach(battle);
            _context.SaveChanges();
        }

        private static void EnlistSamuraiIntoABattleUntracked()
        {
            Battle battle;
            using (var separateOperation = new SamuraiContext())
            {
                battle = separateOperation.Battles.Find(1);
            }

            battle.SamuraiBattles.Add(new SamuraiBattle {SamuraiId = 2});
            _context.Battles.Attach(battle);
            _context.ChangeTracker.DetectChanges();
            _context.SaveChanges();
        }

        private static void EnlistSamuraiIntoBattle()
        {
            var battle = _context.Battles.Find(1);
            battle.SamuraiBattles.Add(new SamuraiBattle {SamuraiId = 3});
            _context.SaveChanges();
        }

        private static void JoinBattleAndSamurai()
        {
            // Kikuchiyo id is 1, Siege of Osaka id is 3
            var sbJoin = new SamuraiBattle {SamuraiId = 1, BattleId = 3};
            _context.Add(sbJoin);
            _context.SaveChanges();
        }

        private static void PrePopulateSamuraisAndBattles()
        {
            _context.AddRange(
                new Samurai {Name = "Kikuchiyo"},
                new Samurai {Name = "Kambei Shimada"},
                new Samurai {Name = "Shichiroji"},
                new Samurai {Name = "Katsushiro Okamoto"},
                new Samurai {Name = "Heihachi Hayashida"},
                new Samurai {Name = "Kyuzo"},
                new Samurai {Name = "Gorobey Katayama"}
            );

            _context.Battles.AddRange(
                new Battle
                {
                    Name = "Battle of Okehazama", StartDate = new DateTime(1560, 05, 1),
                    EndDate = new DateTime(1560, 06, 15)
                },
                new Battle
                {
                    Name = "Battle of Shiroyama", StartDate = new DateTime(1877, 09, 24),
                    EndDate = new DateTime(1877, 09, 24)
                },
                new Battle
                {
                    Name = "Siege of Osaka", StartDate = new DateTime(1614, 01, 1),
                    EndDate = new DateTime(1615, 12, 31)
                },
                new Battle
                {
                    Name = "Boshin War", StartDate = new DateTime(1868, 01, 1),
                    EndDate = new DateTime(1869, 01, 1)
                }
            );

            _context.SaveChanges();
        }
    }
}