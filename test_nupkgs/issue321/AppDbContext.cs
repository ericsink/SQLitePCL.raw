using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WpfAppSqliteTesting
{
    public class AppDbContext : DbContext
    {
        #region PROPERTIES - ENTITIES
        public DbSet<Entity> Entities { get; set; }
        #endregion

        #region CONSTRUCTORS
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        #endregion
    }
}
