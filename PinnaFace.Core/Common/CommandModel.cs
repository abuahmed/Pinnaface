namespace PinnaFace.Core
{
    public class CommandModel
    {
        public CommandModel()
        {
            Id = 0;
        }
        public int Id { get; set; }
    }

    public class ActivationModel
    {
        public ActivationModel()
        {
            DatabaseVersionDate = 0;
            MaximumSystemVersion = 0;
        }
        public int DatabaseVersionDate { get; set; }
        public int MaximumSystemVersion { get; set; }
    }
}