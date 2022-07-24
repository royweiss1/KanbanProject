using IntroSE.Kanban.Backend.BusinessLayer;

namespace IntroSE.Kanban.Backend.ServiceLayer;

internal class Deleter
{

    private UserController _userController;
    private BoardController _boardController;

    public Deleter(UserController us, BoardController bs)
    {
        _userController = us;
        _boardController = bs;

    }

    public void DeleteData()
    {
        _userController.DeleteData();
        _boardController.DeleteData();

    }

    
}