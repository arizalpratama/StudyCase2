using Confluent.Kafka;
using Confluent.Kafka.Admin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace KafkaCreateTopics
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "127.0.0.1:9092",
                ClientId = Dns.GetHostName(),

            };

            var topics = new List<String>();
            topics.Add("Logging");
            topics.Add("Register");
            topics.Add("ChangePassword");
            topics.Add("AddTweet");
            topics.Add("DeleteTweet");
            topics.Add("EditProfile");
            topics.Add("LockUser");
            topics.Add("ChangeUserRole");

            foreach (var topic in topics)
            {
                using (var adminClient = new AdminClientBuilder(config).Build())
                {
                    Console.WriteLine("Creating a topic . . . .");
                    try
                    {
                        await adminClient.CreateTopicsAsync(new List<TopicSpecification>
                        {
                            new TopicSpecification {Name = topic, NumPartitions = 1, ReplicationFactor = 1}
                        });
                    }
                    catch (CreateTopicsException ex)
                    {
                        if (ex.Results[0].Error.Code != ErrorCode.TopicAlreadyExists)
                        {
                            Console.WriteLine($"An error occured creating topic {topic}: {ex.Results[0].Error.Reason}");
                        }
                        else
                        {
                            Console.WriteLine($"Topic already exists");
                        }
                    }
                }
            }

            Trace.Flush();

            return 0;
        }
    }
}