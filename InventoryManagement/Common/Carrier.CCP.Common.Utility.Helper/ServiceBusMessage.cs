using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Carrier.CCP.Common.Utility.Helper
{
    /// <summary>
    ///     Represent the service bus wrapper class.
    /// </summary>
    /// <seealso cref="Ccs.Uno.Utility.Helper.AzureUtility.IServiceBusMessage" />
    public class ServiceBusMessage : IServiceBusMessage
    {
        private readonly QueueClient _notificationQueue;

        private readonly ITopicClient _topicClient;

        public ServiceBusMessage(string connectingString, string queueName, string topicName = null)
        {
            if (!string.IsNullOrWhiteSpace(queueName))
                _notificationQueue = new QueueClient(connectingString, queueName);

            if (!string.IsNullOrWhiteSpace(topicName)) _topicClient = new TopicClient(connectingString, topicName);
        }

        /// <summary>
        ///     Sends the notification message.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task SendNotificationMessageAsync(string message)
        {
            var notificationmessage = new Message(Encoding.UTF8.GetBytes(message));
            await _notificationQueue.SendAsync(notificationmessage).ConfigureAwait(false);
        }

        /// <summary>
        ///     Sends the notification to subscribers.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task SendNotificationToSubscribersAsync(string message)
        {
            await _topicClient.SendAsync(new Message(Encoding.UTF8.GetBytes(message))).ConfigureAwait(false);
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
            var serviceBusmessage = new Message(Encoding.UTF8.GetBytes(message));

            if (timeToLiveMinutes != null)
                if (int.TryParse(timeToLiveMinutes, out var minutes))
                    serviceBusmessage.TimeToLive = new TimeSpan(0, minutes, 0);

            if (messageProperties != null)
                foreach (var property in messageProperties)
                    serviceBusmessage.UserProperties.Add($"{property.Key}", property.Value);
            await _topicClient.SendAsync(serviceBusmessage).ConfigureAwait(false);
        }
    }
}