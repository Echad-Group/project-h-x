using System.Net;
using System.Net.Http;
using System.Text;
using ProjectHX.Mobile.Models.Auth;
using ProjectHX.Mobile.Models.Results;
using ProjectHX.Mobile.Services;
using Xunit;

namespace ProjectHX.Mobile.Tests.Services;

public class ApiSmokeTests
{
    [Fact]
    public async Task AuthLogin_UsesExpectedEndpoint_AndParsesToken()
    {
        var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"token\":\"jwt-token\",\"otpRequired\":false}", Encoding.UTF8, "application/json")
        });

        var sut = new AuthApiService(CreateClient(handler));
        var response = await sut.LoginAsync(new LoginRequest { Email = "a@b.c", Password = "pw" });

        Assert.EndsWith("auth/login", handler.LastRequestPath, StringComparison.Ordinal);
        Assert.Equal(HttpMethod.Post, handler.LastMethod);
        Assert.Equal("jwt-token", response.Token);
    }

    [Fact]
    public async Task TasksList_UsesExpectedEndpoint_AndParsesItems()
    {
        var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("[{\"id\":1,\"title\":\"Do thing\",\"status\":\"Pending\"}]", Encoding.UTF8, "application/json")
        });

        var sut = new TasksApiService(CreateClient(handler));
        var tasks = await sut.GetMyTasksAsync();

        Assert.EndsWith("tasks/my-tasks", handler.LastRequestPath, StringComparison.Ordinal);
        Assert.Equal(HttpMethod.Get, handler.LastMethod);
        Assert.Single(tasks);
        Assert.Equal(1, tasks[0].Id);
    }

    [Fact]
    public async Task InboxRead_UsesExpectedEndpoint()
    {
        var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"message\":\"Read acknowledged.\"}", Encoding.UTF8, "application/json")
        });

        var sut = new InboxApiService(CreateClient(handler));
        var message = await sut.MarkReadAsync(99);

        Assert.EndsWith("messages/99/read", handler.LastRequestPath, StringComparison.Ordinal);
        Assert.Equal(HttpMethod.Post, handler.LastMethod);
        Assert.Equal("Read acknowledged.", message);
    }

    [Fact]
    public async Task ResultSubmit_UsesExpectedEndpoint_AndParsesMessage()
    {
        var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"message\":\"Result captured\"}", Encoding.UTF8, "application/json")
        });

        var sut = new ResultsApiService(CreateClient(handler));
        var response = await sut.SubmitAsync(new ResultSubmissionRequest
        {
            PollingStationCode = "PS-22",
            CandidateA = 1,
            CandidateB = 2,
            CandidateC = 3,
            RejectedVotes = 0,
            RegisteredVoters = 20
        });

        Assert.EndsWith("results/submit", handler.LastRequestPath, StringComparison.Ordinal);
        Assert.Equal(HttpMethod.Post, handler.LastMethod);
        Assert.Equal("Result captured", response);
    }

    private static HttpClient CreateClient(HttpMessageHandler handler)
    {
        return new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost/api/")
        };
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

        public string? LastRequestPath { get; private set; }
        public HttpMethod? LastMethod { get; private set; }

        public RecordingHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequestPath = request.RequestUri?.PathAndQuery.TrimStart('/');
            LastMethod = request.Method;
            return Task.FromResult(_responseFactory(request));
        }
    }
}
