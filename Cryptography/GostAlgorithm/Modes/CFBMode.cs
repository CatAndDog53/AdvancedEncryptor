using System;
using System.Collections.Generic;

namespace Cryptography.Gost.Modes
{
    internal class CFBMode

    {
        private readonly SubstitutionMode substitution;
        private uint n1;
        private uint n2;

        public CFBMode(byte[][] sBlock)
        {
            substitution = new SubstitutionMode(sBlock);
        }

        // Set generator state.
        public void SetIV(byte[] iv)
        {
            n1 = BitConverter.ToUInt32(iv, 0);
            n2 = BitConverter.ToUInt32(iv, 4);
        }

        // CFB encode.
        public byte[] EncodeProcess(byte[] data, List<uint> subKeys)
        {
            var gamma = new byte[8];
            Array.Copy(BitConverter.GetBytes(n1), 0, gamma, 0, 4);
            Array.Copy(BitConverter.GetBytes(n2), 0, gamma, 4, 4);
            gamma = substitution.EncodeProcess(gamma, subKeys);

            var res = XOR(gamma, data);

            if (res.Length == 8)
            {
                n1 = BitConverter.ToUInt32(res, 0);
                n2 = BitConverter.ToUInt32(res, 4);
            }

            return res;
        }

        // CFB decode.
        public byte[] DecodeProcess(byte[] data, List<uint> subKeys)
        {
            var gamma = new byte[8];
            Array.Copy(BitConverter.GetBytes(n1), 0, gamma, 0, 4);
            Array.Copy(BitConverter.GetBytes(n2), 0, gamma, 4, 4);
            gamma = substitution.EncodeProcess(gamma, subKeys);

            var res = XOR(gamma, data);

            if (data.Length == 8)
            {
                n1 = BitConverter.ToUInt32(data, 0);
                n2 = BitConverter.ToUInt32(data, 4);
            }

            return res;
        }

        // XOR
        private byte[] XOR(byte[] gamma, byte[] data)
        {
            var len = data.Length;
            var res = new byte[len];

            for (var i = 0; i != len; i++) res[i] = (byte) (gamma[i] ^ data[i]);

            return res;
        }
    }
}