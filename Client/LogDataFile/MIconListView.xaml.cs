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
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.LogDataFile.Models;

namespace OpenWLS.Client.LogDataFile
{
    public enum measurementItemType { Con = 0, XD, FrameOpen, FrameClose };

    public class measurementItem
    {
        string name;
        Measurement measurement;

        public string Name
        {
            get
            {
                if (measurement == null)
                    return name;
                else
                    return measurement.Head.Name;
            }
        }

        public measurementItemType Type { get; set; }
        public measurementItems measurementildren { get; set; }

        public int[] Dimensions
        {
            get
            {
                if (measurement == null)
                    return null;
                else
                    return measurement.Head.Dimensions;
            }
        }
        public string UOM
        {
            get
            {
                if (measurement == null)
                    return null;
                else
                    return measurement.Head.UOM;
            }
        }
        public string UOIndex
        {
            get
            {
                if (measurement == null)
                    return null;
                else
                    return measurement.Head.UOI;
            }
        }
        // public string Frame { get; set; }           //8
        public double StartIndex
        {
            get
            {
                if (measurement == null )
                    return double.NaN;
                else
                    return (double)measurement.StartIndex;
            }
        }

        public double StopIndex
        {
            get
            {
                if (measurement == null )
                    return double.NaN;
                else
                    return (double)measurement.StopIndex;
            }
        }

        public double Spacing
        {
            get
            {
                if (measurement == null)
                    return double.NaN;
                else
                    return (double)measurement.Head.Spacing;
            }
        }

        public double ValueMin
        {
            get
            {
                if (measurement == null || measurement.Head.VMin == null)
                    return double.NaN;
                else
                    return (double)measurement.Head.VMin;
            }
        }

        public double ValueMax
        {
            get
            {
                if (measurement == null || measurement.Head.VMax == null)
                    return double.NaN;
                else
                    return (double)measurement.Head.VMax;
            }
        }

        public measurementItem(string frame)
        {
            Type = measurementItemType.FrameClose;
            name = frame;
            measurementildren = new measurementItems();
        }

        public measurementItem(Measurement m)
        {
            measurement = m;
            Type = measurement.Head.Dimensions[0] > 1 ? measurementItemType.XD : measurementItemType.Con;
        }

    }

    public class measurementItems : List<measurementItem>
    {
        measurementItem GetItem(string name)
        {
            foreach (measurementItem measurement in this)
                if (measurement.Name == name)
                    return measurement;
            return null;
        }

        void AddFramemeasurementannel(Measurement m)
        {
            measurementItem f = GetItem(m.Head.Frame);
            if (f == null)
                f = new measurementItem(m.Head.Frame);
            f.measurementildren.Add(new measurementItem(m));
        }

        /*
        public void Init(MHeadTbl measurementTbl)
        {
            Clear();
            foreameasurement (MHead measurement in measurementTbl.Heads)
            {
                if (measurement.Frame != null && measurement.Frame.Length > 0)
                    AddFramemeasurementannel(measurement);
                else
                    Add(new measurementItem(measurement));
            }
        }*/
    }
    /// <summary>
    /// Interaction logic for MIconListView.xaml
    /// </summary>
    public partial class MIconListView : UserControl
    {
        public MIconListView()
        {
            InitializeComponent();
        }
    }
}
