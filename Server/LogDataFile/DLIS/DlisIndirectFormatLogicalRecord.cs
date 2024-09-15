using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenWLS.Server.Base;

using System.IO;

namespace OpenWLS.Server.LogDataFile.DLIS;

    public class DlisIndirectFormatLogicalRecord
    {
        int bufferDataLength;
	int origin;
	int copy;
	string indentifier;
	int modifier;
	DataReader reader;
	public string Indentifier
	{
		get
		{
			return indentifier;
		}
	}
	public DataReader DataReader
	{
		get
		{
			return reader;
		}
	}
	public int BufferDataLength
	{
		get
		{
			return bufferDataLength;
		}
	}
	public DlisIndirectFormatLogicalRecord()
	{


	}

	public void ReadDDR(DataReader r, int segmentLength)
	{
		bufferDataLength = segmentLength + r.Position;
		//OBNAME
//			if(firstSegment)
		{
			origin = (int)r.ReadDataObject(DlisDataType.ORIGIN);
			copy = (byte)r.ReadByte();
			indentifier = (string)r.ReadDataObject(DlisDataType.IDENT);

			modifier = (int)r.ReadDataObject(DlisDataType.UVARI);  //V2 ????? 
		}
		this.reader = r;
	}
}   
