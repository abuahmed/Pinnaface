using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnaFace.Core.Models
{
    [Table("UserAgencyAgents")]
    public class UserAgencyAgentDTO : UserEntityBase
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public UserDTO User
        {
            get { return GetValue(() => User); }
            set { SetValue(() => User, value); }
        }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("AgencyAgent")]
        public int AgencyWithAgentId { get; set; }
        public AgencyAgentDTO AgencyAgent 
        {
            get { return GetValue(() => AgencyAgent); }
            set { SetValue(() => AgencyAgent, value); }
        }

        
    }
}