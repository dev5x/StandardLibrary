using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

/* dev5x.com (c) 2020
 * 
 * Encryption Class
 * Dencypt and decrypt strings
*/

namespace dev5x.StandardLibrary
{
    public class Encryption : BaseClass
    {
        private const string _pwd = "5kb2sw@rb.#!";
        private const string _encryptID = "=";

        #region Private Methods
        private string EncryptString(string InputText)
        {
            try
            {
                byte[] CipherBytes = null;
                // Convert the input strings into a byte array.
                byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(InputText);
                // Use salt to make it harder to guess our key using a dictionary attack.
                byte[] Salt = Encoding.ASCII.GetBytes(_pwd.Length.ToString());

                // Create an instance of the Rihndael class. 
                using (RijndaelManaged RijndaelCipher = new RijndaelManaged())
                {
                    // The (Secret Key) will be generated from the specified password and salt.
                    using (PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(_pwd, Salt))
                    {
                        // Create a encryptor from the existing SecretKey bytes.
                        // We use 32 bytes for the secret key (the default Rijndael key length is 256 bit = 32 bytes) and
                        // then 16 bytes for the IV (initialization vector), (the default Rijndael IV length is 128 bit = 16 bytes)
                        ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

                        // Create a MemoryStream to hold the encrypted bytes
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            // Create a CryptoStream through which we are going to be processing our data.
                            // CryptoStreamMode.Write means that we are going to be writing data
                            // to the stream and the output will be written in the MemoryStream
                            // we have provided. (always use write mode for encryption)
                            CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);
                            {
                                // Start the encryption process.
                                cryptoStream.Write(PlainText, 0, PlainText.Length);

                                // Finish encrypting.
                                cryptoStream.FlushFinalBlock();
                            }

                            // Convert our encrypted data from a memoryStream into a byte array.
                            CipherBytes = memoryStream.ToArray();
                        }
                    }
                }

                // Convert encrypted data into a base64-encoded string.
                // A common mistake would be to use an Encoding class for that.
                // It does not work, because not all byte values can be
                // represented by characters. We are going to be using Base64 encoding
                // That is designed exactly for what we are trying to do.
                string EncryptedData = Convert.ToBase64String(CipherBytes);

                // Return encrypted string.
                return EncryptedData;
            }
            catch (Exception ex)
            {
                SetErrorMessage("EncryptString - " + ex.Message);
                return string.Empty;
            }
        }

        private string DecryptString(string InputText)
        {
            try
            {
                byte[] PlainText = null;
                int DecryptedCount = 0;
                byte[] EncryptedData = Convert.FromBase64String(InputText);
                byte[] Salt = Encoding.ASCII.GetBytes(_pwd.Length.ToString());

                using (RijndaelManaged RijndaelCipher = new RijndaelManaged())
                {
                    using (PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(_pwd, Salt))
                    {
                        // Create a decryptor from the existing SecretKey bytes.
                        ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
                        using (MemoryStream memoryStream = new MemoryStream(EncryptedData))
                        {
                            // Create a CryptoStream. (always use Read mode for decryption).
                            CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);
                            // Since at this point we don't know what the size of decrypted data
                            // will be, allocate the buffer long enough to hold EncryptedData;
                            // DecryptedData is never longer than EncryptedData.
                            PlainText = new byte[EncryptedData.Length];

                            // Start decrypting.
                            DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);
                        }
                    }
                }

                // Convert decrypted data into a string.
                string DecryptedData = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);

                // Return decrypted string.  
                return DecryptedData;
            }
            catch (Exception ex)
            {
                SetErrorMessage("DecryptString - " + ex.Message);
                return string.Empty;
            }
        }
        #endregion

        #region Public Methods
        public string Encrypt(string Value)
        {
            // Encrypt string
            try
            {
                return EncryptString(Value);
            }
            catch (Exception ex)
            {
                SetErrorMessage("Encrypt - " + ex.Message);
                return string.Empty;
            }
        }

        public string Decrypt(string Value)
        {
            // Decrypt string
            try
            {
                if (Value.EndsWith(_encryptID) && Value.Length > 23)
                {
                    return DecryptString(Value);
                }
                else
                {
                    return Value;
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage("Decrypt - " + ex.Message);
                return string.Empty;
            }
        }
        #endregion
    }
}