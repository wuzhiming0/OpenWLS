using OpenWLS.Server.Base;

namespace OpenWLS.Edge.Simulator.PLT1
{
    public class PLT1InstModule
    {
        public int Id { get; set; }
        public string Name { get; set; }


        public virtual void ProcGUIMsg(DataReader r)
        {

        }

    }

    public class PLT1InstModules : List<PLT1InstModule>
    {
        public void ProcGUIMsg(DataReader r)
        {
            byte b = r.ReadByte();
            PLT1InstModule? mod = this.Where(a=>a.Id == b).FirstOrDefault();
            if(mod != null)
                mod.ProcGUIMsg(r);
        }
    }
}