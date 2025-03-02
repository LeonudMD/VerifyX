using System.Text;

namespace TestUtility.Logging
{
    public static class Logger
    {
        private static readonly string LogsFolder = "../../../Logs";

        public static void LogTestExecution(string moduleName, string methodName, Dictionary<string, object> parameters, string requestAddress, string response, string errorMessage)
        {
            try
            {
                // Создаем папку для логов, если её нет
                if (!Directory.Exists(LogsFolder))
                    Directory.CreateDirectory(LogsFolder);

                // Имя файла – текущая дата (например, 2025-03-02.log)
                string logFilePath = Path.Combine(LogsFolder, DateTime.Now.ToString("yyyy-MM-dd") + ".log");

                var sb = new StringBuilder();
                sb.AppendLine("--------------------------------------------------");
                sb.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"Module: {moduleName}");
                sb.AppendLine($"Method: {methodName}");
                sb.AppendLine($"Request Address: {requestAddress}");
                sb.AppendLine("Parameters:");
                foreach (var param in parameters)
                {
                    sb.AppendLine($"  {param.Key}: {param.Value}");
                }
                sb.AppendLine("Response:");
                sb.AppendLine(response);
                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    sb.AppendLine("Error:");
                    sb.AppendLine(errorMessage);
                }
                sb.AppendLine("--------------------------------------------------");
                File.AppendAllText(logFilePath, sb.ToString());
            }
            catch(Exception ex)
            {
                // В случае ошибки логирования выводим сообщение в консоль.
                Console.WriteLine("Ошибка при логировании: " + ex.Message);
            }
        }
    }
}
