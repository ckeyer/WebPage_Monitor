using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Website_Monitor.cjclass
{
    class CJMainServer
    {
        public static string EncryptMd5(string strPwd)
        {
            byte[] result = Encoding.Default.GetBytes(strPwd);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            string str = BitConverter.ToString(output).Replace("-", "");
            return str.ToUpper();
        }
    }
}
