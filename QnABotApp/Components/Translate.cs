using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;
using Microsoft.CognitiveServices.Speech;

namespace QnABotApp.Components
{
    internal class Translate
    {
        private static SpeechConfig speechConfig;
        private static SpeechTranslationConfig translationConfig;

        public static async Task<string> TranslateToEnglish()
        {

            // Get config settings from AppSettings
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            string cogSvcKey = configuration["CognitiveServiceKey"];
            string cogSvcRegion = configuration["CognitiveServiceRegion"];


            // Configure translation
            translationConfig = SpeechTranslationConfig.FromSubscription(cogSvcKey, cogSvcRegion);

            //This line of code let the program know what languages to listen to
            var autoDetectSpeech = AutoDetectSourceLanguageConfig.FromLanguages(new string[] { "en-US", "sv-SE"});
            translationConfig.SpeechRecognitionLanguage = "sv-SE";
            translationConfig.AddTargetLanguage("en");

            // Configure speech
            speechConfig = SpeechConfig.FromSubscription(cogSvcKey, cogSvcRegion);

            string translation = "";

            // Translate speech
            using AudioConfig audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            using (var recognizer = new TranslationRecognizer(
                translationConfig,
                autoDetectSpeech,
                audioConfig))
            {
                var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

                if (result.Reason == ResultReason.TranslatedSpeech)
                {
                    var lidResult = result.Properties.GetProperty(PropertyId.SpeechServiceConnection_AutoDetectSourceLanguageResult);

                    foreach (var element in result.Translations)
                    {
                        Console.WriteLine(element.Value);
                        translation = element.Value;
                    }
                }
            }
            return translation;
        }
    }
}

