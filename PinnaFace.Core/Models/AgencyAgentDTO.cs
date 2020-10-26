using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFace.Core.Common;

namespace PinnaFace.Core.Models
{
    [Table("AgencyAgents")]
    public class AgencyAgentDTO : CommonFieldsA
    {
        //Add Field Determine who The ACCOUNT OWNER is, i.e. the AGENCY or the AGENT
        public AgencyAgentDTO()
        {
            Users = new HashSet<UserAgencyAgentDTO>();
        }

        ////[Key] throws number of constrint releationship must be identical exception
        ////[Column(Order = 1)]
        //[ForeignKey("Agency")]
        //public int AgencyId { get; set; }
        //public AgencyDTO Agency
        //{
        //    get { return GetValue(() => Agency); }
        //    set { SetValue(() => Agency, value); }
        //}

        //[Key]
        //[Column(Order = 2)]
        [ForeignKey("Agent")]
        public int AgentId { get; set; }
        public AgentDTO Agent
        {
            get { return GetValue(() => Agent); }
            set { SetValue(() => Agent, value); }
        }

        public ICollection<UserAgencyAgentDTO> Users
        {
            get { return GetValue(() => Users); }
            set { SetValue(() => Users, value); }
        }
    }
}