namespace MedicalPoint.Constants
{
    public static class ConstantVisitStatus
    {
        public const string IN_RECIEPTION = "في الاستقبال";
        public const string IN_CLINIC_DIAGNOSIS = "في الكشف - تشخيص الحالة";
        public const string IN_CLINIC_MEDICINES = "في الكشف - كتابة الأدوية";
        public const string TAKING_MEDICINE = "انتظار صرف الدواء";
        public const string FINISHED = "انتهت";

        public static bool CanChangeStatus(string oldStatus, string newStatus)
        {
            if(!AvailableStatusses.ContainsKey(oldStatus))
            {
                return false;
            }
            return AvailableStatusses[oldStatus].Contains(newStatus);
        }

        public static Dictionary<string, List<string>> AvailableStatusses = new Dictionary<string, List<string>>
        {
            { IN_RECIEPTION, new List<string> { IN_CLINIC_DIAGNOSIS , IN_CLINIC_MEDICINES, TAKING_MEDICINE , FINISHED } },
            { IN_CLINIC_DIAGNOSIS, new List<string> { IN_CLINIC_MEDICINES, TAKING_MEDICINE , FINISHED } },
            { IN_CLINIC_MEDICINES, new List<string> {  TAKING_MEDICINE , FINISHED } },
            { TAKING_MEDICINE, new List<string> {   FINISHED } },
        };

    }
}
