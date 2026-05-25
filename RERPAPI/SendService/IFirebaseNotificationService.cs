using System.Collections.Generic;
using System.Threading.Tasks;

namespace RERPAPI.SendService
{
    public interface IFirebaseNotificationService
    {
        /// <summary>
        /// Sends a push notification to a single device.
        /// </summary>
        Task<bool> SendNotificationAsync(string deviceToken, string title, string body, Dictionary<string, string> data = null);

        /// <summary>
        /// Sends a push notification to multiple devices.
        /// Returns a list of tokens that failed or were invalid (so they can be removed/handled).
        /// </summary>
        Task<List<string>> SendMulticastNotificationAsync(IReadOnlyList<string> deviceTokens, string title, string body, Dictionary<string, string> data = null);
    }
}
