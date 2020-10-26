using System;

namespace PinnaFace.Core
{
    public static class IdHash64
    {
        public static int MaxLength = 14;
        private static int MaxNum = 999999999;
        private static int OutputLength = MaxLength;
        private static int ShiftBits = 6;

        private static string _hashKey = null;

        private static string HashKey
        {
            get
            {
                if (String.IsNullOrEmpty(_hashKey))
                {
                    _hashKey = ConfigManager.Get("Hash64");
                 
                    if (String.IsNullOrEmpty(_hashKey))
                        throw new ArgumentNullException(
                            "Cannot retrieve parameter \"Hash64\" from configuration file");
                }
                return _hashKey;
            }
        }

        public static string GetEncodedValue(int value)
        {
            return GetEncodedValue(value, HashKey);
        }

        public static string GetEncodedValue(int value, string hashKey)
        {
            if (value > 0)
                return Hash.EncodeValue(value, hashKey, MaxNum, OutputLength, ShiftBits);
            
            return String.Empty;
        }

        public static int GetDecodedValue(string value)
        {
            return GetDecodedValue(value, HashKey);
        }

        public static int GetDecodedValue(string value, string hashKey)
        {
            int val = -1;
            Hash.DecodeValue(value, ref val, hashKey, MaxNum, OutputLength, ShiftBits);

            return val;
        }

        /// <summary>
        /// Checks to see if the id value is valid, throws invalid id G6Exception.
        /// </summary>
        /// <param name="value">The value of the id.</param>
        public static string ValidateId(string value)
        {
            //if (GetDecodedValue(value) < -1)
            //throw new G6Exception("EncryptionLib:IdHash64:ValidateId", GenericMessages.Invalid_Id + ":" + value);

            return value;
        }

        public static bool IsValidId(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            try
            {
                int id = GetDecodedValue(value);
                if (id > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void ValidateId(string id, string failVariableName)
        {
            //if (!IsValidId(id))
            //throw new G6Exception("EncryptionLib:IdHash64", "Invalid id for " + failVariableName);
        }


    }
}