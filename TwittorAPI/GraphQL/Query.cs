using HotChocolate;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using TwittorAPI.Kafka;
using TwittorAPI.Models;

namespace TwittorAPI.GraphQL
{
    public class Query
    {
        //Get All Twittor
        public async Task<IQueryable<Twittor>> GetTwittors(
           [Service] TwittorDbContext context,
           [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var key = "GetTwittors-" + DateTime.Now.ToString();
            var val = JObject.FromObject(new { Message = "GraphQL Query GetTwittors" }).ToString(Formatting.None);

            await KafkaHelper.SendMessage(kafkaSettings.Value, "logging", key, val);
            return context.Twittors;
        }

        //Get All Comment
        public async Task<IQueryable<Comment>> GetComments(
           [Service] TwittorDbContext context,
           [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var key = "GetComments-" + DateTime.Now.ToString();
            var val = JObject.FromObject(new { Message = "GraphQL Query GetComments" }).ToString(Formatting.None);

            await KafkaHelper.SendMessage(kafkaSettings.Value, "logging", key, val);
            return context.Comments;
        }

        //Get All Profile
        public async Task<IQueryable<Profile>> GetProfiles(
          [Service] TwittorDbContext context,
          [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var key = "GetProfiles-" + DateTime.Now.ToString();
            var val = JObject.FromObject(new { Message = "GraphQL Query GetProfiles" }).ToString(Formatting.None);

            await KafkaHelper.SendMessage(kafkaSettings.Value, "logging", key, val);
            return context.Profiles;
        }


    }
}
