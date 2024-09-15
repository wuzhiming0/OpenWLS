using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Drawing;

using System.Text.Json.Serialization;

namespace OpenWLS.Server.Base
{
	/// <summary>
	/// Summary description for BinaryDataBlock.
	/// </summary>
	public class BinaryDataBlock
	{
		DataReader r;
		DataWriter w;
		FileStream fileStream;
        DataRow def_row;

		protected long location;

		public BinaryDataBlock()
		{
		}

        public DataRow GetParametersRow(string strName)
        {
            DataTable dt = (DataTable)def_row["Items"];

            foreach (DataRow dr in dt.Rows)
            {
                if ((string)dr["Name"] == strName)
                    return dr;
            }
            return null;
        }

        static public DataRow GetDefRow(DataTable dt, string name)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if( Convert.ToString( dr["Name"])  == name)
                    return dr;
            }
            return null;
        }

        public void SetDefination(DataTable dt, string name, long loc)
        {
            DataRow dr = GetDefRow(dt, name);
            if(dr != null)
                SetDefination( dr,   loc );
        }
        public void SetDefination(DataRow dr,  long loc )
		{
//			fileStream = fs;
            def_row = dr;
//			def = (BinBlockDefination)JsonSerializer.Deserialize<BinBlockDefination>(defStr); ;
            location = loc >= 0? loc : Convert.ToInt64(dr["Location"]);
   //         if(location >=0 )
//			    fs.Seek(location, SeekOrigin.Begin);
            bool b = Convert.ToString(dr["LittleEndian"]) == "false"? false : true;
            r = new DataReader( Convert.ToInt32(dr["Length"]));
			w = new DataWriter( r.GetBuffer());
			r.SetByteOrder(b);
			w.SetByteOrder(b);
		}

        public void LoadDataBlock(Stream stream)
        {
            stream.Seek(location, SeekOrigin.Begin);
            stream.Read(r.GetBuffer(), 0, r.Length);
            r.Seek(0, SeekOrigin.Begin);
            w.Seek(0, SeekOrigin.Begin);
        }

		public object this[string strName]
		{
			get
			{
				return GetDataObject(GetParametersRow(strName));
			}

			set
			{
				SetDataObject(GetParametersRow(strName), value);
			}
		}

		void SetDataObject(DataRow dr, object data)
		{
			if( dr == null || data == null )
				return;
		}


		object GetDataObject(DataRow dr)
		{
			if(dr == null)
				return null;
			r.Seek(Convert.ToInt32(dr["Location"]), SeekOrigin.Begin);
			string dataType = (string)dr["DataType"];
			int count = dr["Count"] != DBNull.Value? Convert.ToInt32(dr["Count"]) : 1;
			if(dataType == "string")
                return r.ReadString(Convert.ToInt32(dr["Count"]));
			if(dataType == "byte")
			{
				if(count == 1)
					return r.ReadByte();
				byte[] ba = new byte[count];
				for(int i = 0; i < count; i++)
					ba[i] = r.ReadByte();
				return ba;
			}
			if(dataType == "sbyte")
			{
				if(count == 1)
					return r.ReadSByte();
				sbyte[] sba = new sbyte[count];
				for(int i = 0; i < count; i++)
					sba[i] = r.ReadSByte();
				return sba;
			}
			if(dataType == "short")
			{
				if(count == 1)
					return r.ReadInt16();
				short[] si16a = new short[count];
				for(int i = 0; i < count; i++)
					si16a[i] = r.ReadInt16();
				return si16a;
			}
			if(dataType == "ushort")
			{
				if(count == 1)
					return r.ReadUInt16();
				ushort[] usi16a = new ushort[count];
				for(int i = 0; i < count; i++)
					usi16a[i] = r.ReadUInt16();
				return usi16a;
			}
			if(dataType == "int")
			{
				if(count == 1)
					return r.ReadInt32();
				int[] i32a = new int[count];
				for(int i = 0; i < count; i++)
					i32a[i] = r.ReadInt32();
				return i32a;
			}
			if(dataType == "uint")
			{
				if(count == 1)
					return r.ReadUInt32();
				uint[] ui32a = new uint[count];
				for(int i = 0; i < count; i++)
					ui32a[i] = r.ReadUInt32();
				return ui32a;
			}
			if(dataType == "single" || dataType == "float")
			{
				if(count == 1)
					return r.ReadSingle();
				float[] sa = new float[count];
				for(int i = 0; i < count; i++)
					sa[i] = r.ReadSingle();
				return sa;
			}
			if(dataType == "double")
			{
				if(count == 1)
					return r.ReadDouble();
				double[] da = new double[count];
				for(int i = 0; i < count; i++)
					da[i] = r.ReadDouble();
				return da;
			}				
			return null;
		}


        public void SetByteOrder(bool littleEndian)
		{
			r.SetByteOrder(littleEndian);
			w.SetByteOrder(littleEndian);
		}

		bool NotEmptyData(DataRow dr)
		{
			string dataType = Convert.ToString(dr["DataType"]);
			int l = dr["count"] == DBNull.Value? 1 : Convert.ToInt32(dr["Count"]);
			this.r.Seek(Convert.ToInt32(dr["Location"]), SeekOrigin.Begin);
			if(dataType == "string")
			{
				l = l * Convert.ToInt32(dr["Length"]);
				for(int i = 0; i < l; i ++)
				{
					byte d = r.ReadByte();
					if( d != 0 && d != 32)
						return true;
				}
			}
			else
			{
				l = l * GetDataTypeSize(dataType);
				for(int i = 0; i < l; i ++)
				{
					if(r.ReadByte() != 0 )
						return true;
				}
			}
			return false;

		}

	
		int GetDataTypeSize(string dataType)
		{
			if(dataType == "short" || dataType == "ushort")
				return 2;
			if(dataType == "int" || dataType == "unit" || dataType == "single" || dataType == "double")
				return 4;
			if(dataType == "long" || dataType == "ulong" || dataType == "double" )
				return 8;
//			if(dataType == "byte" || dataType == "sbyte")
//				return 1;
			return 1;
		}




	}
}
