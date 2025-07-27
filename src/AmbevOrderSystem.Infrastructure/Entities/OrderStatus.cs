namespace AmbevOrderSystem.Infrastructure.Entities
{
    public enum OrderStatus
    {
        Pending,
        SentToAmbev,
        Confirmed,
        Failed,
        Retry
    }
}