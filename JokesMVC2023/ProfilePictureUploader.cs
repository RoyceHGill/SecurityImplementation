using JokesMVC2023.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;

namespace JokesMVC2023
{
    public class ProfilePictureUploader
    {
        string _uploadRootPath;
        string _defaultUploadFileName;

        private readonly UserManager<AppUser> _userManager;
        private readonly EncryptionService _encryptionService;

        public ProfilePictureUploader(IWebHostEnvironment webHostEnvironment, EncryptionService encryptionService, UserManager<AppUser> userManager)
        {
            _uploadRootPath = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
            _encryptionService = encryptionService;
            _userManager = userManager;
            _defaultUploadFileName = "Default.png";
        }



        public string UniqueFileName(string fileName)
        {
            try
            {
                string extension = fileName.Split('.').LastOrDefault();

                if (string.IsNullOrEmpty(extension)) { return null; }

                return $"{Guid.NewGuid()}.{extension}";
            }
            catch (Exception)
            {

                return null;
            }

        }

        public FileInfo LoadFile(string fileName)
        {
            DirectoryInfo dir = new DirectoryInfo(_uploadRootPath);

            if (!dir.EnumerateFiles().Any(f => f.Name.Equals(fileName)))
            {
                return null;
            }

            return dir.EnumerateFiles().Where(f => f.Name.Equals(fileName)).FirstOrDefault();
        }

        public async Task<byte[]> ReadFileIntoMemory(string fileName)
        {
            DirectoryInfo dir = new DirectoryInfo(_uploadRootPath);

            var file = LoadFile(fileName);

            if (file == null) { file = LoadFile("Default.png"); }

            using (var memStream = new MemoryStream())
            {
                using (var fileStream = File.OpenRead(file.FullName))
                {
                    fileStream.CopyTo(memStream);
                    var encryptedData = memStream.ToArray();

                    if (file.Name == "Default.png")
                    {
                        return encryptedData;
                    }

                    return _encryptionService.DecryptByteArray(encryptedData);
                }
            }
        }

        public async Task<string> GetFilePath(string fileName)
        {
            var file = LoadFile(fileName);

            if (file == null)
            {
                return null;
            }

            var directory = file.Directory.Name;

            if (directory.Equals("Uploads"))
            {
                return $"/{directory}/{file.Name}";
            }
            else
            {
                return $"/Uploads/{directory}/{file.Name}";
            }
        }

        public string GetFileExtentsion(string fileName)
        {
            if (new FileExtensionContentTypeProvider().TryGetContentType(fileName, out string contentType))
            {
                return contentType;
            }
            return null;
        }

        public async Task<string> SaveFile(IFormFile file)
        {
            string fileName = UniqueFileName(file.FileName);

            byte[] fileContents;

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                fileContents = stream.ToArray();
            }

            var encryptedData = _encryptionService.EncrypByteArray(fileContents);

            using (var stream = new MemoryStream(encryptedData))
            {
                var targerFile = Path.Combine(_uploadRootPath, fileName);

                using (var fileStream = new FileStream(targerFile, FileMode.Create))
                {
                    stream.WriteTo(fileStream);
                    return fileName;
                }
            }
        }
    }
}
