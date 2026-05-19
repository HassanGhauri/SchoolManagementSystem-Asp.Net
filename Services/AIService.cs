using Newtonsoft.Json;
using SchoolManagementSystem.Api.DTOs;
using System.Text;

namespace SchoolManagementSystem.Api.Services
{
    public class AIService
    {
        private readonly HttpClient _httpClient;

        public AIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AIResponse?> DetectIntent(
            string question)
        {
            var apiKey =
                Environment.GetEnvironmentVariable(
                    "OPENROUTER_API_KEY"
                );

            // =========================================
            // SYSTEM PROMPT
            // =========================================

            var systemPrompt = @"
You are an AI intent parser.

You MUST ONLY return raw JSON.

Do NOT return:
- explanations
- markdown
- code blocks
- extra text

Supported intents:

1. get_students_by_class
2. get_teachers_by_class
3. get_subjects_by_class
4. get_students_by_age

Examples:

{
  ""intent"": ""get_students_by_class"",
  ""className"": ""Grade 1 - A""
}

{
  ""intent"": ""get_teachers_by_class"",
  ""className"": ""Grade 2 - B""
}

{
  ""intent"": ""get_subjects_by_class"",
  ""className"": ""Grade 3 - C""
}

{
  ""intent"": ""get_students_by_age"",
  ""ageLessThan"": 20
}

{
  ""intent"": ""get_students_by_age"",
  ""ageGreaterThan"": 18
}
";

            // =========================================
            // REQUEST BODY
            // =========================================

            var requestBody = new
            {
                model = "deepseek/deepseek-v4-flash:free",

                temperature = 0,

                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = systemPrompt
                    },

                    new
                    {
                        role = "user",
                        content = question
                    }
                }
            };

            var json =
                JsonConvert.SerializeObject(requestBody);

            // =========================================
            // CREATE REQUEST
            // =========================================

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://openrouter.ai/api/v1/chat/completions"
            );

            request.Headers.Add(
                "Authorization",
                $"Bearer {apiKey}"
            );

            request.Headers.Add(
                "HTTP-Referer",
                "http://localhost:5164"
            );

            request.Headers.Add(
                "X-Title",
                "School Management AI"
            );

            request.Content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            // =========================================
            // SEND REQUEST
            // =========================================

            var response =
                await _httpClient.SendAsync(request);

            var responseString =
                await response.Content.ReadAsStringAsync();

            // =========================================
            // DEBUG RESPONSE
            // =========================================

            Console.WriteLine("========== AI RESPONSE ==========");
            Console.WriteLine(responseString);
            Console.WriteLine("=================================");

            // =========================================
            // HANDLE API ERRORS
            // =========================================

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"OpenRouter Error: {responseString}"
                );
            }

            // =========================================
            // DESERIALIZE RESPONSE
            // =========================================

            dynamic? result =
                JsonConvert.DeserializeObject(responseString);

            if (result == null)
            {
                throw new Exception(
                    "AI response is null"
                );
            }

            if (result.choices == null)
            {
                throw new Exception(
                    $"No choices found. Response: {responseString}"
                );
            }

            var aiText =
                result.choices[0]?.message?.content?.ToString();

            if (string.IsNullOrEmpty(aiText))
            {
                throw new Exception(
                    $"AI content empty. Response: {responseString}"
                );
            }

            // =========================================
            // REMOVE CODE BLOCKS
            // =========================================

            aiText = aiText
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            Console.WriteLine("========== CLEANED JSON ==========");
            Console.WriteLine(aiText);
            Console.WriteLine("==================================");

            // =========================================
            // CONVERT JSON → C# OBJECT
            // =========================================

            var intent =
                JsonConvert.DeserializeObject
                <AIResponse>(aiText);

            if (intent == null)
            {
                throw new Exception(
                    "Failed to deserialize AI JSON"
                );
            }

            return intent;
        }
    }
}