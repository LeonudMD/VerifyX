namespace TestUtility.Core
{
    // Интерфейс тестового модуля. Каждый модуль реализует этот контракт.
    public interface ITestModule
    {
        string ModuleName { get; }
        List<TestMethodDescriptor> GetTestMethods();
    }

    
    // Описание тестового метода с набором параметров и тестовых данных.
    public class TestMethodDescriptor
    {
        public string MethodName { get; set; }
        public string Description { get; set; }
        public List<ParameterDefinition> Parameters { get; set; } = new List<ParameterDefinition>();
        public List<TestDataSet> TestDataSets { get; set; } = new List<TestDataSet>();

        // Делегат для асинхронного выполнения теста.
        public Func<Dictionary<string, object>, Task<TestResult>> ExecuteTestAsync { get; set; }
    }

    // Описание набора тестовых данных для метода.
    public class TestDataSet
    {
        public string Name { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }

    // Описание параметра тестового метода.
    public class ParameterDefinition
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Type ParameterType { get; set; }
    }

    // Результат выполнения теста: флаг успеха, ответ сервера, сообщение об ошибке и адрес запроса.
    public class TestResult
    {
        public bool Success { get; set; }
        public string ResponseContent { get; set; }
        public string ErrorMessage { get; set; }
        public string RequestAddress { get; set; }
    }
}