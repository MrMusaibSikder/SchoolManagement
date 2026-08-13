namespace SchoolERP.Domain.Enums;

/// <summary>Delivery status of an outgoing SMS message.</summary>
public enum SmsStatus
{
    Pending = 1,
    Sent = 2,
    Delivered = 3,
    Failed = 4
}
