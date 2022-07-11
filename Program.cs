using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace PixabayAPI
{
    public class PixabaySearchResults
    {
        public int total { get; set; }
        public int totalHits { get; set; }
        public PixabaySearchHits[] hits { get; set; }
    }
    public class PixabaySearchHits
    {
        public int id { get; set; }
        public string userImageURL { get; set; }
        public string type { get; set; }
        public string tags { get; set; }
    }



    public class Program
    {
        public static void Main()
        {
            WebClient wc = new WebClient();

            var result = JsonSerializer.Deserialize<PixabaySearchResults>(wc.DownloadString("https://pixabay.com/api/?key=28501108-d97b0f7079e828f6078c1e754&q=yellow+flowers&image_type=photo&pretty=true"));

            foreach(var hit in result.hits)
            {
                Console.WriteLine($"id: {hit.id}");
                Console.WriteLine($"URL: {hit.userImageURL}");
                Console.WriteLine($"type: {hit.type}");
                Console.WriteLine($"tags: {hit.tags}");
                Console.WriteLine($"-----------------------");
            }
        }
    }
}