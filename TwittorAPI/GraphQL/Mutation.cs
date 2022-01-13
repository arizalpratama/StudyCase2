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
    public class Mutation
    {
        //Add Twit
        public async Task<TransactionStatus> AddTwitAsync(
            TwittorInput input,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var twittor = new Twittor
            {
                Twit = input.Twit,
                Created = DateTime.Now
            };
            var key = "twittor-add-" + DateTime.Now.ToString();
            var val = JObject.FromObject(twittor).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "twittor-add", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to submit data");

            return await Task.FromResult(ret);
        }

        //Add Comment
        public async Task<TransactionStatus> AddCommentAsync(
            CommentInput input,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var comment = new Comment
            {
                TwitId = input.TwitId,
                Comments = input.Comments,
                Created = DateTime.Now
            };
            var key = "comment-add-" + DateTime.Now.ToString();
            var val = JObject.FromObject(comment).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "comment-add", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to submit data");

            return await Task.FromResult(ret);
        }

        //Delete Twit
        public async Task<TransactionStatus> DeleteTwitByIdAsync(
          int id,
          [Service] TwittorDbContext context,
          [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var twittor = context.Twittors.Where(o => o.Id == id).FirstOrDefault();
            if (twittor != null)
            {
                context.Twittors.Remove(twittor);
                await context.SaveChangesAsync();
            }
            var key = "twittor-delete-" + DateTime.Now.ToString();
            var val = JObject.FromObject(twittor).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "twittor-delete", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to delete data");


            return await Task.FromResult(ret);
        }

        //Add Profile
        public async Task<TransactionStatus> AddProfileAsync(
            ProfileInput input,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var profile = new Profile
            {
                UserId = input.UserId,
                Name = input.Name
            };
            var key = "profile-add-" + DateTime.Now.ToString();
            var val = JObject.FromObject(profile).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "profile-add", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to submit data");

            return await Task.FromResult(ret);
        }

        //Update Profile
        public async Task<TransactionStatus> UpdateProfileAsync(
            ProfileInput input,
            [Service] TwittorDbContext context,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var profile = context.Profiles.Where(o => o.Id == input.Id).FirstOrDefault();
            if (profile != null)
            {
                profile.UserId = input.UserId;
                profile.Name = input.Name;

                context.Profiles.Update(profile);
                await context.SaveChangesAsync();
            }
            var key = "profile-update-" + DateTime.Now.ToString();
            var val = JObject.FromObject(profile).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "profile-update", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to update data");


            return await Task.FromResult(ret);
        }
    }
}
