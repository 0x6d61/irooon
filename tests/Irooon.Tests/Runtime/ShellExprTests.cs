using Xunit;
using Irooon.Core.Runtime;
using System;
using System.Runtime.InteropServices;

namespace Irooon.Tests.Runtime;

/// <summary>
/// シェルコマンド実行機能のテスト
/// </summary>
public class ShellExprTests
{
    [Fact]
    public void ExecuteShellCommand_SimpleEcho_ReturnsOutput()
    {
        // Arrange
        string command = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "echo Hello World"
            : "echo Hello World";

        // Act
        var result = RuntimeHelpers.ExecuteShellCommand(command);

        // Assert
        Assert.Equal("Hello World", result);
    }

    [Fact]
    public void ExecuteShellCommand_EmptyCommand_ReturnsEmptyString()
    {
        // Arrange
        string command = "";

        // Act
        var result = RuntimeHelpers.ExecuteShellCommand(command);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void ExecuteShellCommand_WhitespaceCommand_ReturnsEmptyString()
    {
        // Arrange
        string command = "   ";

        // Act
        var result = RuntimeHelpers.ExecuteShellCommand(command);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void ExecuteShellCommand_CommandWithOutput_ReturnsTrimmedOutput()
    {
        // Arrange
        string command = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "echo test"
            : "echo test";

        // Act
        var result = RuntimeHelpers.ExecuteShellCommand(command);

        // Assert
        Assert.Equal("test", result);
    }

    [Fact]
    public void ExecuteShellCommand_InvalidCommand_ThrowsRuntimeException()
    {
        // Arrange
        string command = "this_command_does_not_exist_12345";

        // Act & Assert
        var exception = Assert.Throws<RuntimeException>(() =>
            RuntimeHelpers.ExecuteShellCommand(command));

        Assert.Contains("Shell command failed", exception.Message);
    }

    [Fact]
    public void ExecuteShellCommand_MultilineOutput_ReturnsTrimmedOutput()
    {
        // Arrange
        string command = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "echo line1 && echo line2"
            : "echo line1 && echo line2";

        // Act
        var result = RuntimeHelpers.ExecuteShellCommand(command);

        // Assert
        // 改行を含む出力が返されることを確認
        var resultStr = result?.ToString() ?? "";
        Assert.Contains("line1", resultStr);
        Assert.Contains("line2", resultStr);
    }

    [Fact]
    public void ExecuteShellCommand_GitCommand_ReturnsOutput()
    {
        // このテストはgitがインストールされている環境でのみ実行可能
        // CI環境では実行されない可能性があるため、スキップ条件を設定
        try
        {
            // Arrange
            string command = "git --version";

            // Act
            var result = RuntimeHelpers.ExecuteShellCommand(command);

            // Assert
            var resultStr = result?.ToString() ?? "";
            Assert.Contains("git version", resultStr.ToLower());
        }
        catch (RuntimeException)
        {
            // gitがインストールされていない場合はスキップ
            Assert.True(true);
        }
    }
}
