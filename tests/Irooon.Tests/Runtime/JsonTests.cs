using Irooon.Core;
using Irooon.Core.Runtime;
using Xunit;

namespace Irooon.Tests.Runtime;

/// <summary>
/// JSON操作のテスト（stdlib.iroのirooon実装）
/// </summary>
public class JsonTests
{
    private ScriptEngine CreateEngine()
    {
        return new ScriptEngine();
    }

    #region jsonStringify Tests

    [Fact]
    public void JsonStringify_Null()
    {
        var engine = CreateEngine();
        var result = engine.Execute("jsonStringify(null)");
        Assert.Equal("null", result);
    }

    [Fact]
    public void JsonStringify_True()
    {
        var engine = CreateEngine();
        var result = engine.Execute("jsonStringify(true)");
        Assert.Equal("true", result);
    }

    [Fact]
    public void JsonStringify_False()
    {
        var engine = CreateEngine();
        var result = engine.Execute("jsonStringify(false)");
        Assert.Equal("false", result);
    }

    [Fact]
    public void JsonStringify_Number()
    {
        var engine = CreateEngine();
        var result = engine.Execute("jsonStringify(42)");
        Assert.Equal("42", result);
    }

    [Fact]
    public void JsonStringify_NumberFloat()
    {
        var engine = CreateEngine();
        var result = engine.Execute("jsonStringify(3.14)");
        Assert.Equal("3.14", result);
    }

    [Fact]
    public void JsonStringify_String()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"jsonStringify(""hello"")");
        // Should produce: "hello" (with quotes)
        Assert.Equal("\"hello\"", result);
    }

    [Fact]
    public void JsonStringify_EmptyString()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"jsonStringify("""")");
        Assert.Equal("\"\"", result);
    }

    [Fact]
    public void JsonStringify_EmptyList()
    {
        var engine = CreateEngine();
        var result = engine.Execute("jsonStringify([])");
        Assert.Equal("[]", result);
    }

    [Fact]
    public void JsonStringify_List()
    {
        var engine = CreateEngine();
        var result = engine.Execute("jsonStringify([1, 2, 3])");
        Assert.Equal("[1,2,3]", result);
    }

    [Fact]
    public void JsonStringify_MixedList()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"jsonStringify([1, ""hello"", true, null])");
        Assert.Equal("[1,\"hello\",true,null]", result);
    }

    [Fact]
    public void JsonStringify_EmptyHash()
    {
        var engine = CreateEngine();
        var result = engine.Execute("jsonStringify(__hashNew())");
        Assert.Equal("{}", result);
    }

    [Fact]
    public void JsonStringify_Hash()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let obj = __hashNew()
            obj[""name""] = ""Alice""
            obj[""age""] = 30
            jsonStringify(obj)
        ");
        // Hash key order may vary, check both possible outputs
        var resultStr = (string)result;
        Assert.True(
            resultStr == "{\"name\":\"Alice\",\"age\":30}" ||
            resultStr == "{\"age\":30,\"name\":\"Alice\"}",
            $"Unexpected JSON: {resultStr}"
        );
    }

    [Fact]
    public void JsonStringify_NestedObject()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let inner = __hashNew()
            inner[""x""] = 1
            let outer = __hashNew()
            outer[""data""] = inner
            jsonStringify(outer)
        ");
        Assert.Equal("{\"data\":{\"x\":1}}", result);
    }

    #endregion

    #region jsonParse Tests

    [Fact]
    public void JsonParse_Null()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"jsonParse(""null"")");
        Assert.Null(result);
    }

    [Fact]
    public void JsonParse_True()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"jsonParse(""true"")");
        Assert.Equal(true, result);
    }

    [Fact]
    public void JsonParse_False()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"jsonParse(""false"")");
        Assert.Equal(false, result);
    }

    [Fact]
    public void JsonParse_Integer()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"jsonParse(""42"")");
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void JsonParse_NegativeNumber()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"jsonParse(""-3.14"")");
        Assert.Equal(-3.14, result);
    }

    [Fact]
    public void JsonParse_String()
    {
        var engine = CreateEngine();
        // JSON: "hello" → needs to be embedded in irooon string
        // We use __fromCharCode to build the JSON string with quotes
        var result = engine.Execute(@"
            let dq = __fromCharCode(34)
            let json = ""${dq}hello${dq}""
            jsonParse(json)
        ");
        Assert.Equal("hello", result);
    }

    [Fact]
    public void JsonParse_EmptyArray()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"jsonParse(""[]"")");
        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Empty(list);
    }

    [Fact]
    public void JsonParse_Array()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"jsonParse(""[1,2,3]"")");
        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal(3, list.Count);
        Assert.Equal(1.0, list[0]);
        Assert.Equal(2.0, list[1]);
        Assert.Equal(3.0, list[2]);
    }

    [Fact]
    public void JsonParse_EmptyObject()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"jsonParse(""{}"")");
        Assert.IsType<Dictionary<string, object>>(result);
        var dict = (Dictionary<string, object>)result;
        Assert.Empty(dict);
    }

    [Fact]
    public void JsonParse_SimpleObject()
    {
        var engine = CreateEngine();
        // Build JSON string using __fromCharCode for quotes
        var result = engine.Execute(@"
            let dq = __fromCharCode(34)
            let json = ""{ ${dq}name${dq}: ${dq}Alice${dq}, ${dq}age${dq}: 30 }""
            jsonParse(json)
        ");
        Assert.IsType<Dictionary<string, object>>(result);
        var dict = (Dictionary<string, object>)result;
        Assert.Equal("Alice", dict["name"]);
        Assert.Equal(30.0, dict["age"]);
    }

    [Fact]
    public void JsonParse_NestedObject()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let dq = __fromCharCode(34)
            let json = ""{ ${dq}data${dq}: { ${dq}x${dq}: 1 } }""
            jsonParse(json)
        ");
        Assert.IsType<Dictionary<string, object>>(result);
        var dict = (Dictionary<string, object>)result;
        var inner = (Dictionary<string, object>)dict["data"];
        Assert.Equal(1.0, inner["x"]);
    }

    [Fact]
    public void JsonParse_MixedArray()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let dq = __fromCharCode(34)
            let json = ""[1, ${dq}hello${dq}, true, null]""
            jsonParse(json)
        ");
        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal(4, list.Count);
        Assert.Equal(1.0, list[0]);
        Assert.Equal("hello", list[1]);
        Assert.Equal(true, list[2]);
        Assert.Null(list[3]);
    }

    #endregion

    #region E2E Round-trip Tests

    [Fact]
    public void E2E_RoundTrip_SimpleValue()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let original = 42
            let json = jsonStringify(original)
            jsonParse(json)
        ");
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void E2E_RoundTrip_List()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let original = [1, 2, 3]
            let json = jsonStringify(original)
            let parsed = jsonParse(json)
            parsed[0] + parsed[1] + parsed[2]
        ");
        Assert.Equal(6.0, result);
    }

    [Fact]
    public void E2E_RoundTrip_Hash()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let dq = __fromCharCode(34)
            let json = ""{ ${dq}name${dq}: ${dq}Alice${dq}, ${dq}age${dq}: 30 }""
            let obj = jsonParse(json)
            let json2 = jsonStringify(obj)
            let obj2 = jsonParse(json2)
            obj2[""age""]
        ");
        Assert.Equal(30.0, result);
    }

    #endregion
}
