using ShiJu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WindowsServer.Configuration;
using WindowsServer.Log;

namespace ShiJu.Service
{
    public static class Heart
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static string _connectionString;
        private static readonly long _verificationCodeExpireSeconds = long.Parse(ConfigurationCenter.Global["VerificationCodeExpireSeconds"] ?? "1800");//default 30mins
        private static readonly string _smsVerificationCodeTemplate = ConfigurationCenter.Global["SmsVerificationCodeTemplate"];
        private static readonly string _smsInviteFriendTemplate = ConfigurationCenter.Global["SmsInviteFriendTemplate"];
        private static readonly string _smsGreetTemplate = ConfigurationCenter.Global["SmsGreetTemplate"];

        private static readonly string _feedbackSmtpClientConfiguration = ConfigurationCenter.Global["FeedbackSmtpClient"];
        private static readonly string _imageBaseUrl = ConfigurationCenter.Global["ImageBaseUrl"];
        private static readonly string _getPartyUrl = ConfigurationCenter.Global["GetPartyUrl"];
        private static readonly string _serviceBaseUrl = ConfigurationCenter.Global["ServiceBaseUrl"];

        private static readonly string _shiJuNotificationBaseUrl = ConfigurationCenter.Global["ShiJuNotificationBaseUrl"];
        private static readonly string _wechatAppId = ConfigurationCenter.Global["WechatAppId"];
        private static readonly string _wechatSecret = ConfigurationCenter.Global["WechatSecret"];

        private static readonly string _greetingPartyTitle = ConfigurationCenter.Global["GreetingPartyTitle"];
        private static readonly string _greetingPartyImages = ConfigurationCenter.Global["greetingPartyImages"];



        public static void Initialize()
        {
            _connectionString = ConfigurationCenter.Global["ShiJuDb"];
        }

        public static ShiJuDbContext CreateShiJuDbContext()
        {
            var db = new ShiJuDbContext(_connectionString);
            db.Database.Log += (s) =>
            {
                Heart._logger.Debug(s.Replace('\r', ' ').Replace('\n', ' '));
            };
            return db;
        }

        public static string ConnectionString
        {
            get
            {
                return _connectionString;
            }
        }

        public static long VerificationCodeExpireSeconds
        {
            get { return _verificationCodeExpireSeconds; }
        }

        public static string SmsVerificationCodeTemplate
        {
            get { return _smsVerificationCodeTemplate; }
        }

        public static string SmsInviteFriendTemplate
        {
            get { return _smsInviteFriendTemplate; }
        }

        public static string SmsGreetTemplate
        {
            get { return _smsGreetTemplate; }
        }

        public static string FeedbackSmtpClientConfiguration
        {
            get { return _feedbackSmtpClientConfiguration; }
        }

        public static string ImageBaseUrl
        {
            get { return _imageBaseUrl; }
        }

        public static string GetPartyUrl
        {
            get { return _getPartyUrl; }
        }

        public static string ServiceBaseUrl
        {
            get { return _serviceBaseUrl; }
        }

        public static string ShiJuNotificationBaseUrl
        {
            get { return _shiJuNotificationBaseUrl; }
        }

        public static string WechatAppId
        {
            get { return _wechatAppId; }
        }

        public static string WechatSecret
        {
            get { return _wechatSecret; }
        }

        public static string GreetingPartyTitle
        {
            get { return _greetingPartyTitle; }
        }

        public static string GreetingPartyImages
        {
            get { return _greetingPartyImages; }
        }
    }
}
