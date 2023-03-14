using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Text;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Engine.ERPConnector.Model.ChatGPTConnector;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.ERPConnector
{
    public enum ChatGPTConfigurationKey
    {
        ChatGPTAPIKey,
        ChatGPTCompletionAPIURL,
        ChatGPTFeedbackClassifierModel,
        ChatGPTFeedbackClassifierPrompt
    }

    public class ZnodeChatGPTConnector : BaseERP
    {
        private ERPConnectorControlListModel ERPConfigurationData { get; set; }
        
        public ZnodeChatGPTConnector(){ }

        [RealTime("RealTime")]
        public ClassifiedFeedback ClassifyFeedback(string feedbackText) {
            string aiModel = GetERPConfigurationValue(ChatGPTConfigurationKey.ChatGPTFeedbackClassifierModel);
            string promptInstruction = "Which departments should receive this feedback? Please summarize the content relative to each department in a personalized email summary to them. If the feedback is not relevant to me, do not write a summary email. Do not include advice to the department, only include relevant facts. The feedback was enetered by a user of our b2b e-commerce site, using a builtin feedback submission tool. " +
                                        GetERPConfigurationValue(ChatGPTConfigurationKey.ChatGPTFeedbackClassifierPrompt) +
                                        "\r\n\r\nResponse format should be valid json, using this object shape, \"Emails\" will contain a list if multiple departments should be notified:\r\n{ \\\"Sentiment\\\": <customerSentiment>, \\\"Emails\\\": [{ \\\"Department\\\": <targetDepartment>, \\\"EmailBody\\\": <emailBody>}]}";
            string response = CompletionsApiRequest(new CompletionsApiRequest
            {
                Model = aiModel,
                Prompt = $"{promptInstruction}\r\n{feedbackText}",
                Temparature = 0,
                MaxTokens = 512,
                TopP = 1,
                FrequencyPenalty = 0,
                PresencePenalty = 0
            });
            var classifedFeedback = JsonConvert.DeserializeObject<ClassifiedFeedback>(response);
            return classifedFeedback;
        }

        private string CompletionsApiRequest(CompletionsApiRequest requestData)
        {
            string requestUrl = GetERPConfigurationValue(ChatGPTConfigurationKey.ChatGPTCompletionAPIURL);
            var response = ApiRequest<CompletionsAPIResponse>(requestUrl, JsonConvert.SerializeObject(requestData));
            var stringResponse = response.Choices.FirstOrDefault()?.Text.Trim();
            return stringResponse;
        }

        private T ApiRequest<T>(string requestUrl, string requestData)
        {
            string apiKey = GetERPConfigurationValue(ChatGPTConfigurationKey.ChatGPTAPIKey);
            byte[] bytes = Encoding.ASCII.GetBytes(requestData);
            var request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Headers = new WebHeaderCollection { { "Authorization", $"Bearer {apiKey}" } };
            request.ContentType = "application/json";
            request.ContentLength = bytes.Length;
            request = GetHeadersForRequest(request, ZnodeConstant.Post);

            using (var reqStream = request.GetRequestStream())
                reqStream.Write(bytes, 0, bytes.Length);

            var status = new ApiStatus();
            var response = GetAPIResultFromResponse<T>(request, status);
            return response;
        }

        private void GetERPControlsDataFromDatabase()
        {
            IZnodeRepository<ZnodeERPConfigurator> erpRepository = new ZnodeRepository<ZnodeERPConfigurator>();
            string settings = erpRepository.Table.FirstOrDefault(x => x.IsActive && x.ClassName == "ZnodeChatGPTConnector")?.JsonSetting;
            if (!string.IsNullOrEmpty(settings))
            {
                ERPConfigurationData = JsonConvert.DeserializeObject<ERPConnectorControlListModel>(settings);
            }
        }

        private string GetERPConfigurationValue(ChatGPTConfigurationKey key)
        {
            if (HelperUtility.IsNull(ERPConfigurationData))
            {
                GetERPControlsDataFromDatabase();
            }

            return ERPConfigurationData.ERPConnectorControlList.FirstOrDefault(control => control.Name == key.ToString())?.Value;
        }
    }
}

