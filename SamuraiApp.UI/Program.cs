using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;

namespace SamuraiApp.UI
{
    class Program
    {
        private static SamuraiContext _context = new SamuraiContext();

        private static void Main(string[] args)
        {
            //_context.Database.EnsureCreated();
            //GetSamurais("");
            //QueryAndUpdateBattles_Disconnected();
            //GetSamurais("After Add:");
            ExplicitLoadQuotesWithInclude();
            Console.Write("Press any key...");
            Console.ReadKey();
        }

        private static void AddSamurai(params string[] names)
        {
            foreach (var name in names)
            {
                _context.Samurais.Add(new Samurai { Name = name });
            }

            _context.SaveChanges();
        }
        private static void GetSamurais(string text)
        {
            var samurais = _context.Samurais.TagWith("ConsoleApp.Program.GetSamurais method").ToList();
            Console.WriteLine($"{text}: Samurai count is {samurais.Count}");
            foreach (var samurai in samurais)
            {
                Console.WriteLine(samurai.Name);
            }
        }

        private static void AddVariousTypes()
        {
            _context.AddRange(new Samurai() { Name = "Shimada" }, new Samurai() { Name = "Okamoto" });
            _context.AddRange(new Battle() { Name = "Battle of Anegawa" }, new Battle() { Name = "Battle of Nagashino" });
            _context.SaveChanges();
        }

        private static void QueryAndUpdateBattles_Disconnected()
        {
            List<Battle> disconnectedBattles;
            using (var context1 = new SamuraiContext())
            {
                disconnectedBattles = _context.Battles.ToList();
            } //context1 is disposed
            disconnectedBattles.ForEach(b =>
            {
                b.StartDate = new DateTime(1570, 01, 01);
                b.EndDate = new DateTime(1570, 12, 1);
            });
            using (var context2 = new SamuraiContext())
            {
                context2.UpdateRange(disconnectedBattles);
                context2.SaveChanges();
            }
        }

        private static void InsertNewSamuraiWithAQuote()
        {
            var samurai = new Samurai
            {
                Name = "Kambei Shimada",
                Quotes = new List<Quote>
        {
          new Quote { Text = "I've come to save you" }
        }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }
        private static void InsertNewSamuraiWithManyQuotes()
        {
            var samurai = new Samurai
            {
                Name = "Kyūzō",
                Quotes = new List<Quote> {
            new Quote {Text = "Watch out for my sharp sword!"},
            new Quote {Text="I told you to watch out for the sharp sword! Oh well!" }
        }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }
        private static void AddQuoteToExistingSamuraiWhileTracked()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Quotes.Add(new Quote
            {
                Text = "I bet you're happy that I've saved you!"
            });
            _context.SaveChanges();
        }
        private static void AddQuoteToExistingSamuraiNotTracked(int samuraiId)
        {
            var samurai = _context.Samurais.Find(samuraiId);
            samurai.Quotes.Add(new Quote
            {
                Text = "Now that I saved you, will you feed me dinner?"
            });
            using (var newContext = new SamuraiContext())
            {
                newContext.Samurais.Attach(samurai);
                newContext.SaveChanges();
            }
        }

        private static void Simpler_AddQuoteToExistingSamuraiNotTracked(int samuraiId)
        {
            var quote = new Quote { Text = "Thanks for dinner!", SamuraiId = samuraiId };
            using var newContext = new SamuraiContext();
            newContext.Quotes.Add(quote);
            newContext.SaveChanges();
        }
        private static void ProjectSomeProperties()
        {
            var someProperties = _context.Samurais.Select(s => new { s.Id, s.Name }).ToList();
            var idAndNames = _context.Samurais.Select(s => new IdAndName(s.Id, s.Name)).ToList();
        }
        public struct IdAndName
        {
            public IdAndName(int id, string name)
            {
                Id = id;
                Name = name;
            }
            public int Id;
            public string Name;

        }
        private static void ProjectSamuraisWithQuotes()
        {
            //var somePropsWithQuotes = _context.Samurais
            //    .Select(s => new { s.Id, s.Name, NumberOfQuotes=s.Quotes.Count })
            //    .ToList();
            //var somePropsWithQuotes = _context.Samurais
            //.Select(s => new { s.Id, s.Name, 
            //                   HappyQuotes = s.Quotes.Where(q=>q.Text.Contains("happy")) })
            //.ToList();
            var samuraisAndQuotes = _context.Samurais
                .Select(s => new
                {
                    Samurai = s,
                    HappyQuotes = s.Quotes.Where(q => q.Text.Contains("happy"))
                })
                .ToList();
        }

        private static void ExplicitLoadQuotes()
        {
            //make sure there's a horse in the DB, then clear the context's change tracker
            //_context.Set<Horse>().Add(new Horse { SamuraiId = 1, Name = "Mr. Ed" });
            //_context.SaveChanges();
            //_context.ChangeTracker.Clear();
            //-------------------
            var samurai = _context.Samurais.Find(1);
            _context.Entry(samurai).Collection(s => s.Quotes).Load();
            _context.Entry(samurai).Reference(s => s.Horse).Load();
            Console.ReadLine();
        }

        private static void ExplicitLoadQuotesWithInclude()
        {
            //make sure there's a horse in the DB, then clear the context's change tracker
            //_context.Set<Horse>().Add(new Horse { SamuraiId = 1, Name = "Mr. Ed" });
            //_context.SaveChanges();
            //_context.ChangeTracker.Clear();
            //-------------------
            var samurai = _context.Samurais.Include(s => s.Quotes).FirstOrDefault(s => s.Id == 1);
            Console.ReadLine();

        }

        private static void AddAHorse()
        {
            //make sure there's a horse in the DB, then clear the context's change tracker
            var horse = new Horse { Name = "TimTom", SamuraiId = 2 };
            _context.Add(horse);
            _context.SaveChanges();
            //-------------------
            //var samurai = _context.Samurais.Find(1);
            //_context.Entry(samurai).Collection(s => s.Quotes).Load();
            //_context.Entry(samurai).Reference(s => s.Horse).Load();
        }
    }


}