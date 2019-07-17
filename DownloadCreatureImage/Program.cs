using QuickType;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace DownloadCreatureImage
{
    class Program
    {
        static void Main(string[] args)
        {
            int pageNum = 0;
            string url = $"https://api.scryfall.com/cards/search?q=t:creature&order=cmc&page={pageNum}";
            bool hasMore = true;
            int cardCmcIndex = 1;
            int currentCmc = 0;
            Console.WriteLine("Please give the filepath of the parent folder where images will be saved. (C:/images/)");
            Console.Write(">");
            string rootpath = Console.ReadLine();
            for (int i = 0; i <= 15; i++)
            {
                System.IO.Directory.CreateDirectory(rootpath + i);
            }
            while (hasMore)
            {
                string responseString = "";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(responseStream)) responseString = reader.ReadToEnd();
                dynamic parsedResponse = JsonConvert.DeserializeObject(responseString);
                for (int i = 0; i < parsedResponse["data"].Count; i++) 
                {
                    dynamic card = parsedResponse["data"][i];
                    if (card.image_uris == null || card.set_type == "funny") continue;
                    if (card.cmc != currentCmc)
                    {
                        cardCmcIndex = 1;
                        currentCmc = card.cmc;
                    }
                    string uri = card.image_uris.png;
                    Console.WriteLine($"Downloading {card.name}");
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(new Uri(uri), $"{rootpath}{currentCmc}/{cardCmcIndex}.png");
                    }
                    cardCmcIndex++;
                }
                hasMore = parsedResponse.has_more;
                pageNum++;
            }
        }
    }
}
