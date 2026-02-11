using System.Net;
using Xunit;
using Irooon.Core.Runtime;

namespace Irooon.Tests.Runtime;

/// <summary>
/// HTTP クライアントプリミティブ (__httpRequest) の単体テスト。
/// MockHttpMessageHandler を使用し、実際のネットワーク通信は行わない。
/// </summary>
[Collection("HttpClient")]
public class HttpClientTests : IDisposable
{
    private MockHttpMessageHandler _handler = null!;

    private void SetupMock(HttpStatusCode statusCode = HttpStatusCode.OK, string content = "",
        Dictionary<string, string>? responseHeaders = null)
    {
        _handler = new MockHttpMessageHandler(statusCode, content, responseHeaders);
        RuntimeHelpers.SetHttpClient(new HttpClient(_handler));
    }

    public void Dispose()
    {
        RuntimeHelpers.ResetHttpClient();
    }

    #region GET リクエスト

    [Fact]
    public async Task HttpRequest_Get_ReturnsResponse()
    {
        SetupMock(HttpStatusCode.OK, "Hello World");

        var ctx = new ScriptContext();
        var result = RuntimeHelpers.__httpRequest(ctx, new object[] { "GET", "http://test.example.com/api", null!, null!, null! });

        Assert.IsAssignableFrom<Task<object>>(result);
        var response = await (Task<object>)result;
        var hash = Assert.IsType<Dictionary<string, object>>(response);
        Assert.Equal(200.0, hash["status"]);
        Assert.Equal("Hello World", hash["body"]);
        Assert.Equal(true, hash["ok"]);
    }

    [Fact]
    public async Task HttpRequest_Get_SendsCorrectMethod()
    {
        SetupMock();

        var ctx = new ScriptContext();
        await (Task<object>)RuntimeHelpers.__httpRequest(ctx, new object[] { "GET", "http://test.example.com/api", null!, null!, null! });

        Assert.Equal(HttpMethod.Get, _handler.LastRequest!.Method);
        Assert.Equal("http://test.example.com/api", _handler.LastRequest.RequestUri!.ToString());
    }

    #endregion

    #region POST リクエスト

    [Fact]
    public async Task HttpRequest_Post_SendsBody()
    {
        SetupMock(HttpStatusCode.Created, "{\"id\":1}");

        var ctx = new ScriptContext();
        var headers = new Dictionary<string, object> { ["Content-Type"] = "application/json" };
        var result = await (Task<object>)RuntimeHelpers.__httpRequest(ctx,
            new object[] { "POST", "http://test.example.com/users", headers, "{\"name\":\"Alice\"}", null! });

        var hash = Assert.IsType<Dictionary<string, object>>(result);
        Assert.Equal(201.0, hash["status"]);

        var requestBody = await _handler.LastRequest!.Content!.ReadAsStringAsync();
        Assert.Equal("{\"name\":\"Alice\"}", requestBody);
        Assert.Equal(HttpMethod.Post, _handler.LastRequest.Method);
    }

    #endregion

    #region PUT / DELETE / PATCH

    [Fact]
    public async Task HttpRequest_Put_Method()
    {
        SetupMock();
        var ctx = new ScriptContext();
        await (Task<object>)RuntimeHelpers.__httpRequest(ctx,
            new object[] { "PUT", "http://test.example.com/item/1", null!, "updated", null! });
        Assert.Equal(HttpMethod.Put, _handler.LastRequest!.Method);
    }

    [Fact]
    public async Task HttpRequest_Delete_Method()
    {
        SetupMock();
        var ctx = new ScriptContext();
        await (Task<object>)RuntimeHelpers.__httpRequest(ctx,
            new object[] { "DELETE", "http://test.example.com/item/1", null!, null!, null! });
        Assert.Equal(HttpMethod.Delete, _handler.LastRequest!.Method);
    }

    [Fact]
    public async Task HttpRequest_Patch_Method()
    {
        SetupMock();
        var ctx = new ScriptContext();
        await (Task<object>)RuntimeHelpers.__httpRequest(ctx,
            new object[] { "PATCH", "http://test.example.com/item/1", null!, "patched", null! });
        Assert.Equal(HttpMethod.Patch, _handler.LastRequest!.Method);
    }

    #endregion

    #region レスポンス構造

    [Fact]
    public async Task HttpRequest_Response_HasCorrectStructure()
    {
        SetupMock(HttpStatusCode.OK, "body content");

        var ctx = new ScriptContext();
        var result = await (Task<object>)RuntimeHelpers.__httpRequest(ctx,
            new object[] { "GET", "http://test.example.com/api", null!, null!, null! });

        var hash = Assert.IsType<Dictionary<string, object>>(result);
        Assert.True(hash.ContainsKey("status"));
        Assert.True(hash.ContainsKey("body"));
        Assert.True(hash.ContainsKey("headers"));
        Assert.True(hash.ContainsKey("ok"));
        Assert.IsType<Dictionary<string, object>>(hash["headers"]);
    }

