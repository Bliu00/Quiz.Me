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
        int index, setSize;
        List<QuizletCard> set;

        public Task StartAsync(IDialogContext context)
        {
            // Set index value
            index = 0;

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
            set = JsonConvert.DeserializeObject<List<QuizletCard>>(json);
            setSize = set.Count;

            responseStream.Close();
            myWebResponse.Close();

            context.Wait(AskAsync);

            return Task.CompletedTask;
        }

        private async Task AskAsync(IDialogContext context, IAwaitable<object> result)
        {
            var reply = context.MakeMessage();

            if (index < setSize)
            {
                // Ask User about term
                reply.Text = set[index].term;
                reply.Speak = set[index].term;
                reply.InputHint = InputHints.IgnoringInput;
                await context.PostAsync(reply);
                index++;
            }
          
            // Listen for response
            context.Wait(ResponseAsync);
        }

        private async Task ResponseAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            if((activity.Text.ToUpper()).Equals(set[index].definition.ToUpper())) {
                await context.PostAsync($"Correct");
            } else {
                await context.PostAsync($"Incorrect");
            }

            context.Wait(AskAsync);
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