using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carrier.CCP.Common.Utility.Helper
{
    /// <summary>
    ///     Represent the service bus wrapper class.
    /// </summary>
    /// <seealso cref="Ccs.Uno.Utility.Helper.AzureUtility.IServiceBusMessage" />
    public class AwsMessage : IServiceBusMessage
    {
        private readonly string queueUrl;

        private readonly string topicArn;

        private readonly AmazonSQSClient _notificationQueue;

        private readonly AmazonSimpleNotificationServiceClient _topicClient;

        public AwsMessage(string queueUrl, string topicArn = null)
        {
            if (!string.IsNullOrWhiteSpace(queueUrl))
            {
                _notificationQueue = new AmazonSQSClient();
            }

            if (!string.IsNullOrWhiteSpace(topicArn))
            {
                _topicClient = new AmazonSimpleNotificationServiceClient();
            }

            this.topicArn = topicArn;
            this.queueUrl = queueUrl;
        }

        /// <summary>
        ///     Sends the notification message.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task SendNotificationMessageAsync(string message)
        {
            SendMessageRequest sendMessageRequest = new SendMessageRequest();
            sendMessageRequest.QueueUrl = queueUrl;
            sendMessageRequest.MessageBody = message;
            
            await _notificationQueue.SendMessageAsync(sendMessageRequest).ConfigureAwait(false);
        }

        /// <summary>
        ///     Sends the notification to subscribers.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task SendNotificationToSubscribersAsync(string message)
        {
            PublishRequest pubRequest = new PublishRequest();
            pubRequest.Message = message;
            pubRequest.TopicArn = topicArn;

            await _topicClient.PublishAsync(pubRequest).ConfigureAwait(false);
        }

        /// <summary>
        ///     Sends the notification to subscribers.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageProperties"></param>
        /// <param name="timeToLiveMinutes"></param>
        /// <returns></returns>
        public async Task SendNotificationToSubscribersAsync(string message, string timeToLiveMinutes,
            Dictionary<string, string> messageProperties = null)
        {
            PublishRequest pubRequest = new PublishRequest();
            pubRequest.Message = message;
            
            if (messageProperties != null)
            {
                foreach (var property in messageProperties)
                {
                    pubRequest.MessageAttributes.Add($"{property.Key}", new Amazon.SimpleNotificationService.Model.MessageAttributeValue { StringValue = property.Value, DataType = "String" });
                }
            }

            pubRequest.TopicArn = topicArn;
            await _topicClient.PublishAsync(pubRequest).ConfigureAwait(false);
        }
    }
}