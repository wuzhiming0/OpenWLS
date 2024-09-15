using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OpenWLS.PLT1.PtcA
{
    /*
    public class PtcMeasureTask : CVMeasureTask
    {
        public static string str_read_bg = "Read Background";
        public static string str_read_jig = "Read Jig";
        public MeasureTask(string cvVals, string taskName, CVPhaseTasks ts)
        {
            Name = taskName;
            Tasks = ts;
        }

    }

    public class GrCPCalculateTask : CVMeasureTask
    {
        public GrCPCalculateTask(CVPhaseTasks ts){
            Tasks = ts;
        }
    }

    public class GrVPCalculateTask : CVMeasureTask
    {
        public GrVPCalculateTask(CVPhaseTasks ts)
        {
            Tasks = ts;
        }
    }

    public class GrVABCalculateTask : CVMeasureTask
    {
        public GrVABCalculateTask(CVPhaseTasks ts)
        {
            Tasks = ts;
        }
    }

    public class CPTasks : CVPhaseTasks
    {

        public CPTasks()
        {
            AddTask(new CVReferenceTask("gr_calb", this) );
            AddTask(new GrMeasureTask("gr_slow", GrMeasureTask.str_read_bg, this));
            AddTask(new GrMeasureTask("gr_shigh", GrMeasureTask.str_read_jig, this));
            AddTask(new GrCPCalculateTask(this));
        }
    }
    public class VPTasks : CVPhaseTasks
    {

        public VPTasks()
        {
            AddTask(new GrMeasureTask("gr_slow", GrMeasureTask.str_read_bg, this));
            AddTask(new GrMeasureTask("gr_shigh", GrMeasureTask.str_read_jig, this));
            AddTask(new GrVPCalculateTask(this));
        }
    }
    public class VATasks : CVPhaseTasks
    {

        public VATasks()
        {
            AddTask(new GrMeasureTask("gr_slow", GrMeasureTask.str_read_bg, this));
            AddTask(new GrMeasureTask("gr_shigh", GrMeasureTask.str_read_jig, this));
            AddTask(new GrVABCalculateTask(this));
        }
    }
    public class VBTasks : CVPhaseTasks
    {

        public VBTasks()
        {
            AddTask(new GrMeasureTask("gr_slow", GrMeasureTask.str_read_bg, this));
            AddTask(new GrMeasureTask("gr_shigh", GrMeasureTask.str_read_jig, this));
            AddTask(new GrVABCalculateTask(this));
        }
    }*/
}
