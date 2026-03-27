using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isHistoryOpen = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleHistory();
        }

        private void Overlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_isHistoryOpen)
                ToggleHistory();
        }

        private void ToggleHistory()
        {
            var show = (Storyboard)FindResource("ShowHistory");
            var hide = (Storyboard)FindResource("HideHistory");

            if (_isHistoryOpen)
            {
                hide.Begin();
                Overlay.Visibility = Visibility.Collapsed;
            }
            else
            {
                Overlay.Visibility = Visibility.Visible;
                show.Begin();
            }

            _isHistoryOpen = !_isHistoryOpen;
        }
    }
}