namespace Tausi.RubiksCube
{
    public enum Axis
    {
        X,
        Y,
        Z
    }

    public enum Frontside
    {
        Front,
        Left,
        Back,
        Right
    }

    public enum MessageType
    {
        None,
        Spin
    }

    public enum SolveLevel
    {
        Shuffled,
        Top,
        TopRingT,
        Middle,
        BottomCross,
        BottomCrossRing,
        BottomCornersTwisted,
        AlmostCompleted,
        Completed
    }

    public enum TileColor
    {
        White,
        Red,
        Blue,
        Green,
        Orange,
        Yellow
    }

    public enum TileLocation
    {
        Top,
        Front,
        Left,
        Right,
        Back,
        Bottom
    }
}