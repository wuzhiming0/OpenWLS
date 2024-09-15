using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWLS.Server.Base;

using System.IO;

namespace OpenWLS.Server.LogDataFile.DLIS
{

    [FlagsAttribute]
    //	public enum SegmentAttribute	{Explicitly = 1, NotFirstSegment = 2, NotLastSegment = 4, Encrypted = 8, EncryptionPacket = 16, Checksum = 32, TrailingLength = 64, PadBytes = 128};
    public enum SegmentAttribute { Explicitly = 128, NotFirstSegment = 64, NotLastSegment = 32, Encrypted = 16, EncryptionPacket = 8, Checksum = 4, TrailingLength = 2, PadBytes = 1 };

    public enum EFLogicalRecordType { FHLR = 0, OLR, AXIS, CHANNL, FRAME, STATIC, SCRIPT, UPDATE, UDI, LNAME, SPEC, DICT, RESERVE };
    public enum IFLogicalRecordType { FDATA = 0, NOFORMAT = 1, EOD = 127 };


    public class LRSEncryptionPacket
    {
        int size;
        int cmpanyCode;
        byte[] encryptionInformation;
        public void ReadSEncryptionPacket(DataReader r)
        {
            this.size = (int)r.ReadUInt16();
            this.cmpanyCode = (int)r.ReadUInt16();

        }
    }



    /// <summary>
    /// Summary description for VRecordSegment.
    /// </summary>
    public class DlisVRecordSegment
    {

        //head
        int length;
        int rdPos, rdEnd;

        SegmentAttribute attribute;
        int type;
        //trailer
        ushort checksum;

        //should be same as length in head if present
        int lengthInTrailer;

        //length of the trailer
        int trailerLength;
        int padCount;
        DlisIndirectFormatLogicalRecord iflr;

        public DlisFile DlisFile { get; set; }

        int ReadSegmentHeadTrailer(DataReader r)
        {
            trailerLength = 0;
            padCount = 1;
            //head
            length = (int)r.ReadUInt16();
            lengthInTrailer = length;
            if (length <= 0)
                return -1;
            attribute = (SegmentAttribute)r.ReadByte();
            type = r.ReadByte();
            rdPos = r.Position;
            //}
            //trailer
            r.Seek(this.length - 4, SeekOrigin.Current);

            if ((attribute & SegmentAttribute.TrailingLength) != 0)
            {
                r.Seek(-2, SeekOrigin.Current);
                this.lengthInTrailer = (int)r.ReadUInt16();
                r.Seek(-2, SeekOrigin.Current);
                trailerLength += 2;
            }

            if ((attribute & SegmentAttribute.Checksum) != 0)
            {
                r.Seek(-2, SeekOrigin.Current);
                this.checksum = r.ReadUInt16();
                r.Seek(-2, SeekOrigin.Current);
                trailerLength += 2;
            }

            if ((attribute & SegmentAttribute.PadBytes) != 0)
            {
                r.Seek(-1, SeekOrigin.Current);
                this.padCount = (int)r.ReadByte();
                r.Seek(-1, SeekOrigin.Current);
                //the padCount is the length of whole padding including the pad Count byte.  
                //		trailerLength++;
                trailerLength += padCount;

            }
            rdEnd = r.Position - padCount + 1;
            r.Seek(rdPos, SeekOrigin.Begin);
            return 0;
        }


        int ScanSegment(DataReader r, byte[] bufFromLastVR)
        {
            return 0;
        }

        DataReader GetNewReader(DataReader r, byte[] bufFromLastVR, int size)
        {
            if ((attribute & SegmentAttribute.NotFirstSegment) == 0)
                return r;
            if (bufFromLastVR == null)
                return null;
            byte[] bufTem = new byte[bufFromLastVR.Length + size];
            Buffer.BlockCopy(bufFromLastVR, 0, bufTem, 0, bufFromLastVR.Length);
            Buffer.BlockCopy(r.GetBuffer(), rdPos, bufTem, bufFromLastVR.Length, size);

            DataReader r1 = new DataReader(bufTem);
            r1.SetByteOrder(r.GetByteOrder());
            return r1;
        }

        public int ReadSegment(DataReader r, byte[] bufFromLastVR, bool scan){
            int err = ReadSegmentHeadTrailer(r);
            if (err != 0)
                return err;
            //body
            if ((attribute & SegmentAttribute.Explicitly) != 0 )
            {
                if (scan)
                {
                    while (r.Position < rdEnd)
                    {
                        int descriptor = (int)r.ReadByte();
                        EFLRComponent newComponent = EFLRComponent.CreateComponent(descriptor);

                        DlisFile.BeforeNewComponent(newComponent);
                        newComponent.ReadComponent(r);
                        DlisFile.AfterNewComponent(newComponent);
                    }
                }
            }
            else
            {
                if ((attribute & SegmentAttribute.NotLastSegment) != 0)
                    return length - 4 - trailerLength;
                else
                {
                    int l = length - 4 - trailerLength;                       
                    DataReader nr = GetNewReader(r, bufFromLastVR, l);
                    if (nr == null)
                        return -1;
                    iflr.ReadDDR(nr, r==nr? l : l + bufFromLastVR.Length);
                }
                DlisFile.ReadIndirectFormatLogicalRecord(iflr, scan);
            }

            r.Seek(rdPos + this.length - 4, SeekOrigin.Begin);
            return 0;
        }


        public DlisVRecordSegment()
        {
            iflr = new DlisIndirectFormatLogicalRecord();
            //
            // TODO: Add constructor logic here
            //
        }

    }


    public class DlisVRecordSegments : List<DlisVRecordSegment>
    {
       
    }

}
