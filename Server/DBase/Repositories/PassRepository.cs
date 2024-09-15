using Microsoft.EntityFrameworkCore;
using OpenWLS.Server.DBase.DbContents;
using OpenWLS.Server.DBase.Models.LocalDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.DBase.Repositories
{
    public class PassRepository : IPassRepository
    {
        private readonly LocalDbContent dbContext;

        public PassRepository(LocalDbContent dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Pass>> GetPassList()
        {
            return await dbContext.Passes.Where(e => e.Deleted == null).ToListAsync();
        }

        public async Task<Pass?> GetPass(int id)
        {
            return await dbContext.Passes
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Pass> AddPass(Pass r)
        {
            var result = await dbContext.Passes.AddAsync(r);
            await dbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Pass> UpdatePass(Pass r)
        {
            var result = await dbContext.Passes
                .FirstOrDefaultAsync(e => e.Id == r.Id);

            if (result != null)
            {
                result.TimeStop = r.TimeStop;
                result.Note = r.Note;
                await dbContext.SaveChangesAsync();
                return result;
            }
            return null;
        }

        public async Task<Pass> DeletePass(int id)
        {
            var result = await dbContext.Passes
                .FirstOrDefaultAsync(e => e.Id == id);
            if (result != null)
            {
                //    dbContext.Passs.Remove(result);
                result.Deleted = true;
                await dbContext.SaveChangesAsync();
            }
            return result;
        }
    }

}
