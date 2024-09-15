using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.LogDataFile.Models.NMRecord;

public class SyslogItem : NMRecord
{
    public long Time { get; set; }
    public string Topic { get; set; }
    public string Msg { get; set; }
    public string? Color { get; set; }

    public SyslogItem()
    { }
    public SyslogItem(long dt, string topic, string msg)
    {
        Time = dt;
        Topic = topic;
        Msg = msg;
    }
    public SyslogItem(long dt, string topic, string msg, string c)
    {
        Time = dt;
        Topic = topic;
        Msg = msg;
        Color = c;
    }
}