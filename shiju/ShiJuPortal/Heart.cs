using Portal.Common.Models;
using ShiJu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WindowsServer.Configuration;
using WindowsServer.Log;

namespace Shiju.Portal
{
    public static class Heart
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static readonly string _audioText = ConfigurationCenter.Global["audioText"];
        public static string DbConnectionString { get; set; }
        public static string ShijuServiceBaseUrl { get; set; }

        public static void Initilize()
        {
            DbConnectionString = ConfigurationCenter.Global["ShiJuDbContextDb"];
            ShijuServiceBaseUrl = ConfigurationCenter.Global["ShiJuServiceBaseUrl"];
            PortalHelper.Initialize(DbConnectionString, 3);
        }

        public static ShiJuDbContext CreateShiJuDbContext()
        {
            var db = new ShiJuDbContext(DbConnectionString);
            db.Database.Log += (s) =>
            {
                Heart._logger.Debug(s.Replace('\r', ' ').Replace('\n', ' '));
            };
            return db;
        }

        public static string AudioText
        {
            get {return  _audioText;}
        }

        public static ShiJuDbContext CreatePortalDbContext()
        {
            var db = new ShiJuDbContext(DbConnectionString);
            db.Database.Log += (s) =>
            {
                Heart._logger.Debug(s.Replace('\r', ' ').Replace('\n', ' '));
            };
            return db;
        }
    }
}