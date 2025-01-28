# **Trendyol Price Finder API**

## **Proje Açıklaması**
Trendyol Price Finder API, belirttiğiniz bir ürün modelinin Trendyol üzerinde mevcut olan en düşük fiyatını bulmak için tasarlanmış bir web API'sidir. Bu API, Selenium WebDriver kullanarak Trendyol üzerindeki ürünleri tarar, fiyatları toplar ve kullanıcıya JSON formatında en uygun fiyatı döner. Ayrıca, ChatGPT API entegrasyonu sayesinde ürün fiyatlarını alternatif bir yöntemle arama yeteneği eklenmiştir.

---

## **Özellikler**
- Belirtilen ürün modeli için Trendyol'daki en düşük fiyatı listeler.
- Dinamik web içeriklerini destekler (JavaScript ile yüklenen elemanları tarar).
- Gereksiz sonuçları filtreler (örneğin, "kılıf", "uyumlu", "aksesuar" gibi ürünler hariç tutulur).
- ChatGPT API entegrasyonu ile ürün fiyatlarını arka planda sorgular.
- Kullanıcıdan gelen taleplere JSON formatında yanıt verir.

---

## **Kullanılan Teknolojiler**
- **.NET 8 Web API**: API'nin temel çerçevesi.
- **Selenium WebDriver**: Trendyol üzerindeki ürün ve fiyatları taramak için.
- **ChromeDriver**: WebDriver için tarayıcı desteği.
- **Newtonsoft.Json**: JSON formatında verileri döndürmek için.
- **ChatGPT API**: Ürün fiyatlarını alternatif olarak sorgulamak için.
- **C#**: Uygulamanın geliştirilmesi için kullanılan dil.

---

## **Gereksinimler**
- **.NET SDK 8.0 veya üzeri**
- **Chrome Tarayıcı** (Son sürüm)
- **ChromeDriver** (Selenium için uygun sürüm)
- **NuGet Paketleri**:
  - `Selenium.WebDriver`
  - `Selenium.WebDriver.ChromeDriver`
  - `Newtonsoft.Json`
  - `System.Net.Http.Json`
- **ChatGPT API Anahtarı** (OpenAI platformundan alınabilir)

---

## **Kurulum ve Kullanım**
### 1. **Projeyi Klonlayın**
```bash
git clone https://github.com/username/trendyol-price-finder.git
cd trendyol-price-finder
```

### 2. **Gerekli NuGet Paketlerini Yükleyin**
```bash
dotnet restore
```

### 3. **appsettings.json Ayarları**
`appsettings.json` dosyasını aşağıdaki gibi yapılandırın:
```json
{
  "OpenAI": {
    "ApiUrl": "https://api.openai.com/v1/chat/completions",
    "ApiKey": "YOUR_API_KEY"
  }
}
```

### 4. **Projeyi Çalıştırın**
```bash
dotnet run
```

### 5. **API'ye İstek Atın**
#### **Trendyol'dan Fiyat Sorgulama**
- **URL**:
  ```
  https://localhost:5001/api/Price/GetLowestPrice?model=iphone%2011
  ```

- **Örnek Yanıt**:
  ```json
  {
    "Model": "iphone 11",
    "LowestPrice": 13299.99,
    "Source": "Trendyol"
  }
  ```

#### **ChatGPT ile Fiyat Sorgulama**
- **URL**:
  ```
  https://localhost:5001/api/Price/GetLowestPriceWithChatGPT?model=iphone%2011
  ```

- **Örnek Yanıt**:
  ```json
  {
    "Model": "iphone 11",
    "Suggestion": "The lowest price for iPhone 11 is approximately $599 on Amazon."
  }
  ```

---

## **Proje Mantığı**
1. **URL Üzerinden Model Adı Alınır**: Kullanıcıdan gelen `model` parametresi ile arama gerçekleştirilir.
2. **Selenium Kullanılarak Trendyol Taranır**: 
   - Belirtilen ürünlerin başlıkları ve fiyatları dinamik olarak yüklenir.
   - "uyumlu", "kılıf", "kapak" gibi gereksiz ürünler filtrelenir.
3. **ChatGPT API Kullanımı**:
   - OpenAI'nın ChatGPT API'si kullanılarak ürün fiyatları sorgulanır.
   - Alternatif bir bilgi kaynağı sağlar.
4. **JSON Formatında Yanıt Dönülür**: Kullanıcıya model ve fiyat bilgisi gönderilir.

---

## **Hata Durumları**
API, aşağıdaki durumlarda uygun hata mesajları döner:
1. **Model Adı Boş**:
   ```json
   {
     "error": "Model adı boş olamaz."
   }
   ```
2. **Ürün Bulunamadı**:
   ```json
   {
     "error": "Model 'iphone 11' için uygun fiyatlı ürün bulunamadı."
   }
   ```
3. **Genel Hata**:
   ```json
   {
     "error": "Beklenmeyen bir hata oluştu.",
     "details": "Exception message"
   }
   ```

---

---

## **Lisans**
Bu proje [MIT Lisansı](LICENSE) altında lisanslanmıştır.
