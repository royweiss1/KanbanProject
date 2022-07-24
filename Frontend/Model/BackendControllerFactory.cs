using IntroSE.Kanban.Backend.ServiceLayer;

namespace Frontend.Model;

public static class BackendControllerFactory
{
    private static FactoryService? fs;

    public static FactoryService GetFactory()
    {
        if (fs == null)
        {
            fs = new FactoryService();
            fs.DataService.LoadData();
        }

        return fs;
    }
}