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

namespace OpenWLS.Client.Base
{
    /// <summary>
    /// Interaction logic for IntTextBox.xaml
    /// </summary>
public partial class IntTextBox : TextBox
    {
        string PreviousText = "";
        int BackingResult;

        public IntTextBox()
        {
            TextChanged += IntTextBox_TextChanged;
        }

        public bool HasResult { get; private set; }

        public int Result
        {
            get
            {
                return HasResult ? BackingResult : default(int);
            }
        }

        void IntTextBox_TextChanged(object sender, EventArgs e)
        {
            HasResult = int.TryParse(Text, out BackingResult);

            if (HasResult || string.IsNullOrEmpty(Text))
            {
                // Commit
                PreviousText = Text;
            }
            else
            {
                // Revert
                var changeOffset = Text.Length - PreviousText.Length;
                var previousSelectionStart =
                    Math.Max(0, SelectionStart - changeOffset);

                Text = PreviousText;
                SelectionStart = previousSelectionStart;
            }
        }
    }
}
