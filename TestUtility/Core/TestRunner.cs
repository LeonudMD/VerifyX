using Spectre.Console;
using TestUtility.Logging;

// для вызова логера

namespace TestUtility.Core
{
    public class TestRunner
    {
        private readonly List<ITestModule> modules = new List<ITestModule>();

        public TestRunner()
        {
            // Регистрация модулей тестирования.
            modules.Add(new Modules.TodoApiTestModule());
            // Здесь можно добавить и другие модули (например, для Thrift).
        }

        public async Task RunAsync()
        {
            bool exit = false;
            while (!exit)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(
                    new FigletText("VerifyX")
                        .Centered()
                        .Color(Color.Aquamarine1)
                );

                // Главное меню
                var mainChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Выберите действие:")
                        .AddChoices(new[] { "Выбрать тест", "Выход" })
                );

                if (mainChoice == "Выход")
                {
                    exit = true;
                    break;
                }

                // Меню выбора модуля
                var moduleChoices = new List<string>();
                for (int i = 0; i < modules.Count; i++)
                {
                    moduleChoices.Add($"{i + 1}. {modules[i].ModuleName}");
                }
                moduleChoices.Add("Назад");

                var moduleChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Выберите модуль для тестирования:")
                        .AddChoices(moduleChoices)
                );

                if (moduleChoice == "Назад")
                    continue;

                int selectedModuleIndex = int.Parse(moduleChoice.Split('.')[0]) - 1;
                var selectedModule = modules[selectedModuleIndex];

                // Меню выбора тестового метода внутри модуля
                var methods = selectedModule.GetTestMethods();
                var methodChoices = new List<string>();
                for (int i = 0; i < methods.Count; i++)
                {
                    methodChoices.Add($"{i + 1}. {methods[i].MethodName} - {methods[i].Description}");
                }
                methodChoices.Add("Назад");

                var methodChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Выберите тестовый метод:")
                        .AddChoices(methodChoices)
                );

                if (methodChoice == "Назад")
                    continue;

                int selectedMethodIndex = int.Parse(methodChoice.Split('.')[0]) - 1;
                var selectedMethod = methods[selectedMethodIndex];

                // Получение параметров: выбор набора тестовых данных или ручной ввод.
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                if (selectedMethod.TestDataSets != null && selectedMethod.TestDataSets.Count > 0)
                {
                    var dataSetChoices = new List<string>();
                    foreach (var dataSet in selectedMethod.TestDataSets)
                        dataSetChoices.Add(dataSet.Name);
                    dataSetChoices.Add("Ручной ввод");

                    var dataChoice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Выберите набор тестовых данных:")
                            .AddChoices(dataSetChoices)
                    );

                    if (dataChoice != "Ручной ввод")
                    {
                        var chosenDataSet = selectedMethod.TestDataSets.Find(ds => ds.Name == dataChoice);
                        parameters = chosenDataSet.Parameters;
                    }
                    else
                    {
                        parameters = PromptForParameters(selectedMethod.Parameters);
                    }
                }
                else
                {
                    parameters = PromptForParameters(selectedMethod.Parameters);
                }

                // Выполнение теста
                AnsiConsole.MarkupLine("[green]Выполнение теста...[/]");
                var result = await selectedMethod.ExecuteTestAsync(parameters);

                // Логирование запроса с параметрами и ответом.
                Logger.LogTestExecution(
                    selectedModule.ModuleName,
                    selectedMethod.MethodName,
                    parameters,
                    result.RequestAddress,
                    result.ResponseContent,
                    result.ErrorMessage
                );

                // Вывод результата
                if (result.Success)
                {
                    AnsiConsole.MarkupLine("[green]Тест прошёл успешно![/]");
                    AnsiConsole.MarkupLine($"[green]Ответ:[/] {Markup.Escape(result.ResponseContent)}");
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Тест завершился с ошибкой![/]");
                    if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                        AnsiConsole.MarkupLine($"[red]Ошибка: {result.ErrorMessage}[/]");
                    if (!string.IsNullOrWhiteSpace(result.ResponseContent))
                        AnsiConsole.MarkupLine($"[yellow]Ответ сервера: {result.ResponseContent}[/]");
                }

                AnsiConsole.MarkupLine("[blue]Нажмите Enter для возврата в главное меню...[/]");
                Console.ReadLine();
            }
        }

        // Метод для ручного ввода параметров теста.
        private Dictionary<string, object> PromptForParameters(List<ParameterDefinition> parameterDefinitions)
        {
            var parameters = new Dictionary<string, object>();
            foreach (var param in parameterDefinitions)
            {
                string input = AnsiConsole.Ask<string>($"Введите значение для параметра [blue]{param.Name}[/] ({param.Description}):");
                parameters[param.Name] = input; // Преобразование типов можно добавить при необходимости.
            }
            return parameters;
        }
    }
}