    [Fact]
    public async Task HttpRequest_Response_Ok_True_For2xx()
    {
        SetupMock(HttpStatusCode.OK);
        var ctx = new ScriptContext();
        var result = await (Task<object>)RuntimeHelpers.__httpRequest(ctx,
            new object[] { "GET", "http://test.example.com/api", null!, null!, null! });
        Assert.Equal(true, ((Dictionary<string, object>)result)["ok"]);
    }

    [Fact]
    public async Task HttpRequest_Response_Ok_False_For4xx()
    {
        SetupMock(HttpStatusCode.NotFound);
        var ctx = new ScriptContext();
        var result = await (Task<object>)RuntimeHelpers.__httpRequest(ctx,
            new object[] { "GET", "http://test.example.com/missing", null!, null!, null! });
        var hash = (Dictionary<string, object>)result;
        Assert.Equal(404.0, hash["status"]);
        Assert.Equal(false, hash["ok"]);
    }

    [Fact]
    public async Task HttpRequest_Response_Headers()
    {
        SetupMock(HttpStatusCode.OK, "ok", new Dictionary<string, string> { ["X-Custom"] = "test-value" });
        var ctx = new ScriptContext();
        var result = await (Task<object>)RuntimeHelpers.__httpRequest(ctx,
            new object[] { "GET", "http://test.example.com/api", null!, null!, null! });
        var hash = (Dictionary<string, object>)result;
        var headers = (Dictionary<string, object>)hash["headers"];
        Assert.Equal("test-value", headers["X-Custom"]);
    }

    #endregion

    #region カスタムヘッダ

    [Fact]
    public async Task HttpRequest_CustomHeaders_Sent()
    {
        SetupMock();
        var ctx = new ScriptContext();
        var headers = new Dictionary<string, object>
        {
            ["Authorization"] = "Bearer token123",
            ["Accept"] = "application/json"
        };
        await (Task<object>)RuntimeHelpers.__httpRequest(ctx,
            new object[] { "GET", "http://test.example.com/api", headers, null!, null! });

        Assert.Contains("Bearer token123", _handler.LastRequest!.Headers.GetValues("Authorization"));
        Assert.Contains("application/json", _handler.LastRequest.Headers.GetValues("Accept"));
    }

    [Fact]
    public async Task HttpRequest_ContentType_SetOnBody()
    {
        SetupMock();
        var ctx = new ScriptContext();
        var headers = new Dictionary<string, object> { ["Content-Type"] = "application/json" };
        await (Task<object>)RuntimeHelpers.__httpRequest(ctx,
            new object[] { "POST", "http://test.example.com/api", headers, "{}", null! });

        Assert.NotNull(_handler.LastRequest!.Content);
        Assert.Equal("application/json", _handler.LastRequest.Content!.Headers.ContentType!.MediaType);
    }

    #endregion

    #region エラーハンドリング

    [Fact]
    public void HttpRequest_NullUrl_ThrowsException()
    {
        var ctx = new ScriptContext();
        Assert.Throws<RuntimeException>(() =>
            RuntimeHelpers.__httpRequest(ctx, new object[] { "GET", null!, null!, null!, null! }));
    }

    [Fact]
    public async Task HttpRequest_NullHeaders_Accepted()
    {
        SetupMock(HttpStatusCode.OK, "ok");
        var ctx = new ScriptContext();
        var result = await (Task<object>)RuntimeHelpers.__httpRequest(ctx,
            new object[] { "GET", "http://test.example.com/api", null!, null!, null! });
        var hash = (Dictionary<string, object>)result;
        Assert.Equal(200.0, hash["status"]);
    }

    [Fact]
    public async Task HttpRequest_NullBody_AcceptedForPost()
    {
        SetupMock();
        var ctx = new ScriptContext();
        await (Task<object>)RuntimeHelpers.__httpRequest(ctx,
            new object[] { "POST", "http://test.example.com/api", null!, null!, null! });
        Assert.Null(_handler.LastRequest!.Content);
    }

    #endregion

    #region MockHttpMessageHandler

    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _content;
        private readonly Dictionary<string, string>? _responseHeaders;

        public HttpRequestMessage? LastRequest { get; private set; }

        public MockHttpMessageHandler(HttpStatusCode statusCode = HttpStatusCode.OK,
            string content = "", Dictionary<string, string>? responseHeaders = null)
        {
            _statusCode = statusCode;
            _content = content;
            _responseHeaders = responseHeaders;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            var response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_content)
            };
            if (_responseHeaders != null)
            {
                foreach (var h in _responseHeaders)
                {
                    response.Headers.TryAddWithoutValidation(h.Key, h.Value);
                }
            }
            return Task.FromResult(response);
        }
    }

    #endregion
}
