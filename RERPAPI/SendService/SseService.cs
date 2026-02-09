using System.Text.Json;

namespace RERPAPI.SendService
{

    public class SseService
    {
        private static readonly List<HttpResponse> _clients = new();

        public async Task AddClientAsync(HttpResponse response)
        {
            _clients.Add(response);

            try
            {
                await Task.Delay(Timeout.Infinite);
            }
            catch
            {
                _clients.Remove(response);
            }
        }

        public async Task SendEventAsync(string eventName, object data)
        {
            var json = JsonSerializer.Serialize(data);

            foreach (var client in _clients.ToList())
            {
                try
                {
                    await client.WriteAsync($"event: {eventName}\n");
                    await client.WriteAsync($"data: {json}\n\n");
                    await client.Body.FlushAsync();
                }
                catch
                {
                    _clients.Remove(client);
                }
            }
        }
    }
}
