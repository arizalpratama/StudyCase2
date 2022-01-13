using Confluent.Kafka;
using KafkaApp.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaApp
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                    .AddJsonFile($"appsettings.json", true, true);

            var config = builder.Build();

            var ServerConfig = new ConsumerConfig
            {
                BootstrapServers = config["Settings:KafkaServer"],
                GroupId = "tester",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating (Ctrl +C)
                cts.Cancel();
            };

            Console.WriteLine("----------------.NET Application-------------");
            using (var consumer = new ConsumerBuilder<string, string>(ServerConfig).Build())
            {
                Console.WriteLine("KafkaApp Connected");
                var topics = new string[] { "twittor-add", "comment-add", "twittor-delete", 
                    "profile-add", "profile-update", "userrole-add", "userrole-update", "user-update"};
                consumer.Subscribe(topics);

                Console.WriteLine("Waiting messages....");
                try
                {
                    while (true)
                    {
                        var cr = consumer.Consume(cts.Token);
                        Console.WriteLine($"Consumed record with Topic: {cr.Topic} key: {cr.Message.Key} and value: {cr.Message.Value}");

                        using (var dbcontext = new TwittorDbContext())
                        {
                            if (cr.Topic == "twittor-add")
                            {
                                Twittor twittor = JsonConvert.DeserializeObject<Twittor>(cr.Message.Value);
                                dbcontext.Twittors.Add(twittor);
                            }
                            if (cr.Topic == "comment-add")
                            {
                                Comment comment = JsonConvert.DeserializeObject<Comment>(cr.Message.Value);
                                dbcontext.Comments.Add(comment);
                            }
                            if (cr.Topic == "twittor-delete")
                            {
                                Twittor twittor = JsonConvert.DeserializeObject<Twittor>(cr.Message.Value);
                                dbcontext.Twittors.Remove(twittor);
                            }
                            if (cr.Topic == "profile-add")
                            {
                                Profile profile = JsonConvert.DeserializeObject<Profile>(cr.Message.Value);
                                dbcontext.Profiles.Update(profile);
                            }
                            if (cr.Topic == "profile-update")
                            {
                                Profile profile = JsonConvert.DeserializeObject<Profile>(cr.Message.Value);
                                dbcontext.Profiles.Update(profile);
                            }
                            if (cr.Topic == "userrole-add")
                            {
                                UserRole userRole = JsonConvert.DeserializeObject<UserRole>(cr.Message.Value);
                                dbcontext.UserRoles.Update(userRole);
                            }
                            if (cr.Topic == "userrole-update")
                            {
                                UserRole userRole = JsonConvert.DeserializeObject<UserRole>(cr.Message.Value);
                                dbcontext.UserRoles.Update(userRole);
                            }
                            if (cr.Topic == "user-update")
                            {
                                User user = JsonConvert.DeserializeObject<User>(cr.Message.Value);
                                dbcontext.Users.Update(user);
                            }

                            await dbcontext.SaveChangesAsync();
                            Console.WriteLine("Data was saved into database");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ctrl+C was pressed.
                }
                finally
                {
                    consumer.Close();
                }

            }

            return 1;

        }
    }
}