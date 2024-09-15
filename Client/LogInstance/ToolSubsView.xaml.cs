using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;
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

using Newtonsoft.Json;
using OpenWLS.Client.LogInstance.Instrument;
using OpenWLS.Client.Requests;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.LogInstance;
using OpenWLS.Server.LogInstance.Instrument;
using OpenWLS.Server.LogInstance.OperationDocument;

namespace OpenWLS.Client.LogInstance
{
    /// <summary>
    /// Interaction logic for AcqItemList.xaml
    /// </summary>
    public partial class ToolSubsView : UserControl
    {

        InstSubs subs;
//        List<Server.DBase.Models.GlobalDb.InstSub> subs_db;
//        public List<Server.DBase.Models.GlobalDb.InstSub> SubsDb
//        { get { return subs_db; } }
        public InstSubs Subs
        {
            set 
            {
                subs = value;
                subList.ItemsSource = null;
                subList.ItemsSource = value;
                ICollectionView view = CollectionViewSource.GetDefaultView(subList.ItemsSource);
                view.GroupDescriptions.Clear();
                PropertyGroupDescription groupDescription = new PropertyGroupDescription("IId");
                view.GroupDescriptions.Add(groupDescription);
            }
            get
            {
                return subs;
            }
        }

     //   bool enableEdit;
        public bool EnableEdit   { //get { return enableEdit; }
            set
            {
              //  enableEdit = value;
                editPanel.Visibility = value? Visibility.Visible : editPanel.Visibility = Visibility.Collapsed;
                subList.Columns[1].IsReadOnly = !value;   
            }
        }
        public bool EditAsset { set { subList.Columns[1].IsReadOnly = value; } }
        LiClientMainCntl mainCntl;
        public ToolSubsView()
        {
            InitializeComponent();
            subs = new InstSubs();
            subDbList.ItemsSource = InstSubRequest.GetDownholeAuxList().Result;
            ICollectionView view = CollectionViewSource.GetDefaultView(subDbList.ItemsSource);
            view.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
        }

        public void Init(LiClientMainCntl main_cntl)
        {
            mainCntl= main_cntl;
            Subs = mainCntl.GetOperationDocument().DhTools.Subs;
        }

        private void subList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          //  btnGrid.IsEnabled = acqList.actLV.SelectedItem != null;
        }

        private void subDbList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(subDbList.SelectedItem != null)
            {
                ToolSubsView subView = (ToolSubsView)subDbList.SelectedItem;
                InstSubDb sub_db = (InstSubDb)subView.subDbList.SelectedItem;
                InstSub subC = new();
                subC.CopyFrom(sub_db);
                OperationDocument doc = mainCntl.GetOperationDocument();
                subC.Id = doc.GetNxtSubId();
                doc.DhTools.Subs.Add(subC);
                subView.Subs = null;
                subView.Subs = doc.DhTools.Subs;
            }

        }

        private void tidBtn_Click(object sender, RoutedEventArgs e)
        {
           
        }
        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        void DeleteSub()
        {
            InstSub sub = (InstSub)subList.SelectedItem;
            OperationDocument doc = mainCntl.GetOperationDocument();
            InstrumentOd? inst = doc.DhTools.DeleteInstSub(sub.Id);
            Subs = null;
            Subs = doc.DhTools.Subs;
            if (inst != null) 
                mainCntl.DeleteTool(inst.Id, mainCntl.toolList, doc.DhTools);
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (subList.SelectedItem != null)
                DeleteSub();

        }


        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {

        }
        private void UpBtn_Click(object sender, RoutedEventArgs e)
        {

        }
        private void DownBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void subList_KeyDown(object sender, KeyEventArgs e)
        {
            if (subList.SelectedItem != null && e.Key == Key.Delete)
                DeleteSub();              
        }
        /*
public void UpInstCntl(int iid)
{
   for(int i = 0; i < toolList.Items.Count; i++)
   {
       GInstrument inst = (GInstrument)toolList.Items[i];
       if(inst.Id == iid)
       {
           toolList.Items.RemoveAt(i);
           toolList.Items.Insert(i - 1, inst);
           toolList.SelectedItem = inst;
           return;
       }
   }
}
public void DownInstCntl(int uid)
{
   for(int i = 0; i < toolList.Items.Count-1; i++)
   {
       if (((GInstrument)toolList.Items[i]).Id == uid)
       {
           var inst = toolList.Items[i];
           toolList.Items.RemoveAt(i);
           toolList.Items.Insert(i+1, inst);
           toolList.SelectedItem = inst;
           return;
       }
   }
}
*/



        /*
        public bool MoveUp(int id)
        {
            int s = this[0].AcqDev != null? 2 : 1; 
            for(int i = s; i < Count; i++) 
            {
                if (this[i].Id == id )
                {
                    Instrument inst = this[i];
                    this[i] = this[i - 1];
                    this[i - 1] = inst;
                    return true;
                }
            }
            return false;
        }

        public bool MoveDown(int id)
        {
            for (int i = 0; i < Count-1; i++)
            {
                if (this[i].Id == id)
                {
                    Instrument inst = this[i];
                    this[i] = this[i + 1];
                    this[i + 1] = inst;
                    return true;
                }
            }
            return false;
        }
        */

    }
}
