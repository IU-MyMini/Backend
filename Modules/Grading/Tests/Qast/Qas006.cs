using System.Diagnostics;

using Xunit.Abstractions;

namespace GradingModule.Tests.Qast;

public class Qas006
{
    private readonly HttpClient        _httpClient;
    private readonly ITestOutputHelper _output;

    public Qas006(ITestOutputHelper output)
    {
        _output = output;

        const string testBaseUrlEnv = "TEST_BASE_URL";
        var          baseUrl        = Environment.GetEnvironmentVariable(testBaseUrlEnv);
        if (baseUrl is null)
            Assert.Fail($"{testBaseUrlEnv} env variable is not defined");

        _httpClient             = new HttpClient();
        _httpClient.BaseAddress = new Uri(baseUrl);
    }

    [Fact]
    public async Task Qast006_1()
    {
        const string envVar   = "QAST006_1_ENDPOINT";
        var          endpoint = Environment.GetEnvironmentVariable(envVar);

        if (endpoint is null)
            Assert.Fail($"{envVar} env variable is not defined");

        var          testDuration = new TimeSpan(0, 0, 10);
        const double percentile   = 0.99;
        const double targetTime   = 500; // milliseconds

        var responseTimes = new List<long>(); // milliseconds

        var testTimer = Stopwatch.StartNew();
        while (testTimer.Elapsed < testDuration)
        {
            var stopwatch = Stopwatch.StartNew();

            var resp = await _httpClient.GetAsync(endpoint);

            Assert.True(resp.IsSuccessStatusCode, $"Status code does not indicate success: {resp.StatusCode}");

            responseTimes.Add(stopwatch.ElapsedMilliseconds);
            stopwatch.Stop();
        }

        testTimer.Stop();

        responseTimes.Sort();

        var index      = percentile / 100.0 * (responseTimes.Count - 1);
        var lowerIndex = (int)Math.Floor(index);
        var upperIndex = (int)Math.Ceiling(index);
        var weight     = index - lowerIndex;

        var x = responseTimes[lowerIndex] * (1 - weight) + responseTimes[upperIndex] * weight;

        _output.WriteLine($"{percentile}-percentile: {x} milliseconds");
        Assert.True(x < targetTime);
    }
}