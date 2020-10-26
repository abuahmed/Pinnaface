using System.ComponentModel.DataAnnotations.Schema;


namespace PinnaFace.Core.Models.Interfaces
{
    public interface IObjectState
    {
        [NotMapped]
        ObjectState ObjectState { get; set; }
    }
}
