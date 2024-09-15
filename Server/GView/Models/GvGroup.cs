using OpenWLS.Server.Base;
using OpenWLS.Server.WebSocket;

namespace OpenWLS.Server.GView.Models
{
    public class GvGroup : IGvStreamObject
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Desc { get; set; }   //Description        

        public void Restore(DataReader r)
        {
            Id = r.ReadInt32();                 //4
            Name = r.ReadStringWithSizeByte();
            Desc = r.ReadStringWithSizeByte();
        }

        public byte[] GetBytes(GvDocument doc)
        {
            byte[] name_bs = StringConverter.ToByteArrayWithSizeByte(Name);
            byte[] desc_bs = StringConverter.ToByteArrayWithSizeByte(Desc);

            int s = 6 + name_bs.Length + desc_bs.Length;
            DataWriter w = doc.GetDataWriter(s);
            w.WriteData(WsService.response_gv_group);   //2
            w.WriteData(Id);                            //4
            w.WriteData(name_bs);
            w.WriteData(desc_bs);

            return w.GetBuffer();
        }

    }
    public class GvGroups : List<GvGroup>
    {

    }
}
