using System;
using System.Collections.Generic;

namespace Cryptography.Gost.Modes
{
    internal class SubstitutionMode
    {
        private readonly byte[][] sBlock;

        public SubstitutionMode(byte[][] sBlock)
        {
            this.sBlock = sBlock;
        }

        // ECB encode. Data multiple of 64 bits
        public byte[] EncodeProcess(byte[] data, List<uint> subKeys)
        {
            var little = BitConverter.ToUInt32(data, 0);
            var big = BitConverter.ToUInt32(data, 4);

            for (var i = 0; i != 32; i++)
            {
                var round = big ^ Function(little, subKeys[i]);

                if (i < 31)
                {
                    big = little;
                    little = round;
                }
                else
                {
                    big = round;
                }
            }

            var result = new byte[8];
            Array.Copy(BitConverter.GetBytes(little), 0, result, 0, 4);
            Array.Copy(BitConverter.GetBytes(big), 0, result, 4, 4);
            return result;
        }

        // Substitution decode.
        public byte[] DecodeProcess(byte[] data, List<uint> subKeys)
        {
            return EncodeProcess(data, subKeys);
        }

        // Main func
        public uint Function(uint block, uint subKey)
        {
            block = (block + subKey) % 4294967295;
            block = Substitute(block);
            block = (block << 11) | (block >> 21);
            return block;
        }

        // Substitution
        public uint Substitute(uint value)
        {
            uint res = 0;

            for (var i = 0; i != 8; i++)
            {
                var index = (byte) ((value >> (4 * i)) & 0x0f);
                var block = sBlock[i][index];
                res |= (uint) block << (4 * i);
            }

            return res;
        }
    }
}