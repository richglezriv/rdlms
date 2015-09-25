using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RD.Business
{
    public class Utilities
    {
        private static String KEY = "ZaZaCruREcHA8a7*as";

        internal static string GetStringHashed(string toHash)
        {
            System.Security.Cryptography.SHA1 hashKey = System.Security.Cryptography.SHA1.Create();
            hashKey.ComputeHash(Encoding.UTF8.GetBytes(toHash));
            string hashString = BitConverter.ToString(hashKey.Hash).Replace("-", String.Empty).ToLower();

            return hashString;
        }

        public static string GetSerialHash(string key)
        {
            string hash = GetStringHashed(string.Format("{0}{1}", KEY, key));
            return hash;
        }
    }
}
