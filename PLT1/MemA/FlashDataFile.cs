using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.PLT1.MemA
{
    public class MemFlashDataFile
    {
  //      public uint Sequency { get;  }
  //      public string Name { get; set;}
   //     public uint Size { get;  }
    //    public bool     Uploaded { get; }
     //   public uint StartPage { get; }
     //   public uint StopPage { get; }
     //   public uint StartPageBK { get; }
     //   public uint StopPageBK { get; }        
      //  public DateTime StartTime { get; }
    }

    public class MemFlashDataFiles : List<MemFlashDataFile>
    {
        MemPCDataFiles pcFiles;
 
        public int UploadedDFTbls(byte[] dat)
        {
            return 0;
        }

        public int UploadDFile(uint seq)
        {
            return 0;
        }


    }

    public class MemPCDataFile
    {
    
        public string Name { get; set; }

        protected uint seq;
        protected uint size;
        protected DateTime startTime;


        public uint Sequency { 
            get{ return seq; }
        }

        public uint Size
        {
            get { return size; }
        }

        public DateTime StartTime
        {
            get { return startTime; }
        }
    }

    public class MemPCDataFiles : List<MemPCDataFile>
    {


    }
}
