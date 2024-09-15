using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Drawing;

using System.Text.Json.Serialization;
using System.Reflection;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.Base;
using OpenWLS.Server.GView.Models;
using SizeF = OpenWLS.Server.GView.Models.SizeF;
using OpenWLS.Server.DBase;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json;
//using System.Text.Json;

namespace OpenWLS.Server.GView.ViewDefinition
{
    public class CVIDInfor
    {
        public double Top { get; set; }
        public double Bottom { get; set; }
        public double IndexScale { get; set; }
        public double ViewLMargin { get; set; }
        public double ViewRMargin { get; set; }
        public bool ShowTopInsert { get; set; }
        public bool ShowBottomInsert { get; set; }
        public IndexUnit IndexUnit { get; set; }
        public int? IndexType { get; set; }
        public Orientation Orientation { get; set; }
        public string PatternNames { get; set; }
        public uint IndexBarMaskColor { get; set; }
        public DataTable Curves { get; set; }
        public DataTable Images { get; set; }
        public DataTable Tracks { get; set; }
        public DataTable Fills { get; set; }
        public DataTable Scales { get; set; }
        public DataTable DFiles { get; set; }

        public CVIDInfor()
        {

        }

    }

    public enum Orientation { Auto = 0, Horizontal = 1, Vertical = 2 };
    public class VdDocument
    {
        protected static string emptyDocStr;
        protected static string[] patternNames;

        public static VdDocument FromJson(string json, bool rt)
        {
            var doc = rt? new VdDocumentRt(): new VdDocument();
            CVIDInfor infor = Newtonsoft.Json.JsonConvert.DeserializeObject<CVIDInfor>(json);
            infor.PatternNames = StringConverter.ArrayToString(patternNames, ':');
            doc.OnUpdateInfor(infor);
            return doc;
        }


        public static string[] PatternNames
        {
            get
            {
                if (patternNames == null)
                    patternNames = GvFillImage.GetNames();

                return patternNames;
            }
        }

        int nextFontID;
    //    protected bool realTime;         

        protected double top; 
        protected double bottom;
   //     public double RtViewIncStep { get; set; }
        public  double Top{ get { return top; } set { top = value; } }
        public  double Bottom { get { return bottom; } set { bottom = value; } }
        public LogIndexType? IndexType { get; set; }
        public IndexUnit IndexUnit { get; set; }

        public double IndexScale { get; set; }

        public TrackDateTime DTMinor { get; set; }

        public uint IndexBarMaskColor { get; set; }

        public double viewLMargin;// viewRMargin;

        public  double dpiX, dpiY;

  //      public bool RealTime { get { return realTime; }  }

        protected VdItems items;
        protected  VdTracks tracks;

        protected  VdDFiles dfiles;
        protected  Scales scales;
     //   public GvDocument geDoc;

        protected bool firstSection;

        public Orientation Orientation { get; set; }
        public VdItems Items { get { return items; } }
        public VdTracks Tracks { get { return tracks; } }
        public VdDFiles DFiles { get { return dfiles; } }
        public Scales Scales { get { return scales; } }

  //      public string FileName { get; set; }

        [JsonIgnore]
        public long StartDateTime { get; set; }

        protected bool showTopInsert;
        protected bool showBottomInsert;

        //ref to inch, since device pixel density unit is  DPI
        public double YScale {
            get
            {
                if(IndexUnit == IndexUnit.date_time)
                {
                    double d = 12.0 / 200;
                    switch(DTMinor)
                    {
                        case TrackDateTime.second:
                            return d;
                        case TrackDateTime.minute:
                            return d / 60;
                        case TrackDateTime.hour:
                            return d / 60 / 60;
                        case TrackDateTime.day:
                            return d / 60 / 60 / 24;
                        case TrackDateTime.month:
                            return d / 60 / 60 / 24 / 30;
                        case TrackDateTime.year:
                            return d / 60 / 60 / 24 / 30 / 12;
                    }
                    return d;
                }
                else
                {
                    double d = (IndexUnit == IndexUnit.meter) ? 39.3701 / IndexScale : 12 / IndexScale;
                    return d;
                }

            }
        }
 
        public double  ViewHeight
        {
            get {  return  ( bottom - top ) * YScale; }
        }

        public GvFont ItemNameFont { get; set; }
        public GvFont ItemNameFontFill { get; set; }
        public GvFont ItemDescriptionFont { get; set; }
        public GvFont ItemScaleFont { get; set; }
        public GvFont DepthFont { get; set; }

