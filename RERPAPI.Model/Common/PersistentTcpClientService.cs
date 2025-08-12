using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Model.Common
{
    public class PersistentTcpClientService
    {
        private readonly string _host;
        private readonly int _port;
        private TcpClient _tcpClient;
        private NetworkStream _networkStream;

        private readonly int _connectTimeoutMs;
        private readonly int _sendTimeoutMs;
        private readonly int _receiveTimeoutMs;

        private readonly int _maxReconnectAttempts;
        private readonly int _reconnectDelayMs;

        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);
        private CancellationTokenSource _listeningCts;

        public PersistentTcpClientService(
            string host,
            int port,
            int connectTimeoutMs = 5000,
            int sendTimeoutMs = 5000,
            int receiveTimeoutMs = 5000,
            int maxReconnectAttempts = 5,
            int reconnectDelayMs = 1000)
        {
            _host = host;
            _port = port;
            _connectTimeoutMs = connectTimeoutMs;
            _sendTimeoutMs = sendTimeoutMs;
            _receiveTimeoutMs = receiveTimeoutMs;
            _maxReconnectAttempts = maxReconnectAttempts;
            _reconnectDelayMs = reconnectDelayMs;
        }

        public event EventHandler<string> DataReceived;

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            await _connectionLock.WaitAsync(cancellationToken);
            try
            {
                if (_tcpClient?.Connected == true && IsSocketConnected())
                    return;

                _listeningCts?.Cancel();
                _tcpClient?.Close();
                _tcpClient = new TcpClient();

                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    cts.CancelAfter(_connectTimeoutMs);
                    await _tcpClient.ConnectAsync(_host, _port).WaitAsync(cts.Token);
                }

                _networkStream = _tcpClient.GetStream();
                _networkStream.ReadTimeout = _receiveTimeoutMs;
                _networkStream.WriteTimeout = _sendTimeoutMs;

                //_listeningCts = new CancellationTokenSource();
                //_ = Task.Run(() => ListenLoopAsync(_listeningCts.Token), CancellationToken.None);
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        private bool IsSocketConnected()
        {
            try
            {
                var socket = _tcpClient.Client;
                bool readable = socket.Poll(0, SelectMode.SelectRead);
                bool hasData = socket.Available > 0;
                return !(readable && !hasData);
            }
            catch
            {
                return false;
            }
        }

        private async Task ListenLoopAsync(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    byte[] data;
                    try
                    {
                        data = await ReceiveAsync(4096, ct);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch
                    {
                        await HandleReconnectAsync(ct);
                        continue;
                    }

                    var text = Encoding.UTF8.GetString(data);
                    DataReceived?.Invoke(this, text);
                }
            }
            catch { }
        }

        public async Task SendAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            await EnsureConnectedAsync(cancellationToken);

            try
            {
                await _networkStream.WriteAsync(data, 0, data.Length, cancellationToken);
            }
            catch (Exception ex) when (IsRecoverable(ex))
            {
                await HandleReconnectAsync(cancellationToken);
                await _networkStream.WriteAsync(data, 0, data.Length, cancellationToken);
            }
        }

        public Task SendStringAsync(string message, CancellationToken cancellationToken = default)
        {
            var data = Encoding.UTF8.GetBytes(message);
            return SendAsync(data, cancellationToken);
        }

        public async Task<string> SendAndReceiveStringAsync(string message, CancellationToken cancellationToken = default)
        {
            await SendStringAsync(message, cancellationToken);
            return await ReceiveStringAsync(4096, cancellationToken);
        }

        public async Task<byte[]> ReceiveAsync(int maxBytes = 4096, CancellationToken cancellationToken = default)
        {
            await EnsureConnectedAsync(cancellationToken);

            var buffer = new byte[maxBytes];
            int bytesRead;

            try
            {
                bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
            }
            catch (Exception ex) when (IsRecoverable(ex))
            {
                await HandleReconnectAsync(cancellationToken);
                bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
            }

            if (bytesRead == 0)
                throw new SocketException((int)SocketError.ConnectionReset);

            var result = new byte[bytesRead];
            Array.Copy(buffer, result, bytesRead);
            return result;
        }

        public async Task<string> ReceiveStringAsync(int maxBytes = 4096, CancellationToken cancellationToken = default)
        {
            var data = await ReceiveAsync(maxBytes, cancellationToken);
            return Encoding.UTF8.GetString(data);
        }

        private async Task EnsureConnectedAsync(CancellationToken cancellationToken)
        {
            if (!(_tcpClient?.Client != null && IsSocketConnected() && _networkStream != null))
            {
                await HandleReconnectAsync(cancellationToken);
            }
        }

        private bool IsRecoverable(Exception ex)
        {
            return ex is IOException || ex is SocketException || ex is ObjectDisposedException;
        }

        private async Task HandleReconnectAsync(CancellationToken cancellationToken)
        {
            for (int attempt = 1; attempt <= _maxReconnectAttempts; attempt++)
            {
                try
                {
                    await ConnectAsync(cancellationToken);
                    return;
                }
                catch
                {
                    if (attempt == _maxReconnectAttempts)
                        throw new InvalidOperationException($"Failed to reconnect after {attempt} attempts.");
                    await Task.Delay(_reconnectDelayMs, cancellationToken);
                }
            }
        }

        public void Disconnect()
        {
            _listeningCts?.Cancel();
            _networkStream?.Close();
            _tcpClient?.Close();
        }

        public void Dispose() => Disconnect();
    }
}
