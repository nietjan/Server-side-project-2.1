﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel {
    public class CantineStaffMember {
        [Key]
        public string id { get; set; }

        public required string name { get; set; }

        public int staffNumber { get; set; }

        public Cantine? cantine { get; set; }

       
    }
}
