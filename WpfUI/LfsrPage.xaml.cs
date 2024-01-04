using Cryptography.LfsrAlgorithm;
using Cryptography;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfUI
{
    /// <summary>
    /// Interaction logic for LfsrPage.xaml
    /// </summary>
    public partial class LfsrPage : Page
    {
        public LfsrPage()
        {
            InitializeComponent();
        }

        private void EncryptBitsBtn_Click(object sender, RoutedEventArgs e)
        {
            string polynomialString = PolynomialTextBox.Text;
            string registerDefaultValueBinaryString = RegisterDefaultValueTextBox.Text;
            string bitsToEncryptBinaryString = BitsToEncryptTextBox.Text;

            int[] polynomial = new int[0];
            try
            {
                polynomial = ConvertHelper.StringPolynomialToArray(polynomialString);
            }
            catch
            {
                MessageBox.Show("Polynimial was not in a correct format!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            BigInteger registerDefaultValue = new BigInteger();
            try
            {
                registerDefaultValue = ConvertHelper.BinaryStringToBigInteger(registerDefaultValueBinaryString);
            }
            catch
            {
                MessageBox.Show("Register default value was not in a correct format!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            BigInteger bitsToEncrypt = new BigInteger();
            try
            {
                bitsToEncrypt = ConvertHelper.BinaryStringToBigInteger(bitsToEncryptBinaryString);
            }
            catch
            {
                MessageBox.Show("Bits to encrypt were not in a correct format!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Lfsr lfsr = new Lfsr(registerDefaultValue, registerDefaultValueBinaryString.Length, polynomial);
            BigInteger encryptedBits = lfsr.EncryptBitByBit(bitsToEncrypt);

            string encryptedBitsString = ConvertHelper.BigIntegerToNBase(encryptedBits, 2);
            EncryptedBitsTextBox.Text = encryptedBitsString;
        }

        private void EncryptTextBtn_Click(object sender, RoutedEventArgs e)
        {
            string polynomialString = PolynomialTextBox.Text;
            string registerDefaultValueBinaryString = RegisterDefaultValueTextBox.Text;
            byte[] textToEncrypt = ConvertHelper.StringToByteArray(TextToEncryptTextBox.Text);

            int[] polynomial = new int[0];
            try
            {
                polynomial = ConvertHelper.StringPolynomialToArray(polynomialString);
            }
            catch
            {
                MessageBox.Show("Polynimial was not in a correct format!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            BigInteger registerDefaultValue = new BigInteger();
            try
            {
                registerDefaultValue = ConvertHelper.BinaryStringToBigInteger(registerDefaultValueBinaryString);
            }
            catch
            {
                MessageBox.Show("Register default value was not in a correct format!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            BigInteger bitsToEncrypt = new BigInteger(textToEncrypt);

            Lfsr lfsr = new Lfsr(registerDefaultValue, registerDefaultValueBinaryString.Length, polynomial);
            BigInteger encryptedBits = lfsr.EncryptBitByBit(bitsToEncrypt);
            EncryptedTextTextBox.Text = ConvertHelper.ByteArrayToHexString(encryptedBits.ToByteArray());
        }

        private void DecryptTextBtn_Click(object sender, RoutedEventArgs e)
        {
            string polynomialString = PolynomialTextBox.Text;
            string registerDefaultValueBinaryString = RegisterDefaultValueTextBox.Text;
            byte[] textToEncrypt = ConvertHelper.ConvertHexStringToByteArray(TextToEncryptTextBox.Text);

            int[] polynomial = new int[0];
            try
            {
                polynomial = ConvertHelper.StringPolynomialToArray(polynomialString);
            }
            catch
            {
                MessageBox.Show("Polynimial was not in a correct format!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            BigInteger registerDefaultValue = new BigInteger();
            try
            {
                registerDefaultValue = ConvertHelper.BinaryStringToBigInteger(registerDefaultValueBinaryString);
            }
            catch
            {
                MessageBox.Show("Register default value was not in a correct format!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            BigInteger bitsToEncrypt = new BigInteger(textToEncrypt);

            Lfsr lfsr = new Lfsr(registerDefaultValue, registerDefaultValueBinaryString.Length, polynomial);
            BigInteger encryptedBits = lfsr.EncryptBitByBit(bitsToEncrypt);
            EncryptedTextTextBox.Text = ConvertHelper.ByteArrayToString(encryptedBits.ToByteArray());
        }

        private void EncryptFileButton_Click(object sender, RoutedEventArgs e)
        {
            string? pathToFile = SelectedFileLabel.Content.ToString();

            if (string.IsNullOrEmpty(pathToFile))
            {
                return;
            }

            string polynomialString = PolynomialTextBox.Text;
            string registerDefaultValueBinaryString = RegisterDefaultValueTextBox.Text;
            byte[] bytesToEncrypt = File.ReadAllBytes(pathToFile);

            int[] polynomial = new int[0];
            try
            {
                polynomial = ConvertHelper.StringPolynomialToArray(polynomialString);
            }
            catch
            {
                MessageBox.Show("Polynimial was not in a correct format!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            BigInteger registerDefaultValue = new BigInteger();
            try
            {
                registerDefaultValue = ConvertHelper.BinaryStringToBigInteger(registerDefaultValueBinaryString);
            }
            catch
            {
                MessageBox.Show("Register default value was not in a correct format!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Lfsr lfsr = new Lfsr(registerDefaultValue, registerDefaultValueBinaryString.Length, polynomial);
            byte[] encryptedBytes = new byte[0];

            TimeElapsedForFileLabel.Content = "0 s";
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();


            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;

            bw.DoWork += new DoWorkEventHandler(
            delegate (object o, DoWorkEventArgs args)
            {
                encryptedBytes = lfsr.EncryptBitByBit(new BigInteger(bytesToEncrypt)).ToByteArray();
            });

            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
            delegate (object o, RunWorkerCompletedEventArgs args)
            {
                timer.Stop();
                string saveFilePath;
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = System.IO.Path.GetExtension(pathToFile);

                if (saveFileDialog.ShowDialog() == true)
                {
                    saveFilePath = saveFileDialog.FileName;
                }
                else
                {
                    return;
                }
                File.WriteAllBytes(saveFilePath, encryptedBytes);
            });

            bw.RunWorkerAsync();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            TimeElapsedForFileLabel.Content = (int.Parse(TimeElapsedForFileLabel.Content.ToString().Split(' ')[0]) + 1).ToString() + " s";
        }

        private void FileSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            string pathToFile;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                pathToFile = openFileDialog.FileName;
            }
            else
            {
                return;
            }

            SelectedFileLabel.Content = openFileDialog.FileName;
            long fileSize = GetFileSize(pathToFile);
            string fileSizeInMb = (fileSize / (1024.0 * 1024)).ToString();
            FileSizeLabel.Content = (fileSizeInMb).Substring(
                0,
                fileSizeInMb.LastIndexOf(',') + GetNeededAfterDotLength(fileSizeInMb) + 1
                ) + " MB";
        }

        private static long GetFileSize(string FilePath)
        {
            if (File.Exists(FilePath))
            {
                return new FileInfo(FilePath).Length;
            }
            return 0;
        }

        private static int GetNeededAfterDotLength(string str)
        {
            int neededAfterDotLength = 2;
            int afterDotLength = str.Substring(str.LastIndexOf(',') + 1, str.Length - (str.LastIndexOf(',') + 1)).Length;

            if (afterDotLength < neededAfterDotLength)
            {
                return afterDotLength;
            }
            else
            {
                return neededAfterDotLength;
            }
        }

        private void CipherSelectionMenuButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new CipherSelectionPage());
        }
    }
}
