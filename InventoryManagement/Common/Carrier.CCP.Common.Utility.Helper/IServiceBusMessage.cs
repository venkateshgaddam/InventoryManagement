using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carrier.CCP.Common.Utility.Helper
{
    /// <summary>
    ///     Represent the interface for azure service bus wrapper.
    /// </summary>
    public interface IServiceBusMessage
    {
        /// <summary>
        /// Sends the notification message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        Task SendNotificationMessageAsync(string message);

        /// <summary>
        /// Sends the notification to subscribers.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        Task SendNotificationToSubscribersAsync(string message);

        /// <summary>
        /// Sends the notification to subscribers.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="timeToLiveMinutes">The time to live minutes.</param>
        /// <param name="messageProperties">The message properties.</param>
        /// <returns></returns>
        Task SendNotificationToSubscribersAsync(string message, string timeToLiveMinutes,
            Dictionary<string, string> messageProperties = null);
    }
}