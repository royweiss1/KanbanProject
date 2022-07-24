using Frontend.Model;

namespace Frontend.ViewModel;

public class BoardViewModel : NotifiableObject
{
    public BoardModel Board { get; set; }
    public BackendController Controller { get; private set; }
    public string BacklogTitle { get; private set; } // the title on the top of the screen
    public string InProgressTitle { get; private set; } // the title on the top of the screen
    public string DoneTitle { get; private set; } // the title on the top of the screen

    public BoardViewModel(BoardModel boardModel)
    {
        Controller = new BackendController();
        Board = boardModel;
        BacklogTitle = "Backlog. Limit: " + Board.BacklogLimit;
        InProgressTitle = "InProgress. Limit: " + Board.InProgressLimit;
        DoneTitle = "Done. Limit: " + Board.DoneLimit;

    }
}