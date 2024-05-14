using System.Collections;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class Program
{
   
    static string Reference = "";
    static string CookieValue = "";
    static string CourseId = "";

    static bool changed = false;

    static async Task Main(string[] args)
    {
        while (true) 
        {
            if (Reference.Length <=1)
            {
                Console.WriteLine("请输入Reference：");
                Reference = Console.ReadLine();
            }
            if (CookieValue.Length <= 1 ) 
            {
                Console.WriteLine("请输入Cookie：");
                CookieValue = Console.ReadLine();
            }

            Console.WriteLine("请输入CourseId：");
            CourseId = Console.ReadLine();

            while (true) 
            {
                
                try
                {
                    UpdateRecord(GetBodyInfo());
                    changed = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine("配置错误请重新输入配置信息");
                    break;

                }

                Console.WriteLine("本课程已结束，如果课程进度没有变化请关闭该软件重新输入Refer和Cookie");
                Console.WriteLine("请输入新的CourseId");
                CourseId = Console.ReadLine();

            }
           
        }
        

    }

    static string GetSourceId() 
    {
        var handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip
        };
        // 创建 HttpClient 实例
        using (HttpClient client = new HttpClient(handler))
        {
            string url = $"https://v4.21tb.com/els/html/course/course.getCourseInfoJson.do?courseId={CourseId}";
            // 设置请求头
            client.DefaultRequestHeaders.Add("Host", "v4.21tb.com");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Chromium\";v=\"124\", \"Microsoft Edge\";v=\"124\", \"Not-A.Brand\";v=\"99\"");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("DNT", "1");
            client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36 Edg/124.0.0.0");
            client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
            client.DefaultRequestHeaders.Add("Referer", Reference);
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br, zstd");
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6");
            client.DefaultRequestHeaders.Add("Cookie", CookieValue);

            // 发送 GET 请求
            HttpResponseMessage response = client.GetAsync(url).Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            JObject jsonObject = JObject.Parse(responseBody);
            //Console.WriteLine("GetSourceId: " + jsonObject);

            if (!changed) 
            {
                return (string)jsonObject["bizResult"]["courseDetail"]["courseId"];
            }
            return (string)jsonObject["bizResult"]["sourceCourseId"];
        }
    }

    public static string GetBizCourseId() 
    {
        var handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip
        };
        using (var httpClient = new HttpClient(handler))
        {
            var url = "https://v4.21tb.com/tbc-rms/course/showCourseSettingConfig";
            var bodyContent = new
            {
                courseId = CourseId,
                sourceId = GetSourceId(),
                providerCorpCode = "default"
            };
            //Console.WriteLine("SourceId: "+bodyContent.sourceId);
            string jsonContent = JsonConvert.SerializeObject(bodyContent);
            HttpContent content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Add("Host", "v4.21tb.com");
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            httpClient.DefaultRequestHeaders.Add("sec-ch-ua", "\"Chromium\";v=\"124\", \"Microsoft Edge\";v=\"124\", \"Not-A.Brand\";v=\"99\"");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("-local", "zh_CN");
            httpClient.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36 Edg/124.0.0.0");
            httpClient.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
            httpClient.DefaultRequestHeaders.Add("Origin", "https://v4.21tb.com");
            httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
            httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
            httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
            httpClient.DefaultRequestHeaders.Add("Referer", Reference);
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br, zstd");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8,en-US;q=0.7,ja;q=0.6");
            httpClient.DefaultRequestHeaders.Add("Cookie", CookieValue);

            var response = httpClient.PostAsync(url, content).Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            //Console.WriteLine(responseBody);
            JObject jsonObject = JObject.Parse(responseBody);
            //Console.WriteLine(jsonObject.ToString());           
            // 获取特定字段的值         
            if (((string)jsonObject["bizResult"]["settingId"]) == null) 
            {
                changed = true;
                bodyContent = new
                {
                    courseId = CourseId,
                    sourceId = GetSourceId(),
                    providerCorpCode = "default"
                };
                //Console.WriteLine("SourceId222222: " + bodyContent.sourceId);
                jsonContent = JsonConvert.SerializeObject(bodyContent);
                content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                response = httpClient.PostAsync(url, content).Result;
                responseBody = response.Content.ReadAsStringAsync().Result;
                //Console.WriteLine(responseBody);
                jsonObject = JObject.Parse(responseBody);
                
            }
            //Console.WriteLine("showCourseSettingConfig Request: " + jsonContent);
            //Console.WriteLine("showCourseSettingConfig Respons: "+responseBody);

            return (string)jsonObject["bizResult"]["courseId"];
        }

    }

    private static JObject GetBodyInfo() 
    {
        //showChapter request to get chapterid and resourceid
        var handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip
        };
        // 创建一个HttpClient实例
        using (HttpClient client = new HttpClient(handler))
        {
            // 创建请求的内容
            var bodyContent = new
            {
                //courseId = "69ad5e1181964414ad6f175355fa610b",
                courseId = GetBizCourseId(),
                providerCorpCode = "default"
            };
            string jsonContent = JsonConvert.SerializeObject(bodyContent);
            HttpContent content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            // 设置请求头
            client.DefaultRequestHeaders.Add("Host", "v4.21tb.com");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Chromium\";v=\"124\", \"Microsoft Edge\";v=\"124\", \"Not-A.Brand\";v=\"99\"");
            client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("-local", "zh_CN");
            client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36 Edg/124.0.0.0");
            client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
            client.DefaultRequestHeaders.Add("Origin", "https://v4.21tb.com");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
            client.DefaultRequestHeaders.Add("Referer", Reference);
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br, zstd");
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8,en-US;q=0.7,ja;q=0.6");
            client.DefaultRequestHeaders.Add("Cookie", CookieValue);
            Console.WriteLine("config done");
            
            
            HttpResponseMessage response = client.PostAsync("https://v4.21tb.com/tbc-rms/course/showCourseChapter", content).Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            JObject jsonObject = JObject.Parse(responseBody);
            Console.WriteLine("第一次showCourseChapter Request: " + jsonContent);
            Console.WriteLine("第一次showCourseChapter Respons: " + responseBody);
            //偶尔用showCourseSettingConfig找到得CourseId是无法返回正确chapter内容的，还要用回最初得CourseId。很小概率会有这个问题
            Console.WriteLine(jsonObject["bizResult"].Count());
            if (jsonObject["bizResult"].Count() == 0)
            {
                var bodyContent2 = new
                {
                    courseId = CourseId,
                    providerCorpCode = "default"
                };
                jsonContent = JsonConvert.SerializeObject(bodyContent2);
                content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                response = client.PostAsync("https://v4.21tb.com/tbc-rms/course/showCourseChapter", content).Result;
                responseBody = response.Content.ReadAsStringAsync().Result;
                jsonObject = JObject.Parse(responseBody);
            }

            // 获取特定字段的值
            Console.WriteLine("第二次showCourseChapter Request: " + jsonContent);
            Console.WriteLine("第二次showCourseChapter Respons: " + responseBody);

            return jsonObject;



        }
    }

    private static async void UpdateRecord(JObject job ) 
    {
        

        var handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip
        };
        // 创建一个HttpClient实例
        using (HttpClient client = new HttpClient(handler))
        {
            ArrayList a = new ArrayList();
            
            // 创建请求的内容
            foreach (var item in job["bizResult"]) 
            {

               
                foreach (var item2 in item["resourceDTOS"]) 
                {
                    Random rd = new Random();
                    a.Add(
                       new
                       {
                           recordId = (object)null,
                           courseId = CourseId,
                           sourceId = GetSourceId(),
                           providerCorpCode = "default",
                           chapterId = item["chapterId"],
                           resourceId = item2["resourceId"],
                           timeToFinish = item2["playTime"],
                           currentPosition = rd.Next(1, int.Parse((string)item2["playTime"])),
                           type = "video",
                           currentStudyTime = item2["playTime"],
                           pageIndex = 0,
                           platform = "PC"
                       }
                       );
                } 
                    
            }

            // 设置请求头...
            client.DefaultRequestHeaders.Add("Host", "v4.21tb.com");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Chromium\";v=\"124\", \"Microsoft Edge\";v=\"124\", \"Not-A.Brand\";v=\"99\"");
            client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("-local", "zh_CN"); // 注意这里的请求头名是 "-local"，以减号开头需要特别处理
            client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36 Edg/124.0.0.0");
            client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
            client.DefaultRequestHeaders.Add("Origin", "https://v4.21tb.com");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
            client.DefaultRequestHeaders.Add("Referer", Reference);
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br, zstd");
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8,en-US;q=0.7,ja;q=0.6");
            client.DefaultRequestHeaders.Add("Cookie", CookieValue);

            // 其余请求头...

            // 发送请求
            int i = 1;
            foreach (var item in a) 
            {
                string jsonContent = JsonConvert.SerializeObject(item);
                StringContent content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response =  client.PostAsync("https://v4.21tb.com/tbc-rms/record/updateCourseRecord", content).Result;
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"第{i}次请求");
                Console.WriteLine("Request: "+jsonContent);
                Console.WriteLine("Response: " + responseBody);
                Console.WriteLine();
                i++;
                Thread.Sleep(TimeSpan.FromSeconds(0.3));
            }
            
        }
    }
}