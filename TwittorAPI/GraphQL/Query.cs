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
        public async Task<IQueryable<Twittor>> ShowAllTweets(
           [Service] TwittorDbContext context,
           [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var key = "GetTwittors-" + DateTime.Now.ToString();
            var val = JObject.FromObject(new { Message = "GraphQL Query GetTwittors" }).ToString(Formatting.None);

            await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);
            return context.Twittors;
        }

        //Get All Profile
        public async Task<IQueryable<Profile>> ShowAllProfiles(
          [Service] TwittorDbContext context,
          [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var key = "GetProfiles-" + DateTime.Now.ToString();
            var val = JObject.FromObject(new { Message = "GraphQL Query GetProfiles" }).ToString(Formatting.None);

            await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);
            return context.Profiles;
        }

        //Get All UserRole
        public async Task<IQueryable<UserRole>> ShowAllUserRoles(
          [Service] TwittorDbContext context,
          [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var key = "GetProfiles-" + DateTime.Now.ToString();
            var val = JObject.FromObject(new { Message = "GraphQL Query GetProfiles" }).ToString(Formatting.None);

            await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);
            return context.UserRoles;
        }

        //Get All User
        public async Task<IQueryable<User>> ShowAllUsers(
          [Service] TwittorDbContext context,
          [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var key = "GetProfiles-" + DateTime.Now.ToString();
            var val = JObject.FromObject(new { Message = "GraphQL Query GetProfiles" }).ToString(Formatting.None);

            await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);
            return context.Users;
        }

        //Get All Comments
        public async Task<IQueryable<Comment>> ShowAllComments(
          [Service] TwittorDbContext context,
          [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var key = "GetProfiles-" + DateTime.Now.ToString();
            var val = JObject.FromObject(new { Message = "GraphQL Query GetProfiles" }).ToString(Formatting.None);

            await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);
            return context.Comments;
        }
    }
}
