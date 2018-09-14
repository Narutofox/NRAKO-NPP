namespace NRAKO_IvanCicek.Helpers
{
    public static class Helper
    {
        public static bool CheckImageTypeAllowed(string fileType)
        {
            fileType = fileType.ToLower();
            if (fileType == ".jpg" || fileType == ".jpeg" || fileType == ".png")
            {
                return true;
            }
            return false;
        }
    }
}