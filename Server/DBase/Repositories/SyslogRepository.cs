using Microsoft.EntityFrameworkCore;
using OpenWLS.Server.DBase.DbContents;
using OpenWLS.Server.DBase.Models.LocalDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OpenWLS.Server.DBase
{
    public interface ISyslogRepository
    {
        Task<List<SyslogItem>> GetAll();
        Task<List<SyslogItem>> GetItemsFromId(int id);
        Task<List<SyslogItem>> GetItems(long time_from, long time_to, string mod);
        Task<SyslogItem> AddItem(SyslogItem item);
        Task AddMessage(string msg);
        Task AddMessage(string msg,  uint color);
        Task AddMessage(string msg, uint color, string mod);
        Task AppendMessage(string msg);
        Task DeleteItem(int id);
        void Init();

    }
    public class SyslogRepository : ISyslogRepository
    {

        static int nxtId = -1; 
        private readonly SyslogDbContent dbContext;

        public SyslogRepository(SyslogDbContent dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Init()
        {
            nxtId = dbContext.Items.Max(a=> a.Id) + 1;
        }

        public async Task<List<SyslogItem>> GetAll()
        {
            return await dbContext.Items.ToListAsync();
        }
        public async  Task<List<SyslogItem>> GetItemsFromId(int id)
        {
            if (id < 0)
            {
                id = dbContext.Items.Max(a => a.Id); id++;
                await AddMessage("syslog client started");
            }
            return await dbContext.Items.Where(a => a.Id >= id ).ToListAsync();
        }
        public async Task<List<SyslogItem>> GetItems(long time_from, long time_to, string mod)
        {
            if (time_from == 0)
            {
                if (time_to == 0)
                {
                    if (string.IsNullOrEmpty(mod))
                        return await dbContext.Items.Where(a => string.IsNullOrEmpty(a.Module)).ToListAsync();
                    else
                        return await dbContext.Items.Where(a => a.Module == mod).ToListAsync();
                }
                else
                {
                    if (string.IsNullOrEmpty(mod))
                        return await dbContext.Items.Where(a => string.IsNullOrEmpty(a.Module) && a.Time <= time_to).ToListAsync();
                    else
                        return await dbContext.Items.Where(a => a.Module == mod && a.Time <= time_to).ToListAsync();
                }
            }
            else
            {
                if (time_to == 0)
                {
                    if (string.IsNullOrEmpty(mod))
                        return await dbContext.Items.Where(a => string.IsNullOrEmpty(a.Module) && a.Time >= time_from).ToListAsync();
                    else
                        return await dbContext.Items.Where(a => a.Module == mod && a.Time >= time_from).ToListAsync();
                }
                else
                {
                    if (string.IsNullOrEmpty(mod))
                        return await dbContext.Items.Where(a => string.IsNullOrEmpty(a.Module) && a.Time <= time_to && a.Time >= time_from).ToListAsync();
                    else
                        return await dbContext.Items.Where(a => a.Module == mod && a.Time <= time_to && a.Time >= time_from).ToListAsync();
                }
            }
        }

        public async Task<SyslogItem> AddItem(SyslogItem item)
        {
            item.Id = nxtId++;
            var result = await dbContext.Items.AddAsync(item);
            await dbContext.SaveChangesAsync();
            return item;
        }

        public async Task DeleteItem(int id )
        {
            var items = await dbContext.Items.Where(a => a.Id <= id).ToListAsync();
            foreach (SyslogItem a in items)
                dbContext.Items.Remove(a);
            await dbContext.SaveChangesAsync();          
        }

        public async Task AddMessage(string msg)
        {
            await AddItem(new SyslogItem()
            {
                Time = DateTime.Now.Ticks, 
                Msg = msg
            } );
        }

        public async Task AddMessage(string msg, uint color)
        {
            await AddItem(new SyslogItem()
            {
             //   Id = nxtId++,
                Msg = msg,
                Time = DateTime.Now.Ticks,
                Color = color
            });
        }

        public async Task AddMessage(string msg, uint color, string mod)
        {
            await AddItem(new SyslogItem()
            {
           //     Id = nxtId++,
                Msg = msg,
                Color = color,
                Module = mod
            });
        }
        public async Task AppendMessage(string msg)
        {
            int id = nxtId -1;
            SyslogItem? item = dbContext.Items.FirstOrDefault(a => a.Id == id);
            if(item != null)
            {
                item.Msg = $"{item.Msg}\n{msg}";
                await dbContext.SaveChangesAsync();
            }
        }

    }

}
