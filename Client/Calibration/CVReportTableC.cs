using OpenWLS.Server.LogInstance.Calibration;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Client.Calibration
{
    public  class CVReportTableC : CVReportTable
    {
        /*
        public void DrawDataCell(  AGvDocument doc, CV1DValue val, RectangleF rect, GvFont dataFont, GvFont toleranceFont, string dataFormat, ushort sf)
        {
           // double data = val.Value;
          //  if (!double.IsNaN(data))
            {
                string str = dataFormat == null ? data.ToString() : data.ToString(dataFormat);
                if (recording)
                    str = dataFormat == null ? dataTemp.ToString() : dataTemp.ToString(dataFormat);
                SizeF size = g.MeasureString(str, dataFont);
                while (size.Width >= rect.Width)
                {
                    str = str.Remove(str.Length - 1, 1);
                    size = g.MeasureString(str, dataFont);
                }
                rect.Height = dataFont.Height;
                if (highLight)
                    g.FillRectangle(Brushes.Yellow, rect);
                if (OutOfLimit)
                    g.DrawString(str, dataFont, Brushes.Red, rect, sf);
                else
                    g.DrawString(str, dataFont, Brushes.DarkGreen, rect, sf);
            }
            if (!this.highLight)
            {
                string strLimit = "(";
                if (limitLowAvailable)
                    strLimit = strLimit + GetShortString(limitLow.ToString());
                if (limitHighAvailable)
                    strLimit = strLimit + " , " + GetShortString(limitHigh.ToString()) + ")";
                if (strLimit.Length > 1)
                {
                    rect.Y += dataFont.Height;
                    rect.Height = toleranceFont.Height;
                    g.DrawString(strLimit, toleranceFont, Brushes.Black, rect, sf);
                }
            }
            this.highLight = false;
        }
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="drawAllItems"></param>
		void UpdateReportData(Graphics g, bool drawAllItems)
        {
            int k = 0;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Far;
            float top = columnFont.Size;
            if (unitAvailable)
                top += columnFont.Size;
            top++;
            float left = columnWidths[0];

            float w1 = 0;
            for (int i = 1; i < this.columns; i++)
                w1 += columnWidths[i];
            for (int i = 0; i < rows; i++)
            {
                RectangleF rect = new RectangleF(left, top, 0, rowHeight);
                if (drawAllItems)
                    g.FillRectangle(Brushes.White, columnWidths[0], top, w1, itemFont.Size);

                for (int j = 1; j < columns; j++)
                {
                    rect.Width = columnWidths[j];
                    int m = recordIndexes[k];

                    //			if( m >= 0 && ( drawAllItems || dataRecord[ m ].HighLight) )
                    //				dataRecord[ m ].DrawItem(g, rect, this.itemFont, this.toleranceFont, this.formatStrings[j], sf);
                    k++;
                    rect.X += columnWidths[j];
                }
                top += rowHeight;
            }

        }
    }
}
