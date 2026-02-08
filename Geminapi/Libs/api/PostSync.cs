using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json; // 이 줄이 반드시 있어야 합니다.
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Geminapi
{
    public class PostSync
    {
        readonly string apiKey = "AIzaSyDOvjo36moasktvpyYoqGBQrWTvCSouWjc";
        string url;
        string fileUri;
        string dirCurr;
        string nameCurr;
        string fileCurr;
        string fileId;

        public bool OnSending = false;

        public PostSync(string dirCurr, string nameCurr, string fileId)
        {
            this.dirCurr = dirCurr;
            this.nameCurr = nameCurr;
            this.fileId= fileId;

            url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent?key={apiKey}";
            fileUri = $"https://generativelanguage.googleapis.com/v1beta/files/{fileId}";
            fileCurr = $@"{dirCurr}\{nameCurr}.txt";
        }
        public async Task<string> SendDialog(string textFull4Send)
        {
            //var userInput = "왜 안되는 것이냐?";

            string safeTextFull = textFull4Send.Replace("\"", "\\\""); // 따옴표 이스케이프 처리
            string json = "{\"contents\":[{\"parts\":[{\"text\":\"" + safeTextFull + "\"}]}]}";

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

                return chatResponse;
            }
        }
        public async Task<string> SendDialogRef(string userInput)
        {
            System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072; // TLS 1.2 강제
            string safeInput = userInput.Replace("\"", "\\\""); // 따옴표 이스케이프 처리

            var jsonObj = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new
                            {
                                file_data = new
                                {
                                    file_uri = fileUri,
                                    mime_type = "text/plain"
                                }
                            },
                            new
                            {
                                text = safeInput
                            }
                        }
                    }
                }
            };

            //string jsonx =
            //"{" +
            //    "\"contents\": [{" +
            //        "\"parts\": [" +
            //                "{\"file_data\": {" +
            //                    "\"file_uri\": \"" + fileUri + "\"," +
            //                    "\"mime_type\": \"text/plain\"" +
            //                    "}}," +
            //                "{\"text\": \"" + safeInput + "\"}" +
            //            "]" +
            //    "}]" +
            //"}";

            var jsonx = JsonConvert.SerializeObject(jsonObj);

            using (var client = new HttpClient())
            {
                var content = new StringContent(jsonx, Encoding.UTF8, "application/json");
                var res = await client.PostAsync(url, content);
                //PostAsJsonAsync
                string result = await res.Content.ReadAsStringAsync();
                //Console.WriteLine(result);

                var jo = Newtonsoft.Json.Linq.JObject.Parse(result);
                string chatResponse = jo["candidates"][0]["content"]["parts"][0]["text"].ToString();
                //Console.WriteLine($"Gemini: {chatResponse}");

                chatResponse = chatResponse.Replace("\n", Environment.NewLine);

                return chatResponse;
            }
        }
        public async Task<string> UploadFile(string fileName)
        {
            var filePath = $@"{dirCurr}\{fileName}";

            //string fileName = Path.GetFileName(filePath);
            string urlUp = $"https://generativelanguage.googleapis.com/upload/v1beta/files?key={apiKey}";

            using (var client = new HttpClient())
            {
                // [필수] 구글 업로드 프로토콜 지정
                client.DefaultRequestHeaders.Add("X-Goog-Upload-Protocol", "multipart");

                using (var multipart = new MultipartFormDataContent())
                {
                    // 1. Metadata 파트 설정
                    var metadata = new { file = new { display_name = fileName } };
                    var metadataContent = new StringContent(JsonConvert.SerializeObject(metadata), Encoding.UTF8, "application/json");

                    // 중요: 구글 서버는 Content-Type에 charset=utf-8이 붙으면 Malformed라고 뱉는 경우가 있음
                    metadataContent.Headers.ContentType.CharSet = null;
                    multipart.Add(metadataContent, "metadata");

                    // 2. File 파트 설정
                    var fileBytes = File.ReadAllBytes(filePath);
                    var fileContent = new ByteArrayContent(fileBytes);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                    multipart.Add(fileContent, "file"); // fileName 인자를 빼고 보냄 (더 안전함)

                    // 3. 전송
                    var response = await client.PostAsync(urlUp, multipart);
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("업로드 성공!");
                        Console.WriteLine(result);
                        // 이제 files.list에서 확인 가능합니다.
                    }
                    else
                    {
                        Console.WriteLine($"실패 상태코드: {response.StatusCode}");
                        Console.WriteLine($"실패 원인: {result}");
                    }

                    return result;
                }
            }
        }
        public async Task<string> DeleteGeminiFile(string fileId)
        {
            string urlFile = $"https://generativelanguage.googleapis.com/v1beta/files/{fileId}?key={apiKey}";

            using (var client = new HttpClient())
            {
                // DELETE 요청 전송
                var response = await client.DeleteAsync(urlFile);
                var result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"삭제 성공, fileId : {fileId}");
                    return $"삭제 성공, fileId : {fileId}";
                    // 성공 시 보통 빈 JSON "{}" 이 반환됩니다.
                }
                else
                {
                    Console.WriteLine($"삭제 실패: {response.StatusCode}");
                    Console.WriteLine($"원인: {result}");
                    return $"삭제 실패:\r\n {response.StatusCode}\r\n원인:\r\n {result}";
                }
            }
        }
        public async Task<string> CreateServerCache(string fileUri)
        {
            using (var client = new HttpClient())
            {
                //var json = @"{
                //    'model': 'models/gemini-3-flash-preview',
                //    'contents': [{ 'parts': [{ 'text': '대용량 문서 내용...' }], 'role': 'user' }],
                //    'ttl': '3600s'
                // }";

                var payload = new
                {
                    model = "models/gemini-3-flash-preview",
                    contents = new[] { new { parts = new[] { new { file_data = new { file_uri = fileUri, mime_type = "text/plain" } } } } },
                    ttl = "3600s"
                };
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();

                // 결과에서 생성된 캐시 'name'을 저장하여 질문 시 사용
                Console.WriteLine(result);
                return result;
            }
        }
        public async Task<string> GetFileListAsync()
        {
            // 1. 요청 URL 설정 (v1beta/files 엔드포인트 사용)
            // 참고: API 키를 쿼리 스트링(?key=)으로 전달하거나 헤더로 전달할 수 있습니다.
            string url = "https://generativelanguage.googleapis.com/v1beta/files";

            // 2. 요청 생성
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // 3. 헤더 설정
            // x-goog-api-key 헤더를 통해 API 키 인증
            request.Headers.Add("x-goog-api-key", apiKey);
            request.Headers.Add("Accept", "application/json");

            try
            {
                var client = new System.Net.Http.HttpClient();
                // 4. 요청 전송
                HttpResponseMessage response = await client.SendAsync(request);

                // 5. 결과 처리
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("--- 파일 리스트 조회 성공 ---");
                    Console.WriteLine(result);

                    return result;
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"--- 오류 발생 ({response.StatusCode}) ---");
                    Console.WriteLine(error);
                    return error;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"연결 오류: {ex.Message}");
                return ($"연결 오류: {ex.Message}");
            }
        }
    }
}
