using Cryptography.LfsrAlgorithm;
using System.Numerics;

namespace CryptographyTests
{
    public class LfsrTests
    {
        [Test]
        public void EncryptBitByBit_CorrectInputTest()
        {
            int registerLength = 64;
            int[] taps = new int[] { 59, 6, 5, 4, 3, 1, 0 };
            BigInteger defaultRegisterState = (BigInteger)0b0110100011101110011000000001111010110111010001011000001011100001;
            BigInteger bitsToEncrypt = 0b101000101011;
            BigInteger expectedResult = 0b110010100101;

            Lfsr lfsr = new(defaultRegisterState, registerLength, taps);
            BigInteger encryptedBits = lfsr.EncryptBitByBit(bitsToEncrypt);
            BigInteger decryptedBits = lfsr.EncryptBitByBit(encryptedBits);

            Assert.That(encryptedBits, Is.EqualTo(expectedResult));
            Assert.That(decryptedBits, Is.EqualTo(bitsToEncrypt));
        }
    }
}