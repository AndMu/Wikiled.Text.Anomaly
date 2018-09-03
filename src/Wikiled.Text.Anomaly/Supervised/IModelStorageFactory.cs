namespace Wikiled.Text.Anomaly.Supervised
{
    public interface IModelStorageFactory
    {
        IModelStorage Construct(string name);
    }
}