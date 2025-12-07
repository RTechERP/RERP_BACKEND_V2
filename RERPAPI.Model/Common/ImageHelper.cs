using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                throw new Exception(ex.Message);
			}
        }

    }
}
