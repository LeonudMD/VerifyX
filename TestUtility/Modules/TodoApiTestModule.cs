using System.Net.Http.Json;
using TestUtility.Core;
using TestUtility.Models;

namespace TestUtility.Modules
{
    public class TodoApiTestModule : ITestModule
    {
        public string ModuleName => "Todo API Тестирование";

        // Базовый URL для API (убедитесь, что ваше API запущено).
        private const string BaseUrl = "http://localhost:60157/api/todo";

        public List<TestMethodDescriptor> GetTestMethods()
        {
            return new List<TestMethodDescriptor>()
            {
                // 1. Получить все задачи
                new TestMethodDescriptor {
                    MethodName = "Получить все задачи",
                    Description = "GET /api/todo",
                    ExecuteTestAsync = async (parameters) =>
                    {
                        using(var client = new HttpClient())
                        {
                            try
                            {
                                var response = await client.GetAsync(BaseUrl);
                                var content = await response.Content.ReadAsStringAsync();
                                if(response.IsSuccessStatusCode)
                                {
                                    return new TestResult
                                    {
                                        Success = true,
                                        ResponseContent = content,
                                        RequestAddress = BaseUrl
                                    };
                                }
                                else
                                {
                                    return new TestResult
                                    {
                                        Success = false,
                                        ResponseContent = content,
                                        ErrorMessage = $"Статус: {response.StatusCode}",
                                        RequestAddress = BaseUrl
                                    };
                                }
                            }
                            catch(Exception ex)
                            {
                                return new TestResult { Success = false, ErrorMessage = ex.Message, RequestAddress = BaseUrl };
                            }
                        }
                    }
                },
                // 2. Получить задачу по ID
                new TestMethodDescriptor {
                    MethodName = "Получить задачу по ID",
                    Description = "GET /api/todo/{id}",
                    Parameters = new List<ParameterDefinition> {
                        new ParameterDefinition { Name = "id", Description = "Идентификатор задачи", ParameterType = typeof(int) }
                    },
                    ExecuteTestAsync = async (parameters) =>
                    {
                        using(var client = new HttpClient())
                        {
                            try
                            {
                                if(!parameters.ContainsKey("id"))
                                    throw new ArgumentException("Параметр 'id' не задан.");
                                string id = parameters["id"].ToString();
                                string url = $"{BaseUrl}/{id}";
                                var response = await client.GetAsync(url);
                                var content = await response.Content.ReadAsStringAsync();
                                if(response.IsSuccessStatusCode)
                                {
                                    return new TestResult
                                    {
                                        Success = true,
                                        ResponseContent = content,
                                        RequestAddress = url
                                    };
                                }
                                else
                                {
                                    return new TestResult
                                    {
                                        Success = false,
                                        ResponseContent = content,
                                        ErrorMessage = $"Статус: {response.StatusCode}",
                                        RequestAddress = url
                                    };
                                }
                            }
                            catch(Exception ex)
                            {
                                return new TestResult { Success = false, ErrorMessage = ex.Message, RequestAddress = BaseUrl };
                            }
                        }
                    }
                },
                // 3. Создать задачу
                new TestMethodDescriptor {
                    MethodName = "Создать задачу",
                    Description = "POST /api/todo",
                    Parameters = new List<ParameterDefinition> {
                        new ParameterDefinition { Name = "Title", Description = "Заголовок задачи", ParameterType = typeof(string) },
                        new ParameterDefinition { Name = "Description", Description = "Описание задачи", ParameterType = typeof(string) }
                    },
                    TestDataSets = new List<TestDataSet> {
                        new TestDataSet { Name = "Набор 1", Parameters = new Dictionary<string, object> {
                            {"Title", "Задача 1"},
                            {"Description", "Описание задачи 1"}
                        }},
                        new TestDataSet { Name = "Набор 2", Parameters = new Dictionary<string, object> {
                            {"Title", "Задача 2"},
                            {"Description", "Описание задачи 2"}
                        }}
                    },
                    ExecuteTestAsync = async (parameters) =>
                    {
                        using(var client = new HttpClient())
                        {
                            try
                            {
                                var newTask = new TodoItem
                                {
                                    Title = parameters["Title"].ToString(),
                                    Description = parameters["Description"].ToString()
                                };
                                var response = await client.PostAsJsonAsync(BaseUrl, newTask);
                                var content = await response.Content.ReadAsStringAsync();
                                if(response.IsSuccessStatusCode)
                                {
                                    return new TestResult
                                    {
                                        Success = true,
                                        ResponseContent = content,
                                        RequestAddress = BaseUrl
                                    };
                                }
                                else
                                {
                                    return new TestResult
                                    {
                                        Success = false,
                                        ResponseContent = content,
                                        ErrorMessage = $"Статус: {response.StatusCode}",
                                        RequestAddress = BaseUrl
                                    };
                                }
                            }
                            catch(Exception ex)
                            {
                                return new TestResult { Success = false, ErrorMessage = ex.Message, RequestAddress = BaseUrl };
                            }
                        }
                    }
                },
                // 4. Обновить задачу
                new TestMethodDescriptor {
                    MethodName = "Обновить задачу",
                    Description = "PUT /api/todo/{id}",
                    Parameters = new List<ParameterDefinition> {
                        new ParameterDefinition { Name = "id", Description = "Идентификатор задачи", ParameterType = typeof(int) },
                        new ParameterDefinition { Name = "Title", Description = "Новый заголовок задачи", ParameterType = typeof(string) },
                        new ParameterDefinition { Name = "Description", Description = "Новое описание задачи", ParameterType = typeof(string) }
                    },
                    ExecuteTestAsync = async (parameters) =>
                    {
                        using(var client = new HttpClient())
                        {
                            try
                            {
                                if(!parameters.ContainsKey("id"))
                                    throw new ArgumentException("Параметр 'id' не задан.");
                                string id = parameters["id"].ToString();
                                string url = $"{BaseUrl}/{id}";
                                var updatedTask = new TodoItem
                                {
                                    Title = parameters["Title"].ToString(),
                                    Description = parameters["Description"].ToString()
                                };
                                var response = await client.PutAsJsonAsync(url, updatedTask);
                                var content = await response.Content.ReadAsStringAsync();
                                if(response.IsSuccessStatusCode)
                                {
                                    return new TestResult
                                    {
                                        Success = true,
                                        ResponseContent = content,
                                        RequestAddress = url
                                    };
                                }
                                else
                                {
                                    return new TestResult
                                    {
                                        Success = false,
                                        ResponseContent = content,
                                        ErrorMessage = $"Статус: {response.StatusCode}",
                                        RequestAddress = url
                                    };
                                }
                            }
                            catch(Exception ex)
                            {
                                return new TestResult { Success = false, ErrorMessage = ex.Message, RequestAddress = BaseUrl };
                            }
                        }
                    }
                },
                // 5. Удалить задачу
                new TestMethodDescriptor {
                    MethodName = "Удалить задачу",
                    Description = "DELETE /api/todo/{id}",
                    Parameters = new List<ParameterDefinition> {
                        new ParameterDefinition { Name = "id", Description = "Идентификатор задачи", ParameterType = typeof(int) }
                    },
                    ExecuteTestAsync = async (parameters) =>
                    {
                        using(var client = new HttpClient())
                        {
                            try
                            {
                                if(!parameters.ContainsKey("id"))
                                    throw new ArgumentException("Параметр 'id' не задан.");
                                string id = parameters["id"].ToString();
                                string url = $"{BaseUrl}/{id}";
                                var response = await client.DeleteAsync(url);
                                var content = await response.Content.ReadAsStringAsync();
                                if(response.IsSuccessStatusCode)
                                {
                                    return new TestResult
                                    {
                                        Success = true,
                                        ResponseContent = "Задача успешно удалена.",
                                        RequestAddress = url
                                    };
                                }
                                else
                                {
                                    return new TestResult
                                    {
                                        Success = false,
                                        ResponseContent = content,
                                        ErrorMessage = $"Статус: {response.StatusCode}",
                                        RequestAddress = url
                                    };
                                }
                            }
                            catch(Exception ex)
                            {
                                return new TestResult { Success = false, ErrorMessage = ex.Message, RequestAddress = BaseUrl };
                            }
                        }
                    }
                },
                // 6. Отметить задачу выполненной
                new TestMethodDescriptor {
                    MethodName = "Отметить задачу выполненной",
                    Description = "PATCH /api/todo/{id}/complete",
                    Parameters = new List<ParameterDefinition> {
                        new ParameterDefinition { Name = "id", Description = "Идентификатор задачи", ParameterType = typeof(int) }
                    },
                    ExecuteTestAsync = async (parameters) =>
                    {
                        using(var client = new HttpClient())
                        {
                            try
                            {
                                if(!parameters.ContainsKey("id"))
                                    throw new ArgumentException("Параметр 'id' не задан.");
                                string id = parameters["id"].ToString();
                                string url = $"{BaseUrl}/{id}/complete";
                                var request = new HttpRequestMessage(new HttpMethod("PATCH"), url);
                                var response = await client.SendAsync(request);
                                var content = await response.Content.ReadAsStringAsync();
                                if(response.IsSuccessStatusCode)
                                {
                                    return new TestResult
                                    {
                                        Success = true,
                                        ResponseContent = "Задача успешно отмечена как выполненная.",
                                        RequestAddress = url
                                    };
                                }
                                else
                                {
                                    return new TestResult
                                    {
                                        Success = false,
                                        ResponseContent = content,
                                        ErrorMessage = $"Статус: {response.StatusCode}",
                                        RequestAddress = url
                                    };
                                }
                            }
                            catch(Exception ex)
                            {
                                return new TestResult { Success = false, ErrorMessage = ex.Message, RequestAddress = BaseUrl };
                            }
                        }
                    }
                }
            };
        }
    }
}
