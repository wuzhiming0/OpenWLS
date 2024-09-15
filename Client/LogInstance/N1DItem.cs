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
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.LogInstance.OperationDocument;
using OpenWLS.Server.LogInstance.Instrument;
using OpenWLS.Client.LogInstance.Instrument;

namespace OpenWLS.Client.LogInstance
{
    public class N1DItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Id { get; set; }
        public string Name { get; set; }
        public string Val { get; set; }
        public string? InstName { get; set; }
        public string? Format { get; set; }
        public string? UOM { get; set; }


        public N1DItem()
        {

        }

        public N1DItem(MeasurementOd m, string? instName)
        {
            //Format = "f2";
            Id = m.Id; 
            Name = m.Name;
            UOM = m.UOM;
            InstName = instName;
        }

     
            private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
    
    }

    public class N1dItems : ObservableCollection<N1DItem>
    {
        public N1dItems(List<MeasurementOd> ms, InstrumentCs insts)
        {
            foreach(MeasurementOd m in ms)
            {
                if (m.NuDisp != null)
                {
                    InstrumentC? inst = insts.Where(a=>a.Id == m.IId).FirstOrDefault();
                    string? instName = inst == null? null : inst.FullName;
                    Add(new N1DItem(m, instName));
                }
            }
        }




        /*
         public void ItemChanged()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, this));
        }         
         
        public override string ToString()
        {
            if (Count == 0)
                return null;
            string s = this[0].Name1;

            for (int i = 1; i < Count; i++)
                s = s + "|" + this[i].Name1;
            ; return s;
        }*/
    }
}
