using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
using RepositoryPattern;
using RepositoryPattern.DataContext;

namespace Entities
{
    public class NoisObjectContext : DbContext, IDbContextAsync
    {
        public NoisObjectContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            //this.Configuration.LazyLoadingEnabled = false;
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => !String.IsNullOrEmpty(type.Namespace))
            .Where(type => type.BaseType != null && type.BaseType.IsGenericType &&
                type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));

            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }
            //...or do it manually below. For example,
            //modelBuilder.Configurations.Add(new LanguageMap());

            base.OnModelCreating(modelBuilder);
        }

        public new IDbSet<T> Set<T>() where T : BaseEntity
        {
            return base.Set<T>();
        }
    }

    public class NoisContextDbInitializer : CreateDatabaseIfNotExists<NoisObjectContext>
    {
        protected override void Seed(NoisObjectContext context)
        {
        }
    } 
}
