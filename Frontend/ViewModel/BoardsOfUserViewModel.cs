using System;
using System.Windows;
using Frontend.Model;
using Frontend.View;

namespace Frontend.ViewModel;

public class BoardsOfUserViewModel : NotifiableObject
{
    public BackendController Controller { get; private set; }
    private UserModel user;
    
    public string ScreenTitle { get; private set; }
    public BoardsOfUserModel BoardsOfUser { get; private set; } //is Talked to through BINDING
    private BoardModel _selectedBoard;
    public BoardModel SelectedBoard
    {
        get
        {
            return _selectedBoard;
        }
        set
        {
            _selectedBoard = value;
            EnableForward = value != null;
            RaisePropertyChanged("SelectedBoard"); //TODO:MIND
        }
    }
    private bool _enableForward = false;
    public bool EnableForward
    {
        get => _enableForward;
        private set
        {
            _enableForward = value;
            RaisePropertyChanged("EnableForward");
        }
    }

    public BoardsOfUserViewModel(UserModel u)
    {
        Controller = new BackendController();
        user = u;
        ScreenTitle = "Boards for: " + user.Email; // the title at the top
        BoardsOfUser = new BoardsOfUserModel(Controller, user);
    }

    public BoardModel ChoosenBoard()
    {
        return SelectedBoard;
    }
}