namespace QStack.Framework.Core.Persistent
{
    public interface IDaoCollection
    {
        IDao CurrentDao
        {
            get;
        }

        IDao this[string daoName]
        {
            get;        
        }

        void SetDefaultFactoryName(string name);

    }//end IDaoCollection

}//end namespace Persistent