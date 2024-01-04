using Cryptography.SdesAlgorithm;
using Cryptography;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

namespace WpfUI
{
    /// <summary>
    /// Interaction logic for SdesPage.xaml
    /// </summary>
    public partial class SdesPage : Page
    {
        public SdesPage()
        {
            InitializeComponent();
        }

        private void EncryptBtn_Click(object sender, RoutedEventArgs e)
        {
            string textToEncrypt = TextToEncryptTextBox.Text;
            int input10bitKey;
            if (!int.TryParse(Input10bitKeyTextBox.Text, out input10bitKey))
            {
                return;
            }

            SDES sDES = new SDES(input10bitKey);
            byte[] bytesToEncrypt = ConvertHelper.StringToByteArray(textToEncrypt);
            byte[] encryptedBytes = new byte[bytesToEncrypt.Length];
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            if (EnableParallelBlockEnryptionCheckBox.IsChecked == true)
            {
                if (EncryptToggleButton.IsChecked == true)
                {
                    encryptedBytes = sDES.EncryptParallel(bytesToEncrypt);
                }
                else if (DecryptToggleButton.IsChecked == true)
                {
                    try
                    {
                        bytesToEncrypt = Convert.FromHexString(textToEncrypt);
                    }
                    catch
                    {
                        return;
                    }
                    encryptedBytes = sDES.DecryptParallel(bytesToEncrypt);
                }

            }
            else
            {
                if (EncryptToggleButton.IsChecked == true)
                {
                    encryptedBytes = sDES.Encrypt(bytesToEncrypt);
                }
                else if (DecryptToggleButton?.IsChecked == true)
                {
                    try
                    {
                        bytesToEncrypt = Convert.FromHexString(textToEncrypt);
                    }
                    catch
                    {
                        return;
                    }

                    encryptedBytes = sDES.Decrypt(bytesToEncrypt);
                }

            }
            stopwatch.Stop();

            TimeElapsedLabel.Content = stopwatch.ElapsedMilliseconds + " ms";

            if (EncryptToggleButton.IsChecked == true)
            {
                EncryptedTextTextBox.Text = ConvertHelper.ByteArrayToHexString(encryptedBytes);
            }
            else if (DecryptToggleButton.IsChecked == true)
            {
                EncryptedTextTextBox.Text = ConvertHelper.ByteArrayToString(encryptedBytes);
            }

        }

        private void Input10bitKeyTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(Input10bitKeyTextBox.Text))
            {
                KeyInBinaryStackPanel.Visibility = Visibility.Hidden;
                return;
            }

            KeyInBinaryStackPanel.Visibility = Visibility.Visible;

            int input10bitKey;
            if (!int.TryParse(Input10bitKeyTextBox.Text, out input10bitKey))
            {
                KeyInBinaryTextBox.Text = "ERROR";
                return;
            }

            if (input10bitKey > 1023)
            {
                KeyInBinaryTextBox.Text = "The key is too long!";
                return;
            }

            try
            {
                KeyInBinaryTextBox.Text = Convert.ToString(input10bitKey, 2);
            }
            catch
            {
                KeyInBinaryTextBox.Text = "ERROR";
            }
        }

        private void EncryptToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (EncryptToggleButton.IsChecked == true)
            {
                EncryptBtn.Content = "Encrypt";
                DecryptedTextLabel.Content = "Encrypted text in HEX:";

                EncryptFileButton.Content = "Encrypt";

                DecryptToggleButton.IsChecked = false;
            }
        }

        private void DecryptToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (DecryptToggleButton.IsChecked == true)
            {
                EncryptBtn.Content = "Decrypt";
                DecryptedTextLabel.Content = "Decrypted text:";

                EncryptFileButton.Content = "Decrypt";

                EncryptToggleButton.IsChecked = false;
            }
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

        private void EncryptFileButton_Click(object sender, RoutedEventArgs e)
        {
            string? pathToFile = SelectedFileLabel.Content.ToString();
            int input10bitKey;
            if (!int.TryParse(Input10bitKeyTextBox.Text, out input10bitKey))
            {
                return;
            }

            if (string.IsNullOrEmpty(pathToFile))
            {
                return;
            }

            bool parallelBlockEncryption = EnableParallelFileBlockEnryptionCheckBox.IsChecked == true;
            bool decryption = DecryptToggleButton.IsChecked == true;
            byte[] bytesToEncrypt = File.ReadAllBytes(pathToFile);
            byte[] encryptedBytes = new byte[bytesToEncrypt.Length];
            FileEncryptionProgressBar.Minimum = 0;
            FileEncryptionProgressBar.Maximum = 100;
            FileEncryptionProgressBar.Value = 0;

            Stopwatch stopwatch = new Stopwatch();
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;

            bw.DoWork += new DoWorkEventHandler(
            delegate (object o, DoWorkEventArgs args)
            {
                BackgroundWorker b = o as BackgroundWorker;
                stopwatch.Start();
                encryptedBytes = EncryptOrDecryptFile(bytesToEncrypt, input10bitKey, decryption, parallelBlockEncryption, b);
            });

            bw.ProgressChanged += new ProgressChangedEventHandler(
            delegate (object o, ProgressChangedEventArgs args)
            {
                FileEncryptionProgressBar.Value++;
            });

            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
            delegate (object o, RunWorkerCompletedEventArgs args)
            {
                stopwatch.Stop();
                TimeElapsedForFileLabel.Content = stopwatch.ElapsedMilliseconds + " ms";
                MessageBox.Show("Done", "Progress report", MessageBoxButton.OK, MessageBoxImage.Information);

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

        public byte[] EncryptOrDecryptFile(byte[] bytesToEncrypt, int input10bitKey, bool decryption, bool parallelBlockEncryption, BackgroundWorker bw)
        {
            SDES sDES = new SDES(input10bitKey);
            byte[] encryptedBytes;

            if (parallelBlockEncryption)
            {
                if (!decryption)
                {
                    encryptedBytes = sDES.EncryptParallel(bytesToEncrypt, bw);
                }
                else
                {
                    encryptedBytes = sDES.DecryptParallel(bytesToEncrypt, bw);
                }
            }
            else
            {
                if (!decryption)
                {
                    encryptedBytes = sDES.Encrypt(bytesToEncrypt, bw);
                }
                else
                {
                    encryptedBytes = sDES.Decrypt(bytesToEncrypt, bw);
                }
            }

            return encryptedBytes;
        }

        private void CipherSelectionMenuButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new CipherSelectionPage());
        }
    }
}
