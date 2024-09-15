using Microsoft.EntityFrameworkCore;
using OpenWLS.Server.DBase.DbContents;
using OpenWLS.Server.DBase.Models.GlobalDb;
//using OpenWLS.Server.LogDataFile.Models;

namespace OpenWLS.Server.DBase.Repositories.InstrumentM //M=master
{
    public interface IMeasurementRepository
    {
        Task<List<MeasurementDb>> GetAll();
        Task<MeasurementDb?> Get(string name);
        Task<MeasurementDb?> Get(int id);
        Task Delete(int id);
        Task<MeasurementDb> Add(MeasurementDb m);
        Task<MeasurementDb> Update(MeasurementDb m);

    }



    public class MeasurementRepository : IMeasurementRepository
    {
        private readonly GlobalDbContent dbContext;
        private readonly ISyslogRepository syslog;
        public MeasurementRepository(GlobalDbContent dbContext, ISyslogRepository syslog)
        {
            this.dbContext = dbContext;
            this.syslog = syslog;
        }

        public async Task<List<MeasurementDb>> GetAll()
        {
            return await dbContext.Measurements.Where(e => e.Deleted == null).ToListAsync();
        }

        public async Task<MeasurementDb?> Get(string name)
        {
            return await dbContext.Measurements
                .FirstOrDefaultAsync(e => e.Name == name);
        }
        public async Task<MeasurementDb?> Get(int id)
        {
            return await dbContext.Measurements
                .FirstOrDefaultAsync(e => e.DbId == id);
        }
        public async Task<MeasurementDb> Add(MeasurementDb m)
        {
            var result = await dbContext.Measurements.AddAsync(m);
            await dbContext.Measurements.AddAsync(m);
            await dbContext.SaveChangesAsync();
            return m;
        }

        public async Task<MeasurementDb> Update(MeasurementDb m)
        {
            MeasurementDb? m1 = await dbContext.Measurements.FirstOrDefaultAsync(e => e.Name == m.Name);
            if (m1 == null) return null;

            m1.CopyFrom(m);
            await dbContext.SaveChangesAsync();

            return m1;
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
