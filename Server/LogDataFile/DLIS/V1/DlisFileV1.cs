using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWLS.Server.Base;

using System.IO;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.DBase;
using OpenWLS.Server.WebSocket;
using System.Threading.Channels;
using OpenWLS.Server.LogDataFile.Models.NMRecord;
using static OpenWLS.Server.LogDataFile.Models.NMRecord.FileVersion;
using OpenWLS.Server.LogDataFile.Models;

namespace OpenWLS.Server.LogDataFile.DLIS.V1;


public class DlisFileV1 : DlisFile
{
    public DlisFileV1()
    {
        storageUnitLabel.VersionDLIS = "V1.00";
    }

    public override DataFileInfor? ImportDLIS( int nxtRLength, ISyslogRepository syslog)
    {

        DlisVisibleRecord vr = new DlisVisibleRecord();
        vr.VRSegment.DlisFile = this;
        bool scan = true;
        //   int s = r.ReadUInt16();
        int l = nxtRLength;
            while (l >= 4 && validFile)
            l = vr.ReadVRecord(fileStream, l, scan);

        if (!validFile)
            return null;

        InitFrameChannelSet(sets);

        fileStream.Seek(82, SeekOrigin.Begin);
        scan = false;
        l = nxtRLength;
        while (l >= 4 && validFile)
            l = vr.ReadVRecord(fileStream, l, scan);
        fileStream.Close();


            
        DataFile df = DataFile.CreateDataFile(fileName + DataFile.file_ext, VersionOption.V1, syslog);
        frames.UpdateChannelHeads();

        SaveParameters(df);
        SaveMeasurments(df);

        validFile = true;

    //    frames.SetIndexRange(df.Head);
        foreach (NMRecord nmr in df.NMRecords)
            nmr.AddNMRecordToDb(df);

        //    df.AddHeadToDb();
        df.Close();
        //  df.Frames = frames;
        return df.GetFileInfor();
    }

    protected override void InitFrameChannelSet(SetComponents sets)
    {
        base.InitFrameChannelSet(sets);
        frames.Init(true, sets);

        //when constant space, no need for frame
        //check if measurement with same name in other frames
        //add frame name as postfix if yes
        foreach(DlisFrame f in frames)
        {
            if (f.LevelSpacing != null)
            {
                foreach (Measurement m in f.Measurements)
                {
                    if (CountMeasurement(m.Head.Name) > 1)
                        m.Head.Name = $"{m.Head.Name}_{f.Name}"; 
                }
            }
        }
    }
}
