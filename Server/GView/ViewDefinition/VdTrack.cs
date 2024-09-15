using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;


using System.Text.Json.Serialization;
using System.Windows;
using System.Globalization;
using OpenWLS.Server.GView.Models;
using System.Drawing;
using System.Diagnostics.Metrics;

namespace OpenWLS.Server.GView.ViewDefinition
{
    [Flags]
    public enum TrackDateTime { second = 1, minute = 2,   hour = 4, day = 8, week = 0x10, month = 0x20, year = 0x40 }
    public enum ShowIndex { None = 0, DepthOrTime = 1,  DateTime = 2 }

    public class VdTrack
    {
        public int Id { get; set; }
    //    public string Name { get; set; }
        public int Grids{ get; set; }
        public int LogStart { get; set; }
        public bool Linear{get; set;}

        ShowIndex showIndex;
        public ShowIndex ShowIndex
        {
            get { return showIndex; }
            set { showIndex = value;
                if (showIndex != ShowIndex.DateTime)
                    TrackDateTime = 0;
            }
        }

        public int ShowIndex1
        {
            get { return (int)showIndex; }
            set
            {
                showIndex = (ShowIndex)value;
            }
        }

        [JsonIgnore]
        public bool ShowDateTime
        {
            get { return showIndex == ShowIndex.DateTime; }
        }
        bool inScrollbar;
        public bool InScrollbar
        {
            get { return inScrollbar; }
            set { inScrollbar = value;
                if (!inScrollbar)
                    ScrollbarDateTime = 0;
            }
        }

        public TrackDateTime TrackDateTime { get; set; }
        public TrackDateTime ScrollbarDateTime { get; set; }
        public double Width {get; set;}
        public string Name
        {
            get { return "T" + (ID+1).ToString();  }
        }

        public VdDocument doc;
        public int ID { get; set;  }
      
        public double Left { get; set; }
        public double Right { get { return Left + Width; } }
        public bool ImageTrack { get; set; }

 

        #region show datetime
        /*
        [JsonIgnore]
        public Visibility DtVisibility
        {
            get 
            { 
                if (ShowDateTime) 
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
        }
        [JsonIgnore]
        public Visibility DtSbVisibility
        {
            get
            {
                if (ShowDateTime && inScrollbar)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
        }

        public Visibility LogStartVisibility
        {
            get 
            { 
                if (Linear) 
                    return Visibility.Hidden;
                else
                    return Visibility.Visible;
            }
        }
        */
        [JsonIgnore]
        public bool ShowYear
        {
            get
            {
                return (TrackDateTime & TrackDateTime.year) != 0; 
            }
            set
            {
                if (value)
                    TrackDateTime |= TrackDateTime.year;
                else
                {
                    TrackDateTime |= TrackDateTime.year;
                    TrackDateTime -= TrackDateTime.year;
                }
            }
        }

        [JsonIgnore]
        public bool ShowMonth
        {
            get
            {
                return (TrackDateTime & TrackDateTime.month) != 0;
            }
            set
            {
                if (value)
                    TrackDateTime |= TrackDateTime.month;
                else
                {
                    TrackDateTime |= TrackDateTime.month;
                    TrackDateTime -= TrackDateTime.month;
                }
            }
        }

        [JsonIgnore]
        public bool ShowWeek
        {
            get
            {
                return (TrackDateTime & TrackDateTime.week) != 0;
            }
            set
            {
                if (value)
                    TrackDateTime |= TrackDateTime.week;
                else
                {
                    TrackDateTime |= TrackDateTime.week;
                    TrackDateTime -= TrackDateTime.week;
                }
            }
        }

        [JsonIgnore]
        public bool ShowDay
        {
            get
            {
                return (TrackDateTime & TrackDateTime.day) != 0;
            }
            set
            {
                if (value)
                    TrackDateTime |= TrackDateTime.day;
                else
                {
                    TrackDateTime |= TrackDateTime.day;
                    TrackDateTime -= TrackDateTime.day;
                }
            }
        }
        [JsonIgnore]
        public bool ShowHour
        {
            get
            {
                return (TrackDateTime & TrackDateTime.hour) != 0;
            }
            set
            {
                if (value)
                    TrackDateTime |= TrackDateTime.hour;
                else
                {
                    TrackDateTime |= TrackDateTime.hour;
                    TrackDateTime -= TrackDateTime.hour;
                }
            }
        }

