using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TwittorAPI.Kafka;
using TwittorAPI.Models;

namespace TwittorAPI.GraphQL
{
    public class Mutation
    {

        //===== *Twittor* =====//

        //Add Twit Payload
        [Authorize(Roles = new[] { "MEMBER" })]
        public async Task<AddTwittorPayload> AddTwittorPayloadAsync(
            TwittorInput input,
            [Service] TwittorDbContext context)
        {
            var twittor = new Twittor
            {
                UserId = input.UserId,
                Twit = input.Twit,
                Created = DateTime.Now
            };

            context.Twittors.Add(twittor);
            await context.SaveChangesAsync();

            return new AddTwittorPayload(twittor);
        }

        //Add Twit Kafka
        [Authorize(Roles = new[] { "MEMBER" })]
        public async Task<TransactionStatus> AddTwittorAsync(
           TwittorInput input,
           [Service] TwittorDbContext context,
           [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var twittor = new Twittor
            {
                UserId = input.UserId,
                Twit = input.Twit,
                Created = DateTime.Now
            };
            var key = "Add-Twit-" + DateTime.Now.ToString();
            var val = JObject.FromObject(twittor).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "twittor-add", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to submit data");

            return await Task.FromResult(ret);
        }

        //Update Twit
        [Authorize(Roles = new[] { "MEMBER" })]
        public async Task<TransactionStatus> UpdateTwittorAsync(
            TwittorInput input,
            [Service] TwittorDbContext context,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var twittor = context.Twittors.Where(o => o.Id == input.Id).FirstOrDefault();
            if (twittor != null)
            {
                twittor.UserId = input.UserId;
                twittor.Twit = input.Twit;
                twittor.Created = DateTime.Now;

                context.Twittors.Update(twittor);
                await context.SaveChangesAsync();
            }
            var key = "twittor-update-" + DateTime.Now.ToString();
            var val = JObject.FromObject(twittor).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "profile-update", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to update data");


            return await Task.FromResult(ret);
        }

        //Delete Twit
        [Authorize(Roles = new[] { "MEMBER" })]
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

        //===== *Comment* =====//

        //Add Comment Payload
        [Authorize(Roles = new[] { "MEMBER" })]
        public async Task<AddCommentPayload> AddCommentPayloadAsync(
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

            return new AddCommentPayload(comment);
        }

        //Add Comment Kafka
        [Authorize(Roles = new[] { "MEMBER" })]
        public async Task<TransactionStatus> AddCommentAsync(
            CommentInput input,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var comment = new Comment
            {
                UserId = input.UserId,
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

        //Update Comment
        [Authorize(Roles = new[] { "MEMBER" })]
        public async Task<TransactionStatus> UpdateCommentAsync(
            CommentInput input,
            [Service] TwittorDbContext context,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var comment = context.Comments.Where(o => o.Id == input.Id).FirstOrDefault();
            if (comment != null)
            {
                comment.UserId = input.UserId;
                comment.TwitId = input.TwitId;
                comment.Comments = input.Comments;
                comment.Created = DateTime.Now;

                context.Comments.Update(comment);
                await context.SaveChangesAsync();
            }
            var key = "comment-update-" + DateTime.Now.ToString();
            var val = JObject.FromObject(comment).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "profile-update", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to update data");


            return await Task.FromResult(ret);
        }

        //Delete Comment
        [Authorize(Roles = new[] { "MEMBER" })]
        public async Task<TransactionStatus> DeleteCommentByIdAsync(
          int id,
          [Service] TwittorDbContext context,
          [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var comment = context.Comments.Where(o => o.Id == id).FirstOrDefault();
            if (comment != null)
            {
                context.Comments.Remove(comment);
                await context.SaveChangesAsync();
            }
            var key = "comment-delete-" + DateTime.Now.ToString();
            var val = JObject.FromObject(comment).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "twittor-delete", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to delete data");


            return await Task.FromResult(ret);
        }

        //===== *Profile* =====//

        //Add Profile Payload
        [Authorize(Roles = new[] { "ADMIN","MEMBER" })]
        public async Task<AddProfilePayload> AddProfilePayloadAsync(
            ProfileInput input,
            [Service] TwittorDbContext context)
        {
            var profile = new Profile
            {
                UserId = input.UserId,
                Name = input.Name,
                Age = input.Age
            };

            context.Profiles.Add(profile);
            await context.SaveChangesAsync();

            return new AddProfilePayload(profile);
        }

        //Add Profile
        [Authorize(Roles = new[] { "ADMIN", "MEMBER" })]
        public async Task<TransactionStatus> AddProfileAsync(
            ProfileInput input,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var profile = new Profile
            {
                UserId = input.UserId,
                Name = input.Name,
                Age = input.Age
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
        [Authorize(Roles = new[] { "ADMIN", "MEMBER" })]
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
                profile.Age = input.Age;

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

        //Delete Profile
        [Authorize(Roles = new[] { "ADMIN", "MEMBER" })]
        public async Task<TransactionStatus> DeleteProfileByIdAsync(
          int id,
          [Service] TwittorDbContext context,
          [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var profile = context.Profiles.Where(o => o.Id == id).FirstOrDefault();
            if (profile != null)
            {
                context.Profiles.Remove(profile);
                await context.SaveChangesAsync();
            }
            var key = "profile-delete-" + DateTime.Now.ToString();
            var val = JObject.FromObject(profile).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "twittor-delete", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to delete data");


            return await Task.FromResult(ret);
        }


        //===== *User Role* =====//

        //Add User Role Payload
        [Authorize(Roles = new[] { "ADMIN"})]
        public async Task<AddUserRolePayload> AddUserRolePayloadAsync(
            UserRoleInput input,
            [Service] TwittorDbContext context)
        {
            var userrole = new UserRole
            {
                UserId = input.UserId,
                RoleId = input.UserId
            };

            context.UserRoles.Add(userrole);
            await context.SaveChangesAsync();

            return new AddUserRolePayload(userrole);
        }

        //Add User Role
        [Authorize(Roles = new[] { "ADMIN" })]
        public async Task<TransactionStatus> AddUserRoleAsync(
            UserRoleInput input,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var userrole = new UserRole
            {
                UserId = input.UserId,
                RoleId = input.UserId
            };
            var key = "userrole-add-" + DateTime.Now.ToString();
            var val = JObject.FromObject(userrole).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "userrole-add", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to submit data");


            return await Task.FromResult(ret);
        }

        //Update User Role
        [Authorize(Roles = new[] { "ADMIN" })]
        public async Task<TransactionStatus> UpdateUserRoleAsync(
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
            var key = "userrole-update-" + DateTime.Now.ToString();
            var val = JObject.FromObject(userrole).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "userrole-update", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to update data");


            return await Task.FromResult(ret);
        }

        //Delete User Role
        [Authorize(Roles = new[] { "ADMIN" })]
        public async Task<TransactionStatus> DeleteUserRoleByIdAsync(
          int id,
          [Service] TwittorDbContext context,
          [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var userrole = context.UserRoles.Where(o => o.Id == id).FirstOrDefault();
            if (userrole != null)
            {
                context.UserRoles.Remove(userrole);
                await context.SaveChangesAsync();
            }
            var key = "userrole-delete-" + DateTime.Now.ToString();
            var val = JObject.FromObject(userrole).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "twittor-delete", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to delete data");


            return await Task.FromResult(ret);
        }

        //===== *User* =====

        //Register
        public async Task<UserData> RegisterUserAsync(
            RegisterUser input,
            [Service] TwittorDbContext context)
        {
            var user = context.Users.Where(o => o.Username == input.UserName).FirstOrDefault();
            if (user != null)
            {
                return await Task.FromResult(new UserData());
            }
            var newUser = new User
            {
                FullName = input.FullName,
                Email = input.Email,
                Username = input.UserName,
                Password = BCrypt.Net.BCrypt.HashPassword(input.Password)
            };

            var ret = context.Users.Add(newUser);
            await context.SaveChangesAsync();

            return await Task.FromResult(new UserData
            {
                Id = newUser.Id,
                Username = newUser.Username,
                Email = newUser.Email,
                FullName = newUser.FullName
            });
        }

        //Update Password
        [Authorize(Roles = new[] { "MEMBER", "ADMIN" })]
        public async Task<TransactionStatus> UpdateUserPasswordAsync(
            int id,
            UpdatePasswordInput input,
            [Service] TwittorDbContext context,
            [Service] IOptions<KafkaSettings> kafkaSettings)
        {
            var user = context.Users.Where(o => o.Id == id).FirstOrDefault();
            if (user == null) return await Task.FromResult(new TransactionStatus(false, "User not found"));

            if (user != null)
            {
                var valid = BCrypt.Net.BCrypt.Verify(input.oldPassword, user.Password);
                if (valid)
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(input.newPassword);
                }
                else return new TransactionStatus(false, "Invalid password");
            }

            var key = "Update-User-Password-" + DateTime.Now.ToString();
            var val = JObject.FromObject(user).ToString(Formatting.None);
            var result = await KafkaHelper.SendMessage(kafkaSettings.Value, "user-update", key, val);
            await KafkaHelper.SendMessage(kafkaSettings.Value, "logging", key, val);

            var ret = new TransactionStatus(result, "");
            if (!result)
                ret = new TransactionStatus(result, "Failed to submit data");

            return await Task.FromResult(ret);
        }


        //Login
        public async Task<UserToken> LoginAsync(
           LoginUser input,
           [Service] IOptions<TokenSettings> tokenSettings,
           [Service] TwittorDbContext context)
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

                foreach (var userRole in user.UserRoles)
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

                return await Task.FromResult(
                    new UserToken(new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    expired.ToString(), null));
            }

            return await Task.FromResult(new UserToken(null, null, Message: "Username or password was invalid"));
        }

    }
}
