 
using ServiceFramework.Common;

namespace QStack.Framework.Core.Persistent
{
    public class DaoCollection : IDaoCollection
    {
        private string defaultFactoryName;
        public string DefaultFactoryName
        {
            get { return defaultFactoryName; }
            //set { defaultFactoryName = value; }
        }

        SessionContext sessionContext;
        public DaoCollection(SessionContext sessionContext)
        {
            this.sessionContext = sessionContext;
        }

       
        public DaoCollection(string defaultFactoryName)
        {
            this.defaultFactoryName = defaultFactoryName;
        }

        public IDao CurrentDao
        {
            get
            {
                if (string.IsNullOrWhiteSpace(defaultFactoryName))
                    throw new ServiceFrameworkException("defaultFactoryName not set");
                return GetDao(defaultFactoryName);
            }
        }


        public IDao this[string factoryName]
        {
            get
            {
                return GetDao(factoryName);
            }
        }

        IDao GetDao(string factoryName)
        {           
            //SessionContext sessionContext = SessionContext.CurrentContext;

            if (sessionContext==null)
                throw new ServiceFrameworkException("SessionContext not found");

            return sessionContext.GetDao(factoryName);
        }
         public void SetDefaultFactoryName(string name)
         {
            this.defaultFactoryName = name;
         }

      
    }//end DaoCollection

}//end namespace Persistent