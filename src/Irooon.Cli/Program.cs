using Irooon.Core;
using Irooon.Repl;

// 引数がない場合はREPLモードで起動
if (args.Length == 0)
{
    return RunRepl();
}

// スクリプトファイルモード
return RunScript(args[0]);

static int RunRepl()
{
    Console.WriteLine("irooon v0.3.0 - Interactive REPL");
    Console.WriteLine("Type :help for help, :exit to quit");
    Console.WriteLine();

    var repl = new ReplEngine();

    while (true)
    {
        Console.Write("> ");
        var input = Console.ReadLine();

        if (input == null) break;

        // スペシャルコマンド処理
        if (input.StartsWith(':'))
        {
            if (HandleSpecialCommand(input, repl))
                break; // :exit/:quit
            continue;
        }

        // 空行はスキップ
        if (string.IsNullOrWhiteSpace(input))
            continue;

        // 複数行入力のサポート
        var completeInput = ReadCompleteInput(input);

        // 評価して結果を表示
        var result = repl.Evaluate(completeInput);
        if (result != null)
        {
            Console.WriteLine(result);
        }
    }

    return 0;
}

static string ReadCompleteInput(string firstLine)
{
    var lines = new List<string> { firstLine };

    // 括弧のバランスをチェック
    while (!IsComplete(string.Join("\n", lines)))
    {
        Console.Write("... ");
        var line = Console.ReadLine();
        if (line == null) break;
        lines.Add(line);
    }

    return string.Join("\n", lines);
}

static bool IsComplete(string input)
{
    int openBraces = input.Count(c => c == '{');
    int closeBraces = input.Count(c => c == '}');
    int openParens = input.Count(c => c == '(');
    int closeParens = input.Count(c => c == ')');
    int openBrackets = input.Count(c => c == '[');
    int closeBrackets = input.Count(c => c == ']');

    return openBraces == closeBraces &&
           openParens == closeParens &&
           openBrackets == closeBrackets;
}

static bool HandleSpecialCommand(string command, ReplEngine repl)
{
    switch (command.ToLower())
    {
        case ":help":
        case ":h":
            ShowHelp();
            return false;

        case ":exit":
        case ":quit":
        case ":q":
            Console.WriteLine("Goodbye!");
            return true;

        case ":clear":
        case ":c":
            repl.Clear();
            Console.WriteLine("Context cleared.");
            return false;

        case ":vars":
        case ":v":
            ShowVariables(repl);
            return false;

        default:
            Console.WriteLine($"Unknown command: {command}");
            Console.WriteLine("Type :help for available commands.");
            return false;
    }
}

static void ShowHelp()
{
    Console.WriteLine("Available commands:");
    Console.WriteLine("  :help, :h       - Show this help message");
    Console.WriteLine("  :exit, :quit, :q - Exit the REPL");
    Console.WriteLine("  :clear, :c      - Clear the context (reset all variables)");
    Console.WriteLine("  :vars, :v       - Show defined variables");
    Console.WriteLine();
    Console.WriteLine("Enter any irooon expression or statement to evaluate it.");
    Console.WriteLine("Multi-line input is supported (close all brackets to execute).");
}

static void ShowVariables(ReplEngine repl)
{
    var variables = repl.GetVariables();

    if (variables.Count == 0)
    {
        Console.WriteLine("No variables defined.");
        return;
    }

    Console.WriteLine("Defined variables:");
    foreach (var kvp in variables)
    {
        var value = kvp.Value;
        var valueStr = value?.ToString() ?? "null";

        // 長い値は省略
        if (valueStr.Length > 50)
        {
            valueStr = valueStr.Substring(0, 47) + "...";
        }

        Console.WriteLine($"  {kvp.Key} = {valueStr}");
    }
}

static int RunScript(string scriptPath)
{
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
}
