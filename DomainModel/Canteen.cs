using DomainModel.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel {
    public class Canteen {
        public int id { get; set; }

        public City city { get; set; }

        public required string location { get; set; }

        public bool servesHotMeals { get; set; }

        //Als Canteen relatie heeft met CanteenStaffMember
        //ICollection<CanteenStaffMember> staffMembers { get; set; } = new List<CanteenStaffMember>();

        public ICollection<Packet> packetList { get; set; } = new List<Packet>();
    }
}
