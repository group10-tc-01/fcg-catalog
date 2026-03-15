using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace FCG.Catalog.Application.Observability
{
    public static class CatalogMetrics
    {
        private static readonly Meter Meter = new("FCG.Catalog");

        private static readonly Counter<long> CatalogsProcessedCounter = Meter.CreateCounter<long>(
            name: "catalogs_processed_total",
            unit: "catalogs");

        private static readonly Histogram<double> CatalogsAmount = Meter.CreateHistogram<double>(
            name: "catalogs_amount",
            unit: "currency");

        public static void RecordProcessed(string status, decimal amount, string reason = "none")
        {
            TagList tags = new()
            {
                { "status", status },
                { "reason", reason }
            };

            CatalogsProcessedCounter.Add(1, tags);
            CatalogsAmount.Record(decimal.ToDouble(amount), tags);
        }
    }
}
