﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel;

namespace DomainServices {
    public interface IRepository {
        public IEnumerable<Packet> GetPackets();
        public IEnumerable<Packet> GetReservedPackets(string email);
        public Packet? GetSinglePacket(int id);
        public Task<bool> AddPacket(Packet packet);
        public IEnumerable<Product>? GetAxampleProducts(int id);
        public Task<bool> reservePacket(int packetId, string personEmail);
    }
}
