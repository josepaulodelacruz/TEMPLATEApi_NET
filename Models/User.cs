using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;

namespace TemplateAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email adress")]
        public string Email { get; set; } = "";

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; } = "";

        public string EncryptPassword (string password)
        {
            string _password = password;
            byte[] data = Encoding.UTF8.GetBytes(_password);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(_password));
                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.Mode = CipherMode.ECB;
                    aes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform transform = aes.CreateEncryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    return Convert.ToBase64String(results, 0, results.Length);
                }
            }
        }

    }
}
