namespace OpenWLS.Server.DBase.Models.LocalDb
{
    public class SyslogItem
    {
        public int Id { get; set; }
        public long Time { get; set; }
         public uint? Color { get; set; }
        public string? Module { get; set; }
        public string Msg { get; set; }
    }
}
