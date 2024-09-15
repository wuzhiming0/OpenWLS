using Newtonsoft.Json;
using OpenWLS.Client.LogInstance;
using OpenWLS.Client.Base;
using OpenWLS.Client.Dock;
using OpenWLS.Client.GView.GUI;
using OpenWLS.Client.Requests;
using OpenWLS.Server.LogInstance.OperationDocument;
using OpenWLS.Server.DBase.Models.GlobalDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OpenWLS.Client.LogInstance.Instrument;

namespace OpenWLS.Client
{
    public partial class MainWindow
    {
        LiClientMainCntl? liCntl;
        public LiClientMainCntl? LiClientMain
        {
            get { return liCntl; }
        }
        N1dDisplay n1dDisplay;
        public N1dDisplay N1dDisplay {  get { return n1dDisplay; } }
        public void AddMDisplayCntl(MeasurementDisplay cntl)
        {
            DockControl dc = new DockControl();
            dc.Title = cntl.Name;     
            dc.CanClose = false;
            dc.SetCntl(cntl);
            cntl.DockControl = dc;
            if (_layoutRoot.BottomSide.Children.Count == 0)
                _layoutRoot.BottomSide.Children.Add(displayDockGrp);
       //     if (displayDockGrp.Root == null)
       //         displayDockGrp.Parent = botSideL2;

            displayDockGrp.Children.Add(dc);

            //       displayCntls.Add(dc);
        }

        void ConnectLogInstance(int li_id)
        {
            CreateLogInstanceMainCntl(li_id);
            // liDock.IsVisible = true;
            liCntl.StartWs();
        }

        void CreateNewLogInstance(int ocf_id)
        {
            int sim = ClientGlobals.Simulation == null ? 0 : 1;
            int edge_id = ClientGlobals.PerferedEdge == null ? 1 : (int)ClientGlobals.PerferedEdge;
            Server.LogInstance.LogInstance? li = LogInstanceRequest.CreateAsync(ClientGlobals.ActiveJob.Id, ocf_id, sim, edge_id).Result;
            if (li == null) return;
            ConnectLogInstance(li.Id);
        }

        public void OpenOcf(OperationControlFile ocf, bool edit)
        {
            CloseLogInstance(true);
            if (ocf == null) return;
            if (edit)
            {
                CreateLogInstanceMainCntl(-1);
                liCntl.LiState = edit ? Server.LogInstance.LiState.Edit : Server.LogInstance.LiState.Log_NoEdge;
                liCntl.OCF = ocf;
                liDock.IsVisible = true;
            }
            else
                CreateNewLogInstance(ocf.Id);
        }
        private void NewLi_Click(object sender, RoutedEventArgs e)
        {
            OpenOcfWindow ofw = new OpenOcfWindow()
            {
                MainWindow = this,
                Editable = false,
            };
            ofw.Show();
        }
        private void PlaybackLi_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CloseLi_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ConnectLi_Click(object sender, RoutedEventArgs e)
        {

        }
        private void DisconnectLi_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Edge_Click(object sender, RoutedEventArgs e)
        {
            EdgeItemView ev = new EdgeItemView();
            ev.ShowDialog();
        }

        private void NewOcf_Click(object sender, RoutedEventArgs e)
        {
            //      ClientGlobals.APIClient = new APIClient();
            //      ClientGlobals.APIClient.NewOperationFile();
            CreateLogInstanceMainCntl(-1);
            liCntl.LiState = Server.LogInstance.LiState.Edit;
            liDock.IsVisible = true;
     
            // apiClient.SendRequest("OCF\nNew");
        }
        private void OpenOcf_Click(object sender, RoutedEventArgs e)
        {
            OpenOcfWindow ofw = new OpenOcfWindow()
            {
                MainWindow = this,
                Editable = true,
            };
            ofw.Show();
        }
        private void CheckOcf_Click(object sender, RoutedEventArgs e)
        {

        }
        private void RunOcf_Click(object sender, RoutedEventArgs e)
        {

        }

        OperationControlFile? SaveOcf()
        {
            OperationControlFile ocf = liCntl.OCF;
            if (string.IsNullOrEmpty(ocf.Name))
            {
                MessageBox.Show("Please input name !"); return null;
            }
            liCntl.CalcMPoints();
            OperationDocument doc = liCntl.GetOperationDocument();

            doc.VdDocs = new VdDocumentOds(rtGViews.Select(a => a.GetRtVdDoc(false)).ToList());
            ocf.Body = JsonConvert.SerializeObject(doc);

            ocf = ocf.Id < 0 ? OcfRequest.CreateOcfAsync(ocf, false).Result : OcfRequest.UpdateOcfAsync(ocf, false).Result;
            return ocf;
        }
        private void SaveOcf_Click(object sender, RoutedEventArgs e)
        {
            OperationControlFile ocf = SaveOcf();
            if (ocf == null) 
                return;
            CloseLogInstance(false);
            if (ocf == null)
            {

            }
            else
                OpenOcf(ocf, true);

        }
        private void SaveAsOcf_Click(object sender, RoutedEventArgs e)
        {
            // SaveAsFile(apiClient);
        }

        void CreateLogInstanceMainCntl(int li_id)
        {
            liCntl = new LiClientMainCntl()
            {
                Id = li_id,
            };
            liCntl.MenuStatus.Menu = mainMenu;
            liDock.SetCntl(liCntl);
            liDock.IsVisible = true;
            n1dDisplay = new N1dDisplay();
            n1dDock.SetCntl(n1dDisplay);
            n1dDock.IsVisible = true;
        }
        public void CloseLogInstance( bool save_prompt)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (liCntl != null)
                {
                    if (liCntl.LiState == Server.LogInstance.LiState.Edit)
                    {
                        if (save_prompt)
                        {
                            if (MessageBox.Show("Save OCF?", " ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                SaveOcf();
                        }
                    }else
                    {
                        if (MessageBox.Show($"Close LogInstance {liCntl.Id}?", " ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            liCntl.SendCloseLiRequest();
                    }
                

                    liCntl.Close();
                    liDock.SetCntl(null);
                    liDock.IsVisible = false;
                    n1dDock.IsVisible = false;
                    List<LogGView> gvs = rtGViews.ToList();
                    foreach (LogGView lg in gvs)
                        lg.DockDocument.Close();
                   // rtGViews.Clear();
                    liCntl = null;
                }
            }));
        }

        private void ApiClient_OCFClosed(object sender, EventArgs e)
        {
            CloseLogInstance(true);
        }

        //  #endregion
    }
}
