using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace OpenWLS.Server.GView.Models
{
    [Flags]
    public enum DrawMode { Line = 1, Fill = 2}
    public enum FillMode { Color = 0, Pattern = 1 }
    public class GvRect : GvItem
    {
        public uint LineColor { get; set; }       //4  line color
        public uint FillColor { get; set; }      //8   fill color or pattern
        public int ImageId { get; set; }
        public DrawMode DrawMode { get; set; }         //9
        public byte Thickness { get; set; }     //10   
        public byte FillStyle { get; set; }          //11
        public byte LineStyle { get; set; }        //12

        public GvRect()
        {
            EType = GvType.Rect;
            FillStyle = 0;
            LineColor = 0;
            FillColor = 0;
            DrawMode = 0;
            Thickness = 0;
            //ptns = new List<float>();
        }

        protected override void RestoreExt(byte[] bs)
        {
            DataReader r = new DataReader(bs);

            LineColor = r.ReadUInt32();         //4
            FillColor = r.ReadUInt32();         //8 
            ImageId = r.ReadInt32(); 
            DrawMode = (DrawMode)r.ReadByte();  //9
            Thickness = r.ReadByte();           //10   
            FillStyle = r.ReadByte();           //11
            LineStyle = r.ReadByte();           //12
        }
        public override byte[]? GetItemExtBytes()
        {
            DataWriter w = new DataWriter(16);
            w.WriteData(LineColor);
            w.WriteData(FillColor);
            w.WriteData(ImageId);
            w.WriteData((byte)DrawMode);
            w.WriteData(Thickness);
            w.WriteData(FillStyle);
            w.WriteData(LineStyle);
            return w.GetBuffer();
        }

        
        public void AddRect(float x, float y, float width, float height)
        {
            if (x < Left) Left = x; 
            float r = x + width; if (r > Right) Right = r;
            GvRectSection s = new GvRectSection(Id, x, y, width, height);
            Doc.AddSection(this, s);
        }
    }

    public class GvRectSection : GvSection
    {
        protected float x, y, width, height;
        public override void YOffsetSection(float offset)
        {
            base.YOffsetSection(offset);
            y += offset;
            height += offset;
        }

        public override void ConvertToView(float dpiX, float dpiY)
        {
            base.ConvertToView(dpiX, dpiY);
            x *= dpiX; width *= dpiX;
            y *= dpiY; height *= dpiY;
        }

        public  GvRectSection()
        {

        
        }
        public System.Drawing.RectangleF GetRectF()
        {
            return new System.Drawing.RectangleF(x,y, width, height);   
        }
        public  GvRectSection(int iid, float x, float y, float width, float height)
        {
            //   float r = x + width;
            Top = y;  Bot = y + height;
            IId = iid;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
        public override void RestoreVal(byte[] bs)
        {
            DataReader r = new DataReader(bs);
            x = r.ReadSingle();
            y = r.ReadSingle();
            width = r.ReadSingle();
            height = r.ReadSingle();

        }
        public override byte[] GetValBytes()
        {
            DataWriter w = new DataWriter(16);
            w.WriteData(x);
            w.WriteData(y);
            w.WriteData(width);
            w.WriteData(height);
            return w.GetBuffer();
        }
    }
}
