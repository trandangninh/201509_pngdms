using RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.Frequencys
{
    public class Frequency : BaseEntity
    {
        /// <summary>
        /// get or set name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Get or set mark
        /// </summary>
        public int Mark { get; set; }
        /// <summary>
        /// Get Or Set Code
        /// </summary>
        public string Code { get; set; }
    }

    public enum FrequencyEnum
    {
        Remote = 1,
        Occasionally = 2,
        Probably = 3,
        Frequency = 4,
        Always = 5
    }
}
