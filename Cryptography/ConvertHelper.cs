using System.Globalization;
using System.Numerics;
using System.Text;

namespace Cryptography
{
    public static class ConvertHelper
    {
        public static string ToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes);
        }

        public static string ByteArrayToHexString(byte[] bytes)
        {
            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static byte[] StringToByteArray(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static string ByteArrayToString(byte[] bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public static byte[] HexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;
        }

        public static int[] StringPolynomialToArray(string polynomialString)
        {
            int[] polynomialArray = new int[0];

            polynomialArray = Array.ConvertAll(polynomialString.Split(", "), s => int.Parse(s));

            return polynomialArray;
        }

        public static BigInteger BinaryStringToBigInteger(string binaryString)
        {
            if (binaryString.Any(x => x != '0' && x != '1'))
                throw new ArgumentException("Binary string was not in a correct format!");

            BigInteger result = 0;
            foreach (char c in binaryString)
            {
                result <<= 1;
                result += c == '1' ? 1 : 0;
            }
            return result;
        }

        public static string BigIntegerToNBase(BigInteger number, int n)
        {
            StringBuilder sb = new StringBuilder();
            while (number > 0)
            {
                sb.Insert(0, number % n);
                number /= n;
            }

            return sb.ToString();
        }

        public static byte[] RemoveZerosInTheEnd(byte[] bytes)
        {
            List<byte> result = new List<byte>(bytes);
            for (int i = result.Count - 1; i >= 0; i--)
            {
                if (result[i] == 0) result.RemoveAt(i);
                else break;
            }
            return result.ToArray();
        }

        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;
        }
    }
}