        [JsonIgnore]
        public bool ShowMinute
        {
            get
            {
                return (TrackDateTime & TrackDateTime.minute) != 0;
            }
            set
            {
                if (value)
                    TrackDateTime |= TrackDateTime.minute;
                else
                {
                    TrackDateTime |= TrackDateTime.minute;
                    TrackDateTime -= TrackDateTime.minute;
                }
            }
        }
        [JsonIgnore]
        public bool ShowSecond
        {
            get
            {
                return (TrackDateTime & TrackDateTime.second) != 0;
            }
            set
            {
                if (value)
                    TrackDateTime |= TrackDateTime.second;
                else
                {
                    TrackDateTime |= TrackDateTime.second;
                    TrackDateTime -= TrackDateTime.second;
                }
            }
        }

        #endregion
  
        #region show sb datetime
        [JsonIgnore]
        public bool SbShowYear
        {
            get
            {
                return (ScrollbarDateTime & TrackDateTime.year) != 0;
            }
            set
            {
                if (value)
                    ScrollbarDateTime |= TrackDateTime.year;
                else
                {
                    ScrollbarDateTime |= TrackDateTime.year;
                    ScrollbarDateTime -= TrackDateTime.year;
                }
            }
        }

        [JsonIgnore]
        public bool SbShowMonth
        {
            get
            {
                return (ScrollbarDateTime & TrackDateTime.month) != 0;
            }
            set
            {
                if (value)
                    ScrollbarDateTime |= TrackDateTime.month;
                else
                {
                    ScrollbarDateTime |= TrackDateTime.month;
                    ScrollbarDateTime -= TrackDateTime.month;
                }
            }
        }

        [JsonIgnore]
        public bool SbShowWeek
        {
            get
            {
                return (ScrollbarDateTime & TrackDateTime.week) != 0;
            }
            set
            {
                if (value)
                    ScrollbarDateTime |= TrackDateTime.week;
                else
                {
                    ScrollbarDateTime |= TrackDateTime.week;
                    ScrollbarDateTime -= TrackDateTime.week;
                }
            }
        }

        [JsonIgnore]
        public bool SbShowDay
        {
            get
            {
                return (ScrollbarDateTime & TrackDateTime.day) != 0;
            }
            set
            {
                if (value)
                    ScrollbarDateTime |= TrackDateTime.day;
                else
                {
                    ScrollbarDateTime |= TrackDateTime.day;
                    ScrollbarDateTime -= TrackDateTime.day;
                }
            }
        }
        [JsonIgnore]
        public bool SbShowHour
        {
            get
            {
                return (ScrollbarDateTime & TrackDateTime.hour) != 0;
            }
            set
            {
                if (value)
                    ScrollbarDateTime |= TrackDateTime.hour;
                else
                {
                    ScrollbarDateTime |= TrackDateTime.hour;
                    ScrollbarDateTime -= TrackDateTime.hour;
                }
            }
        }
        [JsonIgnore]
        public bool SbShowMinute
        {
            get
            {
                return (ScrollbarDateTime & TrackDateTime.minute) != 0;
            }
            set
            {
                if (value)
                    ScrollbarDateTime |= TrackDateTime.minute;
                else
                {
                    ScrollbarDateTime |= TrackDateTime.minute;
                    ScrollbarDateTime -= TrackDateTime.minute;
                }
            }
        }
        [JsonIgnore]
        public bool SbShowSecond
        {
            get
            {
                return (ScrollbarDateTime & TrackDateTime.second) != 0;
            }
            set
            {
                if (value)
                    ScrollbarDateTime |= TrackDateTime.second;
                else
                {
                    ScrollbarDateTime |= TrackDateTime.second;
                    ScrollbarDateTime -= TrackDateTime.second;
                }
            }
        }

#endregion
        public VdTrack()
        {
            Grids = 10;
            LogStart = 2;
            Width = 2;
            Linear = true;
        }

