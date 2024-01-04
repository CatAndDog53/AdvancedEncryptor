using System.Numerics;

namespace Cryptography.LfsrAlgorithm
{
    public class Lfsr
    {
        public BigInteger DefaultState { get; private set; }
        public BigInteger State { get; private set; }
        public int RegisterLength { get; private set; }
        private BigInteger StateBitMask { get; set; }
        private bool[] Taps { get; set; }

        public Lfsr(BigInteger seed, int registerLength, int[] tapsIndexes)
        {
            Taps = TapsArrayGenerator(tapsIndexes, registerLength);
            DefaultState = seed;
            State = seed;
            RegisterLength = registerLength;
            StateBitMask = CreateBitmask(RegisterLength);
        }

        public BigInteger EncryptBitByBit(BigInteger bitsToEncrypt)
        {
            BigInteger encryptedBits;
            BigInteger sequenceLength = bitsToEncrypt.GetBitLength();
            BigInteger lfsrSequence = GetSequenceOptimized(sequenceLength);

            encryptedBits = bitsToEncrypt ^ lfsrSequence;
            return encryptedBits;
        }

        public byte[] EncryptByBytes(byte[] bytesToEncrypt)
        {
            byte[] encryptedBytes = new byte[bytesToEncrypt.Length];
            int sequenceLength = bytesToEncrypt.Length * 8;
            byte[] lfsrSequenceInBytes = ConvertHelper.RemoveZerosInTheEnd(GetSequenceOptimized(sequenceLength).ToByteArray());
            
            for (int i = 0; i < bytesToEncrypt.Length; i++)
            {
                encryptedBytes[i] = (byte)(bytesToEncrypt[i] ^ lfsrSequenceInBytes[i]);
            }
            return encryptedBytes;
        }

        private BigInteger GetSequence(BigInteger sequenceLength)
        {
            BigInteger sequence = 0b0000;
            while (sequenceLength-- > 0)
            {
                sequence = sequence << 1 | ((State >> RegisterLength - 1) & 0b0001);
                AdvanceOptimized();
            }
            State = DefaultState;
            return sequence;
        }

        private BigInteger GetSequenceOptimized(BigInteger sequenceLength)
        {
            BigInteger sequence = 0b0000;
            BigInteger bitsGenerated = 0;
            while (bitsGenerated < sequenceLength)
            {
                sequence = sequence << 1 | ((State >> RegisterLength - 1) & 0b0001);
                bitsGenerated++;
                AdvanceOptimized();
                if (State == DefaultState)
                {
                    int periodLength = (int)bitsGenerated;
                    while (bitsGenerated < sequenceLength)
                    {
                        sequence = (sequence << periodLength) | sequence;
                    }
                    if (bitsGenerated > sequenceLength)
                    {
                        sequence = sequence >> (int)(bitsGenerated - sequenceLength);
                    }
                    break;
                }
            }
            State = DefaultState;
            return sequence;
        }

        private void Advance()
        {
            BigInteger temp = 0b0000;
            for (int i = 0; i < Taps.Length; ++i)
            {
                if (Taps[i]) temp ^= State >> (Taps.Length - 1 - i);
            }
            State = State << 1 | (temp & 0b0001);
        }

        private void AdvanceOptimized()
        {
            BigInteger temp = 0b0000;
            for (int i = 0; i < Taps.Length; ++i)
            {
                if (Taps[i]) temp ^= State >> (Taps.Length - 1 - i);
            }
            State = (State << 1 | (temp & 0b0001)) & StateBitMask;
        }

        private static BigInteger CreateBitmask(int length)
        {
            if (length <= 0)
                throw new ArgumentException("Mask length cannot be less than zero!");

            BigInteger bitmask = BigInteger.Zero;
            for (int i = 0; i < length; i++)
            {
                bitmask |= BigInteger.One << i;
            }

            return bitmask;
        }

        private static bool[] TapsArrayGenerator(int[] tapsIndexes, int registerLength)
        {
            bool[] taps = new bool[registerLength];
            for (int i = 0; i < taps.Length; i++)
            {
                if (tapsIndexes.Contains(i)) taps[i] = true;
                else taps[i] = false;
            }
            return taps;
        }
    }
}