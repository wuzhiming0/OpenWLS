using Microsoft.EntityFrameworkCore;
using OpenWLS.Server.DBase.DbContents;
using OpenWLS.Server.DBase.Models.CalibrationDb;
using OpenWLS.Server.DBase.Models.LocalDb;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OpenWLS.Server.DBase
{
    public interface ICalibrationRepository
    {
        Task<List<CVRecord>> GetItems(string[] conds);
        Task<CVRecord> AddItem(CVRecord item, byte[] val);
        Task<CVRecord?> UpdateItem(CVRecord item);
        Task<byte[]?> GetRecordVal(int id);
        Task DeleteItem(int id);
        void Init();

    }
    public class CalibrationRepository : ICalibrationRepository
    {

        static int nxtId = -1; 
        private readonly CalibrationDbContent dbContext;

        public CalibrationRepository(CalibrationDbContent dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Init()
        {
            nxtId = dbContext.CVRecords.Max(a=> a.Id) + 1;
        }

        public async Task<List<CVRecord>> GetItems(string[] conds)
        {
            List<CVRecord> res = await dbContext.CVRecords.ToListAsync();
            foreach(string s in conds)
                res = CVRecord.Filter(res, s);
            return res; 
        }
        public async Task<CVRecord> AddItem(CVRecord item, byte[] val)
        {
            item.Id = nxtId++;
            var result = await dbContext.CVRecords.AddAsync(item);
            BinObject bo = new BinObject()
            {
                Id = item.Id,
                Val = val
            };
            dbContext.BinObjs.Add(bo);
            await dbContext.SaveChangesAsync();
            return item;
        }


        public async Task DeleteItem(int id )
        {
            CVRecord? item = dbContext.CVRecords.FirstOrDefault(a => a.Id == id);
            if (item != null)
                item.Deleted = true;
            await dbContext.SaveChangesAsync();          
        }

        public async Task<CVRecord?> UpdateItem(CVRecord item)
        {
            CVRecord? item_db = dbContext.CVRecords.FirstOrDefault(a => a.Id == item.Id);
            if (item_db != null)
                item_db.CopyFrom(item);
            await dbContext.SaveChangesAsync();
            return item_db;
        }

        public async Task<byte[]? > GetRecordVal(int id)
        {
            BinObject? bo = dbContext.BinObjs.FirstOrDefault(a => a.Id == id);
            if (bo == null) return null;
            return bo.Val;
        }

    }

}
