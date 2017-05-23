using System.Data.Entity;
using RepositoryPattern;
using RepositoryPattern.Infrastructure;
using RepositoryPatternEF6;

namespace Entities
{
    public class EfStartUpTask : IStartupTask
    {
        public void Execute()
        {
            var initializer = new CreateTablesIfNotExist<NoisObjectContext>(new string[0],new string[0] );
            Database.SetInitializer(initializer);
        }

        public int Order
        {
            //ensure that this task is run first 
            get { return -1000; }
        }
    }
}
