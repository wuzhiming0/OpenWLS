using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Drawing;

using System.Data;
using System.IO;


using OpenWLS.Server.GView.Models;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile;
using System.Drawing;
using RectangleF = OpenWLS.Server.GView.Models.RectangleF;
//using OpenWLS.Server.LogInstance.Instrument;
using OpenWLS.Server.LogInstance.RtDataFile;
using OpenWLS.Server.LogInstance;
using System.Diagnostics.Metrics;

namespace OpenWLS.Server.GView.ViewDefinition
{
    public enum LogViewItemType { Curve = 1, Image = 2, Fill = 3, Wave = 4 }

    [Flags]
    public enum CVITemFlags { Enable = 1, ShowHead = 2, ShowName = 4, ShowDescription = 8 }

    public class VdItem 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        protected MVReadByIndex? chReader;
        protected MVReadByIndexRt? chReaderRt;
        protected double? emptyValue;
        protected int dim0;
        protected GvText headNameText;
        //       protected GvText headDescriptionText;
        protected GvText headLeftScaleText;
        protected GvText headRightScaleText;

        protected CVITemFlags flags;
        protected VdDocument doc;

        protected VdxPosition leftPos;
        protected VdxPosition rightPos;
        protected Scale scale;

        protected LogInstance.Instrument.Measurement? m_rt;

        public RectangleF HeadRect;
        public VdMeasurement? Measurement { get; set; }

        public string? Description { get; set; }
        public bool InScrollbar { get; set; }


        public VdxPosition LeftPos { get { return leftPos; } }
        public VdxPosition RightPos { get { return rightPos; } }

        public LogViewItemType Type { get; set; }
        //    public double IndexRt{get; set;}

        public Scale Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public VdDocument Doc
        {
            get { return doc; }
            set
            {
                doc = value;
                InitItem();
            }
        }

        public bool Enable
        {
            get
            {
                return (flags & CVITemFlags.Enable) != 0;
            }
            set
            {
                if (value)
                    flags |= CVITemFlags.Enable;
                else
                    flags &= ~CVITemFlags.Enable;
            }
        }

        public bool ShowHead
        {
            get
            {
                return (flags & CVITemFlags.ShowHead) != 0;
            }
            set
            {
                if (value)
                    flags |= CVITemFlags.ShowHead;
                else
                    flags &= ~CVITemFlags.ShowHead;
            }
        }

        public bool ShowName
        {
            get
            {
                return (flags & CVITemFlags.ShowName) != 0;
            }
            set
            {
                if (value)
                    flags |= CVITemFlags.ShowName;
                else
                    flags &= ~CVITemFlags.ShowName;
            }
        }

        public bool ShowDescription
        {
            get
            {
                return (flags & CVITemFlags.ShowDescription) != 0;
            }
            set
            {
                if (value)
                    flags |= CVITemFlags.ShowDescription;
                else
                    flags &= ~CVITemFlags.ShowDescription;
            }
        }

        public VdItem()
        {
            Id = -1;
            HeadRect.Height = (float)0.5;
            //            headDescriptionText = new GvText();
            //            headLeftScaleText = new GvText();
            //            headRightScaleText = new GvText();
            // IndexRt = double.NaN;
        }
        virtual public void CreateReaderRt(LogInstanceS logInstance)
        {
            if (Measurement == null) return;
            LogInstance.Instrument.Measurement? m = logInstance.Measurements.GetMeasurement(Measurement.Frame, Measurement.Name);
            if (m != null)
            {
                emptyValue = m.MeasurementDf.Head.VEmpty;
                dim0 = m.MeasurementDf.Head.Dimensions[0];
                chReaderRt = new MVReadByIndexRt();
                if (!chReaderRt.InitReaderBI(m, doc.IndexType, LogDataFile.Index.ToUomString(doc.IndexUnit)))
                    chReaderRt = null;
            }
        }

