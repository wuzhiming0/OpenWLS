using OpenWLS.Server.GView.ViewDefinition;
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
using Xceed.Wpf.Toolkit.Primitives;

namespace OpenWLS.Client.GView.GUI
{
    /// <summary>
    /// Interaction logic for VdItemList.xaml
    /// </summary>
    public partial class VdItemList : UserControl
    {
 
        LogViewItemType itemType;
        VdItems cvidItems;
        public event EventHandler<EventArgs> AddNewItem;
        public event EventHandler<EventArgs> DeleteItem;
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
        public VdItems VdItems
        {
            get { return cvidItems; }
            set
            {
                cvidItems = value;
                UpdateItems();
            }
        }

        public LogViewItemType ItemType
        {
            set
            {
                itemType = value;
            }
        }
        public VdItemList()
        {
            InitializeComponent();            
        }



        private void VdItemList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && itemLb.SelectedItem != null)
            {
                if (DeleteItem != null)
                    DeleteItem(itemLb.SelectedItem, e);
            }
        }

        public void UpdateItems()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (cvidItems != null)
                {
                    itemLb.Items.Clear();
                    foreach (VdItem item in cvidItems)
                    {
                        if (item is VdItem && item.Type == itemType)
                            itemLb.Items.Add(item);
                    }
                }
            }));

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddNewItem != null)
                AddNewItem(this, new EventArgs());
        }

        private void itemLb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(SelectionChanged != null)
                SelectionChanged(sender, e);
        }
    }
}
