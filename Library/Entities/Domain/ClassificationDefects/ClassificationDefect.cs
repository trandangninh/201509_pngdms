using Entities.Domain.Suppliers;
using RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.ClassificationDefects
{
    public class ClassificationDefect : BaseEntity
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
        /// <summary>
        /// get or set materials
        /// </summary>
        public string Materials { get; set; }

        private ICollection<Supplier> _suppliers;
        public virtual ICollection<Supplier> Suppliers
        {
            get { return _suppliers ?? (_suppliers = new List<Supplier>()); }
            protected set { _suppliers = value; }
        }
    }
}
