using System.Runtime.Serialization;

namespace E_commerce.Core.Entities.Order;

public enum Status
{
    [EnumMember(Value = "Pending")]
    Pending,
    [EnumMember(Value = "PaymentReceived")]
    PaymentReceived,
    [EnumMember(Value = "PaymentFailed")]
    PaymentFailed
}
