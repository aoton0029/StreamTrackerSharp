using StreamTracker.Items;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace StreamTracker.UserControls
{
    /// <summary>
    /// StreamCard.xaml の相互作用ロジック
    /// </summary>
    public partial class StreamCard : UserControl
    {
        public StreamCard()
        {
            InitializeComponent();
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is StreamCardViewModel vm)
            {
                Process.Start(new ProcessStartInfo(vm.StreamUrl) { UseShellExecute = true });
            }
        }
    }
}
