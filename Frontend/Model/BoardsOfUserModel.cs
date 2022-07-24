using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Frontend.Model;

public class BoardsOfUserModel : NotifiableModelObject
{
    public ObservableCollection<BoardModel> Boards { get; set; }
    public BoardsOfUserModel(BackendController controller, UserModel userModel) :base(controller)
    {
        Boards = new ObservableCollection<BoardModel>(Controller.GetBoardsOfUser(userModel));
    }
}