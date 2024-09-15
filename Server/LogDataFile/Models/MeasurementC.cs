using Microsoft.EntityFrameworkCore.Infrastructure;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.LogDataFile.Models.NMRecord;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace OpenWLS.Server.LogDataFile.Models
{
    public partial class Measurement : INotifyPropertyChanged
    {
        bool selected;
        [JsonIgnore]
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                NotifyPropertyChanged(nameof(Selected));
            }
        }
        [JsonIgnore]
        public string? Dims
        {
            get
            {
                if (Head.DataAxes == null) return "1";
                return string.Join(',', Server.Base.DataAxes.CreateDataAxes(Head.DataAxes).GetDimensions());
            }

        }
        [JsonIgnore]
        public MVReader MVReader { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
    public partial class  Frame
    {
        bool selected;
        [JsonIgnore]
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                //      foreach (Measurement c in ms)
                //          c.Selected = selected;
            }
        }
    }
}
