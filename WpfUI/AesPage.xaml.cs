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

namespace WpfUI
{
    /// <summary>
    /// Interaction logic for AesPage.xaml
    /// </summary>
    public partial class AesPage : Page
    {
        public AesPage()
        {
            InitializeComponent();
        }

        #region AES tab
        #region AES toogle buttons logic
        private void Aes128ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (Aes128ToggleButton.IsChecked == true)
            {
                Aes192ToggleButton.IsChecked = false;
                Aes256ToggleButton.IsChecked = false;
            }
            else
            {
                Aes128ToggleButton.IsChecked = true;
            }

        }

        private void Aes192ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (Aes192ToggleButton.IsChecked == true)
            {
                Aes128ToggleButton.IsChecked = false;
                Aes256ToggleButton.IsChecked = false;
            }
            else
            {
                Aes192ToggleButton.IsChecked = true;
            }
        }

        private void Aes256ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (Aes256ToggleButton.IsChecked == true)
            {
                Aes128ToggleButton.IsChecked = false;
                Aes192ToggleButton.IsChecked = false;
            }
            else
            {
                Aes256ToggleButton.IsChecked = true;
            }
        }

        private void EcbToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (EcbToggleButton.IsChecked == true)
            {
                CbcToggleButton.IsChecked = false;
                IvStackPanel.IsEnabled = false;
                EnableParallelFileBlockEnryptionCheckBox.IsEnabled = true;
                EnableParallelFileBlockEnryptionCheckBox.IsChecked = true;
            }
            else
            {
                EcbToggleButton.IsChecked = true;
                IvStackPanel.IsEnabled = false;
                EnableParallelFileBlockEnryptionCheckBox.IsEnabled = true;
                EnableParallelFileBlockEnryptionCheckBox.IsChecked = true;
            }
        }

        private void CbcToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (CbcToggleButton.IsChecked == true)
            {
                EcbToggleButton.IsChecked = false;
                IvStackPanel.IsEnabled = true;
                EnableParallelFileBlockEnryptionCheckBox.IsEnabled = false;
                EnableParallelFileBlockEnryptionCheckBox.IsChecked = false;
            }
            else
            {
                CbcToggleButton.IsChecked = true;
                IvStackPanel.IsEnabled = true;
                EnableParallelFileBlockEnryptionCheckBox.IsEnabled = false;
                EnableParallelFileBlockEnryptionCheckBox.IsChecked = false;
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
        #endregion

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
            if (Aes128ToggleButton.IsChecked == true) return 16;
            else if (Aes192ToggleButton.IsChecked == true) return 24;
            else return 32;
        }

        private void EncryptBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool encrypt = EncryptToggleButton.IsChecked == true;
                bool decrypt = DecryptToggleButton.IsChecked == true;
                bool useParallelBlockEncryption = EnableParallelFileBlockEnryptionCheckBox.IsChecked == true;
                bool useECB = EcbToggleButton.IsChecked == true;
                bool useCBC = CbcToggleButton.IsChecked == true;
                bool useAes128 = Aes128ToggleButton.IsChecked == true;
                bool useAes192 = Aes192ToggleButton.IsChecked == true;
                bool useAes256 = Aes256ToggleButton.IsChecked == true;
                byte[] key = ConvertHelper.StringToByteArray(KeyTextBox.Text);

                byte[]? iV = null;
                if (useCBC)
                {
                    iV = ConvertHelper.StringToByteArray(IvTextBox.Text);
                    if (iV.Length < 16)
                    {
                        MessageBox.Show("Selected algorithm requires 16-byte IV", "IV is too short!", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    processedBytes = EncryptOrDecryptFile(bytesToProcess, key, iV, useAes128, useAes192, useAes256, encrypt, decrypt, useParallelBlockEncryption, useECB, useCBC);
                    string encryptedText = ConvertHelper.ByteArrayToHexString(processedBytes);
                    EncryptedTextTextBox.Text = encryptedText;
                }
                else if (decrypt && !encrypt)
                {
                    bytesToProcess = ConvertHelper.HexStringToByteArray(TextToEncryptTextBox.Text);
                    processedBytes = EncryptOrDecryptFile(bytesToProcess, key, iV, useAes128, useAes192, useAes256, encrypt, decrypt, useParallelBlockEncryption, useECB, useCBC);
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
                FileEncryptionProgressBar.Value = 0;

                bool encrypt = EncryptToggleButton.IsChecked == true;
                bool decrypt = DecryptToggleButton.IsChecked == true;
                bool useParallelBlockEncryption = EnableParallelFileBlockEnryptionCheckBox.IsChecked == true;
                bool useECB = EcbToggleButton.IsChecked == true;
                bool useCBC = CbcToggleButton.IsChecked == true;
                bool useAes128 = Aes128ToggleButton.IsChecked == true;
                bool useAes192 = Aes192ToggleButton.IsChecked == true;
                bool useAes256 = Aes256ToggleButton.IsChecked == true;
                byte[] key = ConvertHelper.StringToByteArray(KeyTextBox.Text);

                byte[]? iV = null;
                if (useCBC)
                {
                    iV = ConvertHelper.StringToByteArray(IvTextBox.Text);
                    if (iV.Length < 16)
                    {
                        MessageBox.Show("Selected algorithm requires 16-byte IV", "IV is too short!", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                BackgroundWorker backgroundWorker = new BackgroundWorker();
                backgroundWorker.WorkerReportsProgress = true;

                backgroundWorker.DoWork += new DoWorkEventHandler(
                delegate (object o, DoWorkEventArgs args)
                {
                    BackgroundWorker? bw = o as BackgroundWorker;
                    try
                    {
                        processedBytes = EncryptOrDecryptFile(bytesToProcess, key, iV, useAes128, useAes192, useAes256, encrypt, decrypt, useParallelBlockEncryption, useECB, useCBC, bw);

                    }
                    catch
                    {
                        MessageBox.Show("Error!", "Critical error occured", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                });

                backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(
                delegate (object o, ProgressChangedEventArgs args)
                {
                    FileEncryptionProgressBar.Value = args.ProgressPercentage;
                });

                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                delegate (object o, RunWorkerCompletedEventArgs args)
                {
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

                backgroundWorker.RunWorkerAsync();
            }
            catch
            {
                MessageBox.Show("Error!", "Critical error occured", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

        }

        private byte[] EncryptOrDecryptFile(byte[] bytesToProcess, byte[] key, byte[] iV, bool useAes128, bool useAes192,
                                            bool useAes256, bool encrypt, bool decrypt, bool useParallelBlockEncryption, bool useECB, bool useCBC, BackgroundWorker? bw = null)
        {
            Aes aesEncryptor;
            AesVersions aesVersion;
            AesModes aesMode;

            if (useAes128) aesVersion = AesVersions.Aes128;
            else if (useAes192) aesVersion = AesVersions.Aes192;
            else if (useAes256) aesVersion = AesVersions.Aes256;
            else aesVersion = AesVersions.Aes128;

            if (useECB && !useParallelBlockEncryption) aesMode = AesModes.Ecb;
            else if (useCBC) aesMode = AesModes.Cbc;
            else if (useECB && useParallelBlockEncryption) aesMode = AesModes.EcbParallel;
            else aesMode = AesModes.Ecb;

            aesEncryptor = new Aes(key, aesVersion, aesMode, iV);

            byte[] processedBytes = new byte[0];

            if (encrypt)
            {
                processedBytes = aesEncryptor.Encrypt(bytesToProcess, bw);
            }
            else if (decrypt)
            {
                processedBytes = aesEncryptor.Decrypt(bytesToProcess, bw);
            }

            return processedBytes;
        }
        #endregion

        private void CipherSelectionMenuButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new CipherSelectionPage());
        }
    }
}
