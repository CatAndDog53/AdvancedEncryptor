using System.ComponentModel;

namespace Cryptography.SdesAlgorithm
{
    public class SDES
    {
        public int Input10bitKey { get; private set; }
        public int K1 { get; private set; }
        public int K2 { get; private set; }

        private static readonly int[] P10Table = { 3, 5, 2, 7, 4, 10, 1, 9, 8, 6 };
        private static readonly int P10max = 10;
        private static readonly int[] P8Table = { 6, 3, 7, 4, 8, 5, 10, 9 };
        private static readonly int P8max = 10;
        private static readonly int[] P4Table = { 2, 4, 3, 1 };
        private static readonly int P4max = 4;
        private static readonly int[] IPTable = { 2, 6, 3, 1, 4, 8, 5, 7 };
        private static readonly int IPmax = 8;
        private static readonly int[] IPITable = { 4, 1, 3, 5, 7, 2, 8, 6 };
        private static readonly int IPImax = 8;
        private static readonly int[] EPTable = { 4, 1, 2, 3, 2, 3, 4, 1 };
        private static readonly int EPmax = 4;
        private static readonly int[][] S0 = {
                                     new int[] { 1, 0, 3, 2 },
                                     new int[] { 3, 2, 1, 0 },
                                     new int[] { 0, 2, 1, 3 },
                                     new int[] { 3, 1, 3, 2 } };
        private static readonly int[][] S1 = {
                                     new int[] { 0, 1, 2, 3 },
                                     new int[] { 2, 0, 1, 3 },
                                     new int[] { 3, 0, 1, 0 },
                                     new int[] { 2, 1, 0, 3 } };

        public SDES(int input10bitKey)
        {
            Input10bitKey = input10bitKey;
            GenerateKeys(input10bitKey);
        }

        public byte[] EncryptParallel(byte[] input, BackgroundWorker bw)
        {
            byte[] output = new byte[input.Length];
            int progress = 0;

            Parallel.For(0, input.Length, action =>
            {
                output[action] = EncryptBlock(input[action]);

                if (Interlocked.Increment(ref progress) % (input.Length / 100) == 0)
                {
                    bw.ReportProgress(progress * 100 / input.Length);
                }
            });

            bw.ReportProgress(100);
            return output;
        }

        public byte[] DecryptParallel(byte[] input, BackgroundWorker bw)
        {
            byte[] output = new byte[input.Length];
            int progress = 0;

            Parallel.For(0, input.Length, action =>
            {
                output[action] = DecryptBlock(input[action]);

                if (Interlocked.Increment(ref progress) % (input.Length / 100) == 0)
                {
                    bw.ReportProgress(progress * 100 / input.Length);
                }
            });

            bw.ReportProgress(100);
            return output;
        }

        public byte[] EncryptParallel(byte[] input)
        {
            byte[] output = new byte[input.Length];

            Parallel.For(0, input.Length, action =>
            {
                output[action] = EncryptBlock(input[action]);
            });
            return output;
        }

        public byte[] DecryptParallel(byte[] input)
        {
            byte[] output = new byte[input.Length];

            Parallel.For(0, input.Length, action =>
            {
                output[action] = DecryptBlock(input[action]);
            });
            return output;
        }

        public byte[] Encrypt(byte[] input, BackgroundWorker bw)
        {
            byte[] output = new byte[input.Length];
            int progress = 0;

            for (int i = 0; i < input.Length; i++)
            {
                output[i] = EncryptBlock(input[i]);

                if (progress++ % (input.Length / 100) == 0)
                {
                    bw.ReportProgress(progress * 100 / input.Length);
                }
            }
            bw.ReportProgress(100);
            return output;
        }

        public byte[] Decrypt(byte[] input, BackgroundWorker bw)
        {
            byte[] output = new byte[input.Length];
            int progress = 0;

            for (int i = 0; i < input.Length; i++)
            {
                output[i] = DecryptBlock(input[i]);

                if (progress++ % (input.Length / 100) == 0)
                {
                    bw.ReportProgress(progress * 100 / input.Length);
                }
            }
            bw.ReportProgress(100);
            return output;
        }

        public byte[] Encrypt(byte[] input)
        {
            byte[] output = new byte[input.Length];

            for (int i = 0; i < output.Length; i++)
            {
                output[i] = EncryptBlock(input[i]);
            }
            return output;
        }

