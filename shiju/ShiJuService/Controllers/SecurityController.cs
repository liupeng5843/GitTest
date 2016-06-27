using SACommon.Models;
using ShiJu.Models;
using ShiJu.Service.Models;
using ShiJu.Utils;
using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WindowsServer.Json;
using WindowsServer.Log;

namespace ShiJu.Service.Controllers
{
    public enum VerifiicationCodeType : long
    {
        SignUp = 0,
        ForgetPassword = 10
    }
    public class SecurityController : Controller
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [HttpPost]
        [Route("Security/{phoneNumber}/GenerateVerificationCode")]
        public ActionResult GenerateVerificationCode(string phoneNumber, long type)
        {
            var code = VerificationCodeUtility.GenerateCode();

            using (var db = Heart.CreateShiJuDbContext())
            {
                var account = db.Accounts.FirstOrDefault(a => a.Name == phoneNumber && a.Type == AccountType.PhoneNumber);
                if (type == (long)VerifiicationCodeType.SignUp && account != null)
                {
                    return this.JsonResult(-1, "This phoneNumber has been signed up in newbe.");
                }
                else if (type == (long)VerifiicationCodeType.ForgetPassword && account == null)
                {
                    return this.JsonResult(-2, "This phoneNumber has not been signed up in newbe.");
                }

                var verificationCode = db.VerificationCodes.Find(phoneNumber);
                if (verificationCode == null)
                {
                    verificationCode = new VerificationCode()
                    {
                        PhoneNumber = phoneNumber,
                        Code = code,
                        CreatedTime = DateTime.UtcNow,
                    };
                    db.VerificationCodes.Add(verificationCode);
                }
                else
                {
                    verificationCode.Code = code;
                    verificationCode.CreatedTime = DateTime.UtcNow;
                }

                var user = db.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);

                if (user == null)
                {
                    user = new User()
                    {
                        Id = Guid.NewGuid(),
                        Status = UserStatus.Inactive,
                        PhoneNumber = phoneNumber,
                        NickName = string.Empty,
                        Signature = string.Empty,
                        Portrait = Guid.Empty,
                        Gender = UserGender.Unknown,
                        District = string.Empty,
                        NeedNotification = true,
                        BackgroundImage = Guid.Empty,
                        CreatedTime = DateTime.UtcNow,
                        SignUpTime = DateTime.UtcNow,
                    };
                    db.Users.Add(user);
                }
                db.SaveChanges();
            }

            var result = SmsUtility.getBalance();
            _logger.Info("send message: dwCorpId:" + result.dwCorpId +
                "|nResult:" + result.nResult +
                "nStaffId:" + result.nStaffId +
                "fRemain:" + result.fRemain);

