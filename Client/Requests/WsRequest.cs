using OpenWLS.Client.LogInstance;
using OpenWLS.Server.Base;
using OpenWLS.Server.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace OpenWLS.Client.Requests
{
    public interface IWsMsgProc
    {
        void ProcessWsRxMsg(DataReader r);
        void RequestEnd();
   //     ClientWebSocket GetWebSocket();
        void SetWebSocket(ClientWebSocket? ws);

  //      void SetCancellationToken(ClientWebSocket? ws);
    }
        
    public class WsRequest
    {
        public const int ws_buffer_size = 0x10000;
        public async static Task StartWs(IWsMsgProc msg_proc)
        {        
            byte[] rx_buffer = new byte[ws_buffer_size];
           // ClientWebSocket? webSocket;
            var uri = new Uri(ClientGlobals.WsUri);
            var cancellationToken = new CancellationToken();

            while (!cancellationToken.IsCancellationRequested)
            {
                using (var client = new ClientWebSocket())
                {
                    try
                    {
                        await client.ConnectAsync(uri, cancellationToken);
                        msg_proc.SetWebSocket(client);
                        ClientGlobals.SysLog.AppendMessage("Connected to WebSocket server.\n");
                        while (client.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                        {
                            var result = await client.ReceiveAsync(new ArraySegment<byte>(rx_buffer), CancellationToken.None);
                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                                ClientGlobals.SysLog.AddMessage("Server Closed WebSocket.");
                            }
                            else
                            {
                                msg_proc.ProcessWsRxMsg(new DataReader(rx_buffer, 0, result.Count));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if(!((LiClientMainCntl)msg_proc).WsClosed)
                            ClientGlobals.SysLog.AddMessage($"WebSocket error: {ex.Message}", System.Windows.Media.Colors.Red);
                    }
                }
                if (!((LiClientMainCntl)msg_proc).WsClosed)
                {
                    if (MessageBox.Show("Reconnect to server?", "Question", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {   
                        // Wait for a while before reconnecting
                        await Task.Delay(1000, cancellationToken);
                        continue;
                    }
                }
                CancellationTokenSource newSource = new CancellationTokenSource();
                cancellationToken = newSource.Token;
                newSource.Cancel();

            }
            msg_proc.SetWebSocket(null);
        }

        public static async Task Request(byte[] tx_buffer, IWsMsgProc msg_proc)
        {
            ClientWebSocket webSocket = null;
            try
            {

                webSocket = new ClientWebSocket();
                webSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(10);

                await webSocket.ConnectAsync(new Uri(ClientGlobals.WsUri), CancellationToken.None);
                await webSocket.SendAsync(new ArraySegment<byte>(tx_buffer), WebSocketMessageType.Binary, true, CancellationToken.None);
                byte[] rx_buffer = new byte[WsService.ws_buffer_size];
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(rx_buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    }
                    else
                    {
                        msg_proc.ProcessWsRxMsg(new DataReader(rx_buffer, 0, result.Count));
                    }
                }
            }
            catch (Exception ex)
            {
                ClientGlobals.SysLog.AddMessage(ex.Message, 0xffff0000);
            }
            finally
            {
                msg_proc.RequestEnd();
                if (webSocket != null)
                    webSocket.Dispose();
            }
        }


    }
}
