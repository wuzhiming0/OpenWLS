using System;

using System.Collections.Generic;
using OpenWLS.Server.GView.Models;
using OpenWLS.Server.Base;
using System.Windows.Threading;
using System.IO;

using System.Drawing;

namespace OpenWLS.Client.GView.Models
{
    partial class AGvView  //active GvView
    {
      //  public delegate void CustomEventHandler(object sender, EventArgs a);
        public event EventHandler<EventArgs> ViewTotalSizeChanged;
        public event EventHandler<EventArgs> ParaSelectionChanged;

        delegate void UpdateItemsDelegate();
        delegate void UpdateParasDelegate();

        double scrollX;
        public Size sizeView ;  // size in dots
        Size sizeBuf;
        Image imageBuf;
        Graphics gBuf;
        float dpiX;
        float dpiY;

        public double ScrollX
        {
            get { return scrollX; }
            set { scrollX = value;
            }
        }

        public int MaxHightBuf{get; set;}

        double scrollY;
        public double ScrollY
        {
            get { return scrollY; }
            set { scrollY = value; }
        }


        public double bufTop;
        public double bufBot;
    //    public double 

        protected AGvDocument doc;
        protected GvItemCs items;

        public GvItemCs Items { get { return items; } }
        public AGvDocument Doc { get { return doc; }
        set
            {
                doc = value;
                items.CreateFromItems(doc.Items);

                SetupBuffer();
                items.Init(dpiX, dpiY, items);

                UpdateItems();
            }
        }

  /*      public GvParameterCs Paras
        {
            get
            {
               return items.Paras;
            }
        }

        public void ProcessBinMsg(DataReader r)
        {
            IGvItemC ge = GvItemCs.CreateElement(r, doc);
            if (ge != null)
            {
                if (ge is GeBOS)
                    items.Clear();

                if (ge is GeEOS)
                {
                    items.Init(dpiX, dpiY, items);
                    SetupBuffer();
                    UpdateItems();
                    return;
                }
                if (ge is GeYOffset)
                {
                    items.OffsetY(((GeYOffset)ge).Offset, (float)dpiY);
                    return;
                }

                items.Add(ge);
            }
        }
*/
        public int OpenAGVDoc(string fn)
        {
     //       doc.OpenGEFile(fn, false);
            items.CreateFromDoc(doc);

         //   doc.Close();

            SetupBuffer();
            items.Init(dpiX, dpiY, items);

            UpdateItems();
            return 0;
        }

        void SaveOpenAGVDoc(string fn)
        {
    /*        int err = doc.CreateGEFile(fn, false);
            if (err != 0)
                return ;
            foreach (IGvItemC e in items)
                doc.SaveElement((GvItem)e);

            doc.SaveElement(doc.Infors);
            doc.Close();
*/
        }

        public void UpdateValues(string str)
        {
            string[] strs = str.Split(new char[] { '|' });
            foreach (string s in strs)
            {
                int k = s.IndexOf(':');
                string n = s.Remove(k, str.Length - k);
                string v = s.Remove(0, k + 1);
                GvNumberSection gv = items.GetValueSection(n);
                if (gv != null)
                    gv.Value = Convert.ToSingle(v);
            }

        }

        public void UpdateParas()
        {
            if (InvokeRequired)
                BeginInvoke(new UpdateParasDelegate(UpdateParas), null);
            else
            {
                Controls.Clear();
                Controls.Add(hbar);
                Controls.Add(vbar);
                foreach (IGvItemC e in items)
                {
                    if (e is GvParameterC)
                    {
                        GvParameterC p = (GvParameterC)e;
                        p.Init(dpiX, dpiY, items);
                        foreach (GvParameterSectionC s in p.Sections)
                        {
                            Controls.Add(s.Cntl);
                            s.Cntl.GotFocus += Cntl_GotFocus;
                        }
                    }
                }
            }
          //  UpdateView();
        }

