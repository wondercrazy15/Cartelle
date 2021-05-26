using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using PCLStorage;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;
using Xamarin.Forms;
using ModernHttpClient;

namespace Stance.Utils
{
    public class DownloadResources
    {
        String _txt = String.Empty;
        String _fileUrlToDownload = "http://montemagno.com/monkeys.json";
        String _dropZipPathAndFileName = "";
        string _folderPath = "";

        public string _url;

        public async Task ReadFileAsync()
        {
            //String fileLocation = "http://vjs.zencdn.net/v/oceans.mp4";
            //String fileLocation = "http://stanceathletes.com/images/api/1_Account/1_Program/1_Workout/3_Exercise/jumping_jacks_mini_loop.mp4";

            //String diskLocation = @"C:\Download_Testing\oceans.mp4";

            //var url = "http://stanceathletes.com/api/Athletes";

            var client = new HttpClient(new NativeMessageHandler());

            client.BaseAddress = new Uri(_fileUrlToDownload);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync("").Result;

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                _txt = json;
            }
        }


        public void InitiateVideoDownload()
        {
            var t = new Task(ReadVideoAsyncTwo);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            t.Start();
            t.Wait();
            sw.Stop();
            var timeElaps = sw.ElapsedMilliseconds;
        }




        private async static void ReadVideoAsyncThree()
        {
            using (HttpClient client = new HttpClient(new NativeMessageHandler()))
            {
                 string _video1 = "https://res.cloudinary.com/stance/video/upload/v1485929725/720p_2mbps.mp4";
                 string _video2 = "http://vjs.zencdn.net/v/oceans.mp4";

                var array = new string[2];
                array[0] = _video1;
                array[1] = _video2;
                var i = 0;

                foreach(var item in array)
                {
                    IFolder rootFolder = FileSystem.Current.LocalStorage;
                    IFolder folder = await rootFolder.CreateFolderAsync("Programs", CreationCollisionOption.OpenIfExists);
                    IFolder subFolder = await folder.CreateFolderAsync("progID", CreationCollisionOption.OpenIfExists);
                    IFolder subSubFolder = await subFolder.CreateFolderAsync("workoutID", CreationCollisionOption.OpenIfExists);
                    var filename = "";
                    if (i == 0)
                    {
                        filename = "video1.mp4";
                    }
                    else
                    {
                        filename = "video2.mp4";
                    }
                    i++;

                    IFile file = await subSubFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

                    using (HttpResponseMessage response = await client.GetAsync(item, HttpCompletionOption.ResponseHeadersRead))
                    {
                        using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                        {
                            using (Stream streamToWriteTo = await file.OpenAsync(FileAccess.ReadAndWrite))
                            {
                                await streamToReadFrom.CopyToAsync(streamToWriteTo);
                            }
                        }
                    }

                }

            }
        }


        private static async void ReadVideoAsyncTwo()
        {
            int offset = 0;
            long? streamLength = 0;
            var result = new List<byte>();
            var responseBuffer = new byte[500];

            using (HttpClient client = new HttpClient(new NativeMessageHandler()))
            {
                const string url = "http://vjs.zencdn.net/v/oceans.mp4";
                using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    streamLength = response.Content.Headers.ContentLength;

                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                    {
                        int read;

                        do
                        {
                            read = await streamToReadFrom.ReadAsync(responseBuffer, 0, responseBuffer.Length);
                            result.AddRange(responseBuffer);
                            offset += read;
                            var progress = 0; ;

                            if (streamLength != null && streamLength != 0)
                            {
                                progress = (offset / (int)streamLength) * 100;
                            }

                            MessagingCenter.Send(new DownloadResources(), "DownloadValueChanged", progress);
                            // here I want to send percents of downloaded data
                            // offset / (totalSize / 100)

                        } while (read != 0);

                        IFolder rootFolder = FileSystem.Current.LocalStorage;
                        IFile file = await rootFolder.CreateFileAsync("oceans.mp4", CreationCollisionOption.ReplaceExisting);

                        var res = result.ToArray();

                        using (Stream streamToWriteTo = await file.OpenAsync(FileAccess.ReadAndWrite))
                        {
                            for (int i = 0; i < res.Length; i++)
                            {
                                streamToWriteTo.WriteByte(res[i]);
                            }

                            streamToWriteTo.Seek(0, SeekOrigin.Begin);

                            var _file = file;

                        }
                    }
                }
            }
        }


        public async Task PCLStorageSample()
        {
            await ReadFileAsync();
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync("MySubFolder", CreationCollisionOption.OpenIfExists);
            IFile file = await folder.CreateFileAsync("answer.txt", CreationCollisionOption.ReplaceExisting);
            await file.WriteAllTextAsync(_txt);
            var txt = await file.ReadAllTextAsync();
            var i = txt;

        }

        public async Task HttpTesting()
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://google.com");
            request.Method = HttpMethod.Get;
            request.Headers.Add("Accept", "application/json");

            var client = new HttpClient(new NativeMessageHandler());
            //client.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpResponseMessage response = await client.SendAsync(request); //Send is the most powerful method over GET, POST, PUT, DELETE because you are not setting the headers in the latter
         
            if(response.StatusCode == HttpStatusCode.OK)
            {
                HttpContent content = response.Content;
                var json = await content.ReadAsStringAsync();

             }

        }


        public async Task HttpTesting2()
        {

            //sending data
            //Types: byte array (10101010111) - ByteArrayContent - GetByteArrayAsync, stream (image)-StreamContent - GetStreamAsync, string (json)-StringContent-GetStringAsync
            var downloadclient = new HttpClient(new NativeMessageHandler());
            string data = await downloadclient.GetStringAsync("http://google.com/");


            //upload
            var product = new Product() { id = 1 };
            string json = JsonConvert.SerializeObject(product);
            var contentString = new StringContent(json);

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://google.com");
            request.Method = HttpMethod.Post;
            request.Content = contentString;

            var uploadClient = new HttpClient(new NativeMessageHandler());
            HttpResponseMessage response = await uploadClient.SendAsync(request);           

        }

        public async Task HttpTesting3()
        {

            var config = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                UseProxy = true,
                AutomaticDecompression = DecompressionMethods.GZip,
                Credentials = new NetworkCredential("username", "password"),
            };

            var client = new HttpClient(config);

        }


        public class Product
        {
            public int id { get; set; }
        }

        public void asdad()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            using (var client = new HttpClient(handler))
            {
                // your code
            }
        }




    }
}
