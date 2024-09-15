using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.IO;

namespace OpenWLS.Server.DFlash
{
    public interface IDFlashPageProcessor
    {
        void OpenDFPFile(DataReader r);
        void ProcessRun(DFlashRun r);
        void ProcessSection(DFlashRunSection s);        
    }

    public struct SPageLocation
    {
        public int list_pos;
        public int page_pos;

        public long GetOffset(int page_size)
        {
            int n = page_size - 1;
            long s1 = page_size << 10;
            long k = list_pos * s1;
            k += n * page_pos;
            return k;
        }
    }

    public class DFlashPageFile
    {
        public static readonly string file_ext = ".dfp";
        public static readonly string file_filter = "*.dfp";
        public static readonly string sub_dir = @"das/";
        public static readonly int head_size = 13;
        public static readonly int end_size = 4;
        RootDb root;
        DFlashRuns runs;
        FileStream fileStream;
        public FileStream FileStream { get { return fileStream; } }
        int copy;
        int dfChip;
        uint dfAddress;
//        long fileOffset;
      //  int readBlockSize;

        DFlashRun runCur;
        DFlashRunSection sectionCur;
        IDFlashPageProcessor pageProc;

        List<byte[]> dbTypesList;
        public List<byte[]> DbTypesList
        {
            get
            {
                return dbTypesList;
            }
        }
        public bool EndOfDTypeList
        {
            get { return posCur.list_pos >= dbTypesList.Count; }
        }
        public bool EndOfSection
        {
            get { return posCur.list_pos >= dbTypesList.Count; }
        }

        byte[] dbTypesCur;
        SPageLocation posCur;
        //   int dbTypesListPos;
        //   int dbTypesCurPos;
        public uint TotalSize { get; set; }
 //       public uint UnusedSize { get; set; }
        public int Copy { get { return copy; } }
        public RootDb Root { get { return root; } }
        public DFlashRuns Runs { get { return runs; } }

        public string Summary
        {
            get
            {
                return "Version: " + root.Version.ToString();
            }
        }


        public void ProcessRunSection(DFlashRunSection section, IDFlashPageProcessor proc)
        {
            if (pageProc == null)
                return;
            {
              /*  chip = r.ReadByte();
                startAddress = r.ReadUInt32();
                while (!r.END)
                {
                    pageProc.ProcessPage(r);
                }
              */
            }
        }

        void ScanPage(DataReader r)
        {
            byte b = r.ReadByte();
            switch(b)
            {
                case (byte)BlockType.LogHead:
           //         LogHead h = new LogHead();
           //         h.Restore(r);
                    runCur = new DFlashRun();
          //          runCur.LogHead = h;
                    runs.Add(runCur);
                    return;
                case (byte)BlockType.StartStamp:
                    StartStamp startStamp = new StartStamp();
                    startStamp.Restore(r);
                    sectionCur = new DFlashRunSection();
                    sectionCur.StartStamp = startStamp;
                    runCur.Sections.Add(sectionCur);
                    return;
                case (byte)BlockType.StopStamp:
                    StopStamp stopStamp = new StopStamp();
                    stopStamp.Restore(r);
                    sectionCur.StopStamp = stopStamp;
                    return;
            }
        }

        public DFlashPageFile()
        {
            root = new RootDb();
            root.PageSize = 256;
            dbTypesList = new List<byte[]>();
        }

        /*
        public ArchiveRt CreateArchive(LogInstance api)
        {
            string fn = FileStream.Name;
            fn = fn.Substring(0, fn.Length - 4) + Archive.Archive.file_ext;
            ArchiveRt ar = new ArchiveRt(api);
            ar.NxtItemID = 0;
            ar.CreateNew(fn);
            return ar;
        }*/

