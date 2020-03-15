using System.ComponentModel;

namespace STP.Interfaces.Enums
{
    public enum ErrorCode
    {
        UnknownError = 0000,

        [Description("Not Found FIle")]
        FileNotFound = 1404,
        [Description("Invalid FIle")]
        InvalidFile = 1400,

        //Identity Service ErrorCode
        [Description("Not Found User")]
        UserNotFound = 2404,
        [Description("Invalid User")]
        InvalidUser = 2400,
        [Description("Invalid Permission For User")]
        InvalidPermissionUser = 2403,
        [Description("Conflict while uploading avatar")]
        CannotUploadAvatar = 2410,

        //Market Service ErrorCode
        [Description("Not Found Market")]
        MarketNotFount = 3404,
        [Description("Invalid Market")]
        InvalidMarket = 3400,
        [Description("Invalid Permission For Market")]
        InvalidPermissionMarket = 3403,

        //Profile Service ErrorCode
        [Description("Conflict while updating TraderInfo")]
        CannotUpdateTraderInfo = 4300,
        [Description("Conflict while creating TraderInfo")]
        CannotCreateTraderInfo = 4408,
        [Description("TraderInfo already exist")]
        TraderInfoIdExist = 4100,
        [Description("Conflict while creating order")]
        CannotCreateOrder = 4409,
        [Description("Not Found Order")]
        OrderNotFound = 4404,
        [Description("Not Found Order")]
        PositionNotFound = 4405,
        [Description("Not Found TraderInfo")]
        TraderInfoNotFound = 4406,
        [Description("Invalid Profile")]
        InvalidProfile = 4400,
        [Description("Invalid Permission For Profile")]
        InvalidPermissionProfile = 4403,

        //Realtime WebSocketsService 5000-5999
        [Description("Invalid connectionId")]
        InvalidConnectionId = 5000,
        [Description("Invalid userId")]
        InvalidUserId = 5001,
        [Description("Invalid sessionId")]
        InvalidSessionId = 5002,
        [Description("ConnectionId not found")]
        ConnectionIdNotFound = 5003,


    }
}
