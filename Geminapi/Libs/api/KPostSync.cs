using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Geminapi
{
    public class KPostSync
    {
        readonly string apiKey = "AIzaSyDOvjo36moasktvpyYoqGBQrWTvCSouWjc";

        const string 플래시 = "gemini-3-flash-preview";
        const string 프로 = "gemini-3-pro-preview";
        const string 바나나 = "gemini-3-pro-image-preview";

        string URI;
        string dirCurr;
        string nameCurr;
        string fileCurr;
        string cacheId;
        public string MODEL = 플래시;
        public bool OnSending = false;

        public KPostSync(string dirCurr, string nameCurr)
        {
            this.dirCurr = dirCurr;
            this.nameCurr = nameCurr;

            URI = $"https://generativelanguage.googleapis.com/v1beta/models/{MODEL}:generateContent?key={apiKey}";
            fileCurr = $@"{dirCurr}\{nameCurr}.txt";
        }
        public async Task<string> SendDialog(string text4Send)
        {
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
                                text = text4Send
                            }
                        }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(jsonObj);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(300));

            using (var client = new System.Net.Http.HttpClient())
            {
                var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var res = await client.PostAsync(URI, content, cts.Token);
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
        public async Task<string> SendDialogSearch(string text4Send)
        {
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
                                text = text4Send
                            }
                        }
                    }
                },
                tools = new[]
                {
                    new {
                        google_search = new { }

                    //    google_search_retrieval = new {
                    //    //google_search = new {
                    //        dynamic_retrieval_config = new {
                    //            mode = "MODE_DYNAMIC", // 필요한 경우에만 검색 실행
                    //            //mode = "DYNAMIC", // 필요한 경우에만 검색 실행
                    //            dynamic_threshold = 0.3 // 0.0~1.0 사이 (낮을수록 검색을 더 자주 함)
                    //        }
                    //    }
                    }
            }
            };

            var json = JsonConvert.SerializeObject(jsonObj);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(300));

            using (var client = new System.Net.Http.HttpClient())
            {
                var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var res = await client.PostAsync(URI, content, cts.Token);
                string result = await res.Content.ReadAsStringAsync();
                //Console.WriteLine(result);

                if (!res.IsSuccessStatusCode)
                {
                    // 여기서 에러 메시지를 확인하면 정확한 원인을 알 수 있습니다.
                    Console.WriteLine($"API Error: {res.StatusCode} - {result}");
                }

                var jo = Newtonsoft.Json.Linq.JObject.Parse(result);
                string chatResponse = jo["candidates"][0]["content"]["parts"][0]["text"].ToString();
                //onsole.WriteLine($"Gemini: {chatResponse}");
                chatResponse = chatResponse.Replace("\n", Environment.NewLine);

                return chatResponse;
            }
        }
        public async Task<string> SendDialogRef(string text4Send, string fileId)
        {
            var fileUri = $"https://generativelanguage.googleapis.com/v1beta/files/{fileId}";
      
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
                                text = text4Send
                            }
                        }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(jsonObj);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(300));

            using (var client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await client.PostAsync(URI, content, cts.Token);
                
                string result = await res.Content.ReadAsStringAsync();

                var jo = Newtonsoft.Json.Linq.JObject.Parse(result);
                string chatResponse = jo["candidates"][0]["content"]["parts"][0]["text"].ToString();

                chatResponse = chatResponse.Replace("\n", Environment.NewLine);

                return chatResponse;
            }
            /*
            var payload = new { contents = new[] { new { 
                parts = new object[] {
                    new { text = "복수 파일 분석 요청" },
                    new { file_data = new { mime_type = "text/plain", file_uri = "아이디1" } },
                    new { file_data = new { mime_type = "text/plain", file_uri = "아이디2" } }
            } } } };
            */
        }
        public async Task<string> SendDialogRefImage(string text4Send, string fileId)
        {
            var fileUri = $"https://generativelanguage.googleapis.com/v1beta/files/{fileId}";

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
                                    mime_type = "image/jpeg"
                                }
                            },
                            new
                            {
                                //text = safeInput
                                text = text4Send
                            }
                        }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(jsonObj);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(300));

            using (var client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await client.PostAsync(URI, content, cts.Token);
                
                string result = await res.Content.ReadAsStringAsync();

                var jo = Newtonsoft.Json.Linq.JObject.Parse(result);
                string chatResponse = jo["candidates"][0]["content"]["parts"][0]["text"].ToString();

                chatResponse = chatResponse.Replace("\n", Environment.NewLine);

                return chatResponse;
            }
        }
        public async Task<string> SendDialogRefC(string text4Send)
        {
            var jsonObj = new
            {
                // 1. 질문 내용 (텍스트만)
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new object[]
                        {
                            new { text = text4Send }
                        }
                    }
                },
                // 2. 캐시 ID는 contents와 같은 레벨에 위치 (parts 밖, root 안)
                cached_content = "cachedContents/u9wxtnwplh3cd4u76dyv6rx7lc0p5gnv3ksiy72e"
            };

            var json = JsonConvert.SerializeObject(jsonObj);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(300));

            using (var client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await client.PostAsync(URI, content, cts.Token);
                
                string result = await res.Content.ReadAsStringAsync();

                var jo = Newtonsoft.Json.Linq.JObject.Parse(result);
                string chatResponse = jo["candidates"][0]["content"]["parts"][0]["text"].ToString();

                chatResponse = chatResponse.Replace("\n", Environment.NewLine);

                return chatResponse;
            }
        }
        public async Task<string> UploadFile(string fileName, string _MIME_)
        {
            var filePath = $@"{dirCurr}\{fileName}";

            if(_MIME_=="image/jpeg")
                filePath = $@"{dirCurr}\images\{fileName}";

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
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(_MIME_);
                    //fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
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
        public async Task<string> DeleteContextCacheAsync(string cacheId)
        {
            string urlCache = $"https://generativelanguage.googleapis.com/v1beta/cachedContents/{cacheId}?key={apiKey}";

            using (var client = new HttpClient())
            {
                // DELETE 요청 전송
                var response = await client.DeleteAsync(urlCache);
                var result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"캐시 삭제 성공, cacheId : {cacheId}");
                    this.cacheId = ""; // 클래스 내 저장된 cacheId 초기화
                    return $"캐시 삭제 성공, cacheId : {cacheId}";
                }
                else
                {
                    Console.WriteLine($"캐시 삭제 실패: {response.StatusCode}");
                    Console.WriteLine($"원인: {result}");
                    return $"캐시 삭제 실패:\r\n {response.StatusCode}\r\n원인:\r\n {result}";
                }
            }
        }
        public async Task<string> CreateContextCacheAsync(string fileId)
        {
            var fileUri = $"https://generativelanguage.googleapis.com/v1beta/files/{fileId}";

            using (HttpClient client = new HttpClient())
            {
                string urlC = $"https://generativelanguage.googleapis.com/v1beta/cachedContents?key={apiKey}";

                var jsonData = new
                {
                    model = $"models/{MODEL}",
                    contents = new[] {
                        new {
                                role = "user", 
                                parts = new[] {
                                new {
                                    file_data = new {
                                        mime_type = "text/plain",
                                        file_uri = fileUri
                                    }
                                }
                            }
                        }
                    },
                    ttl = "3600s" // 1시간 유지
                };

                string json = JsonConvert.SerializeObject(jsonData);

                using (StringContent content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    // 3. POST 요청 실행
                    HttpResponseMessage response = await client.PostAsync(urlC, content);
                    string responseString = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        // 생성된 캐시 ID(name) 추출
                        JObject result = JObject.Parse(responseString);
                        return result["name"].ToString(); // 예: "cachedContents/xxxx"
                    }
                    else
                    {
                        throw new Exception("캐시 생성 실패: " + responseString);
                    }
                }
            }
        }
        public async Task<string> GetCacheListAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                // GET 방식이며 주소는 cachedContents입니다.
                string url = $"https://generativelanguage.googleapis.com/v1beta/cachedContents?key={apiKey}";

                HttpResponseMessage response = await client.GetAsync(url);
                string responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // 리스트 전체 JSON 반환
                    return responseString;
                }
                else
                {
                    throw new Exception("캐시 목록 조회 실패: " + responseString);
                }
            }
        }
        public async Task<string> GenerateImageWithRef(string text4Send, string fileId, string imageOutName)
        {
            System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072; 
            string safeInput = text4Send.Replace("\"", "\\\""); // 따옴표 이스케이프 처리

            var fileUri = $"https://generativelanguage.googleapis.com/v1beta/files/{fileId}";

            string urlApi = $"https://generativelanguage.googleapis.com/v1beta/models/{바나나}:generateContent?key={apiKey}";

            string imageOutPath = $@"{dirCurr}\images\{imageOutName}";

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
                                    mime_type = "image/jpeg"
                                }
                            },
                            // 2. 수행할 명령 (텍스트)
                            new
                            {
                                text = safeInput
                            }
                        }
                    }
                }
            };

            string jsonPayload = JsonConvert.SerializeObject(jsonObj);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(300));

            // 3. PostAsync 호출
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsync(urlApi, content, cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();

                    // 4. 이미지 데이터 추출 (JSON 파싱 필요)
                    JsonDocument doc = JsonDocument.Parse(result);
                    // 응답 구조에서 base64 데이터가 들어있는 필드 찾기 (예: "imageBytes")
                    JsonElement root = doc.RootElement;

                    // Gemini API 표준 구조로 접근합니다.
                    // candidates -> [0] -> content -> parts -> [0]
                    var part = root.GetProperty("candidates")[0]
                                   .GetProperty("content")
                                   .GetProperty("parts")[0];

                    if (part.TryGetProperty("inlineData", out JsonElement inlineData))
                    {
                        string base64Image = inlineData.GetProperty("data").GetString();
                        byte[] imageBytes = Convert.FromBase64String(base64Image);

                        checkSameNameImage(imageOutPath);
                        File.WriteAllBytes(imageOutPath, imageBytes);
                        return $"이미지 저장 완료! :\r\n  {imageOutPath}";
                    }
                    // 3. 만약 텍스트로 답이 온 경우 (에러 메시지 확인용)
                    else if (part.TryGetProperty("text", out JsonElement textContent))
                    {
                        return $"이미지가 생성되지 않고 텍스트가 반환되었습니다: {textContent.GetString()}";
                    }
                    else
                    {
                        // 이 곳에 Console.WriteLine(result); 를 넣어 전체 구조를 한 번 확인하는 것이 좋습니다.
                        return "예상치 못한 응답 구조입니다.";
                    }
                    // 5. 파일로 저장
                }
                else
                {
                    Console.WriteLine($"에러 발생: {response.StatusCode}");
                    return $"에러 발생:\r\n {response.StatusCode}";
                }

                // 1. Root에서 candidates 접근
            
            }
        }
        void checkSameNameImage(string imageOutPath)
        {
            if (File.Exists(imageOutPath))
            {
                var name = Path.GetFileNameWithoutExtension(imageOutPath);
                var ext = Path.GetExtension(imageOutPath);
                var dir = Path.GetDirectoryName(imageOutPath);

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                var imageNew = $@"{dir}\{name}_{timestamp}{ext}";
                File.Move(imageOutPath, imageNew);
            }
        }
        public byte[] ResizeImageToBytes(string imagePath, int targetWidth)
        {
            using (System.Drawing.Image source = System.Drawing.Image.FromFile(imagePath))
            {
                // 1. 비율에 맞춘 높이 계산
                int targetHeight = (source.Height * targetWidth) / source.Width;

                // 2. 24비트 포맷의 새 비트맵 생성
                using (Bitmap newBmp = new Bitmap(targetWidth, targetHeight, PixelFormat.Format24bppRgb))
                {
                    using (Graphics g = Graphics.FromImage(newBmp))
                    {
                        // 이미지를 깔끔하게 그리기 위한 기본 설정
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(source, 0, 0, targetWidth, targetHeight);
                    }

                    // 3. 메모리 스트림에 JPEG 포맷으로 저장 후 바이트 리턴
                    using (MemoryStream ms = new MemoryStream())
                    {
                        newBmp.Save(ms, ImageFormat.Jpeg);
                        return ms.ToArray();
                    }
                }
            }
        }
        public void ResizeImage(string imageSrc, string imageDst, int targetWidth)
        {
            using (System.Drawing.Image source = System.Drawing.Image.FromFile(imageSrc))
            {
                // 1. 비율에 맞춘 높이 계산
                int targetHeight = (source.Height * targetWidth) / source.Width;

                // 2. 24비트 포맷의 새 비트맵 생성
                using (Bitmap newBmp = new Bitmap(targetWidth, targetHeight, PixelFormat.Format24bppRgb))
                {
                    using (Graphics g = Graphics.FromImage(newBmp))
                    {
                        // 이미지를 깔끔하게 그리기 위한 기본 설정
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(source, 0, 0, targetWidth, targetHeight);
                    }

                    newBmp.Save(imageDst, ImageFormat.Jpeg);
                }
            }
        }
        internal string ChangeModel()
        {
            switch (MODEL)
            {
                case 플래시:
                    MODEL = 프로;
                    break;
                case 프로:
                    MODEL = 바나나;
                    break;
                case 바나나:
                    MODEL = 플래시;
                    break;
                default:
                    MODEL = 플래시;
                    break;
            }

            URI = $"https://generativelanguage.googleapis.com/v1beta/models/{MODEL}:generateContent?key={apiKey}";
            return "모델변경 : " + MODEL;
        }
    }
}
/*

gemini-3-pro-preview
gemini-3-pro-image-preview
gemini-3-flash-preview

text/plain, image/jpeg, image/png

*/