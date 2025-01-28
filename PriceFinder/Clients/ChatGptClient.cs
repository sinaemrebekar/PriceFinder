using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PriceFinder.Clients;
public class ChatGptClient
{
  private readonly HttpClient _httpClient;
  private readonly string? _apiUrl;
  private readonly string? _apiKey;

  public ChatGptClient(IConfiguration configuration)
  {
    _apiUrl = configuration["Configs:ChatGPT:ApiUrl"];
    _apiKey = configuration["Configs:ChatGPT:ApiKey"];
    _httpClient = new HttpClient();
    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
  }

  public async Task<string> GetPriceSuggestionAsync(string model)
  {
    var requestBody = new
    {
      model = "gpt-3.5-turbo",
      messages = new[]
      {
        new { role = "system", content = "You are a price finder bot that searches for phone prices on the internet." },
        new { role = "user", content = $"Find the lowest price for {model} on the internet." }
      }
    };

    var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

    var response = await _httpClient.PostAsync(_apiUrl, content);
    response.EnsureSuccessStatusCode();

    var responseContent = await response.Content.ReadAsStringAsync();
    var responseJson = JsonDocument.Parse(responseContent);

    var result = responseJson.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
    return result;
  }
}
