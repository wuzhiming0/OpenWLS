using Microsoft.EntityFrameworkCore;
using OpenWLS.Server.DBase.DbContents;
using OpenWLS.Server.DBase.Models.GlobalDb;

namespace OpenWLS.Server.DBase.Repositories.InstrumentM
{
    public interface IInstSubRepository
    {
        Task<List<InstSubDb>> GetList();
        Task<List<InstSubDb>> GetDownholeList();
        Task<List<InstSubDb>> GetDownholeAuxList();
        Task<List<InstSubDb>> GetSurfaceList();
        Task<InstSubDb> Get(int id);
        Task Delete(int id);
        Task<InstSubDb> Add(InstSubDb inst);
        Task<InstSubDb> Update(InstSubDb inst);

    }



    public class InstSubRepository : IInstSubRepository
    {
        private readonly GlobalDbContent dbContext;
        private readonly ISyslogRepository syslog;

        public InstSubRepository(GlobalDbContent dbContext, ISyslogRepository syslog)
        {
            this.dbContext = dbContext;
            this.syslog = syslog;
        }

        public async Task<List<InstSubDb>> GetList()
        {
            return await dbContext.Subs.Where(e => e.Deleted == null).ToListAsync();
        }
        public async Task<List<InstSubDb>> GetDownholeList()
        {
            return await dbContext.Subs.Where(e => e.Deleted == null && e.SEqu == null).ToListAsync();
        }
        public async Task<List<InstSubDb>> GetDownholeAuxList()
        {
            return await dbContext.Subs.Where(e => e.Deleted == null && e.SEqu == null && e.Aux != null).ToListAsync();
        }
        public async Task<List<InstSubDb>> GetSurfaceList()
        {
            return await dbContext.Subs.Where(e => e.Deleted == null && e.SEqu != null ).ToListAsync();
        }
        public async Task<InstSubDb> Get(int id)
        {
            return await dbContext.Subs
                .FirstOrDefaultAsync(e => e.DbId == id);
        }

        public async Task<InstSubDb> Add(InstSubDb sub)
        {
            sub.DbId = dbContext.Subs.Any() ? dbContext.Insts.Max(e => e.DbId) : 0;
            sub.DbId++;

            var result = await dbContext.Subs.AddAsync(sub);
            await dbContext.Subs.AddAsync(sub);
            await dbContext.SaveChangesAsync();
            return sub;
        }

        public async Task<InstSubDb> Update(InstSubDb sub)
        {
            InstSubDb sub_db = await dbContext.Subs.FirstOrDefaultAsync(e => e.DbId == sub.DbId);
            if (sub_db == null) return null;

            sub_db.CopyFrom(sub);
            await dbContext.SaveChangesAsync();

            return sub_db;
        }


        public async Task Delete(int id)
        {
            var inst = dbContext.Insts.FirstOrDefaultAsync(e => e.DbId == id).Result;
            if (inst != null)
            {
                inst.Deleted = true;
                await dbContext.SaveChangesAsync();
            }
        }
    }


}
