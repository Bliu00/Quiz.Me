using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace Quiz.me.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var reply = context.MakeMessage();

            // Get Specific Quizlet Set
            var myUri = new Uri("https://api.quizlet.com/2.0/sets/415/terms?client_id=zeSpVr8wzT&whitespace=1");
            var myWebRequest = WebRequest.Create(myUri);
            var myHttpWebRequest = (HttpWebRequest)myWebRequest;
            myHttpWebRequest.PreAuthenticate = true;
            myHttpWebRequest.Headers.Add("Authorization", "Bearer brkshEQjyE");
            myHttpWebRequest.Accept = "application/json";

            // Parse JSON response 
            var myWebResponse = myWebRequest.GetResponse();
            var responseStream = myWebResponse.GetResponseStream();
            if (responseStream == null) Console.WriteLine("RESPONSE STREAM IS NULL");
            var myStreamReader = new StreamReader(responseStream, Encoding.Default);
            var json = myStreamReader.ReadToEnd();

            // Put into QuizletCard Object
            var data = JsonConvert.DeserializeObject<List<QuizletCard>>(json);
            responseStream.Close();
            myWebResponse.Close();

            // Add an InputHint to let Cortana know to expect user input
            reply.Text = data[0].id; ;
            reply.Speak = "This is the text that will be spoken.";
            reply.InputHint = InputHints.IgnoringInput;

            // return our reply to the user
            await context.PostAsync(reply);

            context.Wait(MessageReceivedAsync);
        }
    }
}
/*
 * 
 *             reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = new List<Attachment>();
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            Dictionary<string, string> cardContentList = new Dictionary<string, string>();
            cardContentList.Add("PigLatin", "https://loremflickr.com/320/240");
            cardContentList.Add("Pork Shoulder", "https://loremflickr.com/320/240");
            cardContentList.Add("Bacon", "https://loremflickr.com/320/240");

            foreach (KeyValuePair<string, string> cardContent in cardContentList)
            {
                List<CardImage> cardImages = new List<CardImage>();
                cardImages.Add(new CardImage(url: cardContent.Value));

                List<CardAction> cardButtons = new List<CardAction>();

                CardAction plButton = new CardAction()
                {
                    Value = $"https://en.wikipedia.org/wiki/{cardContent.Key}",
                    Type = "openUrl",
                    Title = "WikiPedia Page"
                };

                cardButtons.Add(plButton);

                HeroCard plCard = new HeroCard()
                {
                    Title = $"I'm a hero card about {cardContent.Key}",
                    Subtitle = $"{cardContent.Key} Wikipedia Page",
                    Images = cardImages,
                    Buttons = cardButtons
                };

                Attachment plAttachment = plCard.ToAttachment();
                reply.Attachments.Add(plAttachment);
            }

            await connector.Conversations.SendToConversationAsync(reply);


*/