        public void CreateFileStream(string path, string name, LvDFlashFS dfs)
        {
            //      fileOffset = 0;
            posCur.page_pos = 0;
            dbTypesCur = new byte[1024];
            dbTypesList.Clear();
            copy = dfs.Copies;
            root.PageSize = dfs.PageSize;
            TotalSize = dfs.TotalSize - dfs.UnusedSize;
           // EndWriteFileStream();
            string fn = StringConverter.GetNxtFileName(path + sub_dir + name, file_ext);
            fileStream = new FileStream(fn, FileMode.Create, FileAccess.ReadWrite);

            DataWriter w = new DataWriter(head_size);
            w.WriteString(file_ext, 4);
        //    w.WriteData(block_size);
            w.WriteData((byte)copy);           
            w.WriteData(dfs.PageSize);
            w.WriteData(TotalSize);
      //      w.WriteData(UnusedSize);
            fileStream.Write(w.GetBuffer(), 0, head_size);
        }

        public void EndWriteFileStream()
        {
            if (fileStream != null)
            {
                if(posCur.page_pos > 0)
                {
                    fileStream.Write(dbTypesCur, 0, posCur.page_pos);
                    dbTypesList.Add(dbTypesCur);
                    posCur.page_pos = 0;
                }
                byte[] bs = StringConverter.ToByteArray(".end");
                fileStream.Write(bs, 0, bs.Length);                
                ScanBlocks();
            }
        }

        public void WritePages(DataReader r)
        {
            if (fileStream != null)
            {    
                int n = root.PageSize - 1;
                while (!r.END)
                {
                    if (posCur.page_pos >= 1024)
                    {
                        fileStream.Write(dbTypesCur, 0, 1024);
                        posCur.page_pos = 0;
                        dbTypesList.Add(dbTypesCur);
                        dbTypesCur = new byte[1024];
                    }
                    dbTypesCur[posCur.page_pos++] = r.ReadByte();
                    fileStream.Write(r.ReadByteArray(n), 0, n);
                }
            }
        }
        void ReadDbTypeList()
        {
            long fSize = fileStream.Length - head_size - end_size;
            int s1 = root.PageSize << 10;
            fileStream.Seek(head_size, SeekOrigin.Begin);
 
            while ( fSize > 0 )
            {
                int s = fSize >= s1? s1 : (int)fSize;
                int s2 = s / root.PageSize;
                long s3 = s - s2;
                fileStream.Seek(s3, SeekOrigin.Current);

                byte[] bs = new byte[s2];
                fileStream.Read(bs, 0, s2);
                dbTypesList.Add(bs);
                fSize -= s;
            }
        }

        public DataReader ReadBlock(byte bt)
        {
            for(int i = posCur.list_pos; i < dbTypesList.Count; i++)
            {
                dbTypesCur = dbTypesList[i];
                for (int j = posCur.page_pos; j < dbTypesCur.Length; j++)
                {
                    if(dbTypesCur[j] == bt)
                    {
                        MoveTo(i, j);
                        return ReadBlockAtCurTypePos();
                    }
                }
            }
            return null;
        }

        public DataReader ReadCurList(SPageLocation stopPos)
        {
            long fs_offset = posCur.GetOffset(root.PageSize);
            fileStream.Seek(fs_offset, SeekOrigin.Begin);
            int n = root.PageSize - 1;

            if(stopPos.list_pos == posCur.list_pos)
                n *= stopPos.page_pos - posCur.page_pos + 1;
            else
                n *= dbTypesList[posCur.list_pos].Length - posCur.page_pos;

            return  new DataReader(n);
        }

        DataReader ReadBlockAtCurTypePos()
        {
            long k = posCur.GetOffset(root.PageSize);
            fileStream.Seek(k + head_size, SeekOrigin.Begin);
            DataReader r = new DataReader(fileStream, 2);
            ushort s = r.ReadUInt16();
            r = new DataReader(fileStream, s);
            return r;
        }

        void NewRun()
        {            
        /*    LogHead lh = new LogHead();
            lh.PagePos = posCur;
            runCur = new DFlashRun();
            DataReader r = ReadBlockAtCurTypePos();
            lh.Restore(r, root.Version, null);

            runCur.LogHead = lh;
            runCur.LogFile = this;
            runs.Add(runCur);
        */
        }

        void NewRunSection()
        {        
            StartStamp ss = new StartStamp();
            sectionCur = new DFlashRunSection();    
            ss.PagePos = posCur; 
            DataReader r = ReadBlockAtCurTypePos();  
            ss.Restore(r);
            sectionCur.StartStamp = ss;
            sectionCur.LogRun = runCur;
            runCur.Sections.Add(sectionCur);
        }

