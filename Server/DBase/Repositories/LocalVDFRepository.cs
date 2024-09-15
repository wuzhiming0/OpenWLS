using Microsoft.EntityFrameworkCore;
using OpenWLS.Server.DBase.DbContents;
using OpenWLS.Server.DBase.Models.GlobalDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.DBase.Repositories
{

    public interface ILocalVDFRepository : IVDFRepository { }
    public class LocalVDFRepository : ILocalVDFRepository
    {
        private readonly LocalDbContent dbContext;

        public LocalVDFRepository(LocalDbContent dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<GViewDefinitionFileBase>> GetVDFList()
        {
            return await dbContext.VDFs.Where(e => e.Deleted == null).ToListAsync();
        }

        public async Task<TextObject> GetVDF(int id)
        {
            return await dbContext.VdfObjs.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<GViewDefinitionFile> AddVDF(GViewDefinitionFile vdf)
        {
            vdf.Id = dbContext.VDFs.Any() ? dbContext.VDFs.Max(e => e.Id) : 0;
            vdf.Id++;
            var txtObj = new TextObjectVdf()
            {
                Val = vdf.Body,
                Id = vdf.Id
            };
            var result = await dbContext.VDFs.AddAsync(vdf);
            await dbContext.VdfObjs.AddAsync(txtObj);
            //     dbContext.Database.EnsureCreated();
            await dbContext.SaveChangesAsync();
            return vdf;
        }

        public async Task<GViewDefinitionFile> UpdateVDF(GViewDefinitionFile vdf)
        {
            var v1 = await dbContext.VDFs.FirstOrDefaultAsync(e => e.Id == vdf.Id);

            if (v1 == null) return null;

            v1.Version = vdf.Version;
            v1.Name = vdf.Name;
       //     await dbContext.SaveChangesAsync();
          var v2 = await dbContext.VdfObjs.FirstOrDefaultAsync(e => e.Id == vdf.Id);
            if (v2 == null) return null;
            v2.Val = vdf.Body;

            await dbContext.SaveChangesAsync();

            return vdf;
        }

        public async Task DeleteVDF(int id)
        {
            var result = await dbContext.VDFs
                .FirstOrDefaultAsync(e => e.Id == id);
            if (result != null)
            {
                //    dbContext.VDFs.Remove(result);
                result.Deleted = true;
                await dbContext.SaveChangesAsync();
            }
        }

    }

}