        public VdDocument()
        {
            dpiY = 100; //geDoc = null; 
       //     realTime = false;
            items = new VdItems();
            tracks = new VdTracks();
            scales = new Scales();
            dfiles = new VdDFiles();

            ItemNameFontFill = new GvFont()
            {
                Style = GvFontStyle.ClearBG
            };
            ItemNameFont = new GvFont();
            ItemDescriptionFont = new GvFont();
            ItemScaleFont = new GvFont();
            DepthFont = new GvFont();
            IndexBarMaskColor = 0xffffffff;
        }


        public void CreateNew()
        {
            if (emptyDocStr == null)
            {           
                Assembly _assembly = Assembly.GetExecutingAssembly();
                using (StreamReader sr = new StreamReader(_assembly.GetManifestResourceStream("OpenWLS.Server.GView.ViewDefinition.Dummy.json")))
                {
                    emptyDocStr = sr.ReadToEnd();

                }
            }
            CVIDInfor infor = Newtonsoft.Json.JsonConvert.DeserializeObject<CVIDInfor>(emptyDocStr);
            infor.PatternNames = StringConverter.ArrayToString(PatternNames, ':');
            OnUpdateInfor(infor);
        }
            
        public int  GetNextFontID(){
            int i = nextFontID; nextFontID++;  return i;
        }

