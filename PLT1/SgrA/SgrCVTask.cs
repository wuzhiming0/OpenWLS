using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenWLS.Server.LogInstance.Calibration;

namespace OpenWLS.PLT1.SgrA
{
    public class SgrMeasureTask : CVMeasureTask
    {
        public static string str_read_bg = "Read Background";
        public static string str_read_jig = "Read Jig";
        public SgrMeasureTask(string cvVals, string taskName, CVPhaseTasks ts)
        {
            Name = taskName;
            Tasks = ts;
        }

    }

    public class SgrCPCalculateTask : CVMeasureTask
    {
        public SgrCPCalculateTask(CVPhaseTasks ts){
            Tasks = ts;
        }
    }

    public class SgrVPCalculateTask : CVMeasureTask
    {
        public SgrVPCalculateTask(CVPhaseTasks ts)
        {
            Tasks = ts;
        }
    }

    public class SgrVABCalculateTask : CVMeasureTask
    {
        public SgrVABCalculateTask(CVPhaseTasks ts)
        {
            Tasks = ts;
        }
    }

    public class CPTasks : CVPhaseTasks
    {

        public CPTasks()
        {
            AddTask(new CVReferenceTask("gr_calb", this) );
            AddTask(new SgrMeasureTask("gr_slow", SgrMeasureTask.str_read_bg, this));
            AddTask(new SgrMeasureTask("gr_shigh", SgrMeasureTask.str_read_jig, this));
            AddTask(new SgrCPCalculateTask(this));
        }
    }
    public class VPTasks : CVPhaseTasks
    {

        public VPTasks()
        {
            AddTask(new SgrMeasureTask("gr_slow", SgrMeasureTask.str_read_bg, this));
            AddTask(new SgrMeasureTask("gr_shigh", SgrMeasureTask.str_read_jig, this));
            AddTask(new SgrVPCalculateTask(this));
        }
    }
    public class VATasks : CVPhaseTasks
    {

        public VATasks()
        {
            AddTask(new SgrMeasureTask("gr_slow", SgrMeasureTask.str_read_bg, this));
            AddTask(new SgrMeasureTask("gr_shigh", SgrMeasureTask.str_read_jig, this));
            AddTask(new SgrVABCalculateTask(this));
        }
    }
    public class VBTasks : CVPhaseTasks
    {

        public VBTasks()
        {
            AddTask(new SgrMeasureTask("gr_slow", SgrMeasureTask.str_read_bg, this));
            AddTask(new SgrMeasureTask("gr_shigh", SgrMeasureTask.str_read_jig, this));
            AddTask(new SgrVABCalculateTask(this));
        }
    }
}
