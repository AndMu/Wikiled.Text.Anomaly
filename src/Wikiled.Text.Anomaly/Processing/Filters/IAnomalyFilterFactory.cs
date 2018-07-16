namespace Wikiled.Text.Anomaly.Processing.Filters
{
    public interface IAnomalyFilterFactory
    {
        IAnomalyFilter Create(FilterTypes type);
    }
}