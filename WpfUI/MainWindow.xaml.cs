using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Cryptography;
using System.ComponentModel;
using Cryptography.AesAlgorithm;
using Cryptography.SdesAlgorithm;
using System.Diagnostics;
using System;
using System.Windows.Navigation;

namespace WpfUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Content = new CipherSelectionPage();
        }
    }
}
