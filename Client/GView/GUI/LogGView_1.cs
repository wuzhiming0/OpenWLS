using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


using System.Windows;
using OpenWLS.Server.GView.ViewDefinition;
using OpenWLS.Client.Requests;
using OpenWLS.Server.Base;
using OpenWLS.Client.LogDataFile;
using OpenWLS.Server.DBase.Models.GlobalDb;
using System.Windows.Forms;
using OpenWLS.Server.WebSocket;
using OpenWLS.Server.GView.Models;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using System.Drawing;
using System.Windows.Shapes;
using OpenWLS.Client.GView.Models;
using OpenWLS.Server.LogDataFile;
using System.Net.WebSockets;
using OpenWLS.Server.LogInstance.OperationDocument;
using OpenWLS.Client.LogInstance;

namespace OpenWLS.Client.GView.GUI
{
    public partial class LogGView : IWsMsgProc
    {
        GvView procView;
        public event EventHandler<EventArgs> ViewTitleChanged;

        //        LogGView logGView;

        public void Init(GViewDefinitionFile vdf, bool rt, bool global, bool horizontal)
        {
            Global = global;
            RealTime = rt;
            ViewDefinitionFile = vdf;
            SetOrientation(horizontal);

            editorCntl.editBar.Name = Name = vdf.Name;
            editorCntl.editBar.nameTb.IsEnabled = vdf.Id < 0;

            if (rt)
            {
                editorCntl.autoScrollCb.Visibility = Visibility.Visible;
                LiClientMainCntl?  mc = LiClientMainCntl.GetLiClientMainCntl();
                if (mc != null)
                {
                    mc.RtVdFile = new VdDFile();
                    mc.RtVdFile.Measurements = new VdMeasurements(mc.GetOperationDocument().Measurements, mc.RtVdFile);
                    editorCntl.RtMeasurements = mc.RtVdFile.Measurements; 
                }

            }
            else
                RequestChannelNames(doc.DFiles);
            editorCntl.DFiles = doc.DFiles;
        }


        public void SaveCVIDFile(string fn, string json)
        {
            vdf.Body = json;
            vdf.Name = fn;
            if (Global) vdf.Id = -1;
            if (vdf.Id < 0)
                VdfRequest.CreateVdfAsync(vdf, false);
            else
                VdfRequest.UpdateVdfAsync(vdf, false);
        }

        public async Task UpdateDisplayAsync(VdDocument doc)
        {
            if (realTime)
            {
                LiClientMainCntl? liClientMain = LiClientMainCntl.GetLiClientMainCntl();
                VdDocumentRt vrt = (VdDocumentRt)doc; 
              //  if(vrt.Connected)
               //     liClientMain.SendUpdateGViewRequest()
              //  else


                    
            }
            else
            {
                procView = insertView;
                await VdfRequest.GenerateGvDocFromVdf(doc.GetJSon(true), RealTime, this);
                editorCntl.tracksCntl.UpdateTracks();
            }
        }


        public async void  RequestChannelNames(VdDFiles dfiles)
        {
            foreach (VdDFile f in dfiles)
            {
                DataFileInfor dfi = await LdfRequest.Open(f.Job, f.Name);
                f.CreateMeasurements(dfi.Measurements);
                f.NMRecords = dfi.NMRecords;
            }
        }

        public void New(bool horizontal, bool rt)
        {
            GViewDefinitionFile vdf = new GViewDefinitionFile();
            Init(vdf, rt, false, horizontal);
        }

        public async void Open(GViewDefinitionFile vdf, bool global, bool rt)
        {
            if (vdf.Body == null)
                vdf.Body = VdfRequest.GetBody(vdf.Id, global).Result.Val;
   //         doc.FileName = fn;
            Init(vdf, rt, global, false);

        }


        public void ProcessWsRxMsg(DataReader r)
        {
            if (procView != null)
            {
                procView.ProcessWsRxMsg(r);
                if (procView == plotView && plotView.EOSge)
                    // scrollBarView.UpdateBar();
                    scrollBarView.ReDrawCurve();
            }
        }

        public void RequestEnd()
        {

        }
        public void SetWebSocket(ClientWebSocket ws)
        {
            // webSocket = ws;
        }
      
        public VdDocumentOd GetRtVdDoc(bool dfs)
        {
            return new VdDocumentOd() {
                Id = (int)Id,
                Name = Name,
                Content = doc.GetJSon(dfs)
            };
        }

    }
    public class LogGViews: List<LogGView>
    {

    }
}
