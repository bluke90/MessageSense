using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MessageSense.Data
{
    public class MessageSenseData :DbContext
    {
        public DbSet<Models.Contact> Contacts { get; set; }

        public MessageSenseData()
        {
            SQLitePCL.Batteries_V2.Init();

            this.Database.EnsureCreated();
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "dataContext.db3");

            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }


    }
}
