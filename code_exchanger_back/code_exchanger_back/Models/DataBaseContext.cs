﻿using Microsoft.EntityFrameworkCore;
using code_exchanger_back.Models;
using static code_exchanger_back.Startup;
using Microsoft.Extensions.Configuration;

namespace code_exchanger_back.Models
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(ConfigrationManage.Configuration.GetConnectionString("PostgreSql"));
        }
    }
}