        void Cntl_GotFocus(object sender, EventArgs e)
        {
            System.Windows.Forms.Control cntl = (System.Windows.Forms.Control)sender;
            items.SelectedPara = (GvParameterSectionC)cntl.Tag;
            if (ParaSelectionChanged != null)
                ParaSelectionChanged(items.SelectedPara, new EventArgs());
        }

        public void ClearParameters()
        {
            bool b = true;
            while (b)
            {
                b = false;
                foreach (IGvItemC e in items)
                {
                    if (e is GvParameterC)
                    {
                        b = true;
                        items.Remove(e);
                        break;
                    }
                }
            }
        }

        public void UpdateItems()
        {
            //   Dispatcher dispUI = Dispatcher.CurrentDispatcher;
            if (InvokeRequired)
                BeginInvoke(new UpdateItemsDelegate(UpdateItems), null);
            else
            {
                CreateBuffer();
                AGvView_SizeChanged(this, new EventArgs());
                UpdateView();
                UpdateParas();
            }

        }

        public void UpdateView()
        {
            if (gBuf == null)
                return;

            double t = scrollY;
            double b = t + Height;
            if (bufTop > t || bufBot < b || double.IsNaN(bufTop))
            {
                bufTop = t - 1000;
                if(bufTop < 0)
                    bufTop = 0;
                bufBot = bufTop + sizeBuf.Height;
                DrawBuffer();
            }

            foreach (IGvItemC p in items)
                p.OffsetControl(-scrollX, -scrollY);

            Graphics g = Graphics.FromHwnd(this.Handle);
            g.Clear(Color.White);
            if (imageBuf != null)
                g.DrawImage(imageBuf, (float)-scrollX, (float)(bufTop - scrollY) );

        }

        void DrawBuffer()
        {
            gBuf.Clear(Color.White);
            items.DrawItems(gBuf, bufTop, bufBot);                      ///
            Parent.Width = sizeView.Width;
            Parent.Height = sizeView.Height;
        }

        void SetupBuffer()
        {
            System.Drawing.SizeF fSize = new System.Drawing.SizeF(doc.Size.Width, doc.Size.Height);
            sizeView.Width = (int)(fSize.Width * dpiX);
            sizeView.Height = (int)(fSize.Height * dpiY);
            sizeBuf.Width = sizeView.Width;
            sizeBuf.Height = sizeView.Height < MaxHightBuf  ? sizeView.Height : MaxHightBuf;
            if (sizeBuf.Width==0 || sizeBuf.Height == 0)
                return;
            if (ViewTotalSizeChanged != null)
                ViewTotalSizeChanged(this, new EventArgs());     
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
            UpdateView();
        }


        public void SaveValueFile(string fn)
        {
            string str = doc.Type + "\n";

  //          foreach (GvParameterC g in Paras)
  //              str = str + ((GvParameter)g).Para.ToString() + "\n";

            FileStream fs = new FileStream(fn, FileMode.OpenOrCreate);
            fs.SetLength(0);
            byte[] bs = StringConverter.ToByteArray(str);
            fs.Write(bs, 0, bs.Length);
            fs.Close();
        }

        public void LoadValueFile(string fn)
        {
            FileStream fs = new FileStream(fn, FileMode.Open);
            byte[] bs = new byte[fs.Length];
            fs.Read(bs, 0, bs.Length);
            fs.Close();
            string str = StringConverter.ToString(bs);
            string[] strs = str.Split(new char[] { '\n' });
            if (doc.Type != strs[0].Trim())
                return;
            char[] cc = new char[] { ':' };
            for (int i = 1; i < strs.Length; i++)
            {
                string[] ss = strs[i].Split(cc);
                if (ss.Length != 4) //name,description,zone,value
                    continue;
                string name = ss[0].Trim();
                foreach (GvItem g in items)
                {
                    if (g is GvParameterC)
                    {
                        GvParameterC p = (GvParameterC)g;
                        foreach (GvParameterSectionC s in p.Sections)
                        {
                            if (s.Para.Name == ss[0].Trim())
                                s.Para.Val= new string[] { ss[3].Trim() };
                        }
                    }
                }
            }
        }
 
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
            components = new System.ComponentModel.Container();
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        }

        #endregion
    }
}
