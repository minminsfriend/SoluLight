using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpToken;
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
//using static System.Net.WebRequestMethods;
//using static System.Net.WebRequestMethods;

namespace Geminapi
{
    public class KPostAsync
    {
        readonly string apiKey = "AIzaSyDOvjo36moasktvpyYoqGBQrWTvCSouWjc";

        const string 플래시 = "gemini-3-flash-preview";
        const string 프로 = "gemini-3-pro-preview";
        const string 바나나 = "gemini-3-pro-image-preview";

        HttpClient client;

        //string URI;
        string fileCurr;
        //string cacheId;
        //public string MODEL = 플래시;
        public bool OnSending = false;
        public KPostAsync()
        {
            client = new System.Net.Http.HttpClient();
        }
        string getMODEL(string _model)
        {
            var model = 플래시;

            switch (_model)
            {
                case "플래시": model = 플래시; break;
                case "프로": model = 프로; break;
                case "바나나": model = 바나나; break;
            }

            return model;
        }
        string preJson2Json(string text4Send, List<string> files, List<string> serverfileIds, string cashId)
        {
            if (cashId != null)
            {
                var jsonObj = new
                {
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

                    cached_content = $"{cashId}"
                };

                return JsonConvert.SerializeObject(jsonObj);
            }
            else if (serverfileIds != null)
            {
                List<string> fileUris = new List<string>();
                foreach (var fid in serverfileIds)
                    fileUris.Add($"https://generativelanguage.googleapis.com/v1beta/{fid}");

                List<dynamic> fileParts = fileUris.Select(fileUri => new
                {
                    file_data = new
                    {
                        file_uri = fileUri,
                        mime_type = "image/jpeg"
                    }
                }).ToList<dynamic>();

                fileParts.Add(new { text = text4Send });

                var jsonObj = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = fileParts.ToArray()
                        }
                    }
                };

