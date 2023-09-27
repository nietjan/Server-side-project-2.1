using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel;
using DomainServices;

namespace Infrastructure {
    public class InMemoryRepository : IRepository {
        private static List<Packet> packets = new List<Packet>();
        public async Task<bool> addPacket(Packet packet) {
            packets.Add(packet);
            return true;
        }

        public IEnumerable<Product> GetAxampleProducts(int id) {
            return packets.ElementAt(id).axampleProducts;
        }

        public IEnumerable<Packet> GetPackets() {
            return packets;
        }

        public IEnumerable<Packet> GetSinglePacket(int id) {
            return packets.Where(i => i.id == id);
        }

        public async Task<bool> reservePacket(int packetId, string personEmail) {
            packets.ElementAt(packetId).reservedBy = personEmail;
            return true;
        }
    }
}
