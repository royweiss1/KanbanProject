using System;
using IntroSE.Kanban.Backend.BusinessLayer;

namespace IntroSE.Kanban.Backend.ServiceLayer;

public class DataService
{
    private Loader Loader { get; }
    private Deleter Deleter { get; }

    internal DataService(Loader loader, Deleter deleter )
    {
        Loader = loader;
        Deleter =deleter;
    }
    ///<summary>This method loads all persisted data.
    ///<para>
    ///<b>IMPORTANT:</b> When starting the system via the GradingService - do not load the data automatically, only through this method. 
    ///In some cases we will call LoadData when the program starts and in other cases we will call DeleteData. Make sure you support both options.
    ///</para>
    /// </summary>
    /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>

    public string LoadData()
    {
        try
        {
            Loader.LoadData();
            return JsonSerializerExtention.Serialize(new Response(null, null));
        }
        catch (Exception e)
        {
            return JsonSerializerExtention.Serialize(new Response(e.Message, null));

        }
    }
    ///<summary>This method deletes all persisted data.
    ///<para>
    ///<b>IMPORTANT:</b> 
    ///In some cases we will call LoadData when the program starts and in other cases we will call DeleteData. Make sure you support both options.
    ///</para>
    /// </summary>
    ///<returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>

    public string DeleteData()
    {
        try
        {
            Deleter.DeleteData();
            return JsonSerializerExtention.Serialize(new Response(null, null));
        }
        catch (Exception e)
        {
            return JsonSerializerExtention.Serialize(new Response(e.Message, null));

        }
        
    }
}