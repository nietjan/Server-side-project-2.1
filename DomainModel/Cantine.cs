using DomainModel.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel {
    public class Cantine {
        public int id { get; set; }

        public City city { get; set; }

        public required string location { get; set; }

        public bool servesHotMeals { get; set; }

        //Als cantine relatie heeft met cantineStaffMember
        //ICollection<CantineStaffMember> staffMembers { get; set; } = new List<CantineStaffMember>();

        public ICollection<Packet> packetList { get; set; } = new List<Packet>();
    }
}
