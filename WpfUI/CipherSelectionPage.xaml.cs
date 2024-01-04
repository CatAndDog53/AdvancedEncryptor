using System;
using System.Collections.Generic;
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
    /// Interaction logic for CipherSelectionPage.xaml
    /// </summary>
    public partial class CipherSelectionPage : Page
    {
        AesPage? _aesPage = null;
        SdesPage? _sdesPage = null;

        public CipherSelectionPage()
        {
            InitializeComponent();
        }

        private void AesListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_aesPage == null)
            {
                _aesPage = new AesPage();
            }

            NavigationService.Navigate(_aesPage);
        }

        private void SdesListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_sdesPage == null)
            {
                _sdesPage = new SdesPage();
            }

            NavigationService.Navigate(_sdesPage);
        }

        private void GostListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new GostPage());
        }

        private void LfsrListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new LfsrPage());
        }
    }
}
