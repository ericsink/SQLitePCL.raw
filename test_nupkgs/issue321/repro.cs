using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoreLinq.Extensions;

namespace WpfAppSqliteTesting
{
    public static class repro
    {
        static async Task go()
        {
            // Delete DB
            File.Delete("Test.db");
            File.Delete("Test.db-shm");
            File.Delete("Test.db-wal");

            // Migrate DB
            using (AppDbContext appDbContext = CreateAppDbContext())
            {
                appDbContext.Database.Migrate();
            }

            // Init
            int numberOfRecordsToCreate = 100000;
            List<Entity> entities = new List<Entity>();

            // Create Entities
            for (int i = 0; i < numberOfRecordsToCreate; i++)
            {
                entities.Add(new Entity()
                {
                    ClassId = random.Next(0, 9999999),
                    ClassName = RandomString(50),
                    GroupId = random.Next(0, 9999999),
                    GroupName = RandomString(50),
                    EventDateTimeUtc = DateTime.UtcNow,
                    EventDateTime = DateTime.Now,
                    CreatedDateTimeUtc = DateTime.UtcNow,
                    CreatedBy = RandomString(30)
                });
            }

            // Save Entities
            List<Task> tasks = new List<Task>();
            var entityBatches = entities.Batch(10_000).Select(_ => _.ToList()).ToList();
            foreach (var entityBatch in entityBatches)
            {
                Task task = Task.Run(async () =>
                {
                    using (AppDbContext appDbContext = CreateAppDbContext())
                    {
                        appDbContext.Entities.AddRange(entityBatch.ToArray());
                        await appDbContext.SaveChangesAsync();

                        entityBatch.Clear();
                    }
                });

                tasks.Add(task);
            }

            await Task.WhenAll(tasks.ToArray());
        }

        static AppDbContext CreateAppDbContext()
        {
            // Init
            var optionsBuilder =
                new DbContextOptionsBuilder<AppDbContext>()
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
                    ;

            optionsBuilder.UseSqlite($"Data Source=Test.db");

            return new AppDbContext(optionsBuilder.Options);
        }

        private static Random random = new Random();

        static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789■ ";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static void Main()
        {
            var num = 20;
            for (var i = 0; i<num; i++)
            {
                Console.Write($"{i + 1} of {num}: ");
                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                go().Wait();
                sw.Stop();
                var ts = sw.Elapsed;
                Console.WriteLine($"{ts.TotalSeconds} s", ts.TotalSeconds);
            }
        }
    }
}

