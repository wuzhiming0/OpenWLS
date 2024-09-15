

using OpenWLS.Server.Base;
using OpenWLS.Server.LogInstance;
using System.Net;
using System.Net.WebSockets;


namespace OpenWLS.Server.WebSocket
{
    public interface IWsService
    {
        void Init(WebApplication app);
    }
    public partial class WsService : IWsService
    {
        public const int ws_buffer_size = 0x10000;

        public const ushort request_ldf_1d = 1;
        public const ushort request_ldf_xd = 2;

        public const ushort request_gvd_ldf = 3;

        public const ushort request_instance = 4;

        public const ushort response_ldf_mv_blocks = 1;
        public const ushort response_ldf_mv_section = 2;

        public const ushort response_gv_group = 3;
        public const ushort response_gv_item = 4;
        public const ushort response_gv_section = 5;

        private readonly IServiceScopeFactory _scopeFactory;

        public WsService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            //  _msgRep = msgRep;
            // Inject other services via constructor.
        }

        public void Init(WebApplication app)
        {
            app.Map("/bin", async context =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    System.Net.WebSockets.WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await WebSocketHandler(context, webSocket);
                }
                else
                {
                    // context.Response.StatusCode = 400;
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            });
        }
        private async Task WebSocketHandler(HttpContext context, System.Net.WebSockets.WebSocket webSocket)
        {
            var buffer = new byte[ws_buffer_size];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            DataReader r = new DataReader(buffer);
            ushort req = r.ReadUInt16();
            if (req == request_instance)
            {
                int msg_type = r.ReadUInt16();
                if (msg_type == (int)LiWsMsg.Connect)
                {
                    int li_id = r.ReadInt32();
                    ClientType ct = (ClientType)r.ReadUInt32();
                    using (LogInstanceWsClient linstanceWsClient = new LogInstanceWsClient(webSocket, li_id, ct, _scopeFactory))
                    {
                        while (webSocket.State == WebSocketState.Open && !result.CloseStatus.HasValue)
                        {
                            //await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                            try
                            {
                                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                                r.Seek(0, SeekOrigin.Begin);
                                linstanceWsClient.ProceRequest(r, result.Count);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
            }
            else
            {
                switch (req)
                {
                    case request_ldf_1d:
                        LdfRead1D(webSocket, r, result.Count);
                        break;
                    case request_ldf_xd:
                        LdfReadXD(webSocket, r, result.Count);
                        break;
                    case request_gvd_ldf:
                        GViewGenerateGvDocFromVdf(webSocket, r, result.Count);
                        break;

                }

            }
            if(webSocket.State == WebSocketState.Open)
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);

            //await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

        }
        /*
        private  async Task WebRxSocketHandler(HttpContext context, System.Net.WebSockets.WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }*/
    }
}
