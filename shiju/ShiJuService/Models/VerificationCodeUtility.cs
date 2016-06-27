using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShiJu.Service.Models
{
    public class VerificationCodeUtility
    {
        public static string GenerateCode()
        {
            System.Random r = new System.Random((int)System.DateTime.Now.Ticks);
            return r.Next(100000, 999999).ToString();
        }
    }
}