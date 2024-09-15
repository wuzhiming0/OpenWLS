using System.Text.Json.Serialization;
using OpenWLS.Server.Base;
using OpenWLS.Server.DBase;
using OpenWLS.Server.GView.Models;
using OpenWLS.Server.GView.ViewDefinition;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.LogDataFile.Models;
using System.Text;

namespace OpenWLS.Server.WebSocket
{
    public partial class WsService
    {
        public void GViewGenerateGvDocFromVdf(System.Net.WebSockets.WebSocket webSocket, DataReader r, int count )
        {
            GvStreamWs streanWs = new GvStreamWs( webSocket );  
            bool rt = r.ReadByte() != 0;
            r.Seek( 1, SeekOrigin.Current );
            string doc_json = r.ReadStringWithSizeInt32();

            if (r.Position == count)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    ISyslogRepository syslog = scope.ServiceProvider.GetRequiredService<ISyslogRepository>();
                    VdDocument vdDocument = VdDocument.FromJson(doc_json, rt);
                    vdDocument.CreateTopInsert(0, null, streanWs).AddItem(new GvEOS());
                    vdDocument.CreatePlotArea(1, null, streanWs, rt, syslog ).AddItem(new GvEOS());
                }
            }
        }
    

    }

    public class GvStreamWs : IGvStream
    {
        System.Net.WebSockets.WebSocket webSocket;

        public GvStreamWs(System.Net.WebSockets.WebSocket ws)
        {
            webSocket = ws;
        }

        public void SendObject(IGvStreamObject ob, GvDocument doc)
        {
            byte[] bytes = ob.GetBytes(doc);
            ArraySegment<byte> bs = new ArraySegment<byte>(bytes, 0, bytes.Length);
            webSocket.SendAsync(bs, System.Net.WebSockets.WebSocketMessageType.Binary, true, CancellationToken.None);
        }


    }
}
