using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenWLS.Server.GView.Models;
using OpenWLS.Server.Base;

namespace OpenWLS.Client.GView.Models
{
    public partial class AGvView : UserControl
    {
       // [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
      //  public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

        System.Windows.Forms.HScrollBar hbar;
        System.Windows.Forms.VScrollBar vbar;

        public AGvView()
        {
            doc = new AGvDocument();
            items = new GvItemCs();
     //       paras = new GvParameterCs();
            scrollX = 0;
            scrollY = 0;

            MaxHightBuf = 4000;
            InitializeComponent();
            Graphics g = Graphics.FromHwnd(this.Handle);

            dpiX = g.DpiX;
            dpiY = g.DpiY;
            hbar = new System.Windows.Forms.HScrollBar();
            vbar = new System.Windows.Forms.VScrollBar();
            hbar.Height = 15;
            vbar.Width = 15;
            hbar.Visible = false;
            vbar.Visible = false;
            Controls.Add(hbar);
            Controls.Add(vbar);

            this.SizeChanged += AGvView_SizeChanged;
            hbar.ValueChanged += hbar_ValueChanged;
            vbar.ValueChanged += vbar_ValueChanged;
        }

        void vbar_ValueChanged(object sender, EventArgs e)
        {
            if (scrollY != vbar.Value)
            {
                scrollY = vbar.Value;
                UpdateView();
            }
        }

        void hbar_ValueChanged(object sender, EventArgs e)
        {
            if (scrollX != hbar.Value)
            {
                scrollX = hbar.Value;
                UpdateView(); 
            }
         
        }

        void AGvView_SizeChanged(object sender, EventArgs e)
        {
            if(imageBuf == null)
                return;
            if (Size.Width >= imageBuf.Width && Size.Height >= imageBuf.Height)
            {
                vbar.Visible = false;
                hbar.Visible = false;
                hbar.Value =0;  vbar.Value =0;
                return;                
            }

            if(Size.Width < imageBuf.Width){
                int h = Size.Height - hbar.Height;
                hbar.Location = new Point(0, h);
                hbar.Width = Size.Width;
                hbar.Visible = true;
                int w = Size.Width;
                if (h >= imageBuf.Height){
                    vbar.Visible = false;
                    vbar.Value = 0;
                }

                else
                {
                     w -= vbar.Width;
                    vbar.Location = new Point(w, 0);
                    vbar.Height = h; 
                    vbar.Visible = true;
                    vbar.Maximum = imageBuf.Height - h;
                }
                hbar.Maximum = imageBuf.Width - w;
                return;
            }
            if (Size.Height < imageBuf.Height)
            {
                int w = Size.Width - vbar.Width;
                vbar.Location = new Point(w, 0);
                vbar.Height = Size.Height;
                vbar.Visible = true;
                if (w >= imageBuf.Width)
                {
                    hbar.Visible = false;
                    hbar.Value = 0;
                }
                else
                {
                    int h = Size.Height - vbar.Height;
                    hbar.Location = new Point(0, h);
                    hbar.Width = w;
                    hbar.Visible = true;
                }
            }
            vbar_ValueChanged(null, null);
            hbar_ValueChanged(null, null);
            
        }



    }
}
