using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;

namespace IDS.Data
{
    public class BaseDbContext : DbContext
    {
        public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //var assembly = Assembly.GetExecutingAssembly();
            var assembly = Assembly.Load("IDS.Entity");
            var assemblyMapping = Assembly.Load("IDS.Mapping");
            /* foreach (Type type in assembly.ExportedTypes)
             {
                 if (type.IsClass && (type.Name != "xxx"))
                 {
                     var method = modelBuilder.GetType().GetMethods().Where(x => x.Name == "Entity").FirstOrDefault();
                     if (method != null)
                     {
                         method = method.MakeGenericMethod(new Type[] { type });
                         method.Invoke(modelBuilder, null);
                     }
                 }
             }*/
            modelBuilder.ApplyConfigurationsFromAssembly(assemblyMapping);

            base.OnModelCreating(modelBuilder);
        }
    }
}
