using logdbcoreapi.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;

namespace logdbcoreapi.DBContext
{
    public class MysqlDBContext: DbContext
    {
        public MysqlDBContext(DbContextOptions<MysqlDBContext> options)
         : base(options)
        {
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    var connectionString = Startup.Configuration["ConnectionStrings:DefaultConnection"];
        //    optionsBuilder.UseMySQL(connectionString);
        //}

        public DbSet<MessageModel> MsgModel { get; set; }
    }
}
