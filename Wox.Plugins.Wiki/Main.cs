using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json.Linq;
using Wox.Plugin;

namespace Wox.Plugins.Wiki
{
    public class Main : IPlugin
    {
        private static readonly string Url = $"https://{Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName}.wikipedia.org";

        public List<Result> Query(Query query)
        {
            var raw = query.Search;
            using (var wc = new WebClient())
            {
                var json =
                    wc.DownloadString(
                        $"{Url}/w/api.php?format=json&action=query&prop=extracts&exintro=&explaintext=&titles={raw}");
                var obj = JObject.Parse(json);
                var id = (string) obj["query"]["pages"].First.First["pageid"];
                var title = (string) obj["query"]["pages"].First.First["title"];
                var description = ((string) obj["query"]["pages"].First.First["extract"]).Substring(0, 130).Replace("\n", " ") + "...";
                return new List<Result>
                {
                    new Result
                    {
                        Title = title,
                        SubTitle = description,
                        IcoPath = "icon.png",
                        Action = ctx =>
                        {
                            Process.Start($"{Url}/?curid={id}");
                            return true;
                        }
                    }
                };
            }
        }

        public void Init(PluginInitContext context)
        {
        }
    }
}