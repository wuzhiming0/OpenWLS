using System;
using System.IO;
using System.Collections;
using System.Drawing.Design;
using System.ComponentModel;


namespace OpenWLS.Server.LogDataFile.XTF
{

	public enum XtfItemType { FileHead = 0, Curve, Wellvar, Namezone, ServiceHeader, Zonelist, AppDefined, UserDefined };
		
	[FlagsAttribute]
	public enum XtfItemStatus { DefinationModified = 1, DataModified = 2, Deleted = 4, NewCreated = 8, Temporary = 16, AllBuffered = 32};
 
	/// <summary>
	/// XtfItem is the base class for all XTF data types
	/// 
	/// </summary>
	public class XtfItem
	{
		/// <summary>
		/// name
		/// </summary>
		public string			name;
		/// <summary>
		/// status
		/// </summary>
		public XtfItemStatus	status;
		/// <summary>
		/// data type
		/// </summary>
		public XtfItemType		itemType;
		/// <summary>
		/// size in block of 4096 bytes 
		/// </summary>
		protected int		dataSize;
		/// <summary>
		/// start position from the begining of the file
		/// </summary>
		protected int		begin;

		/// <summary>
		/// xtf File Stream
		/// </summary>
		[NonSerialized()]protected FileStream fs;

		#region Properties
		[BrowsableAttribute(false)]
		public virtual int Begin
		{
			get 
			{
				return begin;
			}
			set
			{
				begin = value;
			}
		}
		[BrowsableAttribute(false)]
		public int Size
		{
			get
			{
				return dataSize;
			}
			set
			{
				dataSize = value;
			}
		}
		[BrowsableAttribute(false)]
		public int End
		{
			get 
			{
				return begin + dataSize - 1;
			}
		}
		#endregion

		public XtfItem()
		{
			status = 0;
		}
		public virtual int ComputeDataSize()
		{
			return dataSize;
		}


	}





}
