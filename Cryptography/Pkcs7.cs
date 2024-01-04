namespace Cryptography
{
    public static class Pkcs7
    {
        public static byte[] RemovePkcs7Padding(byte[] paddedByteArray)
        {
            if (paddedByteArray == null)
            {
                throw new ArgumentNullException("paddedByteArray can not be null");
            }

            dynamic last = paddedByteArray[paddedByteArray.Length - 1];
            if (paddedByteArray.Length <= last)
            {
                return paddedByteArray;
            }

            for (int i = paddedByteArray.Length - 2; i >= paddedByteArray.Length - last; i += -1)
            {
                if (paddedByteArray[i] != last)
                {
                    return paddedByteArray;
                }
            }

            return SubArray(paddedByteArray, 0, (paddedByteArray.Length - last));
        }

        public static byte[] AddPkcs7Padding(byte[] data, int paddingLength)
        {
            if (data.Length > 256)
            {
                throw new ArgumentOutOfRangeException("data must be <= 256 in length");
            }
            if (paddingLength > 256)
            {
                throw new ArgumentOutOfRangeException("paddingLength must be <= 256");
            }

            if (paddingLength <= data.Length)
            {
                return data;
            }

            var output = new byte[paddingLength];
            Buffer.BlockCopy(data, 0, output, 0, data.Length);
            for (var i = data.Length; i < output.Length; i++)
            {
                output[i] = (byte)(paddingLength - data.Length);
            }
            return output;
        }

        public static T[] SubArray<T>(T[] arr, int start, int length)
        {
            var result = new T[length];
            Buffer.BlockCopy(arr, start, result, 0, length);

            return result;
        }
    }
}
