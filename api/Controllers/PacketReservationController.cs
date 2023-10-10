using DomainModel;
using DomainModel.Api;
using DomainServices;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace api.Controllers {
    [Route("api/Packet/{id}/Reservation")]
    [ApiController]
    public class PacketReservationController : ControllerBase {
        private readonly IRepository repository;

        public PacketReservationController(IRepository repository) {
            this.repository = repository;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReservationScheme))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReservationScheme))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ReservationScheme))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReservationScheme))]
        [Produces("application/json")]
        public async Task<ActionResult> Post(int id, string? identityId) {
            if(identityId == null || id == 0) {
                return BadRequest(new {
                    StatusCode = 400,
                    message = "PacketId or identityId are not defined"
                });
            }
            
            if(id <= 0) {
                return BadRequest(new {
                    StatusCode = 400,
                    message = "packetId can't be negative or 0" 
                });  
            }
            
            var completed = await repository.ReservePacket(id,identityId);

            //409 - Conflict --> packet already reserved
            if (completed == "Packet already reserved") return Conflict(new {
                StatusCode = 409,
                message = "packet already reserved"
            });
            //404 - not found - package does not exist
            else if (completed == "Packet not found") return NotFound(new {
                StatusCode = 404,
                Message = "Packet not found"
            });
            //409 - Conflict --> student already reserved a package
            else if (completed == "Already reserved a package") return Conflict(new {
                StatusCode = 409,
                message = "Already reserved a package"
            });
            //403 - forbidden --> student is forbidden from reserving package due to age
            else if (completed == "Student not old enough to reserve packet") return StatusCode(403, new {
                StatusCode = 403,
                message = "Student not old enough to reserve packet"
            });

            //Else return ok
            return Created(nameof(completed),new {
                StatusCode = 201,
                Message = "Packet reserved"
            });
            
        }
    }
}