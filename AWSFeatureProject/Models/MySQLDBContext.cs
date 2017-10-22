using Microsoft.EntityFrameworkCore;
//using Pomelo.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSFeatureProject.Models
{
    public class MySQLDBContext : DbContext
    {
        public MySQLDBContext(DbContextOptions<MySQLDBContext> options)  
            : base(options)  
        {
        }
        //public static MySQLDBContext Create()
        //{
        //    return new MySQLDBContext();
        //}

        public DbSet<AWSFeatureProject.Models.FileUpdate> FileUpdate { get; set; }
    }
}
