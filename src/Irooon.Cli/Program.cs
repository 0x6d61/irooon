using Irooon.Core;

if (args.Length == 0)
{
    Console.WriteLine("Usage: irooon <script.iro>");
    Console.WriteLine("Example: irooon hello.iro");
    return 1;
}

var scriptPath = args[0];

if (!File.Exists(scriptPath))
{
    Console.Error.WriteLine($"Error: File not found: {scriptPath}");
    return 1;
}

try
{
    // スクリプトを読み込み
    var source = File.ReadAllText(scriptPath);

    // ScriptEngineで実行
    var engine = new ScriptEngine();
    var result = engine.Execute(source);

    // 結果を出力（nullでない場合のみ）
    if (result != null)
    {
        Console.WriteLine(result);
    }

    return 0;
}
catch (ScriptException ex)
{
    Console.Error.WriteLine($"Script error: {ex.Message}");
    return 1;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Unexpected error: {ex.Message}");
    return 1;
}