        virtual public void CreateReader(VdDFiles dfs)
        {
            if (Measurement == null) return;
            LogDataFile.Models.Measurement m = dfs.GetMeasurement(Measurement.FileID, Measurement.Frame, Measurement.Name);
            if (m != null)
            {
                emptyValue = m.Head.VEmpty;
                dim0 = m.Head.Dimensions[0];
                chReader = new MVReadByIndex();
                if (!chReader.InitReaderBI(m, doc.IndexType, LogDataFile.Index.ToUomString(doc.IndexUnit), Measurement.Element))
                    chReader = null;
            }
        }
        protected void CreateNameText(GvDocument gvDoc, float yOffset, uint color)
        {
            if (!ShowName) return;
            headNameText = new GvText()
            {
                GId = Id,
                FId = this is VdFill ? doc.ItemNameFontFill.Id : doc.ItemNameFont.Id,
                Alignment = GvTextAlignment.Center,
                Left = (float)HeadRect.X,
                Right = (float)HeadRect.Right,
                Color = color,
            };

            gvDoc.AddItem(headNameText);

            if (ShowDescription)
                headNameText.WriteText($"{Name}({Description})", HeadRect.Y + yOffset);
            else
                headNameText.WriteText(Name, HeadRect.Y + yOffset);

        }
        protected void CalcHeadXPosition()
        {
            double left = leftPos.GetX();
            double right = rightPos.GetX();
            double width = right - left;
            HeadRect.X = (float)left;
            HeadRect.Width = (float)width;
        }

        protected void CalcScaleXPosition(int scale_id)
        {
            scale = doc.Scales.GetScale(scale_id);
            if (scale != null)
            {
                scale.PosFrom = HeadRect.X;
                scale.PosTo = HeadRect.Right;
                scale.PrepairScale();
            }
        }

        virtual public void Restore(DataRow dr)
        {
            Id = Convert.ToInt32(dr["Id"]);
            Name = (string)dr["Name"];
            flags = (CVITemFlags)Convert.ToInt32(dr["Flags"]);
            if (dr.Table.Columns.Contains("InScrollbar") && dr["Description"] != DBNull.Value)
                Description = (string)dr["Description"];

            flags = (CVITemFlags)Convert.ToInt32(dr["Flags"]);

            if (dr.Table.Columns.Contains("InScrollbar") && dr["InScrollbar"] != DBNull.Value)
                InScrollbar = true;

            if (dr.Table.Columns.Contains("M_Name") && dr["M_Name"] != DBNull.Value)
                Measurement.Restore(dr);
        }

        virtual public void Save(DataRow dr)
        {
            dr["Id"] = Id;
            dr["Name"] = Name;
            dr["Flags"] = (int)flags;
            if (InScrollbar)
                dr["InScrollbar"] = 1;
            if (!string.IsNullOrEmpty(Description))
                dr["Description"] = Description;
            if (Measurement != null)
                Measurement.Save(dr);
            //     dr["HeadRect"] = HeadRect.X.ToString() + ',' +  HeadRect.Y.ToString() + ',' + HeadRect.Width.ToString() + ',' +  HeadRect.Height.ToString(); 
        }

        virtual public void DrawItemHead(GvDocument geDoc, bool firstSection, float yOffset)
        {

        }

        virtual public int DrawItem(double top, double bottom, GvDocument geDoc, bool firstSection, float yOffset)
        {
            return -1;
        }


        /*
        public void InitRtChannelSampleBuffer(Archive ar, LogIndexType lit)
        {
            Measurement ac = ar.Channles.GetChannel(Name);
            if (ac != null)
            {
                rtValSb = ac.SampleBuffer;
                if (rtValSb != null)
                    rtIndexSb = ac.Frame.GetIndex(lit).IndexChannel.SampleBuffer;
            }
            preDataOffsetRt = 0;
            StartLog();
        }
        */
        public virtual void StartLog()
        {

        }
        /*
        public virtual void PLotNewSamples(double top,  GvDocument geDoc, float yOffset)
        {

        }
*/
        virtual public int DrawItemRt(double top, GvDocument geDoc, float yOffset)
        {
            return -1;
        }

