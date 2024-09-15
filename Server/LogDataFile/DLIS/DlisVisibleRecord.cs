using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using OpenWLS.Server.Base;

namespace OpenWLS.Server.LogDataFile.DLIS;

public struct DlisVRecordTrailer
{
    byte[] paddingBytes;
}


public class DlisVisibleRecord
{
    //head
    int length;
    byte valueFF;
    byte formatVersion;
    int sequence;
    short section;

    //Trailer
    int paddingCount;
    short Checksum;
    int lengthTailer;

    //
    DlisVRecordSegment segment;
    DlisVRecordTrailer trailer;
    byte[] bufFromLastVR;


    public DlisVRecordSegment VRSegment
    {
        get
        {
            return segment;
        }
    }

    public DlisVisibleRecord()
    {
        segment = new DlisVRecordSegment();
        //
        // TODO: Add constructor logic here
        //
    }


    public int ReadVRecord(FileStream fs, int vrSize, bool scan)
    {
        //	
        DataReader r = new DataReader(fs,  vrSize);
        r.SetByteOrder(false);
        length = vrSize;
        //head
        valueFF = r.ReadByte();
        formatVersion = r.ReadByte();
        int pos = r.Position;

        int l = 0; 
        while (l == 0 && (this.length - r.Position) > 4)
            l = segment.ReadSegment(r, bufFromLastVR, scan);

        bufFromLastVR = l > 0 ? r.ReadByteArray(l) : null;

        if (l >= 0)
        {
            r.Seek(pos + length - 4, SeekOrigin.Begin);
            return (int)r.ReadUInt16();
        }
        else
            return -1;
    }

}
