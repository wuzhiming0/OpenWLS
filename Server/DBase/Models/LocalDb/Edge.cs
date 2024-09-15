using OpenWLS.Server.LogInstance.Edge;

namespace OpenWLS.Server.DBase.Models.LocalDb
{
    public enum EdgeType { OpenWLS_Std = 0, Edge_xxx = 1, Edge_yyy = 2 };
    public class Edge
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? IpAddr { get; set; }
        public int? Port { get; set; }
        public EdgeType? EType { get; set; }
    //    public bool? Selected { get; set; }
        public bool? Deleted { get; set; }        
        public void CloneFrom(Edge from)
        {
            Id = from.Id;
            Name = from.Name;   
            IpAddr = from.IpAddr;
            Port = from.Port;
            EType = from.EType;
     //       Selected = from.Selected;
        }

    }
}
