using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;

namespace TelegramBot.Model
{
    class Rhymer
    {
        public static ICollection<string> RhymeWord(string input)
        {
            try
            {
                string response = DownloadRifma(input);
                return HttpParser(response);
            }
            catch { return null; }
        }

        static string DownloadRifma(string input)
        {
            string temp = "";

            using (var client = new WebClient())
            {
                temp = client.DownloadString("https://rifma-online.ru/rifma/" + input + "/");
            }
            return temp;
        }

        static ICollection<string> HttpParser(string response)
        {
            const string listStartElement = "<li><a href=\"/rifma/";
            List<string> list = new List<string>();

            while (response.Contains(listStartElement))
            {
                response = response.Remove(0, response.IndexOf(listStartElement) + listStartElement.Length);

                string temp = response.Substring(0, response.IndexOf(@"/"));

                list.Add(HttpUtility.UrlDecode(temp));

            }

            return list;
        }
    }
}
