namespace Wikiled.Text.Anomaly.Processing.Filters
{
    public interface IAnomalyFilter
    {
        FilterTypes Type { get; }


        TextCluster[] Filter(DocumentClusters document);
    }
}