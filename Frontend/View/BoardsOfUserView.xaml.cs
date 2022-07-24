using System.Windows;
using Frontend.Model;
using Frontend.ViewModel;

namespace Frontend.View;

public partial class BoardsOfUserView : Window
{
    private BoardsOfUserViewModel viewModel;
    private UserModel user;
    
    public BoardsOfUserView(UserModel u)
    {
        InitializeComponent();
        user = u;
        viewModel = new BoardsOfUserViewModel(u);
        DataContext = viewModel;
    }
    
    private void Choose_Button(object sender, RoutedEventArgs e)
    {
        BoardModel board = viewModel.ChoosenBoard();
        if (board != null)
        {
            BoardView boardView = new BoardView(user, board); //the chosenBoard Model
            boardView.Show();
            Close();
        }
    }

    private void LogOut_Click(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
        Close();
    }
}