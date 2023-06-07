using System;

namespace Nop.Services.Helpers;

public static class TurkishDayConverter
{
    public static string ConvertToTurkishDay(DayOfWeek dayOfWeek)
    {
        switch (dayOfWeek)
        {
            case DayOfWeek.Monday:
                return "Pazartesi";
            case DayOfWeek.Tuesday:
                return "Salı";
            case DayOfWeek.Wednesday:
                return "Çarşamba";
            case DayOfWeek.Thursday:
                return "Perşembe";
            case DayOfWeek.Friday:
                return "Cuma";
            case DayOfWeek.Saturday:
                return "Cumartesi";
            case DayOfWeek.Sunday:
                return "Pazar";
            default:
                throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, "Invalid day of week value.");
        }
    }
}