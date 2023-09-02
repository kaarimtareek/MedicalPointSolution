namespace MedicalPoint.Constants
{
    public static class ConstantUserType
    {
        public const string SUPER_ADMIN = "قائد النقطة الطبية";
        //public const string ADMIN = "الأدمن";
        public const string Doctor = "الدكتور";
        public const string Pharmacist = "الصيدلي";
        public const string Recieptionist = "الاستقبال";
        public static string[] Types = { SUPER_ADMIN, 
            //ADMIN, 
            Doctor, Pharmacist, Recieptionist };
    }
}
