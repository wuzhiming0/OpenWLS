using System;

using OpenWLS.Server.GView.Models;
using OpenWLS.Server.Base;
using System.Windows.Threading;

using System.Drawing;
using System.Windows.Controls;
using System.Drawing.Drawing2D;
using OpenWLS.Server.WebSocket;
using System.Linq;

namespace OpenWLS.Client.GView.Models
{
    partial class GvView
    {
        //  public delegate void CustomEventHandler(object sender, EventArgs a);
        public event EventHandler<EventArgs> ViewTotalSizeChanged;
        public event EventHandler<EventArgs> EOSReceived;

        double scrollX;
        public Size sizeView;  // size in dots
        Size sizeBuf;
        System.Drawing.Image imageBuf;
        Graphics gBuf;
        double dpiX;
        double dpiY;
        bool realTime;

//        bool rtUpdate;

        public Orientation Orientation { get; set; }


        public bool RealTime {
            get { return realTime; }
            set { realTime = value; }
        }

        public double ScrollX
        {
            get { return scrollX; }
            set { scrollX = value; }
        }

        public int MaxHightBuf { get; set; }

        double scrollY;
        public double ScrollY
        {
            get { return scrollY; }
            set { scrollY = value; }
        }

        public bool EOSge { get; set; }  //end of section

        public double bufTop;
        public double bufBot;

        GvDocument doc;
        GvItemCs items;

        int appendIndex;
        public GvItemCs Items
        {
            get { return items; }
        }
        delegate void UpdateItemsDelegate();
        public void UpdateItems()
        {
            //   Dispatcher dispUI = Dispatcher.CurrentDispatcher;
            if (InvokeRequired)
                BeginInvoke(new UpdateItemsDelegate(UpdateItems), null);
            else
            {
                CreateBuffer(); UpdateView(false);
            }
        }
        delegate void AppendItemsDelegate();
        public void AppendItems()
        {
            //   Dispatcher dispUI = Dispatcher.CurrentDispatcher;
            if (InvokeRequired)
                BeginInvoke(new AppendItemsDelegate(AppendItems), null);
            else
            {
                items.Init((float)dpiX, (float)dpiY, items);
                UpdateView(true);
            }
        }

        void SetIndexAuto()
        {
            if (Orientation == Orientation.Vertical)
            {
                scrollY = doc.Size.Height - sizeView.Height; 
            }
            else
            {
                scrollX = doc.Size.Height - sizeView.Width;
            }
        }
        public void UpdateView()
        {
            UpdateView(false);
        }
        
        public void UpdateView(bool append)
        {
            if (gBuf == null)
                return;
            if (Orientation == Orientation.Vertical)
            {
                if (double.IsNaN(scrollY) || double.IsNegativeInfinity(scrollY))
                    return;       
                double t = scrollY;
                double b = t + Height;
                if ( t < bufBot || b > bufTop  || double.IsNaN(bufTop))
                {
                    bufTop = t - 1000;
                    if (bufTop < 0)
                        bufTop = 0;
                    bufBot = bufTop + sizeBuf.Height;
                    DrawBuffer( append);
                }

                Graphics g = Graphics.FromHwnd(this.Handle);
                if (imageBuf != null)
                    g.DrawImage(imageBuf, (float)-scrollX, (float)(bufTop - scrollY));
            }
            else 
                UpdateViewH( append);
        }

