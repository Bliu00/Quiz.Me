using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace Quiz.me.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        int index, cardCount, totalSets;
        List<QuizletCard> set;
        List<QuizletSet> sets;

        public Task StartAsync(IDialogContext context)
        {

            // Ask for User's Quizlet Username
            context.Wait(GetUsername);

            return Task.CompletedTask;
        }

        private async Task GetUsername(IDialogContext context, IAwaitable<object> result)
        {

            var reply = context.MakeMessage();

            // Ask User for Quizlet Username
            reply.Text = "Hello " + Environment.UserName + ", what is your Quizlet username?";
            reply.Speak = "Hello " + Environment.UserName + ", what is your Quizlet username?";
            reply.InputHint = InputHints.IgnoringInput;

            await context.PostAsync(reply);
            context.Wait(ShowSets);
        }

        private async Task ShowSets(IDialogContext context, IAwaitable<object> result)
        {
            // Set index value
            index = 0;

            var activity = await result as Activity;
            var reply = context.MakeMessage();

            // Exit Protocol 
            if (activity.Text.ToLower().Equals("exit"))
            {
                reply.Text = "GoodBye";
                reply.Speak = "GoodBye";
                reply.InputHint = InputHints.IgnoringInput;
                await context.PostAsync(reply);

                System.Environment.Exit(1);
            }
            else {
                // Get User's Quizlet Sets
                var myUri = new Uri("https://api.quizlet.com/2.0/users/" + activity.Text + "/sets?client_id=zeSpVr8wzT&whitespace=1");
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

                // Put into QuizletSet Objects
                sets = JsonConvert.DeserializeObject<List<QuizletSet>>(json);
                totalSets = sets.Count;

                responseStream.Close();
                myWebResponse.Close();

                // Ask which Set to use
                reply.Text = "What set would you like me to quiz you on?";
                reply.Speak = "What set would you like me to quiz you on?";
                reply.InputHint = InputHints.IgnoringInput;

                await context.PostAsync(reply);


                // TODO: DISPLAY CARDS OF SETS
                for (int i = 0; i < totalSets; i++)
                {

                    reply.Text = "[" + i + "] " + sets[i].title;
                    reply.Speak = "Option " + i + " " + sets[i].title;
                    reply.InputHint = InputHints.IgnoringInput;

                    await context.PostAsync(reply);

                }

                context.Wait(LoadSet);
            }
        }

        private async Task LoadSet(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var reply = context.MakeMessage();

            // Exit Protocol 
            if (activity.Text.ToLower().Equals("exit"))
            {
                reply.Text = "GoodBye";
                reply.Speak = "GoodBye";
                reply.InputHint = InputHints.IgnoringInput;
                await context.PostAsync(reply);

                System.Environment.Exit(1);
            } else {
                reply.Text = "Ok, loading Q&A Session...(Say or Type 'Begin' when you are ready)";
                reply.Speak = "Ok, loading Q&A Session...(Say or Type 'Begin' when you are ready)";
                reply.InputHint = InputHints.IgnoringInput;
                await context.PostAsync(reply);

                set = sets[Convert.ToInt32(activity.Text)].terms;
                cardCount = set.Count;

                context.Wait(AskAsync);
            }
        }

        private async Task AskAsync(IDialogContext context, IAwaitable<object> result)
        {
            var reply = context.MakeMessage();
            var activity = await result as Activity;

            if (index < cardCount && !(activity.Text.ToLower().Equals("exit")))
            {
                // Ask User about term
                reply.Text = set[index].term;
                reply.Speak = set[index].term;
                reply.InputHint = InputHints.IgnoringInput;
                await context.PostAsync(reply);
                index++;

                // Listen for response
                context.Wait(ResponseAsync);
            } else {
                reply.Text = "Ending Q&A Session...";
                reply.Speak = "Ending Q&A Session...";
                reply.InputHint = InputHints.IgnoringInput;
                await context.PostAsync(reply);

                context.Wait(GetUsername);
            }

        }

        private async Task ResponseAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var reply = context.MakeMessage();

            // return our reply to the user
            if(activity.Text.ToLower().Equals("exit"))
            {
                reply.Text = "Ending Q&A Session...";
                reply.Speak = "Ending Q&A Session...";
                reply.InputHint = InputHints.IgnoringInput;
                await context.PostAsync(reply);

                context.Wait(GetUsername);
            } else if ((activity.Text.ToUpper()).Equals(set[index].definition.ToUpper())) {
                await context.PostAsync($"Correct");
                context.Wait(AskAsync);
            } else {
                await context.PostAsync($"Incorrect, definition is: " + set[index].definition);
                context.Wait(AskAsync);
            }
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