        public void Restore(DataRow dr)
        {
            Grids = Convert.ToInt32(dr["Grids"]);
            LogStart = Convert.ToInt32(dr["LogStart"]);
            Linear = Convert.ToInt32(dr["Linear"]) != 0;
            if (dr.Table.Columns.Contains("ShowIndex"))
                ShowIndex = (ShowIndex)Convert.ToInt32(dr["ShowIndex"]);
            if(dr.Table.Columns.Contains("TrackDateTime") )
                TrackDateTime = (TrackDateTime)Convert.ToInt32(dr["TrackDateTime"]);
            if(dr.Table.Columns.Contains("ScrollbarDateTime") )
                ScrollbarDateTime = (TrackDateTime)Convert.ToInt32(dr["ScrollbarDateTime"]);
            Width = Convert.ToDouble(dr["Width"]);
        }

        public void Save(DataRow dr)
        {
            dr["Grids"] = Grids;
            dr["LogStart"] = LogStart;
            dr["Linear"] = Linear;
  //          dr["Depth"] = Depth;
            if (!dr.Table.Columns.Contains("ShowIndex"))
                dr.Table.Columns.Add(new DataColumn("ShowIndex", typeof(int)));
            if (!dr.Table.Columns.Contains("TrackDateTime"))
                dr.Table.Columns.Add(new DataColumn("TrackDateTime", typeof(int)));
            if (!dr.Table.Columns.Contains("ScrollbarDateTime"))
                dr.Table.Columns.Add(new DataColumn("ScrollbarDateTime", typeof(int)));
           dr["ShowIndex"] = (int)ShowIndex;
           dr["TrackDateTime"] = (int)TrackDateTime;
           dr["ScrollbarDateTime"] = (int)ScrollbarDateTime;
            dr["Width"] = Width;
        }

        public double CalcXs(double left, List<float> xs)
        {
           Left = left;
            int i, s;
            xs.Add((float)left);
            double dx = Width / Grids;
            if (!ImageTrack)
            {
                if (Linear)
                {
                    s = Grids + 1;
                    for (i = 1; i < s; i++)
                        xs.Add((float)(i * dx + left));
                }
                else
                {
                    s = 9 * Grids + 1;
                    double ls = Math.Log10(LogStart);
                    int m = LogStart; int n = 0;
                    for (i = 1; i < s; i++)
                    {
                        m++;
                        if (m == 10) { m = 1; n++; }
                        xs.Add(((float)(left + (Math.Log10(m) - ls + n) * dx)));
                    }
                }
            }

            xs.Add((float)Right);
            return Right;
        }

        public double GetX(double pos)
        {
            return Left + Width * pos / 100;
        }

        bool ShowDateTimeInScrollbar(TrackDateTime t)
        {
            switch(t)
            {
                case TrackDateTime.year:
                    return SbShowYear;
                case TrackDateTime.month:
                    return SbShowMonth;
                case TrackDateTime.day:
                    return SbShowDay;
                case TrackDateTime.hour:
                    return SbShowHour;
                case TrackDateTime.minute:
                    return SbShowMinute;
                case TrackDateTime.second:
                    return SbShowSecond;
            }
            return false;
        }

        public int DrawDateTime(GvDocument gvDoc,  float yOffset, float fs, long start, long stop, float h)
        {
            if (stop == start)
                return -1;
         //   TrackDateTime dtType = ( start, stop );
            int t1 = (int)TrackDateTime;
            int t2 = (int)TrackDateTime.year;
            int c = CountBitOnes(t1);
            float dw = (float)(Width / c);
            float x = (float)Left;
            GvLine geLine = new GvLine();
            geLine.Doc = gvDoc;
            geLine.SBar = true;
            int k = 0;
            while (t2 != 0)
            {
                if ( (t1 & t2) != 0)
                {                  
                    DrawDateTime(gvDoc, yOffset, x, x + dw, fs, start, stop, h, (TrackDateTime)t2);
                  //  if(k > 0)
                  //      geLine.AddLine(x, yOffset, x, h);
                    x += dw;
                    k++;
                }
                t2 = t2 >> 1;
            }

            return 0;
        }