        virtual public void DrawRepeat(double top, double bottom, GvDocument geDoc, VdDFile df)
        {

        }

        virtual public void DrawRepeat(double top, double bottom, VdDFile df)
        {

        }

        virtual public int ProcRtData(DataReader r)
        {
            return 0;
        }

        virtual public int InitItem()
        {
            return 0;
        }
        /*
        virtual public void CreateReader(VdDFiles dfs)
        {
            
        }
        */
        virtual public void CreateReaderRt(LogInstance.Instrument.Measurements ms)
        {

        }
        public override string ToString()
        {
            return Name;
        }
    }

    public class VdItems : List<VdItem>
    {
        public float insertH;
        public VdDocument doc;
        int GetNxtID()
        {
            int k = 0;
            bool b = true;
            while (b)
            {
                k++;
                b = false;
                foreach (VdItem s in this)
                    if (s.Id == k)
                        b = true;
            }
            return k;
        }

        public VdItem FindItem(string name, LogViewItemType t)
        {
            foreach (VdItem s in this)
                if (s.Name == name && s.Type == t)
                    return s;
            return null;
        }

        public void AddNew(LogViewItemType t)
        {
            VdItem c = null;
            switch (t)
            {
                case LogViewItemType.Curve:
                    c = new VdCurve();
                    break;
                case LogViewItemType.Image:
                    c = new VdImage();
                    break;
                case LogViewItemType.Fill:
                    c = new VdFill();
                    break;
            }
            if (c != null)
            {
                c.Id = GetNxtID();
                c.Doc = doc;
                c.InitItem();
                Insert(0, c);
            }
        }

        public void Restore(CVIDInfor infor)
        {
            Clear();
            foreach (DataRow dr in infor.Curves.Rows)
            {
                VdCurve c = new VdCurve();
                c.Restore(dr);
                Add(c);
            }

            foreach (DataRow dr in infor.Images.Rows)
            {
                VdImage c = new VdImage();
                c.Restore(dr);
                Add(c);
            }
            if (infor.Fills != null)
            {
                foreach (DataRow dr in infor.Fills.Rows)
                {
                    VdFill c = new VdFill();
                    c.Restore(dr);
                    Add(c);
                }
            }

        }
        void ClearCheckTable(DataTable dt)
        {
            dt.Rows.Clear();
            if (!dt.Columns.Contains("InScrollbar"))
                dt.Columns.Add(new DataColumn("InScrollbar", typeof(int)));
        }

        public void Save(CVIDInfor infor)
        {
            ClearCheckTable(infor.Curves);
            ClearCheckTable(infor.Images);
            ClearCheckTable(infor.Fills);
            if (!infor.Curves.Columns.Contains("ShoeMark"))
                infor.Curves.Columns.Add(new DataColumn("ShowMark", typeof(int)));
            foreach (VdItem item in this)
            {
                DataRow dr;
                switch (item.Type)
                {
                    case LogViewItemType.Curve:
                        dr = infor.Curves.NewRow();
                        item.Save(dr);
                        infor.Curves.Rows.Add(dr);
                        break;
                    case LogViewItemType.Image:
                        dr = infor.Images.NewRow();
                        item.Save(dr);
                        infor.Images.Rows.Add(dr);
                        break;
                    case LogViewItemType.Fill:
                        dr = infor.Fills.NewRow();
                        item.Save(dr);
                        infor.Fills.Rows.Add(dr);
                        break;
                }
            }
        }

        public void DrawItems(double top, double bottom, GvDocument geDoc, bool firstSection, float yOffset)
        {
            foreach (VdItem item in this)
                if (item.Enable)
                    item.DrawItem(top, bottom, geDoc, firstSection, yOffset);
        }

        public void DrawItemsRt(double top, GvDocument geDoc, float yOffset)
        {
            foreach (VdItem item in this)
                if (item.Enable)
                    item.DrawItemRt(top, geDoc, yOffset);
        }
        /*
        public void PLotNewSamples(double top, double bottom, GvDocument geDoc, bool firstSection, float yOffset)
        {
            foreach (VdItem item in this)
                if (item.Enable)
                    item.PLotNewSamples(top,  geDoc,  yOffset);
        }
*/
        public void CreateReaders(VdDFiles dfs)
        {

            foreach (VdItem item in this)
            {
                if (item.Enable)
                    item.CreateReader(dfs);
            }
        }

        public void InitItems(VdDocument doc)
        {
            this.doc = doc;
            foreach (VdItem item in this)
            {
                item.Doc = doc;
                //   item.IndexRt = double.NaN;
                item.InitItem();
            }
        }
        /*
                public void InitRtChannelSampleBuffers(Archive ar, LogIndexType lit)
                {
                    foreach (VdItem item in this)
                        item.InitRtChannelSampleBuffer(ar, lit);
                }
        */
        public void DrawRepeats(double t, double b)
        {
            for (int i = 1; i < doc.DFiles.Count; i++)
            {
                foreach (VdItem p in this)
                    p.DrawRepeat(t, b, doc.DFiles[i]);
            }
        }

        public void DrawRepeats(double t, double b, GvDocument geDoc)
        {
            for (int i = 1; i < doc.DFiles.Count; i++)
            {
                foreach (VdItem p in this)
                    p.DrawRepeat(t, b, geDoc, doc.DFiles[i]);
            }
        }

        /*
        void CloseItems()
        {
            foreach (VdItem item in this)
                item.CloseItem();
        }*/

        void ReOrderHead(int[] orders)
        {
            int count = orders.Length;
            int i, j;
            for (i = 0; i < count; i++)
            {
                orders[i] = i;
                this[i].InitItem();
            }

            for (i = 0; i < count - 1; i++)
            {
                for (j = i + 1; j < count; j++)
                {
                    int oi = orders[i]; int oj = orders[j];
                    if (this[oj].HeadRect.X > this[oi].HeadRect.X)
                        continue;
                    if (this[oj].HeadRect.X == this[oi].HeadRect.X)
                        if (this[oj].HeadRect.Right <= this[oi].HeadRect.Right)
                            continue;
                    orders[i] = oj; orders[j] = oi;
                }
            }
        }

        public VdItem GetItem(LogViewItemType type, int itemID)
        {
            foreach (VdItem item in this)
            {
                if ((item.Id == itemID) && (item.Type == type))
                    return item;
            }
            return null;
        }

        public VdItem GetItem(LogViewItemType type, string name)
        {
            foreach (VdItem item in this)
            {
                if ((item.Name == name) && (item.Type == type))
                    return item;
            }
            return null;
        }

        public float CalcItemsHeadPos(bool topInsert)
        {
            int count = Count;
            int[] os = new int[count];
            ReOrderHead(os);
            //  float b = 0;
            float h = 0;

            int i, j = 0;
            for (i = 0; i < count; i++)
            {
                if (!(this[i].Enable && this[i].ShowHead)) continue;
                //       RectangleF hi = &objects[os[i]]->head;
                this[i].HeadRect.Y = 0;
                for (j = 0; j < i; j++)
                {
                    bool no_intersect = RectangleF.Intersect(this[i].HeadRect, this[j].HeadRect).IsEmpty;
                    if (!no_intersect)
                    {
                        this[i].HeadRect.Y = this[j].HeadRect.Y;
                        if (topInsert)
                            this[i].HeadRect.Y += this[j].HeadRect.Height;
                        else
                            this[i].HeadRect.Y -= this[j].HeadRect.Height;
                    }
                }
                float k = this[i].HeadRect.Bottom;
                if (k > h)
                    h = k;
            }

            if (!topInsert)
            {
                for (i = 0; i < count; i++)
                {
                    //              this[i].infor.Y += this[i].HeadRect.Height;
                }
            }
            insertH = h;
            return insertH;
        }

    }



}
