using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

/* dev5x.com (c) 2012
 * 
 * Encryption Class
 * Dencypt and decrypt strings
 * Updated encryption to handle long strings 8/2023
*/

namespace dev5x.StandardLibrary
{
    public sealed class Encryption : BaseClass
    {
        private const string _pwd = "@rbr#!";
        private readonly byte[] _salt = new byte[] { 0x49, 0x51, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 };

        #region Public Methods
        public string Encrypt(string ClearText)
        {
            try
            {
                string cipherText = string.Empty;
                byte[] clearBytes = Encoding.Unicode.GetBytes(ClearText);

                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(_pwd, _salt);
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                        }
                        cipherText = Convert.ToBase64String(ms.ToArray());
                    }
                }

                return cipherText;
            }
            catch (Exception ex)
            {
                SetErrorMessage("Encrypt - " + ex.Message);
                return ClearText;
            }
        }

        public string Decrypt(string CipherText)
        {
            try
            {
                string clearText = string.Empty;
                byte[] cipherBytes = Convert.FromBase64String(CipherText.Replace(" ", "+"));

                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(_pwd, _salt);
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                        }
                        clearText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }

                return clearText;
            }
            catch (Exception ex)
            {
                SetErrorMessage("Decrypt - " + ex.Message);
                return CipherText;
            }
        }
        #endregion
    }
}