        static int CountBitOnes(int d)
        {
            int c = 0;
            while(d > 0)
            {
                if((d&1) !=0)
                c++;
                d = d >> 1;
            }
            return c;
        }
        /*
                static TrackDateTime GetTrackDateTime(long start, long stop)
                { 
                    TrackDateTime dtType = 0;
                    DateTime dt_start = new DateTime(start);
                    DateTime dt_stop = new DateTime(stop);
                    if (dt_start.Year != dt_stop.Year)
                        dtType = (TrackDateTime)0x7f;
                    else
                    {
                        if (dt_start.Month != dt_stop.Month)
                            dtType = (TrackDateTime)0x3f;
                        else
                        {
                            if (dt_start.DayOfWeek != dt_stop.DayOfWeek)
                                dtType = (TrackDateTime)0x1f;
                            else
                            {
                                if (dt_start.Day != dt_stop.Day)
                                    dtType = (TrackDateTime)0xf;
                                else
                                {
                                    if (dt_start.Hour != dt_stop.Hour)
                                        dtType = (TrackDateTime)0x7;
                                    else
                                    {
                                        if (dt_start.Minute != dt_stop.Minute)
                                            dtType = (TrackDateTime)0x3;
                                        else
                                        {
                                            if (dt_start.Second != dt_stop.Second)
                                                dtType = (TrackDateTime)1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return dtType;
                }
                 */

        public void DrawHorizontalLines(GvDocument gvDoc, uint color, float yOffset, int indexHLInterval)
        {
            if (ShowIndex != ShowIndex.None || ImageTrack) 
                return;
            int dl = indexHLInterval / 10;
            if (dl == 0)
                dl = 1;

            int index = (int)((doc.Top / dl)) * dl;
            double y = (index - doc.Top) * doc.YScale;
            double dy = dl * doc.YScale;
            GvLine gvLine = new(color, 1, 0);
            gvDoc.AddItem(gvLine);
            while (index < doc.Bottom)
            {
                if ((index % indexHLInterval) != 0)
                {
                    float b = (float)y + yOffset;
                    gvLine.AddLine((float)Left, b, (float)Right, b);
                }
                index += dl; y += dy;
            }
            gvLine.FlushSection();
        }
         /// <summary>
         /// draw high hight horizontal line and numbers
         /// </summary>
         /// <param name="gvDoc"></param>
         /// <param name="color"></param>
         /// <param name="yOffset"></param>
         /// <param name="indexHLInterval"></param>
         /// <param name="indexNumberInterval"></param>
        public void DrawHLHLinesAndNumber(GvDocument gvDoc, uint color, float yOffset, int indexHLInterval, int indexNumberInterval)
        { 
            if (ShowIndex == ShowIndex.DepthOrTime)
            {
                GvText gvText = new GvText()
                {
                    GId = Id,
                    FId = doc.DepthFont.Id,
                    Left = (float)(float)(Left),
                    Right = (float)Right,
                    Color = color,
                    Alignment = GvTextAlignment.Center,
                };
                gvDoc.AddItem(gvText);
                int index = (int)((doc.Top / indexNumberInterval)) * indexNumberInterval;
                double y = (index - doc.Top) * doc.YScale;
                double fy = doc.DepthFont.Size / 2;
                double dy = indexNumberInterval * doc.YScale;
                index += indexNumberInterval; y += dy;
                while (index < doc.Bottom)
                {
                    float b = (float)y + yOffset;
                    gvText.WriteText(index.ToString(), (float)(b - fy));
                    index += indexNumberInterval; y += dy;
                }              
            }
            else
            {
                if ((!ImageTrack) && (!ShowDateTime))
                {
                    int index = (int)((doc.Top / indexHLInterval)) * indexHLInterval;
                    double y = (index - doc.Top) * doc.YScale;
                    double dy = indexHLInterval * doc.YScale;
                    index += indexHLInterval; y += dy;
                    GvLine gvLine = new(color, 1, 0);
                    gvDoc.AddItem(gvLine);
                    while (index < doc.Bottom)
                    {
                        float b = (float)y + yOffset;
                        gvLine.AddLine((float)Left, b, (float)Right, b);
                        index += indexHLInterval; y += dy;
                    }
                    gvLine.FlushSection();
                }     
            }    
        }


