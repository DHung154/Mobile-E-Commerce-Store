using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
// using HiTechShop.Models; // Giữ lại nếu cần thiết

// Định nghĩa các lớp cho Request và Response của Gemini API
// Điều này giúp việc tuần tự hóa (Serialization) và giải tuần tự hóa (Deserialization) JSON an toàn hơn
public class ContentPart
{
    public string text { get; set; }
}

public class Content
{
    public ContentPart[] parts { get; set; }
    public string role { get; set; } // Phải là "user" hoặc "model"
}

public class GeminiRequest
{
    // Cấu trúc Request: Chứa lịch sử hội thoại (ở đây chỉ có một tin nhắn)
    public Content[] contents { get; set; }
}

public class Candidate
{
    public Content content { get; set; }
    // Thêm các trường khác nếu cần (như finishReason, v.v.)
}

public class GeminiResponse
{
    // Cấu trúc Response: Chứa các lựa chọn phản hồi
    public Candidate[] candidates { get; set; }
}

public class ChatController : Controller
{
    // 🔑 API KEY CỦA BẠN ĐÃ ĐƯỢC ĐẶT VÀO ĐÂY:
    private readonly string apiKey = "";

    public ActionResult ChatAI()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> SendMessage(string message)
    {
        // 1. Tạo Request Body theo chuẩn Gemini API
        var requestBody = new GeminiRequest
        {
            contents = new Content[]
            {
                new Content
                {
                    role = "user",
                    parts = new ContentPart[] { new ContentPart { text = message } }
                }
            }
        };

        try
        {
            using (var httpClient = new HttpClient())
            {
                // Serializer Request Body thành JSON String
                var content = new StringContent(
                    JsonConvert.SerializeObject(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                // 2. Định nghĩa URL chính xác với API Key
                // Model được sử dụng: gemini-2.5-flash
                string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

                // 3. Gửi POST Request
                var response = await httpClient.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // 4. Phân tích Response Body
                    var chatResponse = JsonConvert.DeserializeObject<GeminiResponse>(responseString);

                    // Trích xuất nội dung từ phản hồi đầu tiên
                    if (chatResponse?.candidates?.Length > 0 &&
                        chatResponse.candidates[0].content.parts?.Length > 0)
                    {
                        string reply = chatResponse.candidates[0].content.parts[0].text;
                        return Content(reply);
                    }
                    else
                    {
                        // Trường hợp API trả về thành công nhưng không có nội dung (ví dụ: bị lọc nội dung)
                        return Content("Lỗi: Không nhận được phản hồi text hợp lệ từ Gemini API.");
                    }
                }
                else
                {
                    // Trả về lỗi chi tiết từ API
                    return Content($"Lỗi API ({response.StatusCode}): {responseString}");
                }
            }
        }
        catch (Exception ex)
        {
            // Xử lý các lỗi kết nối hoặc ngoại lệ khác
            return Content("Lỗi ngoại lệ: " + ex.Message);
        }
    }
}