using System.Text.Json;
using OpenWLS.Server.Base;
using OpenWLS.Server.DBase;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.LogDataFile.Models;
using System.Text;

namespace OpenWLS.Server.WebSocket
{
    public partial class WsService
    {
        public void LdfRead1D(System.Net.WebSockets.WebSocket webSocket, DataReader r, int count )
        {
            int s = r.ReadUInt16();
            int[] ids = new int[s];
            for (int i = 0; i < s; i++) ids[i] = r.ReadInt32();
            string fn = r.ReadStringWithSizeUint16();
            if (r.Position == count)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    ISyslogRepository syslog = scope.ServiceProvider.GetRequiredService<ISyslogRepository>();
                    using (DataFile df = DataFile.OpenDataFile(fn, syslog))
                    {
                        foreach (int mid in ids)
                        {
                            MVBlocksWs mvbs = new MVBlocksWs(df, mid);
                   //         mvbs.SendBlockInfor(webSocket);
                            mvbs.SendValue(df,webSocket);
                        }
                    }
                }
            }
        }
        public void LdfReadXD(System.Net.WebSockets.WebSocket webSocket, DataReader r, int count )
        {
            int mid = r.ReadInt32();
            string fn = r.ReadStringWithSizeUint16();
            if (r.Position == count)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    ISyslogRepository syslog = scope.ServiceProvider.GetRequiredService<ISyslogRepository>();
                    using (DataFile df = DataFile.OpenDataFile(fn, syslog))
                    {
                        MVBlocksWs mvbs = new MVBlocksWs(df, mid);
                //        mvbs.SendBlockInfor(webSocket);
                        mvbs.SendValue(df, webSocket);
                    }
                }
            }
        }

    }

    public class MVBlocksWs : MVBlocks
    {
        public MVBlocksWs(SqliteDataBase db, int mid) : base(db, mid) { }
        public void SendBlockInfor(System.Net.WebSockets.WebSocket webSocket)
        {
            //   int head_size = 6;                          // 2 ( response id ) , 4 (mid)   
            byte[] tx_buffer = new byte[WsService.ws_buffer_size];
            string str = JsonSerializer.Serialize(this);
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            DataWriter w = new DataWriter(tx_buffer);
            w.WriteData(WsService.response_ldf_mv_blocks);
            w.WriteData(MId);
            w.WriteData((ushort)bytes.Length);
            Buffer.BlockCopy(bytes, 0, tx_buffer, 8, bytes.Length);

            ArraySegment<byte> bs = new ArraySegment<byte>(tx_buffer, 0, 8 + bytes.Length);
            webSocket.SendAsync(bs, System.Net.WebSockets.WebSocketMessageType.Text, true, CancellationToken.None);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="df"></param>
        /// <param name="webSocket"></param>
        public void SendValue(DataFile df, System.Net.WebSockets.WebSocket webSocket)
        {
            int head_size = 16;                          // 2 ( response id ) , 4(mid), 4(block id) 4 (offset), 2 (size)    
            int load_size_max = WsService.ws_buffer_size - head_size;
            byte[] tx_buffer = new byte[WsService.ws_buffer_size];
            DataWriter w = new DataWriter(tx_buffer);
            w.WriteData(WsService.response_ldf_mv_section);
            w.WriteData(MId);

            foreach (MVBlock b in this)
            {
                byte[] v = BinObject.GetVal(df, b.Id, MVBlock.val_tble_name);
                int offset = 0;
                int size = v.Length;
                //     w.Seek(6, SeekOrigin.Begin);
                //     w.WriteData(b.Id);
                while (size > 0)
                {
                    int s = Math.Min(load_size_max, size);
                    Buffer.BlockCopy(v, offset, tx_buffer, head_size, s);
                    w.Seek(6, SeekOrigin.Begin);
                    w.WriteData(b.Id);
                    w.WriteData(offset);
                    w.WriteData((ushort)s);
                    ArraySegment<byte> bs = new ArraySegment<byte>(tx_buffer, 0, s);
                    webSocket.SendAsync(bs, System.Net.WebSockets.WebSocketMessageType.Binary, true, CancellationToken.None);
                    //          section++;
                    offset += s;
                    size -= s;
                }
            }
        }
    }
}
