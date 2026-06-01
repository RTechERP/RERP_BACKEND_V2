using System;
using System.IO;

namespace RERPAPI.Model.Common
{
    public static class FileCourseHelper
    {
        /// <summary>
        /// Lấy tên file kèm đuôi file từ đường dẫn server.
        /// Ví dụ:
        /// \\192.168.1.190\Software\Test\UPLOADFILE\CourseLesson/pdfs/test.pdf
        /// => test.pdf
        /// </summary>

        public static string GetFileNameWithExtension(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            // Chuẩn hóa dấu \ và / để xử lý được cả UNC path và URL path
            path = path.Replace("\\", "/");

            return Path.GetFileName(path);
        }
        public static string? CopyFile(string? sourcePath, string destinationFolder)
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
            {

                return null;
            }

            if (string.IsNullOrWhiteSpace(destinationFolder))
                return null;

            // Chuẩn hóa lại path
            sourcePath = sourcePath.Replace("/", "\\");

            if (!File.Exists(sourcePath))
            {
                string filename = GetFileNameWithExtension(sourcePath);

                if (string.IsNullOrWhiteSpace(filename))
                    return null;

                var oldFolders = new List<string>
    {
        @"\\192.168.1.190\Software\ftp\Upload\Course\PDFFileLesson\pdfs",
        @"\\192.168.1.190\Software\ftp\Upload\Course\PDFFileLesson\files",
        @"\\192.168.1.190\Software\ftp\Upload\Course\PDFFileLesson"
    };

                string? foundPath = null;

                foreach (var folder in oldFolders)
                {
                    var checkPath = Path.Combine(folder, filename).Replace("/", "\\");

                    if (File.Exists(checkPath))
                    {
                        foundPath = checkPath;
                        break;
                    }
                }

                if (string.IsNullOrWhiteSpace(foundPath))
                    return null;

                sourcePath = foundPath;
            }

            if (!Directory.Exists(destinationFolder))
                Directory.CreateDirectory(destinationFolder);

            string fileName = Path.GetFileName(sourcePath);
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);

            string newFileName = $"{fileNameWithoutExt}_copy_{DateTime.Now:yyyyMMddHHmmssfff}{extension}";
            string destinationPath = Path.Combine(destinationFolder, newFileName);

            File.Copy(sourcePath, destinationPath, overwrite: false);

            return destinationPath;
        }
        public static string? CopyFileFromFolder(
    string? fileName,
    string oldFolder,
    string newFolder)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return null;

            if (string.IsNullOrWhiteSpace(oldFolder))

                return null;

            if (string.IsNullOrWhiteSpace(newFolder))
                return null;

            oldFolder = oldFolder.Replace("/", "\\");
            newFolder = newFolder.Replace("/", "\\");

            string sourcePath = Path.Combine(oldFolder, fileName);

            if (!File.Exists(sourcePath))
                return null;

            if (!Directory.Exists(newFolder))
                Directory.CreateDirectory(newFolder);

            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);

            string newFileName = $"{fileNameWithoutExt}_copy_{DateTime.Now:yyyyMMddHHmmssfff}{extension}";
            string destinationPath = Path.Combine(newFolder, newFileName);

            File.Copy(sourcePath, destinationPath, overwrite: false);

            // Vì DB CourseQuestion.Image chỉ lưu tên ảnh nên return tên file mới
            return newFileName;
        }



    }
}