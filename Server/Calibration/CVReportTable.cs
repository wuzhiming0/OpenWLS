using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
//using System.Windows.Forms;
using System.Reflection;
using System.IO;

using Newtonsoft.Json;
using OpenWLS.Server.GView.Models;
using RectangleF = OpenWLS.Server.GView.Models.RectangleF;
using OpenWLS.Server.Calibration;

namespace OpenWLS.Server.LogInstance.Calibration
{
    public class CVReportTables : List<CVReportTable>
    {

        public void Init(DataTable dt)
        {
            Clear();
            foreach(DataRow dr in dt.Rows)
            {
                CVReportTable r = new CVReportTable();
                r.Restore(dr);
                Add(r);
            }
        } 
    }


	/// <summary>
	/// Summary description for CVReportTable.
	/// </summary>
	public class CVReportTable   
	{
        public string Name  { get; set; }
        public string Rows { get; set; }
        public DataTable Columns { get; set; }

    //    public string HeadFont{ get; set; }
    //    public string UnitFont{ get; set; }
    //    public string ItemFont{ get; set; }
    //    public string ToleranceFont{ get; set; }
            

          [JsonIgnore]
        public float Height { get; set; } 
          [JsonIgnore]
        public CVRecordValue DataRecord { get { return dataRecord; } }
        protected GvFont  titleFont;
        protected GvFont	unitFont;
		protected GvFont	itemFont;
		protected GvFont	columnFont;
		protected GvFont	toleranceFont;

		protected float		rowHeight;
		protected float		headHeight;
		protected int		rows;
		protected int		columns;	
		protected float[]	columnWidths;
		protected bool unitAvailable;

		protected string[]  columnStrings;
		protected string[]  rowStrings;
		protected string[]  unitStrings;
		protected byte[]  formats;
		protected int[]	  recordIndexes;	

		protected CVRecordValue dataRecord;

		public CVReportTable()
		{
            float s = (float)9.0 / 96;
			columnFont = new GvFont("Arial", s , GvFontStyle.Bold);
            titleFont = new GvFont("Arial", s, GvFontStyle.Bold);
            unitFont = new GvFont("Arial", s);
			itemFont = new GvFont("Tahoma", s);
            s = (float)8.0 / 96;
			toleranceFont = new GvFont("Arial", s, GvFontStyle.Italic);
			// TODO: Add any initialization after the InitializeComponent call
			unitAvailable = false;
		}

