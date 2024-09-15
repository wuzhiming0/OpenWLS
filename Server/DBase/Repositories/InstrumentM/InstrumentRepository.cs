using Microsoft.EntityFrameworkCore;
using OpenWLS.Server.DBase.DbContents;
using OpenWLS.Server.DBase.Models.GlobalDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.DBase.Repositories.InstrumentM
{
    public interface IInstrumentRepository
    {
        Task<List<InstrumentDb>> GetList();
        Task<List<InstrumentDb>> GetDownholeList();
        Task<InstrumentDb> Get(int id);
        Task Delete(int id);
        Task<InstrumentDb> Add(InstrumentDb inst);
        Task<InstrumentDb> Update(InstrumentDb inst);

    }



    public class InstrumentRepository : IInstrumentRepository
    {
        private readonly GlobalDbContent dbContext;
        private readonly ISyslogRepository syslog;
        public InstrumentRepository(GlobalDbContent dbContext, ISyslogRepository syslog)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<InstrumentDb>> GetList()
        {
            return await dbContext.Insts.Where(e => e.Deleted == null).ToListAsync();
        }
        public async Task<List<InstrumentDb>> GetDownholeList()
        {
            return await dbContext.Insts.Where(e => e.Deleted == null && e.SurfaceEqu == null).ToListAsync();
        }
        public async Task<InstrumentDb> Get(int id)
        {
            return await dbContext.Insts
                .FirstOrDefaultAsync(e => e.DbId == id);
        }

        public async Task<InstrumentDb> Add(InstrumentDb inst)
        {
            inst.DbId = dbContext.Insts.Any() ? dbContext.Insts.Max(e => e.DbId) : 0;
            inst.DbId++;

            var result = await dbContext.Insts.AddAsync(inst);
            await dbContext.Insts.AddAsync(inst);
            await dbContext.SaveChangesAsync();
            return inst;
        }

        public async Task<InstrumentDb> Update(InstrumentDb inst)
        {
            InstrumentDb inst_db = await dbContext.Insts.FirstOrDefaultAsync(e => e.DbId == inst.DbId);
            if (inst_db == null) return null;

            inst_db.CopyFrom(inst);
            await dbContext.SaveChangesAsync();

            return inst_db;
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
