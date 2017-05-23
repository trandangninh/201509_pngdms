using RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.ClassificationDefects
{
    public class Material : BaseEntity
    {
        /// <summary>
        /// get or set name
        /// </summary>
        public string Name { get; set; }
    }
}
