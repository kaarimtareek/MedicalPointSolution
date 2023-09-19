namespace MedicalPoint.Constants
{
    public static class ConstantMedicineExpirationStatus
    {
        public static string EXPIRED = "منتهي الصلاحية";
        public static string NEAR_EXPIRATION = "قارب على انتهاء الصلاحية";
        public static string NOT_EXPIRED = "جيد الصلاحية";
        public static string GetAppropiateStatus(DateTime date)
        {
            if (date <= DateTime.Now)
            {
                return EXPIRED;
            }
            else if (date.Subtract(DateTime.Now).Days < 31)
            {
                return NEAR_EXPIRATION;
            }
            else
            {
                return NOT_EXPIRED;
            }
        }
    }
}
