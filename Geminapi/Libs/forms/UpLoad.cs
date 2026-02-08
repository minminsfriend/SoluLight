using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;

namespace Geminapi
{
    public partial class UpLoader : Form
    {
        private readonly string apiKey = "YOUR_GEMINI_API_KEY";
        private string uploadedFileUri = ""; // 서버에 캐싱된 파일의 경로

        public UpLoader()
        {
            InitializeComponent();
        }

        // 1. 파일 업로드 (서버 측 캐싱 시작)
        private async void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK) return;

            string filePath = ofd.FileName;
            string mimeType = GetMimeType(filePath);

            try
            {
                Console.WriteLine( "파일을 서버로 업로드 중...");

                using (var client = new HttpClient())
                {
                    // 1단계: 파일 메타데이터 전송 및 업로드 세션 시작
                    client.DefaultRequestHeaders.Add("X-Goog-Api-Key", apiKey);

                    var fileData = new { file = new { display_name = Path.GetFileName(filePath) } };
                    string jsonMeta = JsonSerializer.Serialize(fileData);

                    var content = new MultipartFormDataContent();

                    // 메타데이터 추가
                    var metaContent = new StringContent(jsonMeta, Encoding.UTF8, "application/json");
                    content.Add(metaContent, "metadata");

                    // 파일 바이트 추가
                    var fileBytes = File.ReadAllBytes(filePath);
                    var byteContent = new ByteArrayContent(fileBytes);
                    byteContent.Headers.ContentType = MediaTypeHeaderValue.Parse(mimeType);
                    content.Add(byteContent, "file");

                    // API 호출 (Files API)
                    var response = await client.PostAsync("https://generativelanguage.googleapis.com", content);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    using (JsonDocument doc = JsonDocument.Parse(responseBody))
                    {
                        uploadedFileUri = doc.RootElement.GetProperty("file").GetProperty("uri").GetString();
                    }

                    Console.WriteLine($"업로드 완료!\n서버 캐시 URI: {uploadedFileUri}\n(이 파일은 48시간 동안 보관됩니다.)");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("업로드 오류: " + ex.Message);
            }
        }

        // 2. 캐싱된 파일을 사용하여 질문하기
        private async void btnAsk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(uploadedFileUri))
            {
                MessageBox.Show("먼저 파일을 업로드하세요.");
                return;
            }

            try
            {
                Console.WriteLine( "Gemini가 답변 생성 중...");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("X-Goog-Api-Key", apiKey);

                    // 요청 바디 구성 (캐싱된 URI 참조)
                    var requestBody = new
                    {
                        contents = new[] {
                            new {
                                parts = new object[] {
                                    new { file_data = new { mime_type = "application/pdf", file_uri = uploadedFileUri } },
                                    new { text = "txtQuestion.Text" }
                                }
                            }
                        }
                    };

                    string jsonRequest = JsonSerializer.Serialize(requestBody);
                    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    // 2026년 표준 모델: gemini-1.5-flash
                    var response = await client.PostAsync(
                        $"https://generativelanguage.googleapis.com{apiKey}",
                        content);

                    var responseBody = await response.Content.ReadAsStringAsync();

                    // 결과 파싱
                    using (JsonDocument doc = JsonDocument.Parse(responseBody))
                    {
                        var text = doc.RootElement
                            .GetProperty("candidates")[0]
                            .GetProperty("content")
                            .GetProperty("parts")[0]
                            .GetProperty("text")
                            .GetString();
                        //txtResponse.Text = text;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("질문 중 오류: " + ex.Message);
            }
        }

        private string GetMimeType(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLower();
            switch( ext)
            {
                case ".pdf": return "application/pdf";
                case ".png": return "image/png";
                case ".jpg": return "image/jpeg";
                case ".jpeg": return "image/jpeg";
                case ".txt" : return "text/plain";
                default: return "application/octet-stream";
            }
        }
    }
}