        public void UpdateViewH(bool append)
        {
            if (imageBuf != null)
            {
                double t = scrollX;
                if (double.IsNaN(scrollX) || double.IsInfinity(scrollX))
                    return;
                double b = t + Height;
                if ( !(b < bufTop || t > bufBot) || double.IsNaN(bufTop))
                {
                    bufTop = t - 1000;
                    if (bufTop < 0)
                        bufTop = 0;
                    bufBot = bufTop + sizeBuf.Height;
                    DrawBuffer(append);
                }

                Graphics g = Graphics.FromHwnd(this.Handle);
                imageBuf.RotateFlip( RotateFlipType.Rotate270FlipNone);
                try
                {
                    g.DrawImage(imageBuf, (float)(bufTop - scrollX), (float)-scrollY );
                }
                catch(Exception e)
                {

                }

                imageBuf.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
        }

        public void DrawBuffer(bool append)
        {
            if(append)
            {
                for (int i = appendIndex; i < doc.Items.Count; i++)
                {
                    ((IGvItemC)doc.Items[i]).DrawItem(gBuf,  (float)bufTop, (float)bufBot);
                }
            }
            else
            {
                gBuf.Clear(Color.White);
                items.DrawItems(gBuf, bufTop, bufBot);     
            }                                           ///
            Parent.Width = sizeView.Width;
            Parent.Height = sizeView.Height;
        }


        void SetupBuffer()
        {
            scrollX = 0;
            scrollY = 0;
            System.Drawing.SizeF fSize = new System.Drawing.SizeF(doc.Size.Width, doc.Size.Height);
            sizeView.Width = (int)(fSize.Width * dpiX);
            sizeView.Height = (int)(fSize.Height * dpiY);

            sizeBuf.Width = sizeView.Width;
            sizeBuf.Height = sizeView.Height < MaxHightBuf || RealTime ? sizeView.Height : MaxHightBuf;
            if (sizeBuf.Width==0 || sizeBuf.Height == 0)
                return;

            items.Init((float)dpiX, (float)dpiY, items);

            if (ViewTotalSizeChanged != null)
                ViewTotalSizeChanged(this, new EventArgs());     
        }


        delegate void ClearViewDelegate();
        public void ClearView()
        {
            //   Dispatcher dispUI = Dispatcher.CurrentDispatcher;
            if (InvokeRequired)
                BeginInvoke(new ClearViewDelegate(AppendItems), null);
            else
            {
                Graphics g = Graphics.FromHwnd(this.Handle);
                g.Clear(System.Drawing.Color.White);
            }
        }


        public void  CreateBuffer(){
            Graphics g = Graphics.FromHwnd(this.Handle);
            imageBuf = new Bitmap(sizeBuf.Width+2, sizeBuf.Height+1, g);
            gBuf = Graphics.FromImage(imageBuf);
            bufTop = double.NaN;
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            UpdateView(false);
        }
        void ProcessGvGroup(DataReader r)
        {

        }
        void ProcessGvItem(DataReader r)
        {
            GvType item_type = (GvType)r.ReadInt32();
            GvItem item = null;
            EOSge = false; 
            switch (item_type)
            {
                case GvType.Font:
                    item = new GvFontC();
                    break;

                case GvType.FillingImage:
                    item = new GvFillImageC();
                    break;
                case GvType.Text:
                    item = new GvTextC();
                    break;
                case GvType.Line:
                    item = new GvLineC();
                    break;
                case GvType.CurveES:
                    item = new GvCurveEsC();
                    break;
                case GvType.CurveVS:
                    item = new GvCurveVsC();
                    break;
                case GvType.Image:
                    item = new GvImageC();
                    break;
                case GvType.Rect:
                    item = new GvRectC();
                    break;
                case GvType.Fill:
                    item = new GvFillC();
                    break;
                case GvType.Symbol:
                    item = new GvSymbolC();
                    break;

                case GvType.BOS:
                    items.Clear();
                    doc.Items.Clear();
                    doc.Groups.Clear();
                    item = new GvBOSC();
                    break;
                case GvType.EOS:
                    SetupBuffer();
                    UpdateItems();
                    EOSge = true;
                    if (EOSReceived != null)
                        EOSReceived(this, null);
                    break;
                case GvType.YOffset:
                    break;
                case GvType.BOA:
                    break;
                case GvType.EOA:    //end od append
                    SetupBuffer();
                    UpdateItems();
                    EOSge = true;
                    break;
                case GvType.BOU:
                    items.Clear();
                    doc.Items.Clear();
                    doc.Groups.Clear();
                    break;
                case GvType.EOU:
                    break;
                case GvType.NumberValueFormat:
                    break;
                case GvType.NumberValue:
                    break;
                case GvType.Parameter:
                    break;
                default: break;
            }
            if (item != null)
            {
                item.Doc = doc;
                item.Restore(r);
                doc.Items.Add(item);
                items.Add((IGvItemC)item);
            }
        }
        void ProcessGvSection(DataReader r)
        {
            int item_id = r.ReadInt32();
            GvItem item = doc.Items.Where(i=> i.Id == item_id).FirstOrDefault();
            if (item == null) {
                return;
            }
            GvSection section = null;
            switch (item.EType)
            {
                case GvType.Text:
                    section = new GvTextSectionC();
                    break;
                case GvType.Line:
                    section = new GvLineSectionC();
                    break;
                case GvType.CurveES:
                    section = new GvCurveSection();
                    break;
                case GvType.CurveVS:
                    section = new GvCurveVsSection();
                    break;
                case GvType.Image:
                    section = new GvImageSectionC();
                    break;
                case GvType.Rect:
                    section = new GvRectSectionC();
                    break;

                case GvType.Symbol:
                    section = new GvSymbolSectionC();
                    break;
        /*        case GvType.Fill:
                    section = new GvFillSectionC();
                    break;
                case GvType.FillingImage:
                    item = new GvFillImageC();
                    break;
                /*
            case GvType.BOS:
                break;
            case GvType.EOS:
                break;
            case GvType.YOffset:
                break;
            case GvType.BOA:
                break;
            case GvType.EOA:    //end od append
                break;
            case GvType.BOU:
                break;
            case GvType.EOU:
                break;
                */
                case GvType.NumberValueFormat:
                    break;
                case GvType.NumberValue:
                    section = new GvNumberSectionC();
                    break;
                case GvType.Parameter:
                    section = new GvParameterSectionC();
                    break;
                default: break;
            }
            if (section != null)
            {
                section.Restore(r);
                item.Sections.Add(section);
                
            }
        }

        public void ProcessWsRxMsg(DataReader r)
        {
            ushort msg_type = r.ReadUInt16();
            switch (msg_type)
            {
                case WsService.response_gv_group:
                    ProcessGvGroup(r);
                    break;
                case WsService.response_gv_item:
                    ProcessGvItem(r);
                    break;
                case WsService.response_gv_section:
                    ProcessGvSection(r);
                    break;
            }
        }
        /*
        public void ProcessBinMsg(DataReader r)
        {
            EOSge = false;
            IGvItemC ge = GvItemCs.CreateElement(r, doc);
            if (ge != null)
            {
                if (ge is GvBOS)
                {
                   items.Clear();
                   return;
                }

                if (ge is GvEOS)
                {
                    SetupBuffer();
                    UpdateItems();
                    EOSge = true;
                    return;
                }

                if (ge is GvBOU)
                {
                    items.Clear();
                    return;
                }

                if (ge is GvEOU)
                {
                    SetupBuffer();
                    UpdateItems();
                    EOSge = true;
                    return;
                }

                if (ge is GvBOA)
                {
                    appendIndex = items.Count;
                    return;
                }
                if (ge is GvEOA)
                {
                    AppendItems();
                    EOSge = true;
            //        appendIndex = items.Count;
                    return;
                }

                if (ge is GvYOffset)
                {
                    items.OffsetY(((GvYOffset)ge).Offset, (float)dpiY);
                    return;
                }
               // if(RealTime && rtUpdate)   
               //     items.AddRealTimeItem(ge);
               // else
                    items.Add(ge);                    
            }
        }*/

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Orientation = Orientation.Vertical;
            components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        }

        #endregion
    }
}
