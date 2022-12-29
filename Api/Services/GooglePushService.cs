using Api.Configs;
using Api.Models.Push;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using PushSharp.Google;
using System.Text;

namespace Api.Services
{
    public class GooglePushService
    {
        private const int MaxPayloadLength = 2048;
        private const int MaxAndroidPayloadLength = 4096;
        private readonly List<string> _messages;
        private readonly PushConfig.GoogleConfig _config;

        public GooglePushService(IOptions<PushConfig> config)
        {
            _messages =  new List<string>();
            if (config.Value.Google == null)
            {
                throw new ArgumentNullException("Google configuration not found");
            }
            _config = config.Value.Google;
        }

        public List<string> SendNotification(string pushToken, PushModel model)
        {
            _messages.Clear();
            var config = new GcmConfiguration(_config.ServerKey);
            config.GcmUrl = _config.GcmUrl;
            var gcmBroker = new GcmServiceBroker(config);
            gcmBroker.OnNotificationFailed += GcmBroker_OnNotificationFailed;
            gcmBroker.OnNotificationSucceeded += GcmBroker_OnNotificationSucceeded;

            gcmBroker.Start();
            var jdata = CreateDataMessage(model.CustomData);

            var notify = new GcmNotification
            {
                RegistrationIds = new List<string> { pushToken },
                Data = jdata,
                Notification = CreateMeassage(model.Alert),
                ContentAvailable = jdata["data"] != null,
            };
            gcmBroker.QueueNotification(notify);
            gcmBroker.Stop();

            return _messages;
        }

        private JObject CreateMeassage(PushModel.AlertModel alert)
        {
            var jNotify = new JObject();
            if (!string.IsNullOrWhiteSpace(alert.Title))
            {
                jNotify["title"] = alert.Title;
            }
            if (!string.IsNullOrWhiteSpace(alert.Body))
            {
                jNotify["body"] = alert.Body;
                var curPayloadLength = Encoding.UTF8.GetBytes(jNotify.ToString(Newtonsoft.Json.Formatting.None)).Length;
                if (curPayloadLength > MaxAndroidPayloadLength)
                {
                    var dif = curPayloadLength - MaxAndroidPayloadLength + 3;
                    jNotify["body"] = alert.Body.Length - dif <= 0 ? null : alert.Body[..^dif] + "...";
                }
            }
            return jNotify;
        }

        private JObject CreateDataMessage(Dictionary<string, object>? customData)
        {
            var jData = new JObject();
            var jCustomData = new JObject();
            if (jCustomData != null)
            {
                jCustomData = JObject.FromObject(customData); 
            }
            jData["data"] = jCustomData;
            return jData;
        }

        private void GcmBroker_OnNotificationSucceeded(GcmNotification notification)
        {
            _messages.Add("An alert has been successfully sent!");
        }

        private void GcmBroker_OnNotificationFailed(GcmNotification notification, AggregateException exception)
        {
            throw new NotImplementedException();
        }
    }
}
