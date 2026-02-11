using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Irooon.Core;
using Irooon.Core.Runtime;

namespace Irooon.Tests.Integration;

/// <summary>
/// HTTP クライアントの E2E テスト。
/// ScriptEngine 経由で fetch / http オブジェクトの動作を検証する。
/// MockHttpMessageHandler を使用し、実際のネットワーク通信は行わない。
/// await はトップレベルで使えないため async fn でラップし、Task として実行する。
/// </summary>
[Collection("HttpClient")]
public class HttpClientE2ETests : IDisposable
{
    private readonly ScriptEngine _engine = new();

    private void SetupMock(HttpStatusCode statusCode = HttpStatusCode.OK, string content = "",
        Dictionary<string, string>? responseHeaders = null)
    {
        var handler = new MockHttpMessageHandler(statusCode, content, responseHeaders);
        RuntimeHelpers.SetHttpClient(new HttpClient(handler));
    }

    public void Dispose()
    {
        RuntimeHelpers.ResetHttpClient();
    }

    /// <summary>async fn でラップしたスクリプトを実行し、Task の結果を返す</summary>
    private object? ExecuteAsync(string asyncBody)
    {
        var script = $@"
            async fn __test__() {{
                {asyncBody}
            }}
            __test__()
        ";
        var task = (Task<object>)_engine.Execute(script)!;
        return task.GetAwaiter().GetResult();
    }

    #region fetch 基本テスト

    [Fact]
    public void Fetch_Get_ReturnsStatus()
    {
        SetupMock(HttpStatusCode.OK, "hello");
        var result = ExecuteAsync(@"
            let res = await fetch(""http://test.example.com/api"")
            res[""status""]
        ");
        Assert.Equal(200.0, result);
    }

    [Fact]
    public void Fetch_Get_ReturnsBody()
    {
        SetupMock(HttpStatusCode.OK, "response body");
        var result = ExecuteAsync(@"
            let res = await fetch(""http://test.example.com/api"")
            res[""body""]
        ");
        Assert.Equal("response body", result);
    }

    [Fact]
    public void Fetch_Get_ReturnsOk()
    {
        SetupMock(HttpStatusCode.OK);
        var result = ExecuteAsync(@"
            let res = await fetch(""http://test.example.com/api"")
            res[""ok""]
        ");
        Assert.Equal(true, result);
    }

    [Fact]
    public void Fetch_Post_WithOptions()
    {
        SetupMock(HttpStatusCode.Created, "{\"id\":1}");
        var result = ExecuteAsync(@"
            let opts = {
                method: ""POST"",
                body: ""{ }"",
                headers: { ""Content-Type"": ""application/json"" }
            }
            let res = await fetch(""http://test.example.com/users"", opts)
            res[""status""]
        ");
        Assert.Equal(201.0, result);
    }

    [Fact]
    public void Fetch_NotFound_OkIsFalse()
    {
        SetupMock(HttpStatusCode.NotFound);
        var result = ExecuteAsync(@"
            let res = await fetch(""http://test.example.com/missing"")
            res[""ok""]
        ");
        Assert.Equal(false, result);
    }

    #endregion

    #region http オブジェクト

    [Fact]
    public void Http_Get_Works()
    {
        SetupMock(HttpStatusCode.OK, "get result");
        var result = ExecuteAsync(@"
            let res = await http.get(""http://test.example.com/api"")
            res[""body""]
        ");
        Assert.Equal("get result", result);
    }

    [Fact]
    public void Http_Post_Works()
    {
        SetupMock(HttpStatusCode.Created, "created");
        var result = ExecuteAsync(@"
            let headers = { ""Content-Type"": ""application/json"" }
            let res = await http.post(""http://test.example.com/api"", ""body data"", headers)
            res[""status""]
        ");
        Assert.Equal(201.0, result);
    }

    [Fact]
    public void Http_Put_Works()
    {
        SetupMock(HttpStatusCode.OK, "updated");
        var result = ExecuteAsync(@"
            let res = await http.put(""http://test.example.com/item/1"", ""new data"", null)
            res[""body""]
        ");
        Assert.Equal("updated", result);
    }

    [Fact]
    public void Http_Delete_Works()
    {
        SetupMock(HttpStatusCode.NoContent, "");
        var result = ExecuteAsync(@"
            let res = await http.delete(""http://test.example.com/item/1"")
            res[""status""]
        ");
        Assert.Equal(204.0, result);
    }

    [Fact]
    public void Http_Patch_Works()
    {
        SetupMock(HttpStatusCode.OK, "patched");
        var result = ExecuteAsync(@"
            let res = await http.patch(""http://test.example.com/item/1"", ""patch data"", null)
            res[""body""]
        ");
        Assert.Equal("patched", result);
    }

    #endregion

    #region JSON 統合

    [Fact]
    public void Fetch_JsonParse_Integration()
    {
        SetupMock(HttpStatusCode.OK, "{\"name\":\"Alice\",\"age\":30}");
        var result = ExecuteAsync(@"
            let res = await fetch(""http://test.example.com/user"")
            let data = jsonParse(res[""body""])
            data[""name""]
        ");
        Assert.Equal("Alice", result);
    }

    [Fact]
    public void Http_Post_JsonStringify_Integration()
    {
        SetupMock(HttpStatusCode.Created, "{\"id\":42}");
        var result = ExecuteAsync(@"
            let headers = { ""Content-Type"": ""application/json"" }
            let body = jsonStringify({ name: ""Bob"" })
            let res = await http.post(""http://test.example.com/users"", body, headers)
            let data = jsonParse(res[""body""])
            data[""id""]
        ");
        Assert.Equal(42.0, result);
    }

    #endregion

    #region エラーハンドリング

    [Fact]
    public void Fetch_InvalidUrl_ThrowsScriptException()
    {
        // URL が空文字列の場合、RuntimeException が ScriptException として伝播する
        SetupMock(HttpStatusCode.OK);
        var ex = Assert.Throws<ScriptException>(() => _engine.Execute(@"fetch("""")"));
        Assert.Contains("URL is required", ex.Message);
    }

    [Fact]
    public void Http_Get_WithHeaders()
    {
        SetupMock(HttpStatusCode.OK, "authenticated");
        var result = ExecuteAsync(@"
            let headers = { ""Authorization"": ""Bearer token123"" }
            let res = await http.get(""http://test.example.com/me"", headers)
            res[""body""]
        ");
        Assert.Equal("authenticated", result);
    }

    #endregion

    #region MockHttpMessageHandler

    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _content;
        private readonly Dictionary<string, string>? _responseHeaders;

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
            var response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_content)
            };
            if (_responseHeaders != null)
            {
                foreach (var h in _responseHeaders)
                    response.Headers.TryAddWithoutValidation(h.Key, h.Value);
            }
            return Task.FromResult(response);
        }
    }

    #endregion
}
