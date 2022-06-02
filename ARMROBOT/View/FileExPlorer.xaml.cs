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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ARMROBOT.View
{
    /// <summary>
    /// Interaction logic for FileExPlorer.xaml
    /// </summary>
    public partial class FileExPlorer : Window
    {
       public static DispatcherTimer timerResfret = new DispatcherTimer();
        public FileExPlorer()
        {
            
            InitializeComponent();
            timerResfret.Tick += TimerResfret_Tick;
            timerResfret.Interval = TimeSpan.FromMilliseconds(0.5);
        }

        private void TimerResfret_Tick(object sender, EventArgs e)
        {
            testr.UpdateLayout();
            testr.Items.Refresh();
            timerResfret.IsEnabled = false;
        }
    }
}
