using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.PLT1
{
    public class PLT1IInstIndex
    {
        uint inst_time1, inst_time2;
        uint sys_time1, sys_time2;
        double depth1, depth2;

        double depth_k;
        double time_k;
        public void AddIndexValues(uint inst_time, uint sys_time, double depth)
        {
            inst_time1 = inst_time2; inst_time2 = inst_time;
            sys_time1 = sys_time2; sys_time2 = sys_time;
            depth1 = depth2; depth2 = depth; 
            double dt = inst_time2 - inst_time1;
            if (dt == 0) return;
            depth_k = (depth2 - depth1) / dt;
            time_k = (sys_time2 - sys_time1) / dt;
        }

        public double GetDepth(uint inst_time)
        {
            return (inst_time - inst_time1)  * depth_k + depth1;
        }
        public uint GetSystemTime(uint inst_time)
        {
            return (uint)((inst_time - inst_time1) * time_k + sys_time1);
        }
    }
}
