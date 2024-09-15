using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using OpenWLS.Server.Base;
using OpenWLS.Server.DBase.Models.LocalDb;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

//using System.Drawing;

namespace OpenWLS.Client.Base
{
    /// <summary>
    /// Interaction logic for SysLog.xaml
    /// </summary>
    public partial class SysLogCntl : RichTextBox
    {
        public static string DFN_Prefix = "data/syslog_";
        public static string DFN_extension = ".rtf";

       // MsgProc proc;

  //      delegate void AddMessageDelegate(List<SyslogItem> items);
        SolidColorBrush brush_d_blu;
        SolidColorBrush brush_blk;
        SolidColorBrush brush_blu;
        SolidColorBrush brush_red;
        SolidColorBrush brush_org;

        public SysLogCntl()
        {
            InitializeComponent();
            SetValue(Paragraph.LineHeightProperty, 1.0);
            brush_d_blu = new SolidColorBrush(Colors.DarkBlue);
            brush_blk = new SolidColorBrush(Colors.Black);
            brush_blu = new SolidColorBrush(Colors.Blue);
            brush_red = new SolidColorBrush(Colors.Red);
            brush_org = new SolidColorBrush(Colors.Orange);
        }

        static public void AddToTxtDocument(FlowDocument doc, string str, Color color)
        {
            SolidColorBrush b = new SolidColorBrush(color);
            AddToTxtDocument(doc, str, b);
        }

        static public void AddToTxtDocument(FlowDocument doc, string str, SolidColorBrush b)
        {
            TextRange tr = new TextRange(doc.ContentEnd, doc.ContentEnd);
            tr.Text = str;
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, b);
        }

        void AddLine(string lineText, Color color)
        {
            try
            {
                if (color == Colors.DarkBlue)
                {
                    AddToTxtDocument(Document, lineText, brush_d_blu);
                    return;
                }
                if (color == Colors.Black)
                {
                    AddToTxtDocument(Document, lineText, brush_blk);
                    return;
                }
                if (color == Colors.Blue)
                {
                    AddToTxtDocument(Document, lineText, brush_blu);
                    return;
                }
                if (color == Colors.Red)
                {
                    AddToTxtDocument(Document, lineText, brush_red);
                    return;
                }
                if (color == Colors.Orange)
                {
                    AddToTxtDocument(Document, lineText, brush_org);
                    return;
                }
                AddToTxtDocument(Document, lineText, color);
            }
            catch(Exception e)
            {
                AddToTxtDocument(Document, e.Message, Colors.Red);
              //  if(lineText.Length > 1)
              //      AddToTxtDocument(Document, lineText, Colors.Black);
            }

        }


        public void AddMessages(List<SyslogItem> items)
        {
            Dispatcher.BeginInvoke(() =>
            {
                foreach (SyslogItem item in items)
                    AddMessage(item);
                ScrollToEnd();
            });
        }

        public void AddMessage(SyslogItem item)
        {
            Dispatcher.BeginInvoke(() =>
            {
                AppendText("\n");
                AddLine(DateTime.FromBinary(item.Time).ToShortTimeString() + "\n", Colors.Black);
                if (item.Color == null)
                    AddLine(item.Msg + "\n", Colors.Black);
                else
                {
                    byte[] bs = BitConverter.GetBytes((uint)item.Color);
                    AddLine(item.Msg + "\n", Color.FromArgb(bs[3], bs[2], bs[1], bs[0]));
                }
            });
        }
        public void AddMessage(string msg)
        {
            SyslogItem si = new SyslogItem()
            {
                Msg = msg,
                Time = DateTime.Now.Ticks 
            };
            AddMessage(si);
        }
        public void AddMessage(string msg, Color c)
        {
            SyslogItem si = new SyslogItem()
            {
                Msg = msg,
                Time = DateTime.Now.Ticks,
                Color = ConvertToUint32(c)
            };
            AddMessage(si);
        }
        public void AddMessage(string msg, uint c)
        {
            SyslogItem si = new SyslogItem()
            {
                Msg = msg,
                Time = DateTime.Now.Ticks,
                Color = c
            };
            AddMessage(si);
        }
        public void AddMessage(string msg, bool normal)
        {
            SyslogItem si = new SyslogItem()
            {
                Msg = msg,
                Time = DateTime.Now.Ticks,
                Color = normal? null : ConvertToUint32(Colors.Red)
            };
            AddMessage(si);
        }

        static public uint ConvertToUint32(System.Windows.Media.Color c)
        {
            return (uint)((c.A << 24) | (c.R << 16) |
                    (c.G << 8) | (c.B << 0));
        }

        public void AppendMessage(string msg)
        {
            Dispatcher.BeginInvoke(() =>
            {
                AppendText(msg);
            });
        }
    }
}
