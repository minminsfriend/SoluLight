using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Geminapi
{
    public partial class Geminapi00 
    {
        readonly string apiKey = "AIzaSyDOvjo36moasktvpyYoqGBQrWTvCSouWjc";
        private string textOutput_Text;
        private string textInput_Text;

        public Geminapi00()
        {
            //InitializeComponent();

            //this.DoubleBuffered = true;
        }
        void btnSend_Click(object sender, EventArgs e)
        {
            SendDialog();

            //SendDialogRef();
        }
        async void btnUpFile_Click(object sender, EventArgs e)
        {
            var filepath = @"d:\Works\vs\SoluLight\ConTest\대화기록\01 설정집\설정 합본.txt";
            await UploadFileAsync(filepath);

            //var res = UploadFileToServer(filepath, apiKey);

            //var fileUri = res.Result;

            //Console.WriteLine(res.Result);
            //textOutput_Text = "캐싱화일 주소:\r\n" + res.Result;

            //await CreateServerCache(fileUri);

            ////textOutput_Text = res.Result;
        }
        void btnRefFile_Click(object sender, EventArgs e)
        {
            Console.WriteLine("참조 시작...");
            SendDialogRef();
        }
        async void SendDialog()
        {
            var userInput = textInput_Text;
            //var userInput = "왜 안되는 것이냐?";

            System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072; // TLS 1.2 강제
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent?key={apiKey}";
            string safeInput = userInput.Replace("\"", "\\\""); // 따옴표 이스케이프 처리
            string json = "{\"contents\":[{\"parts\":[{\"text\":\"" + safeInput + "\"}]}]}";

            using (var client = new System.Net.Http.HttpClient())
            {
                var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var res = await client.PostAsync(url, content);
                //PostAsJsonAsync
                string result = await res.Content.ReadAsStringAsync();
                //Console.WriteLine(result);

                var jo = Newtonsoft.Json.Linq.JObject.Parse(result);
                string chatResponse = jo["candidates"][0]["content"]["parts"][0]["text"].ToString();
                //onsole.WriteLine($"Gemini: {chatResponse}");
                chatResponse = chatResponse.Replace("\n", Environment.NewLine);
                textOutput_Text = chatResponse;

                textInput_Text += "\r\n\r\n◆ " + chatResponse + "\r\n\r\n◇ ";
                //textInput.SelectionStart = textInput_Text.Length;
                //textInput.ScrollToCaret(); // 커서가 보이도록 스크롤
            }
        }
        async void SendDialogRef()
        {
            var userInput = textInput_Text;
            //var userInput = "왜 안되는 것이냐?";

            string fileUri = "https://generativelanguage.googleapis.com/v1beta/files/voxrdqh4jk2k";

            System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072; // TLS 1.2 강제
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent?key={apiKey}";
            string safeInput = userInput.Replace("\"", "\\\""); // 따옴표 이스케이프 처리

            string jsonx = "{" +
                 "\"contents\": [{" +
                     "\"parts\": [" +
                         "{\"file_data\": {" +
                             "\"file_uri\": \"" + fileUri + "\"," +
                             "\"mime_type\": \"text/plain\"" +
                         "}}," +
                         "{\"text\": \"" + safeInput + "\"}" +
                     "]" +
                 "}]" +
             "}";

            using (var client = new System.Net.Http.HttpClient())
            {
                var content = new System.Net.Http.StringContent(jsonx, System.Text.Encoding.UTF8, "application/json");
                var res = await client.PostAsync(url, content);
                //PostAsJsonAsync
                string result = await res.Content.ReadAsStringAsync();
                //Console.WriteLine(result);

                var jo = Newtonsoft.Json.Linq.JObject.Parse(result);
                string chatResponse = jo["candidates"][0]["content"]["parts"][0]["text"].ToString();
                //Console.WriteLine($"Gemini: {chatResponse}");

                chatResponse = chatResponse.Replace("\n", Environment.NewLine);
                textOutput_Text = chatResponse;

                textInput_Text += "\r\n\r\n◆ " + chatResponse + "\r\n\r\n◇ ";
                //textInput.SelectionStart = textInput_Text.Length;
                //textInput.ScrollToCaret(); // 커서가 보이도록 스크롤
            }
        }
        async void UseFlask()
        {
            //var userInput = textInput_Text;
            var userInput = "왜 안되는 것이냐?";

            using (var client = new HttpClient())
            {
                var content = new StringContent("{\"text\": \"왜 안되는 것이냐?\"}", Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://localhost:5000/ask", content);
                var result = await response.Content.ReadAsStringAsync();

                var jo = Newtonsoft.Json.Linq.JObject.Parse(result);
                string chatResponse = jo["candidates"][0]["content"]["parts"][0]["text"].ToString();
                //onsole.WriteLine($"Gemini: {chatResponse}");
                chatResponse = chatResponse.Replace("\n", Environment.NewLine);
                textOutput_Text = chatResponse;

                textInput_Text += "\r\n\r\n◆ " + chatResponse + "\r\n\r\n◇ ";
                //textInput.SelectionStart = textInput_Text.Length;
                //textInput.ScrollToCaret(); // 커서가 보이도록 스크롤
            }
        }
        async Task<string> AskToPython(string userText)
        {
            using (var client = new HttpClient())
            {
                // 파이썬 Flask 서버 주소
                var url = "http://127.0.0.1:5090";

                // 보낼 데이터 (JSON 형식)
                var json = "{\"text\": \"" + userText.Replace("\"", "\\\"") + "\"}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    Console.WriteLine("파이썬 서버에 요청 전송 중...");
                    var response = await client.PostAsync(url, content);
                    Console.WriteLine("var response = await client.PostAsync(url, content);");
                    if (response.IsSuccessStatusCode)
                    {
                        // 파이썬이 리턴한 JSON 결과 읽기
                        return await response.Content.ReadAsStringAsync();
                    }
                    return "에러 발생: " + response.StatusCode;
                }
                catch (Exception ex)
                {
                    return "서버 연결 실패: " + ex.Message;
                }
            }
        }
        async Task UploadFileAsync(string filePath)
        {
            var client = new HttpClient();
            var fileInfo = new FileInfo(filePath);

            // 1. 메타데이터 생성 (JSON)
            var metadata = new { file = new { displayName = fileInfo.Name } };
            var metadataJson = System.Text.Json.JsonSerializer.Serialize(metadata);

            // 2. Multipart 관련 콘텐츠 구성
            var content = new MultipartFormDataContent();

            // 메타데이터 추가
            var metadataContent = new StringContent(metadataJson, Encoding.UTF8, "application/json");
            content.Add(metadataContent, "metadata");

            // 실제 파일 데이터 추가
            var fileStream = new FileStream(filePath, FileMode.Open);
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain"); // 파일 형식에 맞게 변경
            content.Add(fileContent, "file", fileInfo.Name);

            // 3. HTTPS 요청 전송
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent?key={apiKey}";
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                //var result = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("업로드 성공: " + result);
                // 결과에서 추출한 'uri'를 사용하여 질문 시 활용 가능합니다.

                dynamic result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                //return result.file.uri; // 서버에 저장된 고유 URI 반환

                //var jsonString = await response.Content.ReadAsStringAsync();
                //var uploadResult = JsonSerializer.Deserialize<UploadResponse>(jsonString);

                //string fileUri = uploadResult.file.uri; // 이 주소를 보관해두세요.
                Console.WriteLine($"파일 주소(URI): {result.file.uri}");
            }
        }
        async Task<string> UploadFileToServer(string filePath, string apiKey)
        {
            using (var client = new HttpClient())
            {
                var fileData = File.ReadAllBytes(filePath);
                var request = new HttpRequestMessage(HttpMethod.Post, $"https://generativelanguage.googleapis.com/upload/v1beta/files?key={apiKey}");

                // 메타데이터 설정 (파일 이름, 타입 등)
                request.Headers.Add("X-Goog-Upload-Protocol", "multipart");
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(JsonConvert.SerializeObject(new { file = new { display_name = "설정데이터" } })), "metadata");
                content.Add(new ByteArrayContent(fileData), "file");

                var response = await client.SendAsync(request);
                dynamic result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                return result.file.uri; // 서버에 저장된 고유 URI 반환
            }
        }
        public async Task CreateGeminiCache()
        {
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent?key={apiKey}";

            using (var client = new HttpClient())
            {
                // 캐시 생성 데이터 (JSON)
                var jsonPayload = @"{
            'model': 'models/gemini-3-flash-preview',
            'contents': [{ 'parts': [{ 'text': '대용량 문서 내용...' }], 'role': 'user' }],
            'ttl': '3600s'
        }";

                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();

                // 결과에서 생성된 캐시 'name'을 저장하여 질문 시 사용
                Console.WriteLine(result);
            }
        }
        async Task CreateServerCache(string fileUri)
        {
            using (var client = new HttpClient())
            {
                var payload = new
                {
                    model = "models/gemini-3-flash-preview",
                    contents = new[] { new { parts = new[] { new { file_data = new { file_uri = fileUri, mime_type = "text/plain" } } } } },
                    ttl = "3600s" // 1시간 동안 서버 메모리 강제 점유
                };
                var json = JsonConvert.SerializeObject(payload);
                await client.PostAsync($"https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent?key={apiKey}",
                                       new StringContent(json, Encoding.UTF8, "application/json"));
            }
        }

    }
}
