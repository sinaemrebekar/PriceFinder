using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using PriceFinder.Clients;

namespace PriceFinder.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PriceController : ControllerBase
  {
    private readonly ChatGptClient _chatGptClient;

    public PriceController(IConfiguration configuration)
    {
      _chatGptClient = new ChatGptClient(configuration);
    }

    [HttpGet("GetLowestPrice")]
    public IActionResult GetLowestPrice(string model)
    {
      if (string.IsNullOrWhiteSpace(model))
        return BadRequest("Model adı boş olamaz.");

      try
      {
        var options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--no-sandbox");
        options.AddArgument(
            "--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

        using (var driver = new ChromeDriver(options))
        {
          var validPrices = new List<decimal>();

          var searchUrl =
              $"https://www.trendyol.com/sr?q={Uri.EscapeDataString(model)}&qt={Uri.EscapeDataString(model)}&st={Uri.EscapeDataString(model)}&lc=164462&os=1";
          driver.Navigate().GoToUrl(searchUrl);

          var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
          wait.Until(drv => drv.FindElements(By.CssSelector(".p-card-chldrn-cntnr")).Any());

          var productContainers = driver.FindElements(By.CssSelector(".product-down"));
          var unwantedKeywords = new List<string> { "uyumlu", "kılıf", "kapak", "için", "aksesuar", "yenilenmiş" };

          foreach (var container in productContainers)
          {
            var titleElement = container.FindElement(By.CssSelector(".prdct-desc-cntnr-name"));
            var priceElement = container.FindElements(By.CssSelector(".prc-box-dscntd")).FirstOrDefault();

            if (titleElement != null && priceElement != null)
            {
              var title = titleElement.Text.ToLowerInvariant();
              var priceText = priceElement.Text.Replace("TL", "").Replace(".", "").Replace(",", ".").Trim();

              if (decimal.TryParse(priceText, out var price) &&
                  title.Contains(model.ToLowerInvariant()) &&
                  !unwantedKeywords.Any(keyword => title.Contains(keyword.ToLower())))
              {
                validPrices.Add(price);
              }
            }
          }

          if (!validPrices.Any())
          {
            return NotFound($"Model '{model}' için uygun fiyatlı ürün bulunamadı.");
          }

          return Ok(new
          {
            Model = model,
            LowestPrice = validPrices.Min(),
            Source = "Trendyol"
          });
        }
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { Error = ex.Message });
      }
    }

    [HttpGet("GetLowestPriceWithChatGPT")]
    public async Task<IActionResult> GetLowestPriceWithChatGPT(string model)
    {
      if (string.IsNullOrWhiteSpace(model))
      {
        return BadRequest("Model adı boş olamaz.");
      }

      try
      {
        var result = await _chatGptClient.GetPriceSuggestionAsync(model);
        return Ok(new { Model = model, Suggestion = result });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { Error = ex.Message });
      }
    }
  }
}
