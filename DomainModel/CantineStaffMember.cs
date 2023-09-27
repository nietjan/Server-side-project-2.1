using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel {
    public class CantineStaffMember {
        public int id {  get; set; }

        public required string name { get; set; }

        public int staffNumber { get; set; }

        public required Cantine cantine { get; set; }
    }
}
