using DomainModel.enums;
using DomainModel.overload;
using System.ComponentModel.DataAnnotations;


namespace DomainModel {
    public class Student {
        [Key]
        public int id { get; set; }

        public string securityId { get; set; } = string.Empty;

        public required string name { get; set; }

        [MinimumAgeRequirement]
        public DateTime birthday { get; set; }

        public required int studentNumber { get; set; }

        public required City studyCity { get; set; }

        public ICollection<Packet> reservedPackets { get; set; } = new List<Packet>();
    }
}
