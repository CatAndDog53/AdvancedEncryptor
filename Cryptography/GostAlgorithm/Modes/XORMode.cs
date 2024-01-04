using System;
using System.Collections.Generic;

namespace Cryptography.Gost.Modes
{
    internal class XORMode
    {
        private readonly SubstitutionMode substitution;
        private uint n3;
        private uint n4;

        public XORMode(byte[][] sBlock)
        {
            substitution = new SubstitutionMode(sBlock);
        }

        // Первоначальная установка состояния шифра.
        public void SetIV(byte[] iv, List<uint> subKeys)
        {
            var encodedIV = substitution.EncodeProcess(iv, subKeys);

            n3 = BitConverter.ToUInt32(encodedIV, 0);
            n4 = BitConverter.ToUInt32(encodedIV, 4);
        }

        // Процесс шифрования открытого текста
        public byte[] EncodeProcess(byte[] data, List<uint> subKeys)
        {
            n3 += 16843009 % 4294967295;
            n4 += 16843012 % 4294967294;

            var n1 = n3;
            var n2 = n4;

            var gamma = new byte[8];
            Array.Copy(BitConverter.GetBytes(n1), 0, gamma, 0, 4);
            Array.Copy(BitConverter.GetBytes(n2), 0, gamma, 4, 4);
            gamma = substitution.EncodeProcess(gamma, subKeys);

            return XOR(gamma, data);
        }

        // Процесс дешифровки шифротекста.
        public byte[] DecodeProcess(byte[] data, List<uint> subKeys)
        {
            return EncodeProcess(data, subKeys);
        }

        // Применение XOR между гаммой и блоком данных.
        private byte[] XOR(byte[] gamma, byte[] data)
        {
            var len = data.Length;
            var res = new byte[len];

            for (var i = 0; i != len; i++) res[i] = (byte) (gamma[i] ^ data[i]);

            return res;
        }
    }
}