                return JsonConvert.SerializeObject(jsonObj);
            }
            else if (files != null)
            {
                var parts = files.Select(filePath => {
                    string ext = Path.GetExtension(filePath).ToLower();

                    // 확장자에 따른 MIME 타입 설정
                    string mimeType = ext switch
                    {
                        ".jpg" => "image/jpeg",
                        ".jpeg" => "image/jpeg",
                        ".png" => "image/png",
                        ".txt" => "text/plain",
                        _ => "application/octet-stream" // 기본값
                    };

                    return new
                    {
                        inline_data = new
                        {
                            mime_type = mimeType,
                            data = Convert.ToBase64String(File.ReadAllBytes(filePath))
                        }
                    };
                }).Cast<object>().ToList();

                // 마지막에 텍스트 메시지 추가
                parts.Add(new { text = text4Send });

                var jsonObj = new
                {
                    contents = new[] {
                        new { parts = parts.ToArray() }
                    }
                };

                return JsonConvert.SerializeObject(jsonObj);
            }
            else
            {
                var jsonObj = new
                {
                    contents = new[]
                    {
                            new
                            {
                                parts = new object[]
                                {
                                    new { text = text4Send }
                                }
                            }
                    }
                };
                return JsonConvert.SerializeObject(jsonObj);
            }
        }
        public async Task<string> SendDialog(string text4Send, List<string> files, List<string> idsInServer, string cashId, string _model)
        {
            var MODEL = getMODEL(_model);
            var URI = $"https://generativelanguage.googleapis.com/v1beta/models/{MODEL}:generateContent?key={apiKey}";

            var json = preJson2Json(text4Send, files, idsInServer, cashId);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(300));

            var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var res = await client.PostAsync(URI, content, cts.Token);
            //PostAsJsonAsync
            string result = await res.Content.ReadAsStringAsync();
            //Console.WriteLine(result);

            var jo = Newtonsoft.Json.Linq.JObject.Parse(result);
            string textChat = jo["candidates"][0]["content"]["parts"][0]["text"].ToString();
            //Console.WriteLine($"Gemini: {textChat}");
            textChat = textChat.Replace("\n", Environment.NewLine);

            return textChat;
        }
        public async Task<string> SendDialogSearch(string text4Send, string model)
        {
            var MODEL = getMODEL(model);
            var URI = $"https://generativelanguage.googleapis.com/v1beta/models/{MODEL}:generateContent?key={apiKey}";

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
        public async Task<string> SendDialogRefC(string text4Send, string cashid, string mime, string model)
        {
            var MODEL = getMODEL(model);
            var URI = $"https://generativelanguage.googleapis.com/v1beta/models/{MODEL}:generateContent?key={apiKey}";

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
                //cached_content = "cachedContents/u9wxtnwplh3cd4u76dyv6rx7lc0p5gnv3ksiy72e"
                cached_content = $"{cashid}"
            };

            var json = JsonConvert.SerializeObject(jsonObj);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(300));

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await client.PostAsync(URI, content, cts.Token);

            string result = await res.Content.ReadAsStringAsync();

            var jo = Newtonsoft.Json.Linq.JObject.Parse(result);
            string chatResponse = jo["candidates"][0]["content"]["parts"][0]["text"].ToString();

            chatResponse = chatResponse.Replace("\n", Environment.NewLine);

            return chatResponse;
        }
        public async Task<string> UploadServerFile00(string filePath, string displayName, string _MIME_)
        {
            string urlUp = $"https://generativelanguage.googleapis.com/upload/v1beta/files?key={apiKey}";

            client.DefaultRequestHeaders.Add("X-Goog-Upload-Protocol", "multipart");

            using (var multipart = new MultipartFormDataContent())
            {
                var metadata = new { file = new { display_name = displayName } };
                var metadataContent = new StringContent(JsonConvert.SerializeObject(metadata), Encoding.UTF8, "application/json");
                metadataContent.Headers.ContentType.CharSet = null;

                multipart.Add(metadataContent, "metadata");

                var fileBytes = File.ReadAllBytes(filePath);
                var fileContent = new ByteArrayContent(fileBytes);

                fileContent.Headers.ContentType = new MediaTypeHeaderValue(_MIME_);
                multipart.Add(fileContent, "file");

                // 3. 전송
                var response = await client.PostAsync(urlUp, multipart);
                var result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("업로드 성공!");
                    //Console.WriteLine(result);
                }
                else
                {
                    Console.WriteLine($"실패 상태코드: {response.StatusCode}");
                    Console.WriteLine($"실패 원인: {result}");
                }

                return result;
            }
        }
        public async Task<string> UploadFile2Server(string filePath, string displayName, string _MIME_)
        {
            string urlUp = $"https://generativelanguage.googleapis.com/upload/v1beta/files?key={apiKey}";

            using (var request = new HttpRequestMessage(HttpMethod.Post, urlUp))
            using (var multipart = new MultipartFormDataContent())
            {
                var metadataJson = JsonConvert.SerializeObject(new { file = new { display_name = displayName } });
                var metadataContent = new ByteArrayContent(Encoding.UTF8.GetBytes(metadataJson));
                metadataContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                multipart.Add(metadataContent, "metadata");

                var fileBytes = File.ReadAllBytes(filePath);
                var fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(_MIME_);
                multipart.Add(fileContent, "file");

                request.Content = multipart;
                request.Headers.Add("X-Goog-Upload-Protocol", "multipart");

                var response = await client.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("업로드 성공!");
                    //Console.WriteLine(result);
                }
                else
                {
                    Console.WriteLine($"실패 상태코드: {response.StatusCode}");
                    Console.WriteLine($"실패 원인: {result}");
                }

                return result;
            }
        }
        public async Task<string> DeleteServerFile(string fileId)
        {
            string urlFile = $"https://generativelanguage.googleapis.com/v1beta/{fileId}?key={apiKey}";

            // DELETE 요청 전송
            var response = await client.DeleteAsync(urlFile);
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                //Console.WriteLine($"삭제 성공, fileId : {fileId}");
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
        public async Task<string> DeleteContextCache(string cacheId)
        {
            string urlCache = $"https://generativelanguage.googleapis.com/v1beta/{cacheId}?key={apiKey}";

            // DELETE 요청 전송
            var response = await client.DeleteAsync(urlCache);
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"캐시 삭제 성공, cacheId : {cacheId}");
                return $"캐시 삭제 성공, cacheId : {cacheId}";
            }
            else
            {
                Console.WriteLine($"캐시 삭제 실패: {response.StatusCode}");
                Console.WriteLine($"원인: {result}");
                return $"캐시 삭제 실패:\r\n {response.StatusCode}\r\n원인:\r\n {result}";
            }
        }
        public async Task<string> CreateContextCache(string fileId, string model)
        {
            var MODEL = getMODEL(model);
            var fileUri = $"https://generativelanguage.googleapis.com/v1beta/{fileId}";

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
                                        //mime_type = "text/plain",
                                        mime_type = "image/jpeg",
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
        public async Task<string> CreateContextCache(List<string> fileIds, string model, string _displayName)
        {
            var MODEL = getMODEL(model);

            Console.WriteLine($"MODEL {MODEL}");

            List<string> fileUris = fileIds
                .Select(fid => $"https://generativelanguage.googleapis.com/v1beta/{fid}")
                .ToList();

            List<dynamic> fileParts = fileUris.Select(uri => new
            {
                file_data = new
                {
                    file_uri = uri,
                    mime_type = "image/jpeg"
                }
            }).ToList<dynamic>();

            var jsonData = new
            {
                model = $"models/{MODEL}",
                displayName = $"{_displayName}",
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = fileParts.ToArray()
                    }
                },
                ttl = "3600s" 
            };

            string json = JsonConvert.SerializeObject(jsonData);
            string urlC = $"https://generativelanguage.googleapis.com/v1beta/cachedContents?key={apiKey}";

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
        public async Task<string> GenerateImage(string text4Send, string fileIdRef, string imagePathOut, string model)
        {
            var MODEL = getMODEL(model);
            string urlApi = $"https://generativelanguage.googleapis.com/v1beta/models/{MODEL}:generateContent?key={apiKey}";
            var fileUri = $"https://generativelanguage.googleapis.com/v1beta/files/{fileIdRef}";

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
                                text = text4Send
                            }
                        }
                    }
                }
            };

            string jsonPayload = JsonConvert.SerializeObject(jsonObj);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(300));

            // 3. PostAsync 호출
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

                    File.WriteAllBytes(imagePathOut, imageBytes);
                    return $"이미지 저장 완료! :\r\n  {imagePathOut}";
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
        }
        public async Task<string> GetFilesListInServer()
        {
            string url = "https://generativelanguage.googleapis.com/v1beta/files?pageSize=30";
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("x-goog-api-key", apiKey);
            request.Headers.Add("Accept", "application/json");

            try
            {
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("--- 파일 리스트 조회 성공 ---");
                    //Console.WriteLine(result);

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
        public async Task<string> GetCashesList()
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
        public async Task<string> testSend()
        {

            using (var content = new MultipartFormDataContent())
            {
                // 1. 이미지 파일 추가 (예: "chat_image.jpg")
                var fileStream = File.OpenRead("image_path_here.jpg");
                var imageContent = new StreamContent(fileStream);
                // 서버가 기대하는 필드 이름("image")과 실제 파일명을 지정합니다.
                content.Add(imageContent, "image", "chat_image.jpg");

                // 2. 채팅 메시지(텍스트) 추가
                var messageContent = new StringContent("안녕하세요! 이미지를 보냅니다.");
                content.Add(messageContent, "message"); // 서버가 기대하는 필드 이름("message")

                // 3. POST 요청 전송
                var response = await client.PostAsync("https://your-api-url.com", content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("성공적으로 전송되었습니다.");
                }
            }

            return "";
        }
        public Dictionary<string, string> parseFileIds(string result)
        {
            // 1. 전체 텍스트에서 따옴표를 미리 제거
            string cleanText = result.Replace("\"", "");
            var lines = cleanText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            string tempName = null;
            string tempDisplayName = null;

            Dictionary<string, string> fids = new Dictionary<string, string>();

            foreach (var linex in lines)
            {
                string line = linex.Trim();

                // 2. 파일 블록 시작 감지 (객체 초기화)
                if (line.StartsWith("file: {"))
                {
                    tempName = null;
                    tempDisplayName = null;
                }
                // 3. 데이터 추출 (쉼표 제거 포함)
                else if (line.StartsWith("name:"))
                {
                    tempName = line.Split(':')[1].Replace(",", "").Trim();
                }
                else if (line.StartsWith("displayName:"))
                {
                    tempDisplayName = line.Split(':')[1].Replace(",", "").Trim();
                }

                // 4. 둘 다 찾아졌다면 딕셔너리에 삽입
                if (!string.IsNullOrEmpty(tempName) && !string.IsNullOrEmpty(tempDisplayName))
                {
                    fids[tempDisplayName] = tempName;

                    // 중복 방지를 위해 초기화
                    tempName = null;
                    tempDisplayName = null;
                }
            }

            return fids;
        }
        public Dictionary<string, string> parseCashIds(string result)
        {
            // 1. 전체 텍스트에서 따옴표를 미리 제거
            string cleanText = result.Replace("\"", "");
            var lines = cleanText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            string tempName = null;
            string tempModel = null;
            string tempDisplayName = null;

            Dictionary<string, string> cashIds = new Dictionary<string, string>();

            foreach (var linex in lines)
            {
                string line = linex.Trim();

                if (line.StartsWith("name:"))
                {
                    tempName = line.Split(':')[1].Replace(",", "").Trim();
                }
                else if (line.StartsWith("model:"))
                {
                    tempModel = line.Split(':')[1].Replace(",", "").Trim();

                    if (tempModel.Contains("image")) tempModel = "바나나";
                    else if (tempModel.Contains("pro")) tempModel = "프로";
                    else if (tempModel.Contains("flash")) tempModel = "플래시";
                }
                else if (line.StartsWith("displayName:"))
                {
                    tempDisplayName = line.Split(':')[1].Replace(",", "").Trim();

                    if (tempName != null && tempModel != null)
                        cashIds[$"{tempModel} - {tempDisplayName}"] = tempName;

                    tempName = null;
                    tempModel = null;
                    tempDisplayName = null;
                }
            }

            return cashIds;
        }
        internal void parseFileIdsAdd(string result)
        {
            // 1. 전체 텍스트에서 따옴표를 미리 제거
            string cleanText = result.Replace("\"", "");
            var lines = cleanText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            string tempName = null;
            string tempDisplayName = null;

            foreach (var linex in lines)
            {
                string line = linex.Trim();

                // 2. 파일 블록 시작 감지 (객체 초기화)
                if (line.StartsWith("file: {"))
                {
                    tempName = null;
                    tempDisplayName = null;
                }
                // 3. 데이터 추출 (쉼표 제거 포함)
                else if (line.StartsWith("name:"))
                {
                    tempName = line.Split(':')[1].Replace(",", "").Trim();
                }
                else if (line.StartsWith("displayName:"))
                {
                    tempDisplayName = line.Split(':')[1].Replace(",", "").Trim();
                }

                // 4. 둘 다 찾아졌다면 딕셔너리에 삽입
                if (!string.IsNullOrEmpty(tempName) && !string.IsNullOrEmpty(tempDisplayName))
                {
                    //pfcs[tempDisplayName].Add(tempName);
                    break;
                }
            }
        }
    }
}
