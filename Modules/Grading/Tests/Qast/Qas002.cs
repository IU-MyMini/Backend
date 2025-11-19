using System.Net;

namespace GradingModule.Tests.Qast;

public class Qas002
{
    private readonly HttpClient _httpClient;

    public Qas002()
    {
        const string testBaseUrlEnv = "TEST_BASE_URL";
        var          baseUrl        = Environment.GetEnvironmentVariable(testBaseUrlEnv);
        if (baseUrl is null)
            Assert.Fail($"{testBaseUrlEnv} env variable is not defined");

        _httpClient             = new HttpClient();
        _httpClient.BaseAddress = new Uri(baseUrl);
    }

    [Fact]
    public async Task Qast002_2()
    {
        const string envVar    = "QAST002_2_ENDPOINTS";
        var          endpoints = Environment.GetEnvironmentVariable(envVar)?.Split(",");

        if (endpoints is null)
            Assert.Fail($"{envVar} env variable is not defined");

        var tasks   = endpoints.Select(e => _httpClient.GetAsync(e));
        var results = await Task.WhenAll(tasks);

        foreach (var resp in results)
        {
            Assert.True(
                resp.StatusCode is HttpStatusCode.Unauthorized,
                $"Got unauthorized access to {resp.RequestMessage?.Method} {resp.RequestMessage?.RequestUri}"
            );
        }
    }
}