using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenWLS.Server.Base;
using System.Text.Json.Serialization;
using System.Data;

namespace OpenWLS.Server.LogDataFile.Models.NMRecord;

public class FileVersion : NMRecord
{
    public enum VersionOption { V1 = 1 };

    public uint Version { get; set; }
      
    public FileVersion( )
    {

    }
    public FileVersion(VersionOption v)
    {
        RType = (int)NMRecordType.FileVersion;
        Version = (uint)v;
        Ext = Version.ToString();   
    }

    public FileVersion(string ext)
    {
        Ext = ext;
        RestoreExt();
    }

    public override void RestoreExt()
    {
        Version = uint.Parse(Ext);
    }
    public override string ToString()
    {
        return $"{Version}";
    }

    /*
    public void SaveFileVersion(DataFile df)
    {
        Ext = ToString();
        AddNMRecordToDb(df);
    }*/
}