        public byte[] Decrypt(byte[] input)
        {
            byte[] output = new byte[input.Length];

            for (int i = 0; i < output.Length; i++)
            {
                output[i] = DecryptBlock(input[i]);
            }
            return output;
        }

        public byte EncryptBlock(int block)
        {
            block = Permutate(block, IPTable, IPmax);
            block = EpXorP4Combine(block, K1);
            block = SwapLeftAndRightHalves(block);
            block = EpXorP4Combine(block, K2);
            block = Permutate(block, IPITable, IPImax);
            return (byte)block;
        }

        public byte DecryptBlock(int block)
        {
            block = Permutate(block, IPTable, IPmax);
            block = EpXorP4Combine(block, K2);
            block = SwapLeftAndRightHalves(block);
            block = EpXorP4Combine(block, K1);
            block = Permutate(block, IPITable, IPImax);
            return (byte)block;
        }

        private void GenerateKeys(int input10bitKey)
        {
            input10bitKey = Permutate(input10bitKey, P10Table, P10max);
            int leftHalf = (input10bitKey >> 5) & 0x1F;   // Залишаємо 5 бітів зліва
            int rightHalf = input10bitKey & 0x1F;   // Залишаємо 5 бітів справа

            leftHalf = ((leftHalf & 0xF) << 1) | ((leftHalf & 0x10) >> 4);    // 1 bit left round shift
            rightHalf = ((rightHalf & 0xF) << 1) | ((rightHalf & 0x10) >> 4);    // 1 bit left round shift
            int combinedHalves = (leftHalf << 5) | rightHalf; // Об'єднуємо дві половини
            K1 = Permutate(combinedHalves, P8Table, P8max);   // Перестановка за таблицею P8. Отримуємо ключ K1

            leftHalf = ((leftHalf & 0x7) << 2) | ((leftHalf & 0x18) >> 3);   // 2 bit left round shift
            rightHalf = ((rightHalf & 0x7) << 2) | ((rightHalf & 0x18) >> 3);   // 2 bit left round shift
            combinedHalves = (leftHalf << 5) | rightHalf;   // Об'єднуємо дві половини
            K2 = Permutate(combinedHalves, P8Table, P8max);   // Перестановка за таблицею P8. Отримуємо ключ K2
        }

        private static int Permutate(int input, int[] permutationTable, int pmax)
        {
            int output = 0;
            for (int i = 0; i < permutationTable.Length; ++i)
            {
                output <<= 1;
                output |= (input >> (pmax - permutationTable[i])) & 1;
            }
            return output;
        }

        private static int PutIntoSBoxes(int block)
        {
            int leftHalf = (block >> 4) & 0xF;
            int rightHalf = block & 0xF;

            // Для S0 використовуємо ліву половину, для S1 -- праву
            // Беремо 1-ий та 4-ий біти -- рядок, 2-ий та 3-ій біти -- стовпець
            int rowS0 = ((leftHalf & 0x8) >> 2) | (leftHalf & 1);
            int columnS0 = (leftHalf >> 1) & 0x3;
            int rowS1 = ((rightHalf & 0x8) >> 2) | (rightHalf & 1);
            int columnS1 = (rightHalf >> 1) & 0x3;
            leftHalf = S0[rowS0][columnS0];
            rightHalf = S1[rowS1][columnS1];

            int combinedHalves = (leftHalf << 2) | rightHalf;
            return combinedHalves;
        }

        private static int EpXorP4Combine(int block, int K)
        {
            int blockAfterIP = block;
            block = Permutate(block, EPTable, EPmax); // Перестановка за таблицею EP
            block = block ^ K; // XOR з ключем
            block = PutIntoSBoxes(block);
            block = Permutate(block, P4Table, P4max);

            int leftHalfAfterIP = (blockAfterIP >> 4) & 0xF;
            int rightHalfAferIP = blockAfterIP & 0xF;
            block = block ^ leftHalfAfterIP;
            int combined = (block << 4) | rightHalfAferIP;

            return combined;
        }

        private static int SwapLeftAndRightHalves(int x)
        {
            return ((x & 0xF) << 4) | ((x >> 4) & 0xF);
        }
    }
}
