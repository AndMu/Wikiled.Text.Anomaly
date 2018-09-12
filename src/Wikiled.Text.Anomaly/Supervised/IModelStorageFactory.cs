namespace Wikiled.Text.Anomaly.Supervised
{
    public interface IModelStorageFactory
    {
        IModelStorage Construct(string name);

        void Save(string name, IModelStorage storage);
    }
}