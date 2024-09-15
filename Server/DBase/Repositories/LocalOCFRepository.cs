using Microsoft.EntityFrameworkCore;
using OpenWLS.Server.LogInstance.OperationDocument;
using OpenWLS.Server.DBase.DbContents;
using OpenWLS.Server.DBase.Models.GlobalDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.DBase.Repositories
{
    public interface ILocalOCFRepository : IOCFRepository { }
    public class LocalOCFRepository : ILocalOCFRepository
    {
        private readonly LocalDbContent dbContext;
        private readonly GlobalDbContent g_dbContext;
        private readonly ISyslogRepository syslog;

        public LocalOCFRepository(LocalDbContent dbContext, GlobalDbContent g_dbContext, ISyslogRepository syslog)
        {
            this.dbContext = dbContext;
            this.g_dbContext = g_dbContext;
            this.syslog = syslog;
        }

        public async Task<List<OperationControlFileBase>> GetList()
        {
            return await dbContext.OCFs.Where(e => e.Deleted == null).ToListAsync();
        }

        public async Task<TextObject?> GetBody(int id)
        {
            return await dbContext.OcfObjs.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<OperationControlFile> Add(OperationControlFile ocf)
        {          
            ocf.Id = dbContext.OCFs.Any()? dbContext.OCFs.Max(e => e.Id) : 0;
            ocf.Id++;
            var txtObj = new TextObjectOcf()
            {
                Val = ocf.Body,
                Id = ocf.Id,
            };
            await dbContext.OcfObjs.AddAsync(txtObj);
            await dbContext.OCFs.AddAsync(ocf);
     //       dbContext.Database.EnsureCreated();
            await dbContext.SaveChangesAsync();
            return ocf;
        }

        public async Task<OperationControlFile> Update(OperationControlFile ocf)
        {
            var v1 = await dbContext.OcfObjs.FirstOrDefaultAsync(e => e.Id == ocf.Id);

            if (v1 == null) return null;

            v1.Val = ocf.Body;


            var v2 = await dbContext.OCFs.FirstOrDefaultAsync(e => e.Id == ocf.Id);
            if (v2 == null) return null;

            v2.Desc = ocf.Desc;
            v2.Subs = ocf.Subs;
            v2.Version = ocf.Version;
            v2.Name = ocf.Name;
      //      dbContext.Database.EnsureCreated();
            await dbContext.SaveChangesAsync();
   
            return ocf;
        }


        public async Task Delete(int id)
        {
            var ocf = dbContext.OCFs.FirstOrDefaultAsync(e => e.Id == id).Result;
            if (ocf != null)
            {
                ocf.Deleted = true;
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<int> Open(int id)
        {
            TextObject? t_ob = GetBody(id).Result;
            if (t_ob != null)
            {
                OperationDocument od = Newtonsoft.Json.JsonConvert.DeserializeObject<OperationDocument>(t_ob.Val);
                if (od != null)
                {
               //     LogInstance.LogInstanceS li = od.CreateLogInstance(g_dbContext, syslog);
            //        ServerGlobals.AddAPInatnce(li);
                 //   return li.Id;
                }
            }
            return -1;
        }
    }


}

