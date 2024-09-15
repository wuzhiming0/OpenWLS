using OpenWLS.Server.GView.Models;
using System.Data;

namespace OpenWLS.Server.GView.ViewDefinition
{
    public class VdTracks : List<VdTrack>
    {
        GvFont gvFont;
        GvLine gvLine;
     //   GvText gvText;

        public int IndexHLInterval
        {
            get
            {
                // 1/500: 0.078;  1/200:0.198; 1/20: 1.9
                int indexHLInterval = (doc.YScale > 0.15) ? 5 : (doc.YScale > 0.08) ? 10 : 50;
                return indexHLInterval;
            }
        }

        double leftMargin;
        public double LeftMargin
        {
            get { return leftMargin; }
            set
            {
                leftMargin = value;
                if (Count <= 0) return;
                this[0].Left = leftMargin;
                for (int i = 1; i < Count; i++)
                    this[i].Left = this[i - 1].Right;
                width = this[Count - 1].Right;
            }
        }
        double width;
        public double Width { get { return width; } }

        //        bool               RealTime;
        int indexTextInterval;
        VdDocument doc;

        public int GetIndex(VdTrack t)
        {
            for (int i = 0; i < Count; i++)
                if (this[i] == t)
                    return i;
            return -1;
        }

        public void ResetIDs()
        {
            for (int i = 0; i < Count; i++)
                this[i].ID = i;
        }

        public void Restore(DataTable dt, VdDocument _doc)
        {
            doc = _doc;
            //       gvLine = new GvLine();
            //       GvText = new GvText();
            //       gvFont = doc.DepthFont;

            Clear();
            foreach (DataRow dr in dt.Rows)
            {
                VdTrack t = new VdTrack();
                t.doc = doc;
                t.Restore(dr);
                Add(t);
            }
            ResetIDs();
        }

        public void Save(DataTable dt)
        {
            dt.Rows.Clear();
            foreach (VdTrack t in this)
            {
                DataRow dr = dt.NewRow();
                t.Save(dr);
                dt.Rows.Add(dr);
            }
            ResetIDs();
        }
        void CheckImageTrack()
        {
            int i;
            float x;
            int s = doc.Items.Count;
            for (i = 0; i < s; i++)
            {
                VdItem item = doc.Items[i];
                if (item.Type == LogViewItemType.Image)
                {
                    int lt = ((VdImage)item).LeftPos.TrackID;
                    int rt = ((VdImage)item).RightPos.TrackID;
                    this[lt].ImageTrack = true;
                    this[rt].ImageTrack = true;
                }
            }
        }

        void DrawVerticalLines(GvDocument gvDoc, uint color, float yOffset)
        {
            double le = this[0].Left;
            List<float> xs = new List<float>();
            for (int i = 0; i < Count; i++)
                le = this[i].CalcXs(le, xs);
            double h = doc.ViewHeight;

            gvLine = new(color, 1, 0);
            gvDoc.AddItem(gvLine);

            int s = xs.Count;
            float b = yOffset + (float)h;
            for (int i = 0; i < s; i++)
                gvLine.AddLine(xs[i], yOffset, xs[i], b);
            gvLine.FlushSection();
        }


        public int DrawLineAndNumber(GvDocument gvDoc, float yOffset)
        {
            if (Count <= 0) return -1;
            gvFont = doc.DepthFont;
            gvDoc.AddItem(gvFont);

            CheckImageTrack();

            DrawVerticalLines(gvDoc, 0xff7f7f7f, yOffset);
            indexTextInterval = IndexHLInterval == 5 ? IndexHLInterval << 1 : IndexHLInterval;
            foreach (VdTrack t in this)
            {
                t.DrawHorizontalLines(gvDoc, 0xff7f7f7f, yOffset, IndexHLInterval);
                t.DrawHLHLinesAndNumber(gvDoc, 0xff000000, yOffset, IndexHLInterval, indexTextInterval);
            }

            return 0;

        }

        public int DrawDateTime(GvDocument geDoc, float yOffset)
        {
            long start = doc.StartDateTime + (long)(doc.Top * 10000000);
            if (start < 0)
                return -1;
            long stop = doc.StartDateTime + (long)(doc.Bottom * 10000000);
            float h = (float)doc.ViewHeight;
            for (int i = 0; i < Count; i++)
            {
                VdTrack t = this[i];
                if (t.ShowDateTime)
                    t.DrawDateTime(geDoc, yOffset, doc.DepthFont.Size, start, stop, h);
            }
            return 0;
        }
        /*
        public int DrawTrackVLineRt(GvDocument gvDoc)
        {
            if (Count <= 0) return -1;

            geLine.DocGe = geDoc; GvText.DocGe = geDoc; geFont.DocGe = geDoc;
            GvText.Font = doc.DepthFont;

            geFont.OutputGElement();
            CheckImageTrack();

            uint color = 0xff7f7f7f;
            doc.Bottom = float.PositiveInfinity;
            DrawVerticalLines(color, 0);

            return 0;
        }

        public int DrawTrackHLineAndIndexTextRt(GvDocument geDoc)
        {
            return 0;
        }
*/

    }
}

