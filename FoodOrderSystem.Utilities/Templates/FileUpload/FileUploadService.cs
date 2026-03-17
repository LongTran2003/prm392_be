using Microsoft.AspNetCore.Http;

namespace FoodOrderSystem.Utilities.Templates.FileUpload
{
    public class FileUploadService
    {
        private readonly string _uploadPath;

        public FileUploadService(string uploadPath = "wwwroot/uploads")
        {
            _uploadPath = uploadPath;
            EnsureUploadDirectoryExists();
        }

        /// <summary>
        /// Save uploaded file to wwwroot/uploads/{shop|menu}/
        /// Returns relative URL path
        /// </summary>
        public async Task<string> SaveFileAsync(IFormFile file, string folderType = "shop")
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty");
            }

            // Create folder structure
            var folderPath = Path.Combine(_uploadPath, folderType);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative URL (for Android to download)
            return $"/uploads/{folderType}/{fileName}";
        }

        /// <summary>
        /// Delete file from disk
        /// </summary>
        public void DeleteFile(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return;

            var filePath = Path.Combine(_uploadPath, relativePath.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private void EnsureUploadDirectoryExists()
        {
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }
    }
}
