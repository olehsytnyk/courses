using STP.Messages;

namespace STP.Profile.Domain.DTO.Position
{
    public class UserUPL
    {
        public string UserId { get; set; }
        public UPLMessage PositionUPL { get; set; }
    }
    
}
