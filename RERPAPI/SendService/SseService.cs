//using System.Collections.Concurrent;
//using System.Text.Json;

//public class SseService
//{
//    private static readonly ConcurrentDictionary<Guid, (HttpResponse Response, SemaphoreSlim Lock)> _clients = new();

//    public async Task AddClientAsync(HttpResponse response)
//    {
//        var clientId = Guid.NewGuid();
//        var semaphore = new SemaphoreSlim(1, 1);
//        _clients.TryAdd(clientId, (response, semaphore));

//        var cancellation = response.HttpContext.RequestAborted;

//        try
//        {
//            while (!cancellation.IsCancellationRequested)
//            {
//                await semaphore.WaitAsync(cancellation);
//                try
//                {
//                    await response.WriteAsync(": heartbeat\n\n", cancellation);
//                    await response.Body.FlushAsync(cancellation);
//                }
//                finally
//                {
//                    semaphore.Release();
//                }
//                await Task.Delay(TimeSpan.FromSeconds(30), cancellation);
//            }
//        }
//        catch (OperationCanceledException)
//        {
//        }
//        catch (Exception)
//        {
//        }
//        finally
//        {
//            _clients.TryRemove(clientId, out _);
//            semaphore.Dispose();
//        }
//    }

//    public async Task SendEventAsync(string eventName, object data)
//    {
//        var json = JsonSerializer.Serialize(data);
//        var eventPayload = $"event: {eventName}\ndata: {json}\n\n";

//        foreach (var clientId in _clients.Keys)
//        {
//            if (_clients.TryGetValue(clientId, out var client))
//            {
//                try
//                {
//                    await client.Lock.WaitAsync();
//                    try
//                    {
//                        await client.Response.WriteAsync(eventPayload);
//                        await client.Response.Body.FlushAsync();
//                    }
//                    finally
//                    {
//                        client.Lock.Release();
//                    }
//                }
//                catch
//                {
//                    _clients.TryRemove(clientId, out _);
//                }
//            }
//        }
//    }
//}
using System.Collections.Concurrent;
using System.Text.Json;

public class SseService
{
    private static readonly ConcurrentDictionary<Guid, (HttpResponse Response, SemaphoreSlim Lock)> _clients
        = new();

    // ==========================
    // ADD CLIENT
    // ==========================
    public async Task AddClientAsync(HttpResponse response)
    {
        var clientId = Guid.NewGuid();
        var semaphore = new SemaphoreSlim(1, 1);

        _clients.TryAdd(clientId, (response, semaphore));

        var cancellation = response.HttpContext.RequestAborted;

        try
        {
            while (!cancellation.IsCancellationRequested)
            {
                await semaphore.WaitAsync(cancellation);
                try
                {
                    await response.WriteAsync(": heartbeat\n\n", cancellation);
                    await response.Body.FlushAsync(cancellation);
                }
                finally
                {
                    semaphore.Release();
                }

                await Task.Delay(TimeSpan.FromSeconds(30), cancellation);
            }
        }
        catch
        {
            // ignore
        }
        finally
        {
            _clients.TryRemove(clientId, out _);
            semaphore.Dispose();
        }
    }

    // ==========================
    // SEND EVENT (PARALLEL + TIMEOUT)
    // ==========================
    public async Task SendEventAsync(string eventName, object data)
    {
        var json = JsonSerializer.Serialize(data);
        var eventPayload = $"event: {eventName}\ndata: {json}\n\n";

        var tasks = _clients.Select(kvp =>
        {
            var clientId = kvp.Key;
            var client = kvp.Value;

            return SendToClientAsync(clientId, client, eventPayload);
        });

        await Task.WhenAll(tasks);
    }

    private async Task SendToClientAsync(
        Guid clientId,
        (HttpResponse Response, SemaphoreSlim Lock) client,
        string payload)
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            await client.Lock.WaitAsync(cts.Token);

            try
            {
                await client.Response.WriteAsync(payload, cts.Token);
                await client.Response.Body.FlushAsync(cts.Token);
            }
            finally
            {
                client.Lock.Release();
            }
        }
        catch
        {
            // client lỗi / timeout -> remove
            _clients.TryRemove(clientId, out _);
        }
    }
}