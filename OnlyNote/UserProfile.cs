using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlyNote
{
    class UserProfile
    {
        static string userID;
        static string userName = "Vik";
        static string permissionLevel;

        public static string UserID { get => userID; set => userID = value; }
        public static string UserName { get => userName; set => userName = value; }
        public static string PermissionLevel { get => permissionLevel; set => permissionLevel = value; }
    }
}
