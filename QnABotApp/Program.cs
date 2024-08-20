using System;
using Azure;
using Azure.AI.Language.QuestionAnswering;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using QnABotApp.Components;

namespace QnABotApp
{
    class Program
    {
        private static SpeechConfig speechConfig;
        static async Task Main(string[] args)
        {
            try
            {
               
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string cogSvcKey = configuration["CognitiveServiceKey"];
                string cogSvcRegion = configuration["CognitiveServiceRegion"];

                // Konfigurera tal-tjänst
                speechConfig = SpeechConfig.FromSubscription(cogSvcKey, cogSvcRegion);
                //Some intro
                await Intro.Tell();

                // This example requires environment variables named "LANGUAGE_KEY" and "LANGUAGE_ENDPOINT"
                Uri endpoint = new Uri(configuration["AzureLanguangeEndpoint"]);
                AzureKeyCredential credential = new AzureKeyCredential(configuration["AzureLanguangeKey"]);
                string projectName = "LearnFAQ";
                string deploymentName = "production";


                QuestionAnsweringClient client = new QuestionAnsweringClient(endpoint, credential);
                QuestionAnsweringProject project = new QuestionAnsweringProject(projectName, deploymentName);

                while (true)
                {
                    Console.WriteLine();
                    string question = await Translate.TranslateToEnglish();

                    try
                    {

                        Response<AnswersResult> response = client.GetAnswers(question, project);

                        foreach (KnowledgeBaseAnswer answer in response.Value.Answers)
                        {
                            string responseText = $"{answer.Answer}";

                            // Print the response
                            Console.WriteLine(responseText);

                            // Konfigurera talsyntes
                            speechConfig.SpeechSynthesisVoiceName = "en-GB-RyanNeural";
                            using SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer(speechConfig);

                            // Syntetisera talad utdata
                            SpeechSynthesisResult speak = await speechSynthesizer.SpeakTextAsync(responseText);
                            if (speak.Reason != ResultReason.SynthesizingAudioCompleted)
                            {
                                Console.WriteLine(speak.Reason);
                            }



                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    if (question.ToLower() == "goodbye.")
                    {
                        break;
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
