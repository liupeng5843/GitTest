using ShiJu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShiJu.Service.Controllers
{
    public static class Formats
    {
        public static User.Format UserBrief = new User.Format()
        {
            Id = true,
            Status = true,
            PhoneNumber = true,
            NickName = true,
            Signature = true,
            Portrait = true,
            Gender = true,
            District = true,
            NeedNotification = true,
        };

        public static User.Format UserDetail = new User.Format()
        {
            Id = true,
            Status = true,
            PhoneNumber = true,
            NickName = true,
            Signature = true,
            Portrait = true,
            Gender = true,
            District = true,
            BackgroundImage = true,
            CreatedTime = true,
            SignUpTime = true,
            IsFriend = true,
            NeedNotification = true,
        };

        public static Account.Format AccountDetail = new Account.Format()
        {
            Id = true,
            UserId = true,
            Type = true,
            Name = true,
            Password = true,
            CreatedTime = true,
        };

        public static Account.Format AccountBrief = new Account.Format()
        {
            Id = true,
            Type = true,
            Name = true,
            CreatedTime = true,
        };

        public static FriendRequest.Format FriendRequestDetail = new FriendRequest.Format()
        {
            SourceUserId = true,
            TargetUserId = true,
            Text = true,
            CreatedTime = true,
            Status = true,
            TargetUnread = true,
            SourceUnread = true,
            SourceUser = new User.Format
            {
                Id = true,
                Status = true,
                PhoneNumber = true,
                NickName = true,
                Signature = true,
                Portrait = true,
                Gender = true,
            }
        };

        public static FriendRequest.Format FriendRequestBrief = new FriendRequest.Format()
        {
            SourceUserId = true,
            TargetUserId = true,
            Text = true,
            CreatedTime = true,
            Status = true,
            TargetUnread = true,
            SourceUnread = true,
        };

        public static Party.Format PartySmall = new Party.Format()
        {
            Id = true,
            BeginTime = true,
            EndTime = true,
            CreatedTime = true,
        };

        public static Party.Format PartyBrief = new Party.Format()
        {
            Id = true,
            CreatorUserId = true,
            Sponsor = true,
            BeginTime = true,
            EndTime = true,
            Title = true,
            Address = true,
            Images = true,
            Kind = true,
            MaxUserCount = true,
            ParticipantCount = true,
            IsPublic = true,
            LikeCount = true,
            CommentCount = true,
            CreatedTime = true,
            CreatorUser = new User.Format
            {
                Id = true,
                Status = true,
                PhoneNumber = true,
                NickName = true,
                Signature = true,
                Portrait = true,
                Gender = true,
            }
        };

        public static Party.Format PartyDetail = new Party.Format()
        {
            Id = true,
            CreatorUserId = true,
            Sponsor = true,
            BeginTime = true,
            EndTime = true,
            Title = true,
            Description = true,
            Address = true,
            Images = true,
            Kind = true,
            MaxUserCount = true,
            DirectFriendVisible = true,
            IsPublic = true,
            LikeCount = true,
            CommentCount = true,
            VoteTitle = true,
            VoteChoicesJson = true,
            VoteResult0Count = true,
            VoteResult1Count = true,
            VoteResult2Count = true,
            VoteResult3Count = true,
            VoteResult4Count = true,
            IsDisabled = true,
            CreatedTime = true,
            IsUserLiked = true,
            IsUserVoted = true,
            ParticipantCount = true,
            UserParticipant = new Participant.Format()
            {
                UserId = true,
                ProposedBeginTime = true,
                ProposedEndTime = true,
                Status = true,
            },
            Participants = new Participant.Format()
            {
                UserId = true,
                ProposedBeginTime = true,
                ProposedEndTime = true,
                Status = true,
                User = new User.Format()
                {
                    Id = true,
                    Portrait = true,
                    NickName = true,
                }
            },
            PartyComments = new PartyComment.Format()
            {
                Id = true,
                Text = true,
                AudioJson = true,
                VoteResult = true,
                CreatedTime = true,
                User = new User.Format()
                {
                    Id = true,
                    NickName = true,
                    Portrait = true,
                },
                TargetUser = new User.Format()
                {
                    Id = true,
                    NickName = true,
                    Portrait = true,
                }
            },
            CreatorUser = new User.Format
            {
                Id = true,
                Status = true,
                PhoneNumber = true,
                NickName = true,
                Signature = true,
                Portrait = true,
                Gender = true,
            }

        };

        public static Participant.Format ParticipantBrief = new Participant.Format()
        {
            Status = true,
            ProposedBeginTime = true,
            ProposedEndTime = true,
            Unread = true,
            CreatedTime = true,
            User = new User.Format()
            {
                Id = true,
                NickName = true,
                Portrait = true,
                District = true,
            },
        };


        public static Participant.Format ParticipantDetial = new Participant.Format()
        {
            Status = true,
            ProposedBeginTime = true,
            ProposedEndTime = true,
            Unread = true,
            CreatedTime = true,
            User = new User.Format()
            {
                Id = true,
                NickName = true,
                Portrait = true,
            },
            Party = new Party.Format()
            {
                Id = true,
                CreatorUserId = true,
                BeginTime = true,
                EndTime = true,
                Title = true,
                Address = true,
                Images = true,
                Kind = true,
                MaxUserCount = true,
                LikeCount = true,
                CommentCount = true,
                CreatedTime = true,
                CreatorUser = new User.Format
                {
                    Id = true,
                    Status = true,
                    PhoneNumber = true,
                    NickName = true,
                    Signature = true,
                    Portrait = true,
                    Gender = true,
                }
            }
        };

        public static PartyComment.Format PartyCommentBrief = new PartyComment.Format()
        {
            Id = true,
            Text = true,
            AudioJson = true,
            VoteResult = true,
            CreatedTime = true,
            User = new User.Format()
            {
                Id = true,
                NickName = true,
                Portrait = true,
            },
            TargetUser = new User.Format()
            {
                Id = true,
                NickName = true,
                Portrait = true,
            },
        };

        public static UserGreeting.Format UesrGreetingDetail = new UserGreeting.Format()
        {
            AgreeCount = true,
            HasNewGreeting = true,
            HasRead = true,
            LastModifiedTime = true,
            SourceUserId = true,
            TargetUserId = true,
            TotalCount = true,
            SourceUser = new User.Format()
            {
                Id = true,
                NickName = true,
                Portrait = true,
            }
        };


        public static Greeting.Format GreetingDetail = new Greeting.Format()
        {
            CreatedTime = true,
            IsAgreed = true,
            SourceUserId = true,
            TargetPhoneNumber = true,
            Id = true,
            TargetUserId = true,
            SourceUser = new User.Format()
            {
                Id = true,
                NickName = true,
                Portrait = true,
            }
        };

    }
}