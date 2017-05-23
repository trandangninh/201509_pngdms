using RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.Categories
{
    public class Category : BaseEntity
    {
        /// <summary>
        /// get or set name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// get or set note
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// get or set display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