        public int DrawDateTime(GvDocument gvDoc, float yOffset, float left, float right, float fs, long start, long stop, float h, TrackDateTime dtType )
        {
            if (dtType == TrackDateTime.week)
                return 0;
            long dt1 = start;
            float sy = h / (stop - start);    
            float x = (float)((right - left - fs) / 2 + Left);
            float y, y_pre;
            DateTime dt2 = new DateTime(dt1);
            dt2 = GetStartDateTime(dt2, dtType);
            GvLine geLine = new GvLine();
            GvText gvText = new GvText();
            gvDoc.AddItem(geLine);
            gvDoc.AddItem(gvText);
            gvText.Rotation = 90;
            gvText.FId = doc.DepthFont.Id;
            gvText.Left = left;
            gvText.Right = right;
            gvText.Alignment = GvTextAlignment.Center;
            bool showInSBar = ShowDateTimeInScrollbar(dtType);
            geLine.SBar = showInSBar;
            gvText.SBar = showInSBar;

            y_pre = y = (dt1 - start) * sy + yOffset;
            geLine.AddLine(left, y, right, y);   
            int d = GetDateTimeNumber(dt2, dtType);
            dt2 = IncDateTime(dt2, dtType);
            dt1 = dt2.Ticks;
            DateTimeFormatInfo dtfi = CultureInfo.GetCultureInfo("en-US").DateTimeFormat;
            while (dt1 <= stop)
            {
                y = (dt1 - start) * sy + yOffset;
                string txt = dtType == TrackDateTime.month? dtfi.GetAbbreviatedMonthName(d) : d.ToString();              
                gvText.WriteText(txt, y_pre, y);    
                d = GetDateTimeNumber(dt2, dtType);
                geLine.AddLine(left, y, right, y);
                dt2 = IncDateTime(dt2, dtType);
                dt1 = dt2.Ticks;
                y_pre = y;
            }
            if(dt1 != stop)
            {
                y = (stop - start) * sy + yOffset;
                string txt = dtType == TrackDateTime.month ? dtfi.GetAbbreviatedMonthName(d) : d.ToString();
                gvText.WriteText(txt, y_pre, y);
                y_pre = y;
            }
            geLine.AddLine(left, yOffset, left, h);

            return 0;
        }

        int GetDateTimeNumber(DateTime dt, TrackDateTime dtType)
        {
            switch (dtType)
            {
                case TrackDateTime.year:
                    return dt.Year;
                case TrackDateTime.month:
                    return dt.Month;
//                case TrackDateTime.week:
 //                   return (int)dt.DayOfWeek;
                case TrackDateTime.day:
                    return dt.Day;
                case TrackDateTime.hour:
                    return dt.Hour;
                case TrackDateTime.minute:
                    return dt.Minute;
                case TrackDateTime.second:
                    return dt.Second;
            }
            return 0;
        }


        DateTime GetStartDateTime(DateTime dt, TrackDateTime dtType)
        {
            switch (dtType)
            {
                case TrackDateTime.year:
                    return new DateTime(dt.Year, 1 ,1);
                case TrackDateTime.month:
                    return new DateTime(dt.Year, dt.Month, 1);
                case TrackDateTime.day:
                    return new DateTime(dt.Year, dt.Month, dt.Day);
                case TrackDateTime.hour:
                    return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0 );
                case TrackDateTime.minute:
                    return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
                case TrackDateTime.second:
                    return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
            }
            return new DateTime(0);
        }

        DateTime IncDateTime(DateTime dt, TrackDateTime dtType)
        {
            switch (dtType)
            {
                case TrackDateTime.year:
                    return dt.AddYears(1);
                case TrackDateTime.month:
                    return dt.AddMonths(1);    
//                case TrackDateTime.week:
//                    return dt.AddDays(7);
                case TrackDateTime.day:
                    return dt.AddDays(1);
                case TrackDateTime.hour:
                    return dt.AddHours(1);
                case TrackDateTime.minute:
                    return dt.AddMinutes(1);
                case TrackDateTime.second:
                    return dt.AddSeconds(1);
            }
            return new DateTime(0);
        }

    }

}
