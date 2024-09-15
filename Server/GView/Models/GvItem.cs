using OpenWLS.Server.Base;
using OpenWLS.Server.DBase;
using OpenWLS.Server.LogInstance;
using OpenWLS.Server.WebSocket;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenWLS.Server.GView.Models
{
    public interface IGvStreamObject
    {
        void Restore(DataReader r);
        byte[] GetBytes(GvDocument doc);
    }
    public struct RectangleF
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;
        public float Top { get { return Y; } }
        public float Left { get { return X; } }
        public float Bottom { get { return Y + Height; } }
        public float Right { get { return X + Width; } }

        //      [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return (Width <= 0) || (Height <= 0);
            }
        }

        public static readonly RectangleF Empty = new RectangleF();
        public static RectangleF Intersect(RectangleF a, RectangleF b)
        {
            float x1 = Math.Max(a.X, b.X);
            float x2 = Math.Min(a.X + a.Width, b.X + b.Width);
            float y1 = Math.Max(a.Y, b.Y);
            float y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);

            if (x2 >= x1
                && y2 >= y1)
            {

                return new RectangleF(x1, y1, x2 - x1, y2 - y1);
            }
            return RectangleF.Empty;
        }
        /*
                public RectangleF()
                {
                    X = 0;
                    Y = 0;
                    Width = 0;
                    Height = 0;
                }
        */
        public RectangleF(float x, float y, float w, float h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }
    }

    public struct SizeF
    {
        public float Width;
        public float Height;

        public SizeF(float w, float h)
        {
            Width = w;
            Height = h;
        }

        public void Merge(SizeF s)
        {
            Height += s.Height;
            Width = Math.Max(s.Width, Width);
        }

    }

    public enum GvType
    {
        Font = 1,  FillingImage = 0x11,
        Text = 0x101, Line = 0x102, CurveES = 0x103, CurveVS = 0x104, Image = 0x105, Rect = 0x106, Fill = 0x107, Symbol = 0x108,
        //GeTbl = 0x201, 
        BOS = 0x301, EOS = 0x302, YOffset = 0x303, BOA = 0x304, EOA = 0x305, BOU = 0x306, EOU = 0x307,
        NumberValueFormat = 0x401, NumberValue = 0402, Parameter = 0x403
    }
 
    public class GvItem : IGvStreamObject
    {
        public int Id { get; set; }
        public int BId { get; set; }        // Block Id
        public int GId { get; set; }        // Group Id        
        public GvType EType { get; set; }
 //       public string? Name { get; set; }
 //       public string? Desc { get; set; }   //Description        
        public bool? SBar { get; set; }         // show in scrollbar 
        public float Left { get; set; }
        public float Right { get; set; }
        public GvDocument Doc { get; set; }

        protected GvSection? sectionCur;
        protected GvSections sections;
        public GvSections Sections { get { return sections; } }
        public float Width
        {
            get { return Right - Left; }
            set
            {
                Right = Left + value;
            }
        }

   //     public GvSections Sections { get { return sections;  } }
     //   protected GvSection sectionCur;
        public GvItem()
        { 
            Id = -1;
            sections = new GvSections();
            Right = float.NegativeInfinity;
            Left = float.PositiveInfinity;
        }

        public void Restore(DataReader r)
        {
         //   EType = (GvType)r.ReadInt32();      //4
            Id = r.ReadInt32();                 //4
            BId = r.ReadInt32();                //4 
            GId = r.ReadInt32();                //4   
            Left = r.ReadSingle();
            Right = r.ReadSingle();
            SBar = r.ReadByte() == 0 ? null : true; // 1
            r.Seek(1, SeekOrigin.Current);
            if (!r.END)
                RestoreExt(r.ReadByteArrayToEnd());
        }       

        public byte[] GetBytes(GvDocument doc)
        {   
            byte[]? ext_bs = GetItemExtBytes();
            int s = 28;
            if (ext_bs != null)
                s += ext_bs.Length;
            DataWriter w = doc.GetDataWriter(s);
            w.WriteData(WsService.response_gv_item);   //2
            w.WriteData((int)EType);
            w.WriteData(Id);
            w.WriteData(BId);
            w.WriteData(GId);    
            w.WriteData(Left);
            w.WriteData(Right);                
            if(SBar == null)w.WriteData( (byte)0 );
            else w.WriteData( (byte)1 );
            w.Seek(1, SeekOrigin.Current);
            if (ext_bs != null)
                w.WriteData(ext_bs);
            return w.GetBuffer();
        }
        
        protected virtual void RestoreExt(byte[] ext) { }
        public virtual byte[]? GetItemExtBytes()
        {
            return null;
        }

        public virtual void OffsetID(int offset)
        {
            Id += offset;
        }

        public virtual void FlushSection()
        {
            if (sectionCur != null)
            {
                Doc.AddSection(this, sectionCur);
                sectionCur = null;
            }
        }

        /*
        public void AddSection(GvSection section)
        {
            section.IId = Id;
            sections.Add(section);
        }
        */
       
        public void Restore(DataRow dr)
        {
            Id = Convert.ToInt32(dr["Id"]);
            BId = Convert.ToInt32(dr["BId"]);   
            GId = Convert.ToInt32(dr["GId"]);                        
            EType = (GvType)Convert.ToInt32(dr["EType"]);
        //    if (dr["Name"] != DBNull.Value) Name = (string)dr["Name"];
        //    if (dr["Desc"] != DBNull.Value) Desc = (string)dr["Desc"];
            if (dr["SBar"] != DBNull.Value) SBar = true;
            if (dr["Ext"] != DBNull.Value) RestoreExt((byte[])dr["Ext"]);
        }

        public void AddToDb(SqliteDataBase db)
        {
            string sql = $"INSERT INTO Elems ( Id, BId, GId, EType,  SBar, Ext ) VALUES ( {Id}, {BId}, {GId}, {EType}";
        //    if (Name == null) sql = sql + " , NULL";
        //    else sql = sql + " , {Name}";
        //    if (Desc == null) sql = sql + " , NULL";
        //    else sql = sql + " , {Desc}";
            if (SBar == null) sql = sql + " , NULL";
            else sql = sql + " , 1";

            byte[]? bs = GetItemExtBytes();
            if (bs == null) sql = sql + ", NULL )";
            else sql = sql + ", @ext )";
            SQLiteParameter para = new SQLiteParameter("@ext", System.Data.DbType.Binary);
            para.Value = bs;
            db.ExecuteNonQuery(sql, para);

        }
        public virtual void ConvertToView(float dpiX, float dpiY)
        {
            Left *= dpiX;
            Right *= dpiX;
            foreach (GvSection s in sections)
                s.ConvertToView(dpiX, dpiY);
        }

        public virtual void OffsetY(float offset)
        {
            foreach (GvSection s in sections)
                s.YOffsetSection(offset);
        }
    }

    public class GvItems : List<GvItem>
    {

    }



}
