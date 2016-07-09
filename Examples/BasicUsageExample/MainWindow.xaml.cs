using System;
using System.Windows;

namespace BasicUsageExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = _vm = new MainViewModel();
        }

        private int _count = 0;
        private readonly MainViewModel _vm;

        private void Button_ShowInformationClick(object sender, RoutedEventArgs e)
        {
            _vm.ShowInformation(String.Format("{0} Information", _count++));
        }

        private void Button_ShowSuccessClick(object sender, RoutedEventArgs e)
        {
            _vm.ShowSuccess(String.Format("{0} Success", _count++));
        }

        private void Button_ShowWarningClick(object sender, RoutedEventArgs e)
        {
            _vm.ShowWarning(String.Format("{0} Warning", _count++));
        }

        private void Button_ShowErrorClick(object sender, RoutedEventArgs e)
        {
            _vm.ShowError(String.Format("{0} Error", _count++));
        }
    }
}
