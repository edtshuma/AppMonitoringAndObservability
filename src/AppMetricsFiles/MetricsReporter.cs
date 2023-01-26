using System;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace RegistrationAPI.AppMetricsFiles {

    
public class MetricReporter
{
    private readonly ILogger<MetricReporter> _logger;
    private readonly Counter _requestCounter;
    private readonly Gauge _requestGauge;
    private readonly Histogram _responseTimeHistogram;

    public MetricReporter(ILogger<MetricReporter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _requestCounter = Metrics.CreateCounter("total_requests", "The total number of requests serviced by this API.");
        _requestGauge = Metrics.CreateGauge("total_requests_gauge", "The total number of requests serviced by this API.");

        _responseTimeHistogram = Metrics.CreateHistogram("request_duration_seconds",
            "The duration in seconds between the response to a request.", new HistogramConfiguration
            {
                Buckets = Histogram.ExponentialBuckets(0.01, 2, 10),
                LabelNames = new[] { "status_code", "method" , "path"}
            });
    }

    public void RegisterRequest()
    {
        _requestCounter.Inc();
        _requestGauge.Inc();
    }

    public void RegisterResponseTime(int statusCode, string method,string path, TimeSpan elapsed)
    {
        _responseTimeHistogram.Labels(statusCode.ToString(), method, path).Observe(elapsed.TotalSeconds);
    }
}

}