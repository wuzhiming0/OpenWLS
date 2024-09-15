using Microsoft.EntityFrameworkCore;
using OpenWLS.Server.DBase.DbContents;
using OpenWLS.Server.DBase.Models.GlobalDb;
using OpenWLS.Server.DBase.Models.LocalDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.DBase.Repositories
{
    public interface IEdgeRepository
    {
        Task<List<Edge>> GetAll();
        Task<Edge?> Get(int id);
        Task<Edge?> Delete(int id);
        Task<Edge> Add(Edge e);
        Task<Edge?> Update(Edge e);

    }
    public class EdgeRepository : IEdgeRepository
    {
        private readonly LocalDbContent dbContext;

        public EdgeRepository(LocalDbContent dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Edge>> GetAll()
        {
            return await dbContext.Edges.Where(e => e.Deleted == null).ToListAsync();
        }

        public async Task<Edge?> Get(int id)
        {
            return await dbContext.Edges
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Edge> Add(Edge e)
        {
            e.Id = dbContext.Edges.Max(e => e.Id) + 1;
            var result = await dbContext.Edges.AddAsync(e);
            await dbContext.SaveChangesAsync();
            return e;
        }

        public async Task<Edge> Update(Edge e)
        {
            var res = await dbContext.Edges
                .FirstOrDefaultAsync(e => e.Id == e.Id);
            if (res != null)
            {
                res.CloneFrom(e);
                dbContext.SaveChangesAsync();
            }
            return res;
        }

        public async Task<Edge> Delete(int id)
        {
            var result = await dbContext.Edges
                .FirstOrDefaultAsync(e => e.Id == id);
            if (result != null)
            {
                //    dbContext.Edges.Remove(result);
                result.Deleted = true;
                await dbContext.SaveChangesAsync();
            }
            return result;
        }


    }

}
