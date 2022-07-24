using IntroSE.Kanban.Backend.BusinessLayer;

namespace IntroSE.Kanban.Backend.ServiceLayer;

internal class Loader
{
    private UserController _userController;
    private BoardController _boardController;

    public Loader(UserController us, BoardController bs)
    {
        _userController = us;
        _boardController = bs;

    }

    public void LoadData()
    {
        _userController.LoadData();
       _boardController.LoadData();

    }

}