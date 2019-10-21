namespace Wikiled.Text.Anomaly.Structure
{
    public interface IModelFactory<T>
        where T : class, IModel
    {
        T CreateNew();

        T Load (string path);

        void Save(string path, T model);
    }
}