        public void Restore(DataRow dr)
        {
            DataTable dt = dr.Table;
            
            if (dt.Columns.Contains("HeadFont"))
            {
                if (dr["HeadFont"] != DBNull.Value)
                columnFont = GvFont.CreateFont((string)dr["headFont"]);
            }
            if (dt.Columns.Contains("UnitFont"))
            {
                if (dr["UnitFont"] != DBNull.Value)
                    unitFont = GvFont.CreateFont((string)dr["UnitFont"]);
            }
            if (dt.Columns.Contains("ItemFont"))
            {
                if (dr["ItemFont"] != DBNull.Value)
                    itemFont = GvFont.CreateFont((string)dr["ItemFont"]);
            }
            if (dt.Columns.Contains("ToleranceFont"))
            {
                if (dr["ToleranceFont"] != DBNull.Value)
                    toleranceFont = GvFont.CreateFont((string)dr["ToleranceFont"]);
            }

            Columns = (DataTable)dr["Columns"];
            Rows = (string)dr["Rows"];
            Name = (string)dr["Name"];
           
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="dRecord"></param>
		/// <returns>the height of thie report</returns>
		public void LoadDatRecord(CVRecordValue dRecord)
		{

			dataRecord = dRecord;
            string[] strItems = Rows.Split(new char[] { ';', ',' });
            columns = Columns.Rows.Count;
			columnStrings = new string[columns];
			unitStrings = new string[columns];
			formats = new byte[columns];
			columnWidths = new float[columns];

			for(int i = 0; i < columns; i++)
			{
                object ob = Columns.Rows[i]["Text"];
                if(ob != DBNull.Value)
                    columnStrings[i] = (string)ob;
                columnWidths[i] = Convert.ToSingle(Columns.Rows[i]["Width"]);

                if (Columns.Columns.Contains("Unit") && Columns.Rows[i]["Unit"] != DBNull.Value)
				{
                    unitStrings[i] = "(" + (string)Columns.Rows[i]["Unit"] + ")";
					unitAvailable = true;
				}
                if (Columns.Columns.Contains("Format"))
                {
                    ob = Columns.Rows[i]["Format"];
                    if (ob != DBNull.Value)
                        formats[i] = Convert.ToByte(ob);
                }

			}
			rows = strItems.Length / columns;
			rowStrings = new string[rows];
			int k = 0;
			int m = 0;
			recordIndexes = new int[(columns-1) * rows ];
			for(int i = 0; i < rows; i++)
			{
				rowStrings[i] = strItems[k++].Trim();
				for(int j = 1; j < columns; j++)
					recordIndexes[m++] =  this.dataRecord.GetCV1DValueIndex(strItems[k++].Trim());
			}
            float g = (float)0.02;
            rowHeight = (toleranceFont.Size + itemFont.Size) * (float)1.5 + g;
            headHeight = columnFont.Size * (float)1.5 + g;

            if (!string.IsNullOrEmpty(Name))
                headHeight += titleFont.Size * (float)1.5;
            if (unitAvailable)
                headHeight += unitFont.Size * (float)1.5;

            Height = rows * rowHeight + headHeight + g;
            //	Height =  rows * rowHeight + headHeight + g + (float)0.5;		
        }

        public float AddItemsToAGDoc(AGvDocument doc, float yOffset)
        {
            float g = (float)0.02;
            float height_row = columnFont.Size * (float)1.5 + g;
            float height_unit = unitFont.Size * (float)1.5 + g;
            RectangleF rect;
            doc.AddItem(unitFont);
            doc.AddItem(itemFont);
            doc.AddItem(columnFont);
            doc.AddItem(toleranceFont);
            uint colorDBlue = 0xff000088;
            float liftMargin = (float)0.5;
            float left = liftMargin;
            float w;

            //    float t = headHeight + yOffset;   
            if (!string.IsNullOrEmpty(Name))
            {
                w = 0;
                for (int i = 0; i < this.columns; i++)
                    w += columnWidths[i];
                rect = new RectangleF(columnWidths[0], yOffset, w, columnFont.Size * (float)1.5);
                doc.AddText(Name, rect.X, rect.Y, rect.Width, colorDBlue, columnFont, (ushort)GvTextAlignment.Center);
                yOffset += rect.Height + g; 
            }

            for (int i = 0; i < this.columns; i++)
            {
                w = columnWidths[i];
                rect = new RectangleF(left, yOffset, w, height_row);
                //				left += w;
                //column names row
                if (columnStrings[i] != null)
                    doc.AddText(columnStrings[i], rect.X, rect.Y,  rect.Width, colorDBlue,  columnFont, (ushort)GvTextAlignment.Center);

                rect.Y = rect.Height  + yOffset;
                if (unitAvailable)
                {
                    rect.Height = unitFont.Size * (float)1.5 ;
                    //units row
                    if (unitStrings[i] != null)
                        doc.AddText(unitStrings[i], rect.X, rect.Y, rect.Width, colorDBlue, unitFont, (ushort)GvTextAlignment.Center);
                    rect.Y = rect.Bottom;
                }
            //    rect.Y += (float)0.04;
                if (i == 0)
                {
                    for (int j = 0; j < rows; j++)
                    {
                        doc.AddText(rowStrings[j], rect.X, rect.Y, rect.Width- (float)0.04, colorDBlue, columnFont, (ushort)GvTextAlignment.Right);
                        rect.Y += rowHeight;
                    }
                }
                left += w;
            }

            GvNumber[]  fs = new GvNumber[columns];
            for (int j = 0; j < columns; j++)
            {
                GvNumber f = doc.GetFormat(formats[j], itemFont, toleranceFont);
                if(f == null){
                    f = new GvNumber(formats[j], itemFont, toleranceFont);
                    doc.AddItem(f);
                }
                fs[j] = f;
            }

            left = columnWidths[0] + liftMargin;
            //    float top = yOffset + height_row + (float)0.04;
            float top = yOffset + height_row;
            if (unitAvailable)
                top += height_unit;

            int k = 0;
            for (int i = 0; i < rows; i++)
            {
                rect = new RectangleF(left, top, 0, rowHeight);
                for (int j = 1; j < columns; j++)
                {
                    rect.Width = columnWidths[j];
                    int m = recordIndexes[k];

                    if( m >= 0  )
                    {
                       // GvNumber v = GvNumber.FromCV1DValue(dataRecord[m], fs[j], rect.X, rect.Y, rect.Width, rect.Height);
                       // doc.AddItem(v);
                    }
                    k++;
                    rect.X += columnWidths[j];
                }
                top += rowHeight;
            }
            return Height + yOffset;
        }



        

/*
        void UpdateReportData(C1.C1Pdf.C1PdfDocument pdfDoc, bool drawAllItems, int y)
        {
            int k = 0;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Far;
            int top = columnFont.Height + y;
            if (unitAvailable)
                top += columnFont.Height;
            top++;
            int left = columnWidths[0];

            int w1 = 0;
            for (int i = 1; i < this.columns; i++)
                w1 += columnWidths[i];
            for (int i = 0; i < rows; i++)
            {
                RectangleF rect = new RectangleF(left, top, 0, rowHeight);
                if (drawAllItems)
                    pdfDoc.FillRectangle(Brushes.White, columnWidths[0], top, w1, itemFont.Height);

                for (int j = 1; j < columns; j++)
                {
                    rect.Width = columnWidths[j];
                    int m = recordIndexes[k];

                    if (m >= 0 && (drawAllItems || dataRecord[m].HighLight))
                        dataRecord[m].DrawItem(pdfDoc, rect, this.itemFont, this.toleranceFont, this.formatStrings[j], sf);
                    k++;
                    rect.X += columnWidths[j];
                }
                top += rowHeight;
            }
        }


		public void DrawReport(Graphics g)
		{

			//RectangleF rect;
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Far;

			int t = headHeight;
			g.FillRectangle(Brushes.Gainsboro,0,0,this.Width, this.Height);

			int left = 0;
			for(int i = 0; i < this.columns; i++ )
			{
				int w = this.columnWidths[i];
				RectangleF rect = new RectangleF(left, 0, w, columnFont.Height);
				//				left += w;
				//column names row
				if(columnStrings[i] != null)
					g.DrawString(columnStrings[i], this.columnFont, Brushes.DarkBlue, rect, sf);

				rect.Y = rect.Height;
				if(unitAvailable)
				{
					rect.Height = unitFont.Height;
					//units row
					if(unitStrings[i] != null)
						g.DrawString(unitStrings[i], this.unitFont, Brushes.DarkBlue, rect, sf);	

					rect.Y = rect.Bottom;
				}
				rect.Y += 1;
				if( i == 0)
				{
					for(int j = 0; j < rows; j ++ )
					{
						g.DrawString(this.rowStrings[j], this.columnFont, Brushes.DarkBlue, rect, sf);	
						rect.Y +=  rowHeight;
					}
				}

				left += w; 
			}
			UpdateReportData(g, true);
		}

        public void DrawReport(C1.C1Pdf.C1PdfDocument pdfDoc, int y)
        {

            //RectangleF rect;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Far;

            int t = headHeight;
            pdfDoc.FillRectangle(Brushes.Gainsboro, 0, 0, this.Width, this.Height);

            int left = 0;
            for (int i = 0; i < this.columns; i++)
            {
                int w = this.columnWidths[i];
                RectangleF rect = new RectangleF(left, y, w, columnFont.Height);
                //				left += w;
                //column names row
                if (columnStrings[i] != null)
                    pdfDoc.DrawString(columnStrings[i], this.columnFont, Brushes.DarkBlue, rect, sf);

                rect.Y += rect.Height;
                if (unitAvailable)
                {
                    rect.Height = unitFont.Height;
                    //units row
                    if (unitStrings[i] != null)
                        pdfDoc.DrawString(unitStrings[i], this.unitFont, Brushes.DarkBlue, rect, sf);

                    rect.Y = rect.Bottom;
                }
                rect.Y += 1;
                if (i == 0)
                {
                    for (int j = 0; j < rows; j++)
                    {
                        pdfDoc.DrawString(this.rowStrings[j], this.columnFont, Brushes.DarkBlue, rect, sf);
                        rect.Y += rowHeight;
                    }
                }

                left += w;
            }
            UpdateReportData(pdfDoc, true, y);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="strName"></param>
		public void StartInputItem(string strName)
		{
			int k = 0;
			int top = columnFont.Height;
			if(unitAvailable)
				top += columnFont.Height;
			top++;
			int left = columnWidths[0];
			for(int i = 0; i < rows; i ++ )
			{
				RectangleF rect = new Rectangle(left, top, 0, rowHeight);
				for(int j = 1; j < columns; j ++ )
				{
					rect.Width = columnWidths[j];
					int m = recordIndexes[k];

					if( m >= 0 && dataRecord[ m ].Name == strName)
					{
						TextBox tb = new TextBox();
						tb.Text = dataRecord[ m ].Value.ToString();
						tb.Tag = dataRecord[ m ];
						tb.SetBounds((int)rect.X, (int)rect.Y-3, (int)rect.Width, (int)rect.Height);
						this.Controls.Add(tb);
						tb.Enabled = true;
                        tb.TextChanged += new EventHandler(tb_TextChanged);
					}
					k++;
					rect.X += columnWidths[j];
				}
				top += rowHeight;
			}
		}

        void tb_TextChanged(object sender, EventArgs e)
        {
           CVPhaseControl ob = (CVPhaseControl)this.Parent.Parent;
            ob.TouchTaskData();
        }

		/// <summary>
		/// 
		/// </summary>
        public void StopAllInputItems()
		{
			bool b;
			do
			{
				b = false;
				foreach(Control cntrl in this.Controls)
				{
					if(cntrl is TextBox)
					{
						CV1DValue cv1DV = (CV1DValue)(cntrl.Tag);
						try
						{
							cv1DV.SetValue(Convert.ToSingle(cntrl.Text));
                            cv1DV.HighLight = false;
						}
						catch(Exception e1)
						{
						}
						this.Controls.Remove(cntrl);
						b = true;
						break;
					}
				}
			}while(b); 

		}
*/
		
	}
}
