using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        private const string Token = "[TOKEN]";
        private const string Uri = "https://openapi.zalo.me/";
        public const string Version = "v2.0";
        private const string GetUser = "/oa/getfollowers?data=%7B%22offset%22%3A0%2C%22count%22%3A5%7D&access_token=";
        private const string SendSMS = "/oa/message?access_token=";
        //private const string SMSMsg = "{\"recipient\":{\"user_id\": \"[USERID]\"},\"message\": {\"text\": \"[MESSAGE]\"}}";

        static async Task Main(string[] args)
        {
            using var client = new HttpClient();

            // lấy thông tin người folow
            var result = await client.GetStringAsync($"{Uri}{Version}{GetUser}{Token}");
            Console.WriteLine(result);
            ResponseData a = JsonConvert.DeserializeObject<ResponseData>(result);

            // gửi sms
            HttpResponseMessage r;
            if (a.error == 0)
            {
                Parallel.ForEach(a.data.followers, async _user =>
                {
                    SMSMsg msg = new SMSMsg() {
                        message = new msg() { text = $"Helle {_user.user_id}, Thread Id= {Thread.CurrentThread.ManagedThreadId}" },
                        recipient = new user() { user_id = _user.user_id }
                    };
                    //.Replace("[USERID]", _user.user_id).Replace("[MESSAGE]", $"Helle {_user.user_id}, Thread Id= {Thread.CurrentThread.ManagedThreadId}");
                    Console.WriteLine("Fruit Name: {0}, Thread Id= {1}", _user.user_id, Thread.CurrentThread.ManagedThreadId);
                    r = await client.PostAsJsonAsync($"{Uri}{Version}{SendSMS}{Token}", msg);
                    if (r.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine($"OK {(await r.Content.ReadAsStringAsync())} {JsonConvert.SerializeObject(msg)}");
                    }
                    else
                    {
                        Console.WriteLine($"Fail {r.StatusCode} {JsonConvert.SerializeObject(msg)}");
                    }
                });
            }
            Console.Read();
        }
    }
    public class ResponseData
    {
        public _data data { get; set; }
        public int error { get; set; }
        public string message { get; set; }
    }
    public class user
    {
        public string user_id { get; set; }
    }
    public class msg
    {
        public string text { get; set; }
    }
    public class SMSMsg
    {
        public user recipient { get; set; }
        public msg message { get; set; }
    }
    public class _data
    {
        public int total { get; set; }
        public System.Collections.Generic.List<user> followers { get; set; }
    }

    
}

namespace SMSWithImage
{
    public class Recipient
    {
        public string user_id { get; set; }
    }

    public class Element
    {
        public string media_type { get; set; }
        public string attachment_id { get; set; }
    }

    public class Payload
    {
        public List<Element> elements { get; set; }
        public string template_type { get; set; }
    }

    public class Attachment
    {
        public Payload payload { get; set; }
        public string type { get; set; }
    }

    public class Message
    {
        public Attachment attachment { get; set; }
        public string text { get; set; }
    }

    public class Root
    {
        public Recipient recipient { get; set; }
        public Message message { get; set; }
    }
}
namespace SMS
{
    public class Recipient
    {
        public string user_id { get; set; }
    }

    public class Message
    {
        public string text { get; set; }
    }

    public class Root
    {
        public Recipient recipient { get; set; }
        public Message message { get; set; }
    }
}