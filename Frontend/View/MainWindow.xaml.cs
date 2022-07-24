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
using Frontend.Model;
using Frontend.ViewModel;

namespace Frontend.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;
        

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            viewModel = (MainViewModel)DataContext;
        }

        
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            UserModel u = viewModel.Login();
            if (u != null)
            {
                BoardsOfUserView boardsOfUserView = new BoardsOfUserView(u);
                boardsOfUserView.Show();
                Close();
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            UserModel u=viewModel.Register();
            {
                BoardsOfUserView boardsOfUserView = new BoardsOfUserView(u);
                boardsOfUserView.Show();
                Close();
            }
        }
    }
}