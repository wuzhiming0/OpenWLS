using System;
using System.IO;
using System.ComponentModel;

using OpenWLS.Server.Base;



namespace OpenWLS.Server.LogDataFile.XTF
{
	/// <summary>
	/// 
	/// </summary>
	public class XtfCompositeCurveHead
	{
		public string	elementName;
		public short	dataType;
		public short	numberDims;
		public short[]	dims = new short[4];
		public int		byteOffset;
		public string	elementUnits;
		public string	description;
		public int		emptyValue;
//		internal XtfCurveData	data;
//		int[]	spares = new int[4];
		#region  Properties
		[Category("Definition")]
		[Description("Name")]
		public string ElementName
		{	
			get
			{	return	elementName;	}
//			set
//			{		}
		}
		[Category("Definition")]
		[Description("Name")]
		public short DataType
		{	
			get
			{	return	dataType;	}
//			set
//			{		}
		}
		[Category("Definition")]
		[Description("Number of dimensions")]
		public short NumberDims
		{	
			get
			{	return	numberDims;	}
//			set
//			{		}
		}
		[Category("Definition")]
		[Description("Array of four (4) dimensions")]
		public short[] Dims
		{	
			get
			{	return	dims;	}
//			set
//			{		}
		}
		[Category("Definition")]
		[Description("Data start point")]
		public int ByteOffset
		{	
			get
			{	return	byteOffset;	}
//			set
//			{		}
		}
		[Category("Definition")]
		[Description("Unit of measure")]
		public string ElementUnits
		{	
			get
			{	return	elementUnits;	}
//			set
//			{		}
		}
		[Category("Definition")]
		[Description("Description")]
		public string Description
		{	
			get
			{	return	description;	}
//			set
//			{		}
		}
		[Category("Definition")]
		[Description("Empty or initialization value")]
		public int EmptyValue
		{	
			get
			{	return	emptyValue;	}
//			set
//			{		}
		}

		#endregion
			
		public XtfCompositeCurveHead()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public void ReadDefinition(FileStream fs, bool bNormalOrder)
		{
			DataReader r = new DataReader(fs,  128);
			r.SetByteOrder(bNormalOrder);
			elementName = r.ReadString(20);
			dataType = r.ReadInt16();
			numberDims = r.ReadInt16();
			for(int i = 0; i < dims.Length; i++)
				dims[i] = r.ReadInt16();
			byteOffset =  r.ReadInt32();
			elementUnits = r.ReadString(20);
			description = r.ReadString(48);
			emptyValue = r.ReadInt32();
//			for(int i = 0; i < spares.Length; i++)
//				spares[i] = r.ReadInt32();
		}


	}
}
