using OpenWLS.Server.LogInstance.Calibration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Client.Calibration
{
    public class CVPhaseC : CVPhase
    {
        System.Windows.Forms.ListViewItem listItem;

		public override CVPhaseStatus Status
		{
			get
			{
				return status;
			}
			set
			{
				if(subItemIndex >= 0 && listItem != null)
					listItem.SubItems[subItemIndex].Text = this.status.ToString();
				status = value;
			}
		}
        /*
                 public int Progress
                 {
                     get
                     {
                         return progress;
                     }
                 }

                 public CVPhaseTask SelectedTask
                 {
                     get;  set;
                 } 
         */
        public void UpdateStatus()
        {
          //  status = dataRec.Passed ? CVPhaseStatus.OK : CVPhaseStatus.Failed;
            listItem.SubItems[subItemIndex].Text = this.status.ToString();
        }

        /// <summary>
        /// update the record time and matrix data
        /// </summary>
        /// <param name="newCVMatrixes">matrix data</param>
        public void AfterRecord(double[] newCVMatrixes)
        {
            //dataRec.AfterRecord(newCVMatrixes);
            //inforRec.DateTime = DateTime.Now.Ticks;

            UpdateStatus();
        }

    }
}
