using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QnABotApp.Components
{
    public class Intro
    {
        private static SpeechConfig speechConfig;
        public static async Task Tell()
        {
            // Get config settings from AppSettings
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            string cogSvcKey = configuration["CognitiveServiceKey"];
            string cogSvcRegion = configuration["CognitiveServiceRegion"];

            // Konfigurera tal-tjänst
            speechConfig = SpeechConfig.FromSubscription(cogSvcKey, cogSvcRegion);

            // Konfigurera röst
            speechConfig.SpeechSynthesisVoiceName = "en-US-AriaNeural";

            var now = DateTime.Now;
            string responseText = "Hello and welcome. Feel free to ask anything and I'll try to answer it.";

            // Konfigurera talsyntes
            speechConfig.SpeechSynthesisVoiceName = "en-GB-RyanNeural";
            using SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer(speechConfig);

            // Print the response
            Console.WriteLine(responseText);

            // Syntetisera talad utdata
            SpeechSynthesisResult speak = await speechSynthesizer.SpeakTextAsync(responseText);
            if (speak.Reason != ResultReason.SynthesizingAudioCompleted)
            {
                Console.WriteLine(speak.Reason);
            }
        }
    }
}
