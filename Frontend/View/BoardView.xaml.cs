using System.Windows;
using Frontend.Model;
using Frontend.ViewModel;

namespace Frontend.View;

public partial class BoardView : Window
{
    private BoardViewModel viewModel;
    private UserModel user;

    public BoardView(UserModel u, BoardModel bm)
    {
        InitializeComponent();
        user = u;
        viewModel = new BoardViewModel(bm);
        DataContext = viewModel;
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        BoardsOfUserView boardsOfUserView = new BoardsOfUserView(user);
        boardsOfUserView.Show();
        this.Close();
    }
}