namespace RERPAPI.Model.Common
{
    public static class ImageHelper
    {
        public static string ImageToBase64(string filePath)
        {
            try
            {
                byte[] imageBytes = File.ReadAllBytes(filePath);
                return Convert.ToBase64String(imageBytes);
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}