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
    public partial class GvView : UserControl
    {
       // [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
      //  public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

    

        public GvView()
        {
            doc = new GvDocument();
            items = new GvItemCs();
            scrollX = 0;
            scrollY = 0;

            MaxHightBuf = 4000;
            InitializeComponent();
            Graphics g = Graphics.FromHwnd(this.Handle);

            dpiX = g.DpiX;
            dpiY = g.DpiY;
 

           // IntPtr desktop = g.GetHdc();

         //   dpiX = GetDeviceCaps(desktop, (int)DeviceCap.LOGPIXELSX);
         //   dpiY = GetDeviceCaps(desktop, (int)DeviceCap.LOGPIXELSY);    
        }



    }
}