        public void MoveTo(int list_offset, int page_offset )
        {
            posCur.list_pos = list_offset;
            posCur.page_pos = page_offset;
        }
        public void MoveToNxtList()
        {
            posCur.list_pos++;
            posCur.page_pos = 0;
        }

        public void MoveTo(SPageLocation pos)
        {
            posCur = pos;
        }

        public void ScanBlocks()
        {
            MoveTo(0, 0);
            runs = new DFlashRuns();
            dbTypesCur = dbTypesList[posCur.list_pos];
            while(dbTypesCur != null)
            {
                switch(dbTypesCur[posCur.page_pos])
                {
                    case (byte)BlockType.LogHead:
                        NewRun();
                        break;
                    case (byte)BlockType.StartStamp:
                        NewRunSection();
                        break;
                    case (byte)BlockType.StopStamp:
                        StopStamp ss = new StopStamp();
                        ss.PagePos = posCur;
                        DataReader r = ReadBlockAtCurTypePos();
                        ss.Restore(r);
                        sectionCur.StopStamp = ss;
                        break;
                }
                posCur.page_pos++;
                if(posCur.page_pos >= dbTypesCur.Length)
                {
                    posCur.list_pos++;
                    if (posCur.list_pos >= dbTypesList.Count)
                        dbTypesCur = null;
                    else
                    {
                        posCur.page_pos = 0;
                        dbTypesCur = dbTypesList[posCur.list_pos];
                    }
                }
            }
        }

        public void Close()
        {
            if(fileStream != null)
            {
                fileStream.Close();
                fileStream = null;
            }
        }

        public TreeNode GetTreeNode()
        {
            TreeNode tn = new TreeNode();
            tn.Text = root.GetSummary();
            tn.Children = new List<TreeNode>();
            foreach(DFlashRun dr in Runs)
            {
                TreeNode tn1 = new TreeNode();
                tn1.Children = new List<TreeNode>();
               // tn1.Text = dr.LogHead.GetSummary();
                foreach (DFlashRunSection drs in dr.Sections)
                {
                    TreeNode tn2 = new TreeNode();
                    tn2.Text = drs.StartStamp.GetSummary() + "\n" + drs.StopStamp.GetSummary();
                    tn1.Children.Add(tn2);
                }
                tn1.ChildrenName = dr.Sections.Count.ToString() + " sections";
                tn.Children.Add(tn1);
            }
            tn.ChildrenName = Runs.Count.ToString() + " runs";
            return tn;
        }

        public bool Open(string fn)
        {
            try
            {
                fileStream = new FileStream(fn, FileMode.Open, FileAccess.Read);
                DataReader r = new DataReader(fileStream, head_size+2);              //15 = 13 + 2 (block size of first block )
                if (r.ReadString(4) != file_ext)
                {
                    fileStream.Close();
                    fileStream = null;
                    return false;
                }
                copy = r.ReadByte();
                root.PageSize = r.ReadInt32();
                TotalSize = r.ReadUInt32();
                //     UnusedSize = r.ReadUInt32();
                int s = r.ReadUInt16();
                r = new DataReader(fileStream, s);
                root.Restore(r);
                ReadDbTypeList();
                ScanBlocks();

                return true;
            }
            catch (Exception e)
            {
            }
            return false;
        }


    }

    public class DFlashRunSection
    {
        public StartStamp StartStamp { get; set; }
        public StopStamp StopStamp { get; set; }
        public DFlashRun LogRun { get; set; }
    }

    public class DFlashRunSections : List<DFlashRunSection>
    {

    }

    public class DFlashRun
    {
  //      public LogHead LogHead { get; set; }
//        public InstDb Insts { get; set; }
  //      public ActDb Act { get; set; }
   //     public ParaDb Paras { get; set; }

        public DFlashRunSections Sections { get; set; }

        public DFlashPageFile LogFile { get; set; }

        public DFlashRun()
        {
            Sections = new DFlashRunSections();
        }

    }

    public class DFlashRuns : List<DFlashRun>
    {

    }

}
