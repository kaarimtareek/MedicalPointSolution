namespace MedicalPoint.Common
{
    public static class DateTimeHelper
    {
        public static bool IsValidFromToDate(DateTime from, DateTime to)
        {
            return from <= to;
        }
    }
}
