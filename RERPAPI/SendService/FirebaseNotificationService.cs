using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;

namespace RERPAPI.SendService
{
    public class FirebaseNotificationService : IFirebaseNotificationService
    {
        private readonly ILogger<FirebaseNotificationService> _logger;

        public FirebaseNotificationService(ILogger<FirebaseNotificationService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SendNotificationAsync(string deviceToken, string title, string body, Dictionary<string, string> data = null)
        {
            if (string.IsNullOrEmpty(deviceToken))
            {
                _logger.LogWarning("Cannot send FCM notification: deviceToken is empty.");
                return false;
            }

            var message = new Message()
            {
                Token = deviceToken,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body,
                },
                Data = data ?? new Dictionary<string, string>()
            };

            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                _logger.LogInformation($"Successfully sent message: {response}");
                return true;
            }
            catch (FirebaseMessagingException ex)
            {
                _logger.LogError(ex, $"Error sending FCM message to token: {deviceToken}. Reason: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error sending FCM message to token: {deviceToken}");
                return false;
            }
        }

        public async Task<List<string>> SendMulticastNotificationAsync(IReadOnlyList<string> deviceTokens, string title, string body, Dictionary<string, string> data = null)
        {
            var invalidTokens = new List<string>();

            if (deviceTokens == null || !deviceTokens.Any())
            {
                _logger.LogWarning("Cannot send FCM multicast notification: deviceTokens list is empty.");
                return invalidTokens;
            }

            var message = new MulticastMessage()
            {
                Tokens = deviceTokens.ToList(),
                Notification = new Notification()
                {
                    Title = title,
                    Body = body,
                },
                Data = data ?? new Dictionary<string, string>()
            };

            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);
                _logger.LogInformation($"{response.SuccessCount} messages were sent successfully out of {deviceTokens.Count}");

                if (response.FailureCount > 0)
                {
                    for (int i = 0; i < response.Responses.Count; i++)
                    {
                        var result = response.Responses[i];
                        if (!result.IsSuccess)
                        {
                            var error = result.Exception.MessagingErrorCode;
                            _logger.LogWarning($"Failed to send to token: {deviceTokens[i]} due to {error}");

                            // Detect invalid tokens (so callers can remove them from DB/memory)
                            if (error == MessagingErrorCode.InvalidArgument ||
                                error == MessagingErrorCode.Unregistered)
                            {
                                invalidTokens.Add(deviceTokens[i]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error sending FCM multicast message.");
            }

            return invalidTokens;
        }
    }
}
