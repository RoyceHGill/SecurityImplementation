using System.Security.Cryptography;

namespace JokesMVC2023
{
    public class EncryptionService
    {
        private readonly string _secretKey;

        public EncryptionService(IConfiguration configuration)
        {
#if DEBUG
            _secretKey = configuration["SecretKey"];
#else
            _secretKey = Environment.GetEnvironmentVariable("ENCKEY");
#endif
        }

        public byte[] EncrypByteArray(byte[] fileData)
        {
            using (AesManaged aesAlgorithm = new AesManaged())
            {
                aesAlgorithm.Key = System.Text.Encoding.UTF8.GetBytes(_secretKey);

                ICryptoTransform encryptor = aesAlgorithm.CreateEncryptor(aesAlgorithm.Key, aesAlgorithm.IV);

                using (MemoryStream memoryStreamEncrypt = new MemoryStream())
                {
                    memoryStreamEncrypt.Write(aesAlgorithm.IV, 0, 16);

                    using (CryptoStream csEncrypt = new CryptoStream(memoryStreamEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(fileData, 0, fileData.Length);
                        csEncrypt.FlushFinalBlock();
                        return memoryStreamEncrypt.ToArray();
                    }
                }
            }
        }

        public byte[] DecryptByteArray(byte[] encryptedFileData)
        {
            using (AesManaged aesAlgoritm = new AesManaged())
            {
                aesAlgoritm.Key = System.Text.Encoding.UTF8.GetBytes(_secretKey);

                byte[] IV = new byte[16];
                Array.Copy(encryptedFileData, 0, IV, 0, 16);

                ICryptoTransform decryptor = aesAlgoritm.CreateDecryptor(aesAlgoritm.Key, IV);
                using (MemoryStream memoryStream = new MemoryStream(encryptedFileData))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(encryptedFileData, 16, encryptedFileData.Length - 16);
                        cryptoStream.FlushFinalBlock();
                        return memoryStream.ToArray();
                    }
                }
            }

        }
    }
}
