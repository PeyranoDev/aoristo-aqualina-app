using Expo.Server.Client;
using Expo.Server.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class NotificationService
{
    private readonly PushApiClient _expoClient;

    public NotificationService()
    {
        _expoClient = new PushApiClient();
    }

    public async Task SchedulePushNotificationAsync(
        string pushToken,
        string title,
        string message,
        TimeSpan delay
    )
    {
        await Task.Delay(delay);

        var pushTicketRequest = new PushTicketRequest()
        {
            PushTo = new List<string> { pushToken },
            PushTitle = title,  // ¡Nombre correcto de la propiedad!
            PushBody = message, // "PushBody" en lugar de "Body"
            PushSound = "default",
            PushChannelId = "default"
        };

        var result = await _expoClient.PushSendAsync(pushTicketRequest);

        if (result?.PushTicketErrors?.Count > 0)
        {
            foreach (var error in result.PushTicketErrors)
            {
                Console.WriteLine($"Error: {error.ErrorCode} - {error.ErrorMessage}");
            }
        }
    }
}