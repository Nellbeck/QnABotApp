using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace QnABotApp.Components
{
    class Transcribe
    {
        private static SpeechConfig speechConfig;
        public static async Task<string> TranscribeCommand()
        {
            // Get config settings from AppSettings
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            string cogSvcKey = configuration["CognitiveServiceKey"];
            string cogSvcRegion = configuration["CognitiveServiceRegion"];

            // Konfigurera tal-tjänst
            speechConfig = SpeechConfig.FromSubscription(cogSvcKey, cogSvcRegion);

            string question = "";

            // Konfigurera taligenkänning
            using AudioConfig audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            using SpeechRecognizer speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

            // Bearbeta talinmatning
            SpeechRecognitionResult speech = await speechRecognizer.RecognizeOnceAsync();
            if (speech.Reason == ResultReason.RecognizedSpeech)
            {
                question = speech.Text;
                Console.WriteLine(question);
            }
            else
            {
                Console.WriteLine(speech.Reason);
                if (speech.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(speech);
                    Console.WriteLine(cancellation.Reason);
                    Console.WriteLine(cancellation.ErrorDetails);
                }
            }

            // Return the command
            return question;
        }
    }
}
