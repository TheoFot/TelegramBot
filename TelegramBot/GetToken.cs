using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot
{
    public class GetToken
    {
        public static string GetTokenPath()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            var result = new FileInfo(location.AbsolutePath).Directory;
            return Path.Combine(result.FullName, "TelegramBotToken.txt");
            
        }
        public static string GetTokenLine()
        {
            string tokenline;
            using (StreamReader stream = new StreamReader(GetTokenPath(), System.Text.Encoding.Default))
            {
                tokenline = stream.ReadToEnd();
            }
            return tokenline;
        }
    }
}

