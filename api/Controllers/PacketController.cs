using DomainModel;
using DomainServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers {
    [Route("api/Packet/[action]")]
    [ApiController]
    public class PacketController : ControllerBase {
        private readonly IRepository repository;

        public PacketController(IRepository repository) {
            this.repository = repository;
        }

        [HttpPost]
        public async Task<ActionResult> Reserve(String personEmail, int packetId) {
            if(packetId <= 0) {
                return BadRequest(new {
                    StatusCode = 400,
                    message = "packetId can't be negative or 0" 
                });  
            }
            
            var completed = await repository.reservePacket(packetId, personEmail);

            //check if packet is already reserved
            if (completed == "Packet already reserved") return BadRequest(new {
                StatusCode = 400,
                message = "packet already reserved"
            });
            else if (completed == "Packet not found") return NotFound(new {
                StatusCode = 404,
                Message = "Packet not found"
            });
            else if (completed == "Already reserved a package") return BadRequest(new {
                StatusCode = 400,
                message = "Already reserved a package"
            });

            //Else return ok
            return Ok(new {
                StatusCode = 201,
                Message = "Packet reserved"
            });
            
        }
    }
}
