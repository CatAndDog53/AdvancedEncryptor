using Cryptography.AesAlgorithm;
using Cryptography;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Cryptography.Gost;
using System.Windows.Threading;
using Cryptography.LfsrAlgorithm;
using System.Numerics;

namespace WpfUI
{
    /// <summary>
    /// Interaction logic for GostPage.xaml
    /// </summary>
    public partial class GostPage : Page
    {
        public GostPage()
        {
            InitializeComponent();
        }

        private void EcbToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (EcbToggleButton.IsChecked == true)
            {
                XorToggleButton.IsChecked = false;
                CfbToggleButton.IsChecked = false;
                IvStackPanel.IsEnabled = false;
            }
            else
            {
                EcbToggleButton.IsChecked = true;
                IvStackPanel.IsEnabled = false;
            }
        }

        private void XorToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (XorToggleButton.IsChecked == true)
            {
                EcbToggleButton.IsChecked = false;
                CfbToggleButton.IsChecked = false;
                IvStackPanel.IsEnabled = true;
            }
            else
            {
                XorToggleButton.IsChecked = true;
                IvStackPanel.IsEnabled = true;
            }
        }

        private void CfbToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (CfbToggleButton.IsChecked == true)
            {
                EcbToggleButton.IsChecked = false;
                XorToggleButton.IsChecked = false;
                IvStackPanel.IsEnabled = true;
            }
            else
            {
                CfbToggleButton.IsChecked = true;
                IvStackPanel.IsEnabled = true;
            }
        }

        private void EncryptToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (EncryptToggleButton.IsChecked == true)
            {
                DecryptToggleButton.IsChecked = false;
                EncryptBtn.Content = "Encrypt";
                TextToEncryptTextLabel.Content = "Enter the text to encrypt:";
                DecryptedTextLabel.Content = "Encrypted text in HEX:";
                EncryptFileButton.Content = "Encrypt";
            }
            else
            {
                EncryptToggleButton.IsChecked = true;
                EncryptBtn.Content = "Encrypt";
                TextToEncryptTextLabel.Content = "Enter the text to encrypt:";
                DecryptedTextLabel.Content = "Encrypted text in HEX:";
                EncryptFileButton.Content = "Encrypt";
            }
        }

        private void DecryptToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (DecryptToggleButton.IsChecked == true)
            {
                EncryptToggleButton.IsChecked = false;
                EncryptBtn.Content = "Decrypt";
                TextToEncryptTextLabel.Content = "Enter the text to decrypt:";
                DecryptedTextLabel.Content = "Decrypted text:";
                EncryptFileButton.Content = "Decrypt";
            }
            else
            {
                DecryptToggleButton.IsChecked = true;
                EncryptBtn.Content = "Decrypt";
                TextToEncryptTextLabel.Content = "Enter the text to decrypt:";
                DecryptedTextLabel.Content = "Decrypted text:";
                EncryptFileButton.Content = "Decrypt";
            }
        }






        private void FileSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            long GetFileSize(string FilePath)
            {
                if (File.Exists(FilePath))
                {
                    return new FileInfo(FilePath).Length;
                }
                return 0;
            }

            int GetNeededAfterDotLength(string str)
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

        private void KeyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ConvertHelper.StringToByteArray(KeyTextBox.Text).Length < GetNeededKeyLength())
            {
                KeyIsTooShortLabel.Visibility = Visibility.Visible;
            }
            else
            {
                KeyIsTooShortLabel.Visibility = Visibility.Hidden;
            }
        }

        private int GetNeededKeyLength()
        {
            return 32;
        }

        private void EncryptBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool encrypt = EncryptToggleButton.IsChecked == true;
                bool decrypt = DecryptToggleButton.IsChecked == true;
                bool useEcb = EcbToggleButton.IsChecked == true;
                bool useXor = XorToggleButton.IsChecked == true;
                bool useCfb = CfbToggleButton.IsChecked == true;
                
                byte[] key = ConvertHelper.StringToByteArray(KeyTextBox.Text);
                byte[]? iV = null;
                if (useXor || useCfb)
                {
                    iV = ConvertHelper.StringToByteArray(IvTextBox.Text);
                    if (iV.Length < 8)
                    {
                        MessageBox.Show("Selected algorithm requires 8-byte IV", "IV is too short!", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                if (key.Length < GetNeededKeyLength())
                {
                    MessageBox.Show("Selected algorithm requires " + GetNeededKeyLength() + "-byte key.", "Key is too short!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                byte[] bytesToProcess = new byte[0];
                byte[] processedBytes = new byte[0];

                if (encrypt && !decrypt)
                {
                    bytesToProcess = ConvertHelper.StringToByteArray(TextToEncryptTextBox.Text);
                    processedBytes = EncryptOrDecryptFile(bytesToProcess, key, iV, encrypt, decrypt, useEcb, useXor, useCfb);
                    string encryptedText = ConvertHelper.ByteArrayToHexString(processedBytes);
                    EncryptedTextTextBox.Text = encryptedText;
                }
                else if (decrypt && !encrypt)
                {
                    bytesToProcess = ConvertHelper.HexStringToByteArray(TextToEncryptTextBox.Text);
                    processedBytes = EncryptOrDecryptFile(bytesToProcess, key, iV, encrypt, decrypt, useEcb, useXor, useCfb);
                    string encryptedText = ConvertHelper.ByteArrayToString(processedBytes);
                    EncryptedTextTextBox.Text = encryptedText;
                }
            }
            catch
            {
                MessageBox.Show("Error!", "Critical error occured", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void EncryptFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool encrypt = EncryptToggleButton.IsChecked == true;
                bool decrypt = DecryptToggleButton.IsChecked == true;
                bool useEcb = EcbToggleButton.IsChecked == true;
                bool useXor = XorToggleButton.IsChecked == true;
                bool useCfb = CfbToggleButton.IsChecked == true;

                byte[] key = ConvertHelper.StringToByteArray(KeyTextBox.Text);
                byte[]? iV = null;
                if (useXor || useCfb)
                {
                    iV = ConvertHelper.StringToByteArray(IvTextBox.Text);
                    if (iV.Length < 8)
                    {
                        MessageBox.Show("Selected algorithm requires 8-byte IV", "IV is too short!", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                if (SelectedFileLabel.Content == null)
                {
                    return;
                }
                string? pathToFile = SelectedFileLabel.Content.ToString();
                if (string.IsNullOrEmpty(pathToFile))
                {
                    return;
                }

                if (key.Length < GetNeededKeyLength())
                {
                    MessageBox.Show("Selected algorithm requires " + GetNeededKeyLength() + "-byte key.", "Key is too short!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                byte[] bytesToProcess = File.ReadAllBytes(pathToFile);
                byte[] processedBytes = new byte[0];

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
                    try
                    {
                        processedBytes = EncryptOrDecryptFile(bytesToProcess, key, iV, encrypt, decrypt, useEcb, useXor, useCfb);

                    }
                    catch
                    {
                        MessageBox.Show("Error!", "Critical error occured", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                });

                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
            delegate (object o, RunWorkerCompletedEventArgs args)
            {
                timer.Stop();
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
                File.WriteAllBytes(saveFilePath, processedBytes);
            });

                bw.RunWorkerAsync();

                
            }
            catch
            {
                MessageBox.Show("Error!", "Critical error occured", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            TimeElapsedForFileLabel.Content = (int.Parse(TimeElapsedForFileLabel.Content.ToString().Split(' ')[0]) + 1).ToString() + " s";
        }

        private byte[] EncryptOrDecryptFile(byte[] bytesToProcess, byte[] key, byte[] iV, bool encrypt, bool decrypt, bool useEcb, bool useXor, bool useCfb)
        {
            Gost28147 gost = new Gost28147(key, iV);

            byte[] processedBytes = new byte[0];

            if (encrypt || !decrypt)
            {
                if (useEcb) processedBytes = gost.SubstitutionEncode(key, bytesToProcess);
                else if (useXor) processedBytes = gost.XOREncode(key, iV, bytesToProcess);
                else if (useCfb) processedBytes = gost.CFBEncode(key, iV, bytesToProcess);
            }
            else if (decrypt || !encrypt)
            {
                if (useEcb) processedBytes = gost.SubstitutionDecode(key, bytesToProcess);
                else if (useXor) processedBytes = gost.XORDecode(key, iV, bytesToProcess);
                else if (useCfb) processedBytes = gost.CFBDecode(key, iV, bytesToProcess);
            }

            gost.Dispose();
            return processedBytes;
        }

        private void CipherSelectionMenuButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new CipherSelectionPage());
        }
    }
}
