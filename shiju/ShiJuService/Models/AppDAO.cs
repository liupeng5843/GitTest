using ShiJu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShiJu.Service.Models
{
    public static class AppDAO
    {
        public static string GetPreferenceValue(ShiJuDbContext dbContext, string name)
        {
            //return dbContext.Preferences.Where(p => p.Name == name).Select(p => p.Value).FirstOrDefault() ?? string.Empty;
            return null;
        }
    }
}