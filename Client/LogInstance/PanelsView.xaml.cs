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
using OpenWLS.Server.LogInstance;
using OpenWLS.Server.LogInstance.Instrument;
using OpenWLS.Server.LogInstance.OperationDocument;

namespace OpenWLS.Client.LogInstance
{
    public enum SubEditMode { none = 0, asset=1, all = 2 };
    /// <summary>
    /// Interaction logic for AcqItemList.xaml
    /// </summary>
    public partial class PanelsView : UserControl
    {


        InstSubs pannels;
        List<Server.DBase.Models.GlobalDb.InstSubDb> pannels_all;

        public InstSubs Pannels
        {
            set 
            {
                pannels = value;
                subList.ItemsSource = null;
                subList.ItemsSource = value;
                ICollectionView view = CollectionViewSource.GetDefaultView(subList.ItemsSource);
                view.GroupDescriptions.Clear();
                PropertyGroupDescription groupDescription = new PropertyGroupDescription("IId");
                view.GroupDescriptions.Add(groupDescription);
            }
            get
            {
                return pannels;
            }
        }

        protected SubEditMode editMode;


        public SubEditMode EditMode
        { //get { return editMode; }
            set
            {
                editMode = value;
                switch(editMode){
                    case SubEditMode.none:
                        subList.Columns[1].IsReadOnly = true;
                        subDbList.Visibility = Visibility.Collapsed;
                        editBtnGrid.Visibility = Visibility.Collapsed;
                        break;
                    case SubEditMode.asset:
                        subList.Columns[1].IsReadOnly = false;
                        subDbList.Visibility = Visibility.Collapsed;
                        editBtnGrid.Visibility = Visibility.Collapsed;
                        break;
                    default:
                        subList.Columns[1].IsReadOnly = true;
                        subDbList.Visibility = Visibility.Visible;
                        editBtnGrid.Visibility = Visibility.Visible;
                        break;
                }
               
            }
        }

        public PanelsView()
        {
            InitializeComponent();
            pannels_all = InstSubRequest.GetSurfaceList().Result;
            pannels = new InstSubs();
            subDbList.ItemsSource = pannels_all.Where(a=>a.Aux != null).ToList();
            ICollectionView view = CollectionViewSource.GetDefaultView(subDbList.ItemsSource);
            view.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
        }

        public void UpdateSubs()
        {
            Dispatcher.Invoke(() =>
            {
                subList.ItemsSource = null;
                subList.ItemsSource = pannels;
            });          
        }


        private void subList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          //  btnGrid.IsEnabled = acqList.actLV.SelectedItem != null;
        }

        private void subDbList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(subDbList.SelectedItem != null)
            {
                Server.DBase.Models.GlobalDb.InstSubDb sub_db = (Server.DBase.Models.GlobalDb.InstSubDb)subDbList.SelectedItem;
                InstSub subC = new ();
                subC.CopyFrom(sub_db);
                subC.Id = pannels.Count == 0 ? 1 : pannels.Max(a => a.Id) + 1;
                pannels.Add(subC);
                subList.ItemsSource = null;
                subList.ItemsSource = pannels;
            }
        }
        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {

        }
        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {

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

        private void edgeDevCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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



    }
}
