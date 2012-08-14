using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Services.Configuration;

namespace MS.Katusha.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly ICryptoTransform _encryptor;
        private readonly ICryptoTransform _decryptor;

        public EncryptionService()
        {
            var encryptionData = KatushaConfigurationManager.Instance.GetEncryption();
            var initVectorBytes = Encoding.ASCII.GetBytes(encryptionData.InitVector);
            var saltValueBytes = Encoding.ASCII.GetBytes(encryptionData.SaltValue);
            var password = new PasswordDeriveBytes(encryptionData.PassPhrase, saltValueBytes, encryptionData.HashAlgorithm, encryptionData.PasswordIterations);
            var keyBytes = password.GetBytes(encryptionData.KeySize / 8);
            var symmetricKey = new RijndaelManaged { Mode = CipherMode.CBC };
            _encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            _decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
        }

        public string Encrypt(string plainText) { 
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using(var memoryStream = new MemoryStream()) {
                using(var cryptoStream = new CryptoStream(memoryStream, _encryptor, CryptoStreamMode.Write)) {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    var cipherTextBytes = memoryStream.ToArray();
                    var cipherText = Convert.ToBase64String(cipherTextBytes);
                    return cipherText;
                }
            }
        }

        public string Decrypt(string cipherText)
        {
            var cipherTextBytes = Convert.FromBase64String(cipherText);
            using (var memoryStream = new MemoryStream(cipherTextBytes)) {
                using (var cryptoStream = new CryptoStream(memoryStream, _decryptor, CryptoStreamMode.Read)) {
                    var plainTextBytes = new byte[cipherTextBytes.Length];
                    var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                    var plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                    return plainText;
                }
            }
        }
    }
}