            try
            {
                var respCode = SmsUtility.sendOnce(phoneNumber, GetVerificationCodeMessage(code));
                _logger.Info("send message return code:" + respCode);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("error on sending message to " + phoneNumber, ex);
            }
            return this.JsonResponseJson(0, "code", code);
        }


        [Route("Validate/{phoneNumber}/Code/{verificationcode}")]
        public ActionResult VerifyCode(string phoneNumber, string verificationcode)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                // Check verification code
                if (!ValidateVerificationCode(db, phoneNumber, verificationcode))
                {
                    return this.JsonResult(-1, "Invalid verification code.");
                }
            }
            return this.JsonResult(0, "Succeeded");
        }

        [HttpPost]
        [Route("Users/SignUpWithPhoneNumber")]
        public ActionResult SignUpWithPhoneNumber()
        {
            var utcNow = DateTime.UtcNow;

            var json = this.GetRequestContentJson() as JsonObject;
            var phoneNumber = json.GetStringValue("PhoneNumber");
            var password = json.GetStringValue("Password");
            var verificationCode = json.GetStringValue("VerificationCode");

            using (var db = Heart.CreateShiJuDbContext())
            {
                // Check verification code
                if (!ValidateVerificationCode(db, phoneNumber, verificationCode))
                {
                    return this.JsonResult(-1, "Invalid verification code.");
                }

                // Check if this account exist
                var accountExist = (from a in db.Accounts
                                    where (a.Type == AccountType.PhoneNumber) && (a.Name == phoneNumber)
                                    select a).Any();
                if (accountExist)
                {
                    return this.JsonResult(-2, "The account with the same phone number exists.");
                }

                var user = db.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber && u.Status==UserStatus.Inactive);
                if (user == null)
                {
                    user = new User()
                    {
                        Id = Guid.NewGuid(),
                        Status = UserStatus.Active,
                        PhoneNumber = phoneNumber,
                        NickName = string.Empty,
                        Signature = string.Empty,
                        Portrait = Guid.Empty,
                        Gender = UserGender.Unknown,
                        District = string.Empty,
                        NeedNotification = true,
                        BackgroundImage = Guid.Empty,
                        CreatedTime = utcNow,
                        SignUpTime = utcNow,
                    };
                    db.Users.Add(user);
                }
                
                user.Status = UserStatus.Active;
                user.NeedNotification = true;

                var account = new Account()
                {
                    Id = Guid.NewGuid(),
                    Type = AccountType.PhoneNumber,
                    UserId = user.Id,
                    Name = phoneNumber,
                    Password = password,
                    CreatedTime = utcNow,
                };

                db.Accounts.Add(account);
                db.SaveChanges();

                var authenticationAccessToken = AccessTokenManager.AddAccessToken(user.Id, Request.UserHostAddress, Request.UserAgent);

                var writer = new JsonWriter();
                ShiJu.Models.User.Serialize(writer, user, Formats.UserDetail);
                var extraJsons = new Dictionary<string, string>();
                extraJsons["User"] = writer.ToString();
                var extraValues = new Dictionary<string, object>();
                extraValues["AccessToken"] = authenticationAccessToken.AccessToken;

                return this.JsonResponseJson(0, "Succeeded", extraJsons, extraValues);
            }

        }

        private static bool ValidateVerificationCode(ShiJuDbContext db, string phoneNumber, string verificationCode)
        {
            const int verificationCodeExpireInMinites = 30;
            var vefificationeCodeTime = DateTime.UtcNow.AddMinutes(0 - verificationCodeExpireInMinites);
            return (from vc in db.VerificationCodes
                    where (vc.PhoneNumber == phoneNumber) && (vc.Code == verificationCode) && (vc.CreatedTime > vefificationeCodeTime)
                    select vc).Any();
        }

        [HttpPost]
        [Route("User/SignInWithPhoneNumber")]
        public ActionResult SignInWithPhoneNumber()
        {
            var json = this.GetRequestContentJson() as JsonObject;
            var phoneNumber = json.GetStringValue("PhoneNumber");
            var password = json.GetStringValue("Password");

            using (var db = Heart.CreateShiJuDbContext())
            {
                var account = (from a in db.Accounts
                               where (a.Type == AccountType.PhoneNumber) && (a.Name == phoneNumber)
                               select a).FirstOrDefault();

                if (account == null)
                {
                    return this.JsonResult(-3, "Invalid  accountName.");
                }
                
                if (account.Password != password)
                {
                    return this.JsonResult(-1, "Invalid password or phone number does not exist.");
                }
                
                var user = db.Users.Find(account.UserId);
                if (user.Status == UserStatus.Disabled)
                {
                    return this.JsonResult(-2, "The user has been disabled.");
                }

                var authenticationAccessToken = AccessTokenManager.AddAccessToken(user.Id, Request.UserHostAddress, Request.UserAgent);

                var writer = new JsonWriter();
                ShiJu.Models.User.Serialize(writer, user, Formats.UserDetail);
                var extraJsons = new Dictionary<string, string>();
                extraJsons["User"] = writer.ToString();
                var extraValues = new Dictionary<string, object>();
                extraValues["AccessToken"] = authenticationAccessToken.AccessToken;

                return this.JsonResponseJson(0, "Succeeded", extraJsons, extraValues);
            }
        }

        [HttpPost]
        [Route("User/SignInWithWechat")]
        public ActionResult SignInWithWechat()
        {
            var utcNow = DateTime.UtcNow;
            var code = 0;
            var json = this.GetRequestContentJson() as JsonObject;
            var openId = json.GetStringValue("WechatUnionId");

            if (string.IsNullOrEmpty(openId))
            {
                return this.JsonResult(-1, "WechatUnionId must be provided and cannot be an empty string.");
            }

            using (var db = Heart.CreateShiJuDbContext())
            {
                ShiJu.Models.User user;
                var account = (from a in db.Accounts
                               where (a.Type == AccountType.Wechat) && (a.Name == openId)
                               select a).FirstOrDefault();
                if (account != null)
                {
                    user = db.Users.Find(account.UserId);
                    if (user.Status == UserStatus.Disabled)
                    {
                        return this.JsonResult(-2, "The user has been disabled.");
                    }
                }
                else
                {
                    code = 1;
                    // It is the first time for this wechat user
                    // Create the account and user
                    user = new User()
                    {
                        Id = Guid.NewGuid(),
                        Status = UserStatus.Active,
                        PhoneNumber = string.Empty,
                        NickName = string.Empty,
                        Signature = string.Empty,
                        Portrait = Guid.Empty,
                        Gender = UserGender.Unknown,
                        District = string.Empty,
                        NeedNotification = true,
                        BackgroundImage = Guid.Empty,
                        CreatedTime = utcNow,
                        SignUpTime = utcNow,
                    };

                    account = new Account()
                    {
                        Id = Guid.NewGuid(),
                        Type = AccountType.Wechat,
                        UserId = user.Id,
                        Name = openId,
                        Password = string.Empty,
                        CreatedTime = utcNow,
                    };

                    db.Accounts.Add(account);
                    user.NeedNotification = true;
                    db.Users.Add(user);
                    db.SaveChanges();
                }

                var authenticationAccessToken = AccessTokenManager.AddAccessToken(user.Id, Request.UserHostAddress, Request.UserAgent);

                var writer = new JsonWriter();
                ShiJu.Models.User.Serialize(writer, user, Formats.UserDetail);
                var extraJsons = new Dictionary<string, string>();
                extraJsons["User"] = writer.ToString();
                extraJsons["AccessToken"] = '"' + authenticationAccessToken.AccessToken + '"';

                return this.JsonResponseJson(code, "Succeeded", extraJsons);
            }
        }

        [HttpPost]
        [Route("Users/ResetPassword")]
        public ActionResult ResetPassword()
        {
            var json = this.GetRequestContentJson() as JsonObject;
            var phoneNumber = json.GetStringValue("PhoneNumber");
            var password = json.GetStringValue("Password");
            var verificationCode = json.GetStringValue("VerificationCode");

            //const int verificationCodeExpireInMinites = 30;
            using (var db = Heart.CreateShiJuDbContext())
            {
                // Check verification code
                if (!ValidateVerificationCode(db, phoneNumber, verificationCode))
                {
                    return this.JsonResult(-1, "Invalid verification code.");
                }

                // Check if this account exist
                var account = (from a in db.Accounts
                               where (a.Type == AccountType.PhoneNumber) && (a.Name == phoneNumber)
                               select a).FirstOrDefault();
                if (account == null)
                {
                    return this.JsonResult(-2, "The account with the specified phone number does not exist.");
                }

                // Update password
                account.Password = password;
                db.SaveChanges();

                return this.JsonResult(0, "Password has been updated.");
            }
        }

        [HttpPost]
        [Route("Users/{userId}/ChangePassword")]
        public ActionResult ChangePassword(Guid userId)
        {
            var json = this.GetRequestContentJson() as JsonObject;

            var oldPassword = json.GetStringValue("OldPassword");
            var newPassword = json.GetStringValue("NewPassword");
            using (var db = Heart.CreateShiJuDbContext())
            {
                var exist = db.Accounts.Any(a => a.Type == AccountType.PhoneNumber && a.UserId == userId);
                if (!exist)
                {
                    return this.JsonResult(-1, "this account has not bind with phoneNumber!");
                }

                var account = db.Accounts.FirstOrDefault(a => a.UserId == userId && a.Password == oldPassword && a.Type == AccountType.PhoneNumber);

                if (account == null)
                {
                    return this.JsonResult(-2, "The account with the specified phone number  password is incorrect.");
                }

                // Update password
                account.Password = newPassword;
                db.SaveChanges();

                return this.JsonResult(0, "Password has been updated.");
            }
        }


        [HttpPost]
        [Route("Users/{userId}/Accounts/CreateWithPhoneNumber")]
        public ActionResult CreateAccountWithPhoneNumber(Guid userId)
        {
            var utcNow = DateTime.UtcNow;

            var json = this.GetRequestContentJson() as JsonObject;
            var phoneNumber = json.GetStringValue("PhoneNumber");
            var password = json.GetStringValue("Password");
            var verificationCode = json.GetStringValue("VerificationCode");

            using (var db = Heart.CreateShiJuDbContext())
            {
                // Check verification code
                if (!ValidateVerificationCode(db, phoneNumber, verificationCode))
                {
                    return this.JsonResult(-1, "Invalid verification code.");
                }

                // Check if this account exist
                var accountExist = (from a in db.Accounts
                                    where (a.Type == AccountType.PhoneNumber) && (a.Name == phoneNumber)
                                    select a).Any();
                if (accountExist)
                {
                    return this.JsonResult(-2, "The account with the same phone number exists.");
                }

                // Create the account
                var account = new Account()
                {
                    Id = Guid.NewGuid(),
                    Type = AccountType.PhoneNumber,
                    UserId = userId,
                    Name = phoneNumber,
                    Password = password,
                    CreatedTime = utcNow,
                };

                var user = db.Users.FirstOrDefault(u => u.Id == userId);
                user.PhoneNumber = phoneNumber;

                db.Accounts.Add(account);
                db.SaveChanges();

                var writer = new JsonWriter();
                Account.Serialize(writer, account, Formats.AccountDetail);
                return this.JsonResponseJson(0, "Account", writer.ToString());
            }
        }

        [HttpPost]
        [Route("Users/{userId}/Accounts/CreateWithWechat")]
        public ActionResult CreateAccountWithWechat(Guid userId)
        {
            var utcNow = DateTime.UtcNow;

            var json = this.GetRequestContentJson() as JsonObject;
            var openId = json.GetStringValue("WechatUnionId");

            if (string.IsNullOrEmpty(openId))
            {
                return this.JsonResult(-1, "WechatUnionId must be provided and cannot be an empty string.");
            }


            using (var db = Heart.CreateShiJuDbContext())
            {
                // Check if this account exist
                var accountExist = (from a in db.Accounts
                                    where (a.Type == AccountType.Wechat) && (a.Name == openId)
                                    select a).Any();
                if (accountExist)
                {
                    return this.JsonResult(-2, "The account with the same wechat openid exists.");
                }

                // Create the account
                var account = new Account()
                {
                    Id = Guid.NewGuid(),
                    Type = AccountType.Wechat,
                    UserId = userId,
                    Name = openId,
                    Password = string.Empty,
                    CreatedTime = utcNow,
                };

                db.Accounts.Add(account);
                db.SaveChanges();

                var writer = new JsonWriter();
                Account.Serialize(writer, account, Formats.AccountDetail);
                return this.JsonResponseJson(0, "Account", writer.ToString());
            }
        }

        [Route("Users/{userId}/Accounts")]
        public ActionResult GetAccounts(Guid userId)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var accounts = db.Accounts.Where(a => a.UserId == userId).ToList();
                var writer = new JsonWriter();
                Account.Serialize(writer, accounts, Formats.AccountBrief);
                return this.JsonResponseJson(0, "Accounts", writer.ToString());
            }
        }

        [HttpPost]
        [Route("Users/{userId}/Accounts/Delete")]//解绑
        public ActionResult DeleteAccount(Guid userId, AccountType accountType)
        {
            using (var db = Heart.CreateShiJuDbContext())
            {
                var account = db.Accounts.Where(a => a.UserId == userId && a.Type == accountType).FirstOrDefault();
                if (account != null)
                {
                    db.Accounts.Remove(account);
                    db.SaveChanges();
                    return this.JsonResult(0, "delete succeessfully");
                }
                else
                {
                    return this.JsonResult(-1, "this account is not exist");
                }
            }
        }


        [HttpPost]
        [Route("User/SignOut")]
        public ActionResult SignOut()
        {
            var accecssToken = Request.Headers["AccessToken"];
            AccessTokenManager.RemoveAccessToken(accecssToken);

            return this.JsonResult(true);
        }

        private string GetVerificationCodeMessage(string verificationCode)
        {
            return string.Format(Heart.SmsVerificationCodeTemplate, verificationCode);
        }
    }
}
