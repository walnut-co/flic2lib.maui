namespace flic2lib.Maui;

public interface IFlicManager
{
    IEnumerable<FlicButton> Buttons { get; }
}

public partial class FlicManager : IFlicManager
{
    public FlicManager()
    {
    }

    public IEnumerable<FlicButton> Buttons => null;
    //public IEnumerable<FlicButton> Buttons => GetButtons();

    partial void Initialize();
    //public partial IEnumerable<FlicButton> GetButtons()
    //{
    //    return Buttons;
    //}
}

