using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using System.Globalization;
using System.Text;

namespace TelegramBot
{
 
    class Program
    {
        
        public static List<Exchange> Request()
        {
            string sURL;
            sURL = "https://api.privatbank.ua/p24api/pubinfo?json&exchange&coursid=5";

            WebRequest webreq;
            webreq = WebRequest.Create(sURL);

            Stream objStream;
            objStream = webreq.GetResponse().GetResponseStream();

            System.IO.StreamReader objReader = new StreamReader(objStream);

            string sLine = objReader.ReadToEnd();
            List<Exchange> result = JsonSerializer.Deserialize<List<Exchange>>(sLine);

            return result;

        }

        private static double exchange =0;
        
        private static TelegramBotClient client;
        static void Main(string[] args)
        {
          
            client = new TelegramBotClient(GetToken.GetTokenLine());
            client.StartReceiving();
            client.OnMessage += OnMessageHandler;
            client.OnCallbackQuery += ConvertationOptions;          
            Console.ReadLine();
            client.StopReceiving();
          
        }

        private static async void ConvertationOptions(object sender, CallbackQueryEventArgs e)
        {
            List<Exchange> exchanges = new List<Exchange>();
            exchanges = Request();
            string buttontext = e.CallbackQuery.Data;
            string name = $"{e.CallbackQuery.From.FirstName}{e.CallbackQuery.From.LastName}";
            if (buttontext == "Dollar")
            {
                string currancy = $" {exchanges[0].ccy} - {exchanges[0].base_ccy} \n" +
                    $"Buy: {exchanges[0].buy} \n " +
                    $"Sell: {exchanges[0].sale} \n ";
                Console.WriteLine($"{name} press the button {buttontext}");
                await client.AnswerCallbackQueryAsync(e.CallbackQuery.Id, currancy);
            }
            else if (buttontext == "Euro")
            {
                string currancy = $" {exchanges[1].ccy} - {exchanges[1].base_ccy} \n " +
                    $"Buy: {exchanges[1].buy} \n " +
                    $" Sell: {exchanges[1].sale} \n ";
                Console.WriteLine($"{name} press the button {buttontext}");
                await client.AnswerCallbackQueryAsync(e.CallbackQuery.Id, currancy);
            }           
        }
        private static async void OnMessageHandler(object sender, MessageEventArgs e)
        {
            List<Exchange> exchanges = new List<Exchange>();
            exchanges = Request();                      
            var msg = e.Message;

            string name = $"{msg.From.FirstName}{msg.From.LastName}";

            if (msg.Text != null)
            {
                Console.OutputEncoding = Encoding.UTF8;
                Console.WriteLine($"  {msg.From.FirstName}{msg.From.LastName} wrote : {msg.Text}  ");
            }
            switch (msg.Text)
            {
                case "Hello":
                   
                    await client.SendTextMessageAsync(msg.Chat.Id, $" Hello,{name} ", replyToMessageId: msg.MessageId);
                    break;
                case "hi":
                    
                    await client.SendTextMessageAsync(msg.Chat.Id, $" Hi,{name} ", replyToMessageId: msg.MessageId);
                    break;

                case "/Dollar":
                   
                    await client.SendTextMessageAsync(msg.Chat.Id, $" Exchange rate \n {exchanges[0].ccy}  - {exchanges[0].base_ccy} \n Buy: {exchanges[0].buy} \n  Sell: {exchanges[0].sale} \n ",
                         replyToMessageId: msg.MessageId);
                    break;
                case "/Euro":
                 
                    await client.SendTextMessageAsync(msg.Chat.Id, $" Exchange rate \n {exchanges[1].ccy}  - {exchanges[1].base_ccy} \n Buy: {exchanges[1].buy} \n  Sell: {exchanges[1].sale} \n ",
                   replyToMessageId: msg.MessageId);
                    break;
                case "/ExchangeRates":                                      
                    var inlinekeyboard = new InlineKeyboardMarkup(new[] 
                    {
                        new[]
                        {   InlineKeyboardButton.WithCallbackData("Dollar"),
                            InlineKeyboardButton.WithCallbackData("Euro")

                        }
                    });
                    await client.SendTextMessageAsync(msg.From.Id, "Choose a currency", replyMarkup: inlinekeyboard);
                    break;
                case "Бля":
                    await client.SendTextMessageAsync(msg.From.Id, $" Не матюкайся ");
                    break;
                case "бля":
                    await client.SendTextMessageAsync(msg.From.Id, $" Не матюкайся ");
                    break;
                case "сука":
                    await client.SendTextMessageAsync(msg.From.Id, $" Не матюкайся ");
                    break;
                case "курва":
                    await client.SendTextMessageAsync(msg.From.Id, $" Не матюкайся ");
                    break;
                case "лох":
                    await client.SendTextMessageAsync(msg.From.Id, $" Не обзивайся ");
                    break;
                case "/ExchangeCurrency":                 
                    string tex = @"List of commands:
                        /USDtoUAH - exchange USD to UAH
                        /EurotoUAH -  exchange EU to UAH
                        /UAHtoUSD - exchange UAH to USD
                        /UAHtoEU - exchange UAH to EU
                          ";
                    await client.SendTextMessageAsync(msg.From.Id, tex);
                    break;
                    
                        case "/USDtoUAH":                   
                    exchange = Changes(exchanges[0].buy);
                            //string USDtoUAH = e.Message.Text;
                            await client.SendTextMessageAsync(msg.From.Id, " How many dollars do you want to exchange ?");
                            break;
                case "/EurotoUAH":
                    exchange=Changes(exchanges[1].buy);
                    //string EUtoUA = e.Message.Text;                 
                    await client.SendTextMessageAsync(msg.From.Id, $" How many euros you want to change ?");
                    break;               
                case "/UAHtoUSD":                  
                    exchange = Changes(exchanges[0].sale);
                    //string UAHtoUSD = e.Message.Text;
                    await client.SendTextMessageAsync(msg.From.Id, $"How many hryvnias do you want to exchange for dollars ?");
                    break;
                case "/UAHtoEU":                   
                    exchange = Changes(exchanges[1].sale);
                   // string UAHtoEU = e.Message.Text;
                    await client.SendTextMessageAsync(msg.From.Id, $"How many hryvnias do you want to exchange for euros ?");
                    break;
                default:
                    try
                    {
                        if (exchange == Changes(exchanges[0].buy))
                        {
                            double exchangevalue = Math.Round(exchange * (Convert.ToDouble(e.Message.Text)),2);
                            string reply = $" You get {exchangevalue} hryvnias ";
                            Console.WriteLine($"  {msg.From.FirstName}{msg.From.LastName} bought : {exchangevalue} hryvnias  ");
                            await client.SendTextMessageAsync(msg.From.Id, reply);
                            break;
                        }
                        else if (exchange == Changes(exchanges[1].buy))
                        {
                            double exchangevalue = Math.Round(exchange * (Convert.ToDouble(e.Message.Text)),2);
                            string reply = $" You get {exchangevalue} hryvnias ";
                            Console.WriteLine($"  {msg.From.FirstName}{msg.From.LastName} bought : {exchangevalue} hryvnias  ");
                            await client.SendTextMessageAsync(msg.From.Id, reply);
                            break;
                        }
                        else if (exchange == Changes(exchanges[0].sale))
                        {
                            double exchangevalue =Math.Round((Convert.ToDouble(e.Message.Text)) / exchange,2);
                            string reply = $" You get {exchangevalue} dollars ";
                            Console.WriteLine($"  {msg.From.FirstName}{msg.From.LastName} bought : {exchangevalue} dollars ");
                            await client.SendTextMessageAsync(msg.From.Id, reply);
                            break;
                        }
                        else if (exchange == Changes(exchanges[1].sale))
                        {
                            double exchangevalue = Math.Round((Convert.ToDouble(e.Message.Text)) / exchange,2);
                            string reply = $" You get {exchangevalue} euros ";
                            Console.WriteLine($"  {msg.From.FirstName}{msg.From.LastName} bought : {exchangevalue} dollars  ");
                            await client.SendTextMessageAsync(msg.From.Id, reply);
                            break;
                        }
                    }
                    catch
                    {
                        
                    }                  
                    string text = $"List of commands: \n " +
                    "/Dollar - dollar exchange rate \n" +
                    "/Euro - euro exchange rate \n" +
                    "/ExchangeRates \n" +
                    "/ExchangeCurrency - Calculate currency exchange\n ";
                        await client.SendTextMessageAsync(msg.From.Id, text);
                        exchange = 0;
                        break;                                                                            
            }
        }
        public static double Changes(string text)
        {
            double value ;
            value = Convert.ToDouble(text,CultureInfo.InvariantCulture);                       
            return value;
        }
    }
}
