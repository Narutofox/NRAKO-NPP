namespace NRAKO_IvanCicek.Helpers
{
    public enum RecordStatus
    {
        Deleted = 10,
        Active = 20
    }

    public enum UserType
    {
        Normal = 10,
        Admin = 50
    }

    public enum Visibility
    {
        Javno = 10,
        Samo_Prijatelji = 20,
        Privatno = 30
    }

    public enum NotificationType
    {
        Error, Warning, Info, Success
    }
}