
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;


namespace registerAPI.Common
{
    public class Message
    {
        public static string Success ="Success";
        public static string ErrorFound ="Error Found";
        public static string UserAlreadyCreated ="User already created ,please Login";
        public static string VerifyMail = "User already created , please verify your email";
        public static string InvalidUser = "Invalid user, Please create account";
        public static string MailSent = "Mail Sent";
        public static string UserCreatedVerifyMail = "User created , check mail, Click link and verify.";
    }
}