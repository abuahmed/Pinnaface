using System.Text;

namespace PinnaFace.Core
{
    public sealed class Hash
    {
        public static int HashMaxLength = 14;

        /// <summary>Encodes an integer value.</summary>
        /// <returns>14 char alpha-numeric encoded string.</returns>
        public static string EncodeValue(int idToEncrypt, string hashKey, int maxNum, int outputLength, int shiftBits)
        {
            StringBuilder hashed = new StringBuilder();
            int delta = 0, keyMax = hashKey.Length - 1;


            // hash ID to string..
            idToEncrypt = maxNum ^ idToEncrypt;
            for (int i = 0; i < outputLength / 2; i++)
            {
                delta += idToEncrypt & keyMax;
                hashed.Append(hashKey[idToEncrypt & keyMax].ToString());
                idToEncrypt = idToEncrypt >> shiftBits;
            }

            // interleave checksum and hashed ID..
            StringBuilder checksum = new StringBuilder();
            for (int i = 1; i <= outputLength / 2; i++)
            {
                delta++;
                char x = hashed[i - 1];
                int val = ((hashKey.IndexOf(x) + i) * delta) & keyMax;
                checksum.Append(x.ToString() + hashKey[val].ToString());
            }

            return (checksum.ToString());
        }

        /// <summary>Decodes a 14 char alpha-numeric string.</summary>
        /// <returns>True if id was decoded successfully, false otherwise.</returns>
        public static bool DecodeValue(string encId, ref int refId, string hashKey, int maxNum, int outputLength, int shiftBits)
        {
            if (null == encId)
                return false;

            int tempID = 0;
            bool result = true;

            if (encId.Length == outputLength)
            {
                string hashed = "", checksum = "";
                // de-interleave hashed id and checksum..
                for (int i = 0; i < outputLength; i++)
                {
                    hashed += encId[i];
                    checksum += encId[++i];
                }

                // unhash id..
                int delta = 0;
                for (int i = ((outputLength / 2) - 1); i >= 0; i--)
                {
                    int x = hashKey.IndexOf(hashed[i]);
                    if (x == -1)
                    {
                        result = false;
                        break;
                    }
                    delta += x;
                    tempID = (tempID << shiftBits) | x;
                }

                if (result)
                {
                    // calculate checksum..
                    StringBuilder calcsum = new StringBuilder();
                    for (int i = 1; i <= outputLength / 2; i++)
                    {
                        delta++;
                        calcsum.Append(hashKey[((hashKey.IndexOf(hashed[i - 1]) + i) * delta) & (hashKey.Length - 1)].ToString());
                    }
                    if (calcsum.ToString() == checksum)
                    {
                        refId = tempID ^ maxNum;
                        result = true;
                    }
                    else
                        result = false; // bad checksum
                }
                else
                    result = false; // invalid hash code
            }
            else
                result = false; // wrong length

            return (result);
        }

    }
}