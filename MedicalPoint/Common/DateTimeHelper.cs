namespace MedicalPoint.Common
{
    public static class DateTimeHelper
    {
        public static string DefaultFormat = "yyyy-MM-dd";
        public static bool IsValidFromToDate(DateTime from, DateTime to)
        {
            return from <= to;
        }
    }
}
