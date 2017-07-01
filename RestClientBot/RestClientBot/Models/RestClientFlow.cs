using Flurl.Http;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestClientBot.Models
{
    public enum MethodOptions
    {
        POST,
        GET,
        PUT,
        DELETE
    }

    public enum BodyOptions
    {
        FormData,
        XWWWWFormUrlEncoded,
        RawText,
        RawJson,
        RawJavascript,
        RawXml,
        RawHtml,
        //Binary
    }
    [Serializable]
    public class RestClientFlow
    {
        public MethodOptions? MethodOption;
        public string Url;
        public bool? HasBody;
        public BodyOptions? BodyOption;
        public string BodyJson;

        private static ActiveDelegate<RestClientFlow> hasBody = flow => flow.HasBody.HasValue && flow.HasBody.Value;

        private static OnCompletionAsyncDelegate<RestClientFlow> process =async (context, state) => {
            var activity = context.MakeMessage();
            try
            {
                //await context.PostAsync("We are currently processing your sandwich. We will message you the status.");
                
                activity.Text = "Processing your request...";
                await context.PostAsync(activity);
                if(state.MethodOption==MethodOptions.GET)
                {
                    var response = await state.Url.GetAsync();
                    activity.Text = "Response: "+response.StatusCode;
                    await context.PostAsync(activity);
                    activity.Text = await response.Content.ReadAsStringAsync();
                    await context.PostAsync(activity);
                }
                else
                {
                    activity.Text = "Not support!";
                    await context.PostAsync(activity);
                }
            }
            catch
            {
                //await context.po
            }
        };
        public static IForm<RestClientFlow> BuildForm()
        {
            return new FormBuilder<RestClientFlow>()
                .Message("Welcome to RestClientBot, please make a selection!")
                .Field(nameof(MethodOption))
                .Field(nameof(Url))
                .Field(nameof(HasBody))
                .Field(nameof(BodyJson), hasBody)
                .OnCompletion(process)
                .Build();
        }
    }
}