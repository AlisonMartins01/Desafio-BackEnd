namespace Rentals.Api.Infrastructure
{
    public static class DateMapping
    {
        public static DateTime ToUtcDateTime(this DateOnly d) =>
            DateTime.SpecifyKind(d.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

        public static DateTime? ToUtcDateTime(this DateOnly? d) =>
            d.HasValue ? d.Value.ToUtcDateTime() : (DateTime?)null;
    }
}
