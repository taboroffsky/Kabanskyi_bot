using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Configuration;
using System.IO;
using TelegramBot.Model;
using System.Threading;

namespace TelegramBot
{
    class Controller
    {
        public Controller()
        {  
            lastUpdateId = Convert.ToInt32(ConfigurationManager.AppSettings["LastUpdateId"]);
        }
      
        int lastUpdateId;




        public void GetUpdates()
        {
            using (var client = new WebClient())
            {
                string response = client.DownloadString("https://api.telegram.org/bot" + BasicInformation.Token + "/getupdates" + "?offset=" + (lastUpdateId + 1));
                if (response.Length < 25) return;
                SaveLastUpdateIdToConfig(lastUpdateId);

                var resultNode = JSON.Parse(response);

                foreach (JSONNode node in resultNode["result"].AsArray)
                {
                    lastUpdateId = node["update_id"].AsInt;

                    string message = node["message"]["text"];
                    int messageId = node["message"]["message_id"];
                    int clientId = node["message"]["chat"]["id"];                    
                    string clientName = node["message"]["chat"]["first_name"];
                    string language = node["message"]["from"]["language_code"];

                    if(message == "/start")
                        Console.WriteLine("start!!!!!!!");
                    else
                    SendRhymeCollectionAsync(clientId, message, messageId);
                    
                   

                    Console.WriteLine("{0}, {1}, {2}, {3}, {4}", clientId, clientName, messageId, message, language);
                }

            }
        }

        public void SendMessage(int clientId, string message)
        {
            using (var client = new WebClient())
            {
                var collection = new NameValueCollection();
                collection.Add("text", message);
                collection.Add("chat_id", clientId.ToString());

                client.UploadValues("https://api.telegram.org/bot" + BasicInformation.Token + "/sendMessage", collection);
            }
        }   

        public void EditMessageText(int clientId, int messageId, string newText)
        {
            using (var client = new WebClient())
            {
                var collection = new NameValueCollection();

                collection.Add("chat_id", clientId.ToString());
                collection.Add("message_id", (messageId+1).ToString());                
                collection.Add("text", newText);

                client.UploadValues("https://api.telegram.org/bot" + BasicInformation.Token + "/editMessageText", collection);
            }
        }

        async public void SendRhymeCollectionAsync(int clientId, string input, int messageId)
        {
            Action<object> method = SendRhymeCollection;
            var parameter = new { clientId = clientId, input = input, messageId = messageId };
           
            await Task.Factory.StartNew(method, parameter);          
            
        }




        private void SendRhymeCollection(dynamic input)
        {
            int clientId = input.clientId;
            int messageId = input.messageId;
            string text = input.input;

            SendMessage(clientId, "Римую,чекай)");

            ICollection<string> collection = Rhymer.RhymeWord(text);            

            foreach (string item in collection)
            {
                EditMessageText(clientId, messageId, item);
                Thread.Sleep(1000);
            }
        }

        private void SaveLastUpdateIdToConfig(int lastUpdateId)
        {
            Configuration currentConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            currentConfig.AppSettings.Settings["LastUpdateId"].Value = (lastUpdateId + 1).ToString();           
            currentConfig.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

        }
    }
}
