﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Kahla.Server.Models
{
    public class GroupConversation : Conversation
    {
        [JsonIgnore]
        [InverseProperty(nameof(UserGroupRelation.Group))]
        public List<UserGroupRelation> Users { get; set; }
        public string GroupImage { get; set; }
        public string GroupName { get; set; }
        public override string GetDisplayImage(string userId) => GroupImage;
        public override string GetDisplayName(string userId) => GroupName;
        public override int GetUnReadAmount(string userId)
        {
            var relation = Users.SingleOrDefault(t => t.UserId == userId);
            return Messages.Where(t => t.SendTime > relation.ReadTimeStamp).Count();
        }

        public override Message GetLatestMessage()
        {
            try
            {
                return this.Messages.OrderByDescending(p => p.SendTime).First();
            }
            catch (InvalidOperationException)
            {
                return new Message
                {
                    Content = $"You have successfully joined {this.GroupName}!",
                    SendTime = this.ConversationCreateTime
                };
            }
        }
    }
}