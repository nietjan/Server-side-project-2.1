using DomainModel.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel {
    public class Student {
        [Key]
        public string id { get; set; }

        public required string name { get; set; }

        public required int studentNumber { get; set; }

        public required City studyCity { get; set; }

        public ICollection<Packet> reservedPackets { get; set; } = new List<Packet>();
    }
}
