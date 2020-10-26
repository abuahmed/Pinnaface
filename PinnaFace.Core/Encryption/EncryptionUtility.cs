using System;
using System;
using System.Collections.Generic;

namespace PinnaFace.Core
{
    public static class EncryptionUtility
    {
        private const string Txt32Key = "abcdefghijklmnopqrstuvwxyz012345";
        private const string Txt64Key = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ^!0123456789";
        private const string TxtKey = "0123456789";

        public static int Hash64Decode(string txtHash64String)
        {
            if (txtHash64String.Length < 1)
                return 0;

            var txtHash64Int = 0;
            try
            {
                txtHash64Int = IdHash64.GetDecodedValue(txtHash64String, Txt64Key);
            }
            catch
            {
            }
            return txtHash64Int;
        }

        public static string Hash64Encode(int txtHash64Int)
        {
            if (txtHash64Int < 1)
                return "";
            var txtHash64String = "";
            try
            {
                txtHash64String = IdHash64.GetEncodedValue(Convert.ToInt32(txtHash64Int), Txt64Key);
            }
            catch
            {
            }
            return txtHash64String;
        }

        public static int Hash32Decode(string txtHash32String)
        {
            if (txtHash32String.Length < 1)
                return 0;
            var txtHash32Int = 0;
            try
            {
                txtHash32Int = IdHash32.GetDecodedValue(txtHash32String, Txt32Key);
            }
            catch
            {
            }
            return txtHash32Int;
        }

        public static string Hash32Encode(int txtHash32Int)
        {
            if (txtHash32Int < 1)
                return "";
            var txtHash32String = "";
            try
            {
                txtHash32String = IdHash32.GetEncodedValue(Convert.ToInt32(txtHash32Int), Txt32Key);
            }
            catch
            {
            }
            return txtHash32String;
        }

        public static string Decrypt(string txtEncryptedString)
        {
            if (txtEncryptedString.Length < 1)
                return "";
            var txtDecryptedString = "";
            try
            {
                if (TxtKey.Length > 0)
                    txtDecryptedString = Crypto.DecryptString(txtEncryptedString, TxtKey);
                else
                    txtDecryptedString = Crypto.DecryptString(txtEncryptedString);
            }
            catch
            {
            }
            return txtDecryptedString;
        }

        public static string Encrypt(string txtDecryptedString)
        {
            if (txtDecryptedString.Length < 1)
                return "";
            var txtEncryptedString = "";
            try
            {
                if (TxtKey.Length > 0)
                    txtEncryptedString = Crypto.EncryptString(txtDecryptedString, TxtKey);
                else
                    txtEncryptedString = Crypto.EncryptString(txtDecryptedString);
            }
            catch
            {
            }
            return txtEncryptedString;
        }

        public static string Md5Encrypt(string stringToEncrypt)
        {
            var x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var data = System.Text.Encoding.ASCII.GetBytes(stringToEncrypt);
            data = x.ComputeHash(data);
            var md5Hash = System.Text.Encoding.ASCII.GetString(data);

            return md5Hash;
        }


    }
}