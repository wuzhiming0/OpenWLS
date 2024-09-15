
using System.Net;

namespace OpenWLS.Server.WebSocket
{
    public class WsTxBuffer
    {
        public int MaxSize { get; set; }
        System.Net.WebSockets.WebSocket webSocket;
        Queue<byte[]> tx_q;
        bool tx_busy;
        public WsTxBuffer(System.Net.WebSockets.WebSocket ws)
        {
            webSocket = ws;
            tx_q = new Queue<byte[]>();
            tx_busy = false;
            MaxSize = 1024;
        }

        public bool WriteBuffer(byte[] bs)
        {
            if (tx_q.Count >= MaxSize)
                return false;
            tx_q.Enqueue(bs);
            if(!tx_busy)
                Task.Run(() => TxLoop());
            return true;
        }

        void TxLoop()
        {
            tx_busy = true;
            while(tx_q.Count > 0)
            {
                byte[]? bs;
                if( tx_q.TryDequeue(out bs) && bs != null)
                    webSocket.SendAsync(bs, System.Net.WebSockets.WebSocketMessageType.Binary, true, CancellationToken.None);
            }
            tx_busy = false;
        }


    }
}
