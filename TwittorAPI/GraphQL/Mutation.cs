using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TwittorAPI.Kafka;
using TwittorAPI.Models;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwittorAPI.GraphQL
{
    public class Mutation
    {
        //Register
        public async Task<TransactionStatus> RegisterUserAsync(
            RegisterUser input,
            [Service] TwittorDbContext context,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var user = context.Users.Where(o => o.Username == input.UserName).FirstOrDefault();
            if (user != null)
            {
                return new TransactionStatus(false, "Username already exist");
            }
            var newUser = new User
            {
                FullName = input.FullName,
                Email = input.Email,
                Username = input.UserName,
                Password = BCrypt.Net.BCrypt.HashPassword(input.Password)
            };
            var key = "User-Add-" + DateTime.Now.ToString();
            var val = JObject.FromObject(newUser).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "Register", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to submit data");

            return await Task.FromResult(ret);
        }

        //Login
        public async Task<UserToken> LoginAsync(
            LoginUser input,
            [Service] IOptions<TokenSettings> tokenSettings,
            [Service] TwittorDbContext context,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var user = context.Users.Where(o => o.Username == input.Username).FirstOrDefault();
            if (user == null)
            {
                return await Task.FromResult(new UserToken(null, null, "Username or password was invalid"));
            }
            bool valid = BCrypt.Net.BCrypt.Verify(input.Password, user.Password);
            if (valid)
            {
                var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Value.Key));
                var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, user.Username));

                var userRoles = context.UserRoles.Where(o => o.UserId == user.Id).ToList();

                foreach (var userRole in userRoles)
                {
                    var role = context.Roles.Where(o => o.Id == userRole.RoleId).FirstOrDefault();
                    if (role != null)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.Name));
                    }
                }

                var expired = DateTime.Now.AddHours(3);
                var jwtToken = new JwtSecurityToken(
                    issuer: tokenSettings.Value.Issuer,
                    audience: tokenSettings.Value.Audience,
                    expires: expired,
                    claims: claims,
                    signingCredentials: credentials
                );

                var key = "SignIn-" + DateTime.Now.ToString();
                var val = JObject.FromObject(new { Message = $"{input.Username} has signed in" }).ToString(Formatting.None);
                await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);

                return await Task.FromResult(
                    new UserToken(new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    expired.ToString(), null));
            }

            return await Task.FromResult(new UserToken(null, null, Message: "Username or password was invalid"));
        }

        //Change Password
        [Authorize(Roles = new[] { "ADMIN" })]
        public async Task<TransactionStatus> ChangePasswordAsync(
            ChangePasswordInput input,
            [Service] TwittorDbContext context,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var user = context.Users.Where(o => o.Username == input.Username).FirstOrDefault();
            if (user != null)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);
                var key = "Change-Password-" + DateTime.Now.ToString();
                var val = JObject.FromObject(user).ToString(Formatting.None);
                var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "ChangePassword", key, val);
                await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);

                var ret = new TransactionStatus(result, "");
                if (!result)
                    ret = new TransactionStatus(result, "Failed to submit data");
                return await Task.FromResult(ret);
            }
            else
            {
                return new TransactionStatus(false, "User doesn't exist");
            }
        }

        //Add Tweet
        [Authorize(Roles = new[] { "MEMBER" })]
        public async Task<TransactionStatus> AddTweetAsync(
            TwittorInput input,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var twittor = new Twittor
            {
                UserId = input.UserId,
                Twit = input.Twit,
                Created = DateTime.Now
            };
            var key = "Tweet-Add-" + DateTime.Now.ToString();
            var val = JObject.FromObject(twittor).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "AddTweet", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to submit data");
            return await Task.FromResult(ret);
        }

        //Comment Tweet
        [Authorize(Roles = new[] { "MEMBER" })]
        public async Task<CommentTweet> CommentTweetAsync(
           CommentInput input,
           [Service] TwittorDbContext context)
        {
            var comment = new Comment
            {
                UserId = input.UserId,
                TwitId = input.TwitId,
                Comments = input.Comments,
                Created = DateTime.Now
            };

            context.Comments.Add(comment);
            await context.SaveChangesAsync();

            return new CommentTweet(comment);
        }

        //Delete Tweet
        [Authorize(Roles = new[] { "MEMBER" })]
        public async Task<TransactionStatus> DeleteTweetAsync(
             int userId,
             [Service] TwittorDbContext context,
             [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var twittor = context.Twittors.Where(o => o.UserId == userId).ToList();
            bool check = false;
            if (twittor != null)
            {
                foreach (var twit in twittor)
                {
                    var key = "Delete-Tweet-" + DateTime.Now.ToString();
                    var val = JObject.FromObject(twit).ToString(Formatting.None);
                    var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "DeleteTweet", key, val);
                    await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);
                    var ret = new TransactionStatus(result, "");
                    check = true;
                }

                if (!check)
                    return new TransactionStatus(false, "Failed to submit data");
                return await Task.FromResult(new TransactionStatus(true, ""));
            }
            else
            {
                return new TransactionStatus(false, "User has zero twit");
            }
        }

        //Edit Profile
        [Authorize(Roles = new[] { "ADMIN", "MEMBER" })]
        public async Task<TransactionStatus> EditProfileAsync(
            ProfileInput input,
            [Service] TwittorDbContext context,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var profile = context.Profiles.Where(o => o.Id == input.Id).FirstOrDefault();
            if (profile != null)
            {
                profile.UserId = input.UserId;
                profile.Name = input.Name;
                profile.Age = input.Age;

                context.Profiles.Update(profile);
                await context.SaveChangesAsync();
            }
            var key = "Profile-Edit-" + DateTime.Now.ToString();
            var val = JObject.FromObject(profile).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "EditProfile", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to update data");


            return await Task.FromResult(ret);
        }

        //Lock User
        [Authorize(Roles = new[] { "ADMIN" })] 
        public async Task<TransactionStatus> LockUserAsync(
            int userId,
            [Service] TwittorDbContext context,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var userRoles = context.UserRoles.Where(o => o.UserId == userId).ToList();
            bool check = false;
            if (userRoles != null)
            {
                foreach (var userRole in userRoles)
                {
                    var key = "Lock-User-" + DateTime.Now.ToString();
                    var val = JObject.FromObject(userRole).ToString(Formatting.None);
                    var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "LockUser", key, val);
                    await KafkaHelper.SendMessage(kafkaSettings.Value, "Logging", key, val);
                    var ret = new TransactionStatus(result, "");
                    check = true;
                };

                if (!check)
                    return new TransactionStatus(false, "Failed to submit data");
                return await Task.FromResult(new TransactionStatus(true, ""));
            }
            else
            {
                return new TransactionStatus(false, "User doesnt have any role yet");
            }
        }

        //Change User Role
        [Authorize(Roles = new[] { "ADMIN" })]
        public async Task<TransactionStatus> ChangeUserRoleAsync(
           UserRoleInput input,
           [Service] TwittorDbContext context,
           [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var userrole = context.UserRoles.Where(o => o.Id == input.Id).FirstOrDefault();
            if (userrole != null)
            {
                userrole.UserId = input.UserId;
                userrole.RoleId = input.RoleId;

                context.UserRoles.Update(userrole);
                await context.SaveChangesAsync();
            }
            var key = "Change-User-Role-" + DateTime.Now.ToString();
            var val = JObject.FromObject(userrole).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "ChangeUserRole", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to update data");


            return await Task.FromResult(ret);
        }
    }
}
