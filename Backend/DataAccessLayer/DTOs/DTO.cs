using log4net;
using System.Reflection;
using log4net.Config;
using System.IO;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{

    public abstract class DTO
    {
        protected DalMapper _mapper;
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected DTO(DalMapper dalMapper)
        {
            _mapper = dalMapper;
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        public abstract void Persist();
    }
}