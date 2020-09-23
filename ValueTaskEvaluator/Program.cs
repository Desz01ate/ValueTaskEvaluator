using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ValueTaskEvaluator
{
    [MemoryDiagnoser]
    [IterationCount(100)]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class TasksBenchmark
    {
        private readonly Dictionary<string, string> cache;
        private readonly HttpClient httpClient;
        public IEnumerable<(string, string)> dataset => new (string, string)[] {
            ("taylorswift","red"),
            ("queen","bohemianrhapsody"),
            ("westlife","mylove"),
            ("taylorswift","backtodecember"),
            ("eagles","hotelcalifornia"),
            ("undefined","null"),
            ("taylorswift","red"),
        };

        [ParamsSource(nameof(dataset))]
        public (string, string) data { get; set; }
        public TasksBenchmark()
        {
            this.cache = new Dictionary<string, string>();
            this.httpClient = new HttpClient();
        }

        [Benchmark]
        public async ValueTask<string> ValueTask_WithCache_RunnerAsync()
        {
            var artist = data.Item1;
            var song = data.Item2;
            var key = $"{artist}_{song}";
            if (cache.TryGetValue(key, out var lyricsCache))
            {
                return lyricsCache;
            }
            var url = $"https://www.azlyrics.com/lyrics/{artist}/{song}.html";
            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return "";

            var html = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var lyrics = doc.DocumentNode.SelectNodes("//div").OrderByDescending(x => x.InnerText.Length).Select(x => x.InnerText).FirstOrDefault();
            lyrics = lyrics.Trim();
            cache.Add(key, lyrics);
            return lyrics ?? "";
        }

        [Benchmark]
        public async Task<string> Task_WithCache_RunnerAsync()
        {
            var artist = data.Item1;
            var song = data.Item2;
            var key = $"{artist}_{song}";
            if (cache.TryGetValue(key, out var lyricsCache))
            {
                return lyricsCache;
            }
            var url = $"https://www.azlyrics.com/lyrics/{artist}/{song}.html";
            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return "";

            var html = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var lyrics = doc.DocumentNode.SelectNodes("//div").OrderByDescending(x => x.InnerText.Length).Select(x => x.InnerText).FirstOrDefault();
            lyrics = lyrics.Trim();
            cache.Add(key, lyrics);
            return lyrics ?? "";
        }

        [Benchmark]
        public async ValueTask<string> ValueTask_NoCache_RunnerAsync()
        {
            var artist = data.Item1;
            var song = data.Item2;
            var url = $"https://www.azlyrics.com/lyrics/{artist}/{song}.html";
            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return "";

            var html = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var lyrics = doc.DocumentNode.SelectNodes("//div").OrderByDescending(x => x.InnerText.Length).Select(x => x.InnerText).FirstOrDefault();
            lyrics = lyrics.Trim();
            return lyrics ?? "";
        }

        [Benchmark]
        public async Task<string> Task_NoCache_RunnerAsync()
        {
            var artist = data.Item1;
            var song = data.Item2;
            var url = $"https://www.azlyrics.com/lyrics/{artist}/{song}.html";
            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return "";

            var html = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var lyrics = doc.DocumentNode.SelectNodes("//div").OrderByDescending(x => x.InnerText.Length).Select(x => x.InnerText).FirstOrDefault();
            lyrics = lyrics.Trim();
            return lyrics ?? "";
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<TasksBenchmark>();
            //test Naja
        }
    }
}