        public int UpdateDocument(string json )
        {
            CVIDInfor infor = Newtonsoft.Json.JsonConvert.DeserializeObject<CVIDInfor>(json);
            OnUpdateInfor(infor);
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dfs">save data files? </param>
        /// <returns></returns>
        public string GetJSon(bool dfs)
        {
            if (emptyDocStr == null)
            {
                Assembly _assembly = Assembly.GetExecutingAssembly();
                using (StreamReader sr = new StreamReader(_assembly.GetManifestResourceStream("OpenWLS.Server.GView.ViewDefinition.Dummy.json")))
                {
                    emptyDocStr = sr.ReadToEnd();
                }
            }
            CVIDInfor infor = Newtonsoft.Json.JsonConvert.DeserializeObject<CVIDInfor>(emptyDocStr);
            infor.Top = top;
            infor.Bottom = bottom;
            infor.ViewLMargin = tracks.LeftMargin;
            infor.IndexScale = IndexScale;
            infor.IndexUnit = IndexUnit;
            infor.Orientation = Orientation;
            infor.PatternNames = StringConverter.ArrayToString(PatternNames, ':');
            if (IndexType != null)
                infor.IndexType = (int)IndexType;
            infor.IndexBarMaskColor = IndexBarMaskColor;

            items.Save(infor);
            scales.Save(infor.Scales);
            tracks.Save(infor.Tracks);
            if (dfs)
                dfiles.Save(infor.DFiles);

            Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
            settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            string str = Newtonsoft.Json.JsonConvert.SerializeObject(infor, settings);

            return str;
        }
    
        protected  int OnUpdateInfor(CVIDInfor infor)
        {
            if(infor == null)return -1;

            patternNames = infor.PatternNames.Split(new char[] { ':' });
            VdFill.PatternNames = patternNames;
            if (infor.IndexType != null)
                IndexType = (LogIndexType)infor.IndexType;
            IndexUnit = infor.IndexUnit;
            Orientation = infor.Orientation;

            top = infor.Top; bottom = infor.Bottom;
            IndexScale = infor.IndexScale;
            IndexUnit = infor.IndexUnit;

            viewLMargin = infor.ViewLMargin; //viewRMargin = infor.ViewRMargin;
            showTopInsert = infor.ShowTopInsert;
            showBottomInsert = infor.ShowBottomInsert;
            if (infor.IndexBarMaskColor != 0)
                IndexBarMaskColor = infor.IndexBarMaskColor;


            items.Restore(infor);
            scales.Restore(infor.Scales);
            tracks.Restore(infor.Tracks, this);

            dfiles.Restore(infor.DFiles);
            tracks.LeftMargin = infor.ViewLMargin;
    
            items.InitItems(this);

            return 0;
        }
  
        public void AddAuxItems(GvDocument gvDoc)
        {
            gvDoc.AddItem(ItemNameFont);
            gvDoc.AddItem(ItemNameFontFill);
            gvDoc.AddItem(ItemDescriptionFont);
            gvDoc.AddItem(ItemScaleFont);
            gvDoc.AddItem(DepthFont);
        }
        protected virtual void OnNewGvDocument(GvDocument gvDoc) { }

        public GvDocument CreateTopInsert( int block_id, SqliteDataBase file, IGvStream stream)
        {
            GvDocument gvDoc = new GvDocument(block_id, file, stream);
            OnNewGvDocument(gvDoc);
            tracks.LeftMargin = viewLMargin;
            firstSection = true;
            DrawItemHeads(gvDoc, true, true,  0);
            return gvDoc;
        }

        public GvDocument CreateBottomInsert(int block_id,  SqliteDataBase file, IGvStream stream)
        {
            GvDocument gvDoc =  new GvDocument(block_id, file, stream);
            OnNewGvDocument(gvDoc);

            gvDoc.AddItem(new GvBOS());

            //    string geFn = Name + "_bi";
            //    geDoc.CreateGEFile(geFn, true);
            firstSection = true;
            DrawItemHeads(gvDoc, false, true,  0);
            return gvDoc;
        }

        public GView.Models.SizeF DrawItemHeads(GvDocument gvDoc, bool topInsert, bool newDoc, float yOffset)
        {
            float insertH = items.CalcItemsHeadPos(topInsert);
            SizeF s = new SizeF((float)Tracks.Width, insertH);
            if (newDoc)
            {
                gvDoc.Size = s;
                gvDoc.AddItem(new GvBOS());
            }
           if (firstSection)
                AddAuxItems(gvDoc);
            foreach (VdItem item in items)
            {
                if (item.Enable && item.ShowHead)
                    item.DrawItemHead(gvDoc, firstSection, yOffset);
            }
            return s;
        }

        protected virtual void DrawItemsRt(GvDocument gvDoc, float yOffset, ISyslogRepository sysLog)
        {
            dfiles.LoadDataFiles(sysLog);
            items.CreateReaders(dfiles);
            items.DrawItems(top, bottom, gvDoc, firstSection, yOffset);
            items.DrawItemsRt(top,  gvDoc,  yOffset);
        }

        protected virtual void DrawItems(GvDocument gvDoc,float yOffset, ISyslogRepository sysLog)
        {
            dfiles.LoadDataFiles(sysLog);
            items.CreateReaders(dfiles);
            items.DrawItems(top, bottom, gvDoc, firstSection, yOffset);
        }

        public SizeF DrawPlotArea(GvDocument gvDoc, bool newDoc,  float yOffset, bool rt, ISyslogRepository sysLog)
        {   
            SizeF s = new SizeF((float)tracks.Width, (float)((bottom-top)*YScale));
            tracks.LeftMargin = viewLMargin;
            if(newDoc || rt)
            {
                gvDoc.Size = s;
                gvDoc.AddItem(new GvBOS()); 
            }
            if (firstSection)
                AddAuxItems(gvDoc);
            tracks.DrawLineAndNumber(gvDoc, yOffset);
            tracks.DrawDateTime(gvDoc, yOffset);
            items.InitItems(this);
            DrawItems(gvDoc, yOffset, sysLog);
            return s;
        } 

        public GvDocument CreatePlotArea( int block_id, SqliteDataBase file, IGvStream stream,  bool rt, ISyslogRepository sysLog)
        {
            GvDocument gvDoc = new GvDocument( block_id, file, stream );
            OnNewGvDocument(gvDoc);

            gvDoc.AddItem(new GvBOS());
            if (tracks.Count == 0)
                return null;
            firstSection = true;
            DrawPlotArea(gvDoc, true,  0, rt, sysLog);

            gvDoc.AddItem(new GvEOS());
            return gvDoc;
        }
        /*
        public SizeF DrawElements(GvDocument geDoc, float yOffset, ISyslogRepository sysLog)
        {
            SizeF s = new SizeF(0, 0);
            firstSection = true;
            float y = yOffset;
            if (showTopInsert)
            {
                SizeF s1 = DrawItemHeads(geDoc, true, false, yOffset);
                s.Merge(s1);
                firstSection = false;
                y += s1.Height;
            }

            SizeF s2 = DrawPlotArea(geDoc, false, y, false, sysLog);
            s.Merge(s2);
            firstSection = false;
            y += s2.Height;
            if (showBottomInsert)
            {
                SizeF s1 = DrawItemHeads(geDoc, false, false, y);
                s.Merge(s1);
            }
            return s;
        }
          */ 
    }
}
