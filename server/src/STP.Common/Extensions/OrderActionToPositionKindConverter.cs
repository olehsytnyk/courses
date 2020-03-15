using AutoMapper;
using STP.Interfaces.Enums;

namespace STP.Common.Extensions
{
    public class OrderActionToPositionKindConverter : IValueConverter<OrderAction, PositionKind>, ITypeConverter<OrderAction, PositionKind>
    {
        public static PositionKind Converter(OrderAction source)
        {
            return source == OrderAction.Buy ? PositionKind.Long : PositionKind.Short;
        }
        public PositionKind Convert(OrderAction source, ResolutionContext context) => Converter(source);
        public PositionKind Convert(OrderAction source, PositionKind destination, 
            ResolutionContext context) => Converter(source);
    }
}
