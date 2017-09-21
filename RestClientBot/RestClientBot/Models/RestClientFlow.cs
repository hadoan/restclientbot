using Flurl.Http;
using Microsoft.Bot.Builder.FormFlow;
using Newtonsoft.Json;
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
        //RawText,
        RawJson,
        //RawJavascript,
        //RawXml,
        //RawHtml,
        //Binary
    }
    [Serializable]
    public class RestClientFlow
    {
        public MethodOptions? MethodOption;
        public string Url;
        public bool? HasBody;
        public bool HasHeader;
        public BodyOptions? BodyOption;
        public string BodyRawJson;
        public string HeaderJson;
        public string BodyFormData;
        public string BodyXWWWFORM;
       
        //public string Json
        public static IForm<RestClientFlow> BuildForm()
        {
            return new FormBuilder<RestClientFlow>()
                .Message("Welcome to RestClientBot, please make a selection!")
                .Field(nameof(MethodOption))
                .Field(nameof(Url))
                .Field(nameof(HasHeader))
                .Field(nameof(HeaderJson),hasHeaderJson)
                .Field(nameof(HasBody))
                .Field(nameof(BodyOption),hasBody)
                .Field(nameof(BodyRawJson), hasJsonBody)
                .Field(nameof(BodyFormData),hasBodyFormData)
                .Field(nameof(BodyXWWWFORM),hasBodyXwww)
                .OnCompletion(process)
                .Build();
        }

        #region Private methods
        private static ActiveDelegate<RestClientFlow> hasBody = flow => flow.HasBody.HasValue && flow.HasBody.Value;
        private static ActiveDelegate<RestClientFlow> hasHeaderJson = flow => flow.HasHeader;
        private static ActiveDelegate<RestClientFlow> hasJsonBody = flow => flow.HasBody.HasValue && flow.HasBody.Value && flow.BodyOption.Value == BodyOptions.RawJson;
        private static ActiveDelegate<RestClientFlow> hasBodyFormData = flow => flow.HasBody.HasValue && flow.HasBody.Value && flow.BodyOption == BodyOptions.FormData;
        private static ActiveDelegate<RestClientFlow> hasBodyXwww = flow => flow.HasBody.HasValue && flow.HasBody.Value && flow.BodyOption == BodyOptions.XWWWWFormUrlEncoded;

        private static OnCompletionAsyncDelegate<RestClientFlow> process = async (context, state) => {
            var activity = context.MakeMessage();
            try
            {
                //await context.PostAsync("We are currently processing your sandwich. We will message you the status.");

                activity.Text = "Processing your request...";
                await context.PostAsync(activity);
                if (state.MethodOption == MethodOptions.GET)
                {
                    var response = await state.Url.GetAsync();
                    activity.Text = "Response: " + response.StatusCode;
                    await context.PostAsync(activity);
                    activity.Text = await response.Content.ReadAsStringAsync();
                    await context.PostAsync(activity);
                }
                else if (state.MethodOption == MethodOptions.POST)
                {
                    object bodyData = null;
                    if (state.HasBody.HasValue && state.HasBody.Value)
                    {
                        bodyData = JsonConvert.DeserializeObject(state.BodyRawJson);

                    }
                    var postResponse = await state.Url.PostJsonAsync(bodyData);
                    //var postResponse = await state.Url.PostStringAsync(state.BodyJson);

                    await context.PostAsync(activity);
                    activity.Text = "Response: " + postResponse.StatusCode;
                    await context.PostAsync(activity);
                    activity.Text = await postResponse.Content.ReadAsStringAsync();
                    await context.PostAsync(activity);

                }
                else
                {
                    activity.Text = "Not support!";
                    await context.PostAsync(activity);
                }
            }
            catch (Exception ex)
            {
                activity.Text = "<b>Shit Happen!</b>";
                await context.PostAsync(activity);
                activity.Text = ex.Message;
                await context.PostAsync(activity);
            }
        };
        #endregion
    }
}