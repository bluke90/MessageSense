using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MessageSenseServer.Components.Net;

namespace MessageSenseServer.Data
{
    public class ServerContext : DbContext
    {
        public DbSet<AppUser> Users { get; set; }

        public ServerContext()
        {
            SQLitePCL.Batteries_V2.Init();

            this.Database.EnsureCreated();
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(Environment.CurrentDirectory, "ServerContext.db3");

            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }
    }
}
