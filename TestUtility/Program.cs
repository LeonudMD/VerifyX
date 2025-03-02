using Spectre.Console;
using TestUtility.Core;

namespace TestUtility
{
    public abstract class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var runner = new TestRunner();
                await runner.RunAsync();
            }
            catch(Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Критическая ошибка: {ex.Message}[/]");
            }
        }
    }
}