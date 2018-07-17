namespace Wikiled.Text.Anomaly.Processing.Filters
{
    public interface IAnomalyFilter
    {
        FilterTypes Type { get; }

        DetectionResults Filter(DocumentClusters document);
    }
}