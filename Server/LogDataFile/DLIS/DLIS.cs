using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenWLS.Server.Base;


namespace OpenWLS.Server.LogDataFile.DLIS;

public class DLIS
{
    public static int GetSamples(object dims)
    {
        if (dims is object[])
        {
            object[] obs = (object[])dims;
            int l = 1;
            foreach (int i in obs)
                l = l * i;
            return l;
        }
        else
            return (int)dims;
    }

    public static int GetDataBytes(DlisDataType dt)
    {
        switch (dt)
        {
            case DlisDataType.SSHORT:
            case DlisDataType.ATTREF:
            case DlisDataType.USHORT:
            case DlisDataType.STATUS:
            case DlisDataType.LOGICL:
                return 1;
            case DlisDataType.FSHORT:
            case DlisDataType.SNORM:
            case DlisDataType.UNORM:
            case DlisDataType.ISNORM:
                return 2;
            case DlisDataType.FSINGL:
            case DlisDataType.ISINGL:
            case DlisDataType.VSINGL:
            case DlisDataType.SLONG:
            case DlisDataType.ULONG:
            case DlisDataType.RNORM:
            case DlisDataType.ISLONG:
            case DlisDataType.IULONG:
            case DlisDataType.IRNORM:
                return 4;
            case DlisDataType.FSING1:
            case DlisDataType.FDOUBL:
            case DlisDataType.CSINGL:
            case DlisDataType.DTIME:
            case DlisDataType.RLONG:
            case DlisDataType.IRLONG:
            case DlisDataType.FRATIO:
                return 8;
            case DlisDataType.FSING2:
                return 12;
            case DlisDataType.FDOUB1:
            case DlisDataType.CDOUBL:
            case DlisDataType.DRATIO:
                return 16;
            case DlisDataType.FDOUB2:
            default:
                return -1;

        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    /// need check the byte order online and recode
    public static bool GetLittleEndian(DlisDataType dt)
    {
        switch (dt)
        {
            case DlisDataType.SSHORT:
            case DlisDataType.ATTREF:
            case DlisDataType.USHORT:
            case DlisDataType.STATUS:
            case DlisDataType.LOGICL:
                return true;
            case DlisDataType.FSHORT:
            case DlisDataType.SNORM:
            case DlisDataType.UNORM:
            case DlisDataType.ISNORM:
                return true;
            case DlisDataType.FSINGL:
            //return false;
            case DlisDataType.ISINGL:
            case DlisDataType.VSINGL:
            case DlisDataType.SLONG:
            case DlisDataType.ULONG:
            case DlisDataType.RNORM:
            case DlisDataType.ISLONG:
            case DlisDataType.IULONG:
            case DlisDataType.IRNORM:
                return true;
            case DlisDataType.FSING1:
            case DlisDataType.FDOUBL:
            case DlisDataType.CSINGL:
            case DlisDataType.DTIME:
            case DlisDataType.RLONG:
            case DlisDataType.IRLONG:
            case DlisDataType.FRATIO:
                return true;
            case DlisDataType.FSING2:
                return true;
            case DlisDataType.FDOUB1:
            case DlisDataType.CDOUBL:
            case DlisDataType.DRATIO:
                return true;
            case DlisDataType.FDOUB2:
            default:
                return true;

        }
    }

    public static int GetDlisDataLength(DlisDataType type)
    {
        switch (type)
        {
            case DlisDataType.USHORT:
                return 1;
            case DlisDataType.SNORM:
            case DlisDataType.UNORM:
                return 2;
            case DlisDataType.ULONG:
            case DlisDataType.FSINGL:
            case DlisDataType.SLONG:
                return 4;
            case DlisDataType.FDOUBL:
                return 8;

        }
        return 0;
    }

    public static XtfDataType GetXtfDataType(DlisDataType dlisDataType)
    {
        switch (dlisDataType)
        {
            case DlisDataType.USHORT:
                return XtfDataType.Byte;
            case DlisDataType.UNORM:
                return XtfDataType.UInt2;
            case DlisDataType.SNORM:
                return XtfDataType.Int2;
            case DlisDataType.SLONG:
                return XtfDataType.Int4;
            case DlisDataType.ULONG:
                return XtfDataType.UInt4;
            case DlisDataType.FSINGL:
                return XtfDataType.Single;
            case DlisDataType.FDOUBL:
                return XtfDataType.Double;
            default:
                return XtfDataType.Unknown;
        }
    }

    public static int ReadType(string strtype)
    {
        if (strtype == null)
            return -1;
        EFLogicalRecordType type;
        if (strtype.ToLower() == "fhlr")
            type = EFLogicalRecordType.FHLR;
        else if (strtype.ToLower() == "olr")
            type = EFLogicalRecordType.OLR;
        else if (strtype.ToLower() == "axis")
            type = EFLogicalRecordType.AXIS;
        else if (strtype.ToLower() == "channel")
            type = EFLogicalRecordType.CHANNL;
        else if (strtype.ToLower() == "frame")
            type = EFLogicalRecordType.FRAME;
        else if (strtype.ToLower() == "static")
            type = EFLogicalRecordType.STATIC;
        else if (strtype.ToLower() == "script")
            type = EFLogicalRecordType.SCRIPT;
        else if (strtype.ToLower() == "update")
            type = EFLogicalRecordType.UPDATE;
        else if (strtype.ToLower() == "udi")
            type = EFLogicalRecordType.UDI;
        else if (strtype.ToLower() == "lname")
            type = EFLogicalRecordType.LNAME;
        else if (strtype == "spe")
            type = EFLogicalRecordType.SPEC;
        else if (strtype.ToLower() == "dict")
            type = EFLogicalRecordType.DICT;
        else
            type = EFLogicalRecordType.RESERVE;

        return type.GetHashCode(); ;
    }

    public DlisDataType ConvertXtfToDlisDataType(XtfDataType xtfDataType)
    {
        switch (xtfDataType)
        {
            case XtfDataType.Byte:
                return DlisDataType.USHORT;

            case XtfDataType.UInt2:
                return DlisDataType.UNORM;

            case XtfDataType.Int2:
                return DlisDataType.SNORM;

            case XtfDataType.Int4:
                return DlisDataType.SLONG;

            case XtfDataType.UInt4:
                return DlisDataType.ULONG;

            case XtfDataType.Single:
                return DlisDataType.FSINGL;

            case XtfDataType.Double:
                return DlisDataType.FDOUBL;

        }

        return 0;
    }

}
