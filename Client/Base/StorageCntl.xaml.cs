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
using System.Drawing;
using OpenWLS.Server.Base;

namespace OpenWLS.Client.Base
{
    /// <summary>
    /// Interaction logic for StorageCntl.xaml
    /// </summary>
    public partial class StorageCntl : UserControl
    {
        public int Total { get; set; }
        public int Bad { get; set; }
        public int Used { get; set; }


        Dictionary<string, int> tags;

        public StorageCntl()
        {
            InitializeComponent();

            Bad = 0;
            Used = 0;
            Total = 0;         
            textBox.SetValue(Paragraph.LineHeightProperty, 1.0);
            UpdateSpaces();
  //          UpdateSpaces();
        }

        public void SetSpaces(int[] spaces)
        {

           Bad = spaces[0];
           Used = spaces[1];
           Total = spaces[2];
           UpdateSpaces();
        }

        public void UpdateSpaces()
        {
            Dispatcher.Invoke(() =>
            {
               /* chart1.Series[0].Points[0].SetValueY(Bad);
                chart1.Series[0].Points[1].SetValueY(Used);
                chart1.Series[0].Points[2].SetValueY(Total - Bad - Used);     // "Unused"
                textBox.Document.Blocks.Clear();

                StringConverter.AddToTxtDocument(textBox.Document, "Bad: " + Bad.ToString(), Colors.Red);
                StringConverter.AddToTxtDocument(textBox.Document, "\nUsed: " + Used.ToString(), Colors.Blue);
                StringConverter.AddToTxtDocument(textBox.Document, "Bad: " + "\nTotal: " + Total.ToString() + "(MBytes)", Colors.Black);
               */
            });
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            chartGrid.Width = (e.NewSize.Width < 60) ? e.NewSize.Width : 60;
        }
    }
}
