using STP.Interfaces.Enums;

namespace STP.Common.Extensions
{
    public static class PositionKindExtension
    {
        public static bool IsOpened(this PositionKind position)
        {
            return position != PositionKind.Flat;
        }
    }
}
