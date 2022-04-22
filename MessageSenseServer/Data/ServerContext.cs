using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MessageSenseServer.Components.Net;
using MessageSenseServer.Components.Models;

namespace MessageSenseServer.Data
{
    public class ServerContext : DbContext
    {
        public DbSet<AppUser> Users { get; set; }
        public DbSet<AuthenticationToken> AuthenticationTokens { get; set; }
        public DbSet<Message> Messages { get; set; }

        public ServerContext()
        {
            SQLitePCL.Batteries_V2.Init();

            this.Database.EnsureCreated();
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(Environment.CurrentDirectory, "ServerContext.db3");

            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MessageSense;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }
    }
}
