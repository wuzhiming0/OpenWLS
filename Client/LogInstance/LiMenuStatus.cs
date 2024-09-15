using OpenWLS.Server.LogInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;


namespace OpenWLS.Client.LogInstance
{
   /* public  interface IMenuStatus
    {
        void SetApiState(LiState s);
    }*/
    public class LiMenuStatus
    {
        public Menu Menu { get; set; }

        public bool New { get; set; }
        public bool Open { get; set; }
        public bool Check { get; set; }
        public bool Save { get; set; }
        public bool SaveAs { get; set; }
        public bool Close{ get; set; }
//        public bool  Edit{ get; set; }
        public bool CalLoad { get; set; }

        bool calStoredInTool;
        public bool CalStoredInTool
        {
            get { return calStoredInTool; }
            set
            {
                calStoredInTool = value;
                Menu.DataContext = null;
                Menu.DataContext = this;
            }
        }

        public bool Job { get; set; }


        public void SetApiState(LiState s)
        {
            CalStoredInTool = false;
            switch (s)
            {
                case LiState.Blank:
                    New = true;
                    Open = true;
                    Check = false;
                    Save = false;
                    SaveAs = false;
                    Close = false;
                    CalLoad = false;
                    Job = true;
                    break;
                case LiState.Edit:
                    New = true;
                    Open = true;
                    Save = true;
                    SaveAs = true;
                    Close = true;
                    Check = true;
                    CalLoad = false;
                    Job = false;
                    break;
                    /*
                case LiState.Ready:
                    New = true;
                    Open = true;
                    Check = false;
                    Save = true;
                    SaveAs = true;
                    Close = true;
                    CalLoad = true;
                    Job = false;
                    break;*/
                case LiState.Log_Standby:
                    New = true;
                    Open = true;
                    Check = false;
                    Save = true;
                    SaveAs = true;
                    Close = true;
                    CalLoad = true;
                    Job = false;
                    break;
                case LiState.Playback_Standby:
                    New = true;
                    Open = true;
                    Check = false;
                    Save = true;
                    SaveAs = true;
                    Close = true;
                    CalLoad = true;
                    Job = false;
                    break;
                case LiState.Log:
                    New = false;
                    Open = false;
                    Check = false;
                    Save = true;
                    SaveAs = true;
                    Close = false;
                    CalLoad = false;
                    Job = false;
                    break;
                case LiState.CV:
                    New = false;
                    Open = false;
                    Check = false;
                    Save = false;
                    SaveAs = false;
                    Close = false;
                    CalLoad = false;
                    Job = false;
                    break;
                case LiState.Playback:
                    New = true;
                    Open = true;
                    Check = false;
                    Save = false;
                    SaveAs = false;
                    Close = false;
                    CalLoad = false;
                    Job = false;
                    break;
            }
            Menu.DataContext = null;
            Menu.DataContext = this;
        }
    }


    /*public class MenuStatuss : List<IMenuStatus>
    {
        public void SetApiState(LiState s)
        {
            foreach (IMenuStatus m in this)
                m.SetApiState(s);
        }
    }
    */
}
