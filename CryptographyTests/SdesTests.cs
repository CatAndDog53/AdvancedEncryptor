using Cryptography.SdesAlgorithm;
using System.Text;

namespace SDESTest
{
    public class SDESTests
    {
        [Test]
        public void K1AndK2Test_KeysShouldBeGeneratedCorrectly()
        {
            int input10bitKey = 642;
            SDES sDesEncryptor = new SDES(input10bitKey);

            Assert.That(sDesEncryptor.K1, Is.EqualTo(164));
            Assert.That(sDesEncryptor.K2, Is.EqualTo(67));
        }

        [Test]
        public void EncryptBlock_ShouldReturnCorrectCipherText()
        {
            int input10bitKey = 642;
            int plainText = 151;
            int expectedCipherText = 56;

            SDES sDesEncryptor = new SDES(input10bitKey);
            int actualCipherText = sDesEncryptor.EncryptBlock(plainText);

            Assert.That(actualCipherText, Is.EqualTo(expectedCipherText));
        }

        [Test]
        public void Encrypt_ShouldReturnCorrectCipherText()
        {
            int input10bitKey = 296;
            string plainText = "Keep your friends close, but your enemies closer";
            string expectedCipherText = "52f2f2a05a3e7b55385ac8389df2468b1b5a8aa57b1bf2315aad55c55a3e7b55385af246f2dc9df21b5a8aa57b1bf238";

            byte[] bytes = Encoding.ASCII.GetBytes(plainText);
            SDES sDesEncryptor = new SDES(input10bitKey);
            byte[] encryptedBytes = sDesEncryptor.Encrypt(bytes);
            string actualCipherText = ByteArrayToString(encryptedBytes);

            Assert.That(actualCipherText, Is.EqualTo(expectedCipherText));
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}