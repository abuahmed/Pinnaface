using System.ComponentModel.DataAnnotations;
using PinnaFace.Core.Enumerations;


namespace PinnaFace.Core.Models
{    
    public class ListDTO : EntityBase
    {
        //[Key]
        //public int Id { get; set; }

        public ListTypes Type
        {
            get { return GetValue(() => Type); }
            set { SetValue(() => Type, value); }
        }
        [Required]
        public string DisplayName
        {
            get { return GetValue(() => DisplayName); }
            set { SetValue(() => DisplayName, value); }
        }
    }
}
