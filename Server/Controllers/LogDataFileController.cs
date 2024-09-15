using Microsoft.AspNetCore.Mvc;
using OpenWLS.Server;
using OpenWLS.Server.Base;
using OpenWLS.Server.DBase;
using OpenWLS.Server.DBase.Models.LocalDb;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.LogDataFile.DLIS;
using OpenWLS.Server.LogDataFile.DLIS.V1;
using OpenWLS.Server.LogDataFile.LAS;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.LogDataFile.XTF;
using System.Threading.Channels;
using System.Xml.Linq;

namespace OpenWLS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogDataFileController : ControllerBase
    {
        private readonly ILogger<LogDataFileController> _logger;
        private readonly ISyslogRepository _repSyslog;
        public LogDataFileController(ISyslogRepository repSyslog,  ILogger<LogDataFileController> logger)
        {
            _logger = logger;
            _repSyslog = repSyslog;
        }


        [HttpPost]
        [Route("FolderFileNames")]
        public Task<List<string>> GetDfileNamesOfFolder([FromBody]string path )
        {
            string[] fs = Directory.GetFiles(path);
            int c = path.Length;
            List<string> res = new List<string>();
            foreach (string f in fs)
            {
                if (f.EndsWith(DataFile.file_ext))
                    res.Add(f.Substring(c, f.Length-c-4));
            }
            return Task.FromResult(res);
        }

        [HttpGet]
        [Route("JobFileNames/{job_name}")]
        public Task<List<string>> GetDfileNamesOfJob(string job_name)
        {
            return GetDfileNamesOfFolder(Job.GetJobDirectory(job_name));
        }

        [HttpPost]
        [Route("Open")]
        public Task<DataFileInfor> Open([FromBody] string file_name)
        {
            if (!file_name.EndsWith(".ldf"))
                file_name = file_name + ".ldf";
            DataFile df = DataFile.OpenDataFile(file_name, _repSyslog);
            DataFileInfor dfi = df.GetFileInfor();
            df.Close();
            return  Task.FromResult(dfi);
        }
        /*
        [HttpPost]
        [Route("1DData/{mids_str}")]
        public Task<byte[][]> Get1DNumberData([FromBody] string file_name, string mids_str)
        {
            int[] mids = StringConverter.ToIntArray(mids_str, ',');
            byte[][] res = new byte[mids.Length][];
            using (DataFile df = DataFile.OpenDataFile(file_name, _repSyslog))
            {
                int k = 0;
                foreach (int mid in mids)
                {
                    MHead head = new MHead(mid, df);
            //        Measurement m = new Measurement(head, df);
                    MVReader r = new MVReader(head);
                    res[k++] = r.VBuffer;
                }
            }
            return Task.FromResult(res);
        }

        [HttpPost]
        [Route("xDData/{mid}")]
        public Task<byte[]> GetxDNumberData([FromBody] string file_name, int mid)
        {
            using (DataFile df = DataFile.OpenDataFile(file_name, _repSyslog))
            {
                MHead head = new MHead(mid, df);
                Measurement m = new Measurement(head, df);
                MVReader r = new MVReader(m);
                return Task.FromResult(r.VBuffer);
            }
        }


        [HttpPost]
        [Route("NMData/{id}")]
        public Task<byte[]> GetNMData([FromBody] string file_name, int id)
        {
            using (DataFile df = DataFile.OpenDataFile(file_name, _repSyslog))
            {
                return Task.FromResult(BinObject.GetVal(df, id));
            }
        }
        */
        [HttpPost]
        [Route("Import")]
        public Task<DataFileInfor> Import([FromBody] string file_name)
        {
           return  Task.FromResult(ImportDataFile(file_name));
        }

        [HttpPost]
        [Route("Export/{file_type}")]
        public void Export([FromBody] string file_name, int file_type)
        {
        //    ExportDataFile(file_name, file_type);
        }

        DataFileInfor? ImportDataFile(string fn)
        {
            if (fn.EndsWith(".xtf"))
            {
                XtfFile f = new XtfFile();
                return f.ImportXtf(fn, _repSyslog);

            }
            else
            {
                if (fn.EndsWith(".dlis"))
                {
                    DlisFile.ImportDLIS(fn, _repSyslog);
                }
            }
            if (fn.EndsWith(".las") || fn.EndsWith(".LAS"))
                return LasDataFile.Import(fn, _repSyslog);
            return null;
        }

        string ExportLas(int version, string fn, string m_names, string las)
        {
            LasVersion v = (LasVersion)version;
            LasDataFile.Export(fn, v, _repSyslog, m_names.Split(','));
            return null;
        }

    }
}