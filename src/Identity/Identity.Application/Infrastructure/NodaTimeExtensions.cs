using NodaTime;

namespace Catalog.Application.Infrastructure;

public static class NodaTimeExtensions
{
    private static readonly DateTimeZone _defaultDateTimeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();

    public static LocalDateTime GetNow(this IClock clock)
    {
        return clock.GetCurrentLocalDateTime();
    }

    public static LocalDate GetToday(this IClock clock)
    {
        return clock.GetCurrentInstant().InZone(_defaultDateTimeZone).Date;
    }

    public static LocalDateTime GetCurrentLocalDateTime(this IClock clock)
    {
        return clock.GetCurrentInstant().InUtc().LocalDateTime;
    }

    public static LocalDateTime Trim(this LocalDateTime localDate, long roundTicks)
    {
        var date = localDate.ToDateTimeUnspecified();
        return LocalDateTime.FromDateTime(new DateTime(date.Ticks - date.Ticks % roundTicks, date.Kind));
    }
}