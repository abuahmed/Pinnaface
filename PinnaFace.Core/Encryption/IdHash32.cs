namespace PinnaFace.Core
{
    public static class IdHash32
    {
        public static int MaxLength = 14;

        private static string HashKey = ConfigManager.Get("Hash32");
        private static int MaxNum = 999999999;
        private static int OutputLength = 14;
        private static int ShiftBits = 5;
        
        public static string GetEncodedValue(int value)
        {
            return GetEncodedValue(value, HashKey);
        }

        public static string GetEncodedValue(int value, string hashKey)
        {
            return Hash.EncodeValue(value, hashKey, MaxNum, OutputLength, ShiftBits);
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
            //if (GetDecodedValue(value) < 0)
            //throw new G6Exception(MethodBase.GetCurrentMethod(), GenericMessages.Invalid_Id);

            return value;
        }
    }
}