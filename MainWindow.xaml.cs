using System.Windows;
using BubbelvriendWPF.Data;

namespace BubbelvriendWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AppData.LaadVoorbeeldData();
        }
    }
}
