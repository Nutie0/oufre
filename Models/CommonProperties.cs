using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace registerAPI.Models
{
    public class CommonProperties
    {
        public DateTime Created { get; set; } = new DateTime(1986 , 1 , 1);
        public DateTime Update { get; set; } = new DateTime(1986 , 1 , 1);
        public string  Message{ get; set; }
    }
}