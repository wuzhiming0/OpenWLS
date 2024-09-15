using Microsoft.EntityFrameworkCore;
using OpenWLS.Server.DBase.DbContents;
using OpenWLS.Server.DBase.Models.GlobalDb;

namespace OpenWLS.Server.DBase.Repositories.InstrumentM   //M=master
{
    public interface IMGroupRepository
    {
        Task<List<MGroup>> GetAll();
        Task<List<MGroup>> GetListOfInst(int iid);
        Task<MGroup> Get(int iid, int id);
        Task Delete(int id);
        Task<MGroup> Add(MGroup mg);
        Task<MGroup> Update(MGroup mg);

    }



    public class MGroupRepository : IMGroupRepository
    {
        private readonly GlobalDbContent dbContext;
        private readonly ISyslogRepository syslog;
        public MGroupRepository(GlobalDbContent dbContext, ISyslogRepository syslog)
        {
            this.dbContext = dbContext;
            this.syslog = syslog;
        }
        public async  Task<List<MGroup>> GetAll()
        {
            return await dbContext.MGroups.Where(e => e.Deleted == null).ToListAsync();
        }
        public async Task<List<MGroup>> GetListOfInst(int iid)
        {
            return await dbContext.MGroups.Where(e => e.Deleted == null && e.IDbId == iid).ToListAsync();
        }

        public async Task<MGroup> Get(int iid, int id)
        {
            return await dbContext.MGroups
                .FirstOrDefaultAsync(e => e.IDbId == id && e.Id == id);
        }

        public async Task<MGroup> Add(MGroup mg)
        {
            var result = await dbContext.MGroups.AddAsync(mg);
            await dbContext.MGroups.AddAsync(mg);
            await dbContext.SaveChangesAsync();
            return mg;
        }

        public async Task<MGroup> Update(MGroup mg)
        {
            MGroup sub_db = await dbContext.MGroups.FirstOrDefaultAsync(e => e.IDbId == mg.IDbId && e.Id == mg.Id);
            if (sub_db == null) return null;

            mg.CopyFrom(mg);
            await dbContext.SaveChangesAsync();

            return sub_db;
        }


        public async Task Delete(int id)
        {
            var inst = dbContext.MGroups.FirstOrDefaultAsync(e => e.DbId == id).Result;
            if (inst != null)
            {
                inst.Deleted = true;
                await dbContext.SaveChangesAsync();
            }
        }
    }


}
