using RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.Classifications
{
    public partial class Classification : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public int Severity { get; set; }
        public int Dectability { get; set; }
        public int? FoundByFunctionId { get; set; }
    }
    public partial class ClassificationFullObject : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public int Severity { get; set; }
        public int Dectability { get; set; }
        public int? FoundByFunctionId { get; set; }
        public string FoundByFunctionName { get; set; }
    }
}
