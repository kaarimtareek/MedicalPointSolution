namespace MedicalPoint.Constants
{
    public static class ConstantMessageCodes
    {
        public const string ERROR_MESSAGE_KEY = "ErrorMessage";
        public const string ACTION_MESSAGE_KEY = "ActionName";
        public const string CONTROLLER_MESSAGE_KEY = "ControllerName";

        public static string NameAlreadyExist = "الاسم موجود بالفعل";
        public static string PhoneNumberAlreadyExist = "رقم الهاتف موجود بالفعل";
        public static string MilitaryNumberAlreadyExist = "الرقم العسكري موجود بالفعل";
        public static string NationalNumberAlreadyExist = "الرقم القومي موجود بالفعل";
        public static string GeneralNumberAlreadyExist = "الرقم العام موجود بالفعل";
        
        public static string PatientNotFound = "المريض غير موجود";
        public static string PatientAlreadyUnderObservation = "المريض محجوز بالفعل";
        public static string PatientHasAlreadyActiveVisit = "المريض لديه كشف لم ينتهي بعد";
        public static string VisitNotFound = "الكشف غير موجود";
        public static string CannotChangeVisitStatus = "لا يمكن تغيير حالة الكشف";
        public static string VisitDiagnosisIsNotWritten = "لم يتم كتابة تشخيص بعد";
        public static string DiagnosisIsEmpty = "لا يمكن حفظ تشخيص فارغ";

        public static string InvalidQuantity = "كمية غير صحيحة";
        public static string InvalidRestDaysNumber = "عدد ايام الراحة غير صحيح";
        public static string VisitRestAlreadyExist = "الاورنيك للكشف موجود بالفعل";
        public static string VisitRestNotFound = "الاورنيك غير موجود";
        public static string VisitTypeNotFound = "نوع الكشف غير موجود";
        public static string CannotEditVisit = "لا يمكن التعديل في الكشف";

        public static string MedicineNotFound = "لا يمكن التعديل في الكشف";
        public static string ImageNotFound = "الصورة غير موجودة";
        public static string CannotUploadImageMoreThan2mb = "لا يمكن رفع صورة اكثر من 2 ميجا بايت";
        
        public static string VisitMedicineNotFound = "علاج الكشف غير موجود";
        public static string VisitMedicineAlreadyExist = "علاج الكشف موجود بالفعل";
        public static string NoVisitMedicinesOrAlreadyGiven = "لا يوجد علاج في الكشف او تم صرفهم بالفعل";
        public static string VisitMedicineQuantityMoreThanMedicineQuantity = "كمية العلاج في الكشف اكبر من كمية المخزون";
        public static string UserNameOrPasswordIsIncorrect = "اسم المستخدم او كلمة المرور غير صحيحة";
        public static string UserIsInActive = "المستخدم غير مفعل";
        public static string UserNotFound = "المستخدم غير موجود";
        public static string EmailAlreadyExist = "اسم السمتخدم ( البريد الالكتروني) موجود بالفعل";
        
        public static string DegreeAlreadyExist = "الرتبة \\ الدرجة موجودة بالفعل";
        public static string DegreeNotFound = "الرتبة \\ الدرجة غير موجودة";
        
        public static string ClinicNotFound = "العيادة غير موجودة";
        public static string ClinicNameAlreadyExist = "اسم العيادة  موجود بالفعل";
        public static string DepartmentNotFound = "القسم غير موجود";
        public static string BedNotFound = "السرير غير موجود";
        public static string CannotRemovePatientFromBed = "لا يمكن إزالة المريض من هذا السرير";
        public static string CannotAddPatientToBed = "لا يمكن اضافة مريض لهذا السرير ";


    }
}
