using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class CEncryptDecrpt
    {
        //Initialized Key
        byte[] key = { };
        //Initilized Digest
        byte[] IV = { 0X12, 0X34, 0X56, 0X78, 0X90, 0XAB, 0XCD, 0XEF };
        //Define Encryption Key
        string mstrEncryptionKey = "1234567890";
        //If name of the documeny is Requied

        #region Encrypt
        public string FalconEncrypt(string stringToEncrypt)
        {
            return TripleDESEncode(Encrypt(stringToEncrypt));
        }
        #endregion

        #region Dcrypt
        public string FalconDcrypt(string stringToEncrypt)
        {
            return Decrypt(TripleDESDecode(stringToEncrypt));
        }
        #endregion

        #region DESCrypto
        public string Encrypt(string stringToEncrypt)
        {
            try
            {
                string SEncryptionKey = mstrEncryptionKey;
                key = Encoding.UTF8.GetBytes(SEncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                //Convert.ToBase64String(ms.ToArray()).Replace('=', '@');
                return Convert.ToBase64String(ms.ToArray()).Replace('=', '@').Replace("+", "~!");
                //return Convert.ToBase64String(ms.ToArray()).Replace('=', '@');

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
            }

        }
        #endregion

        #region DESDrypto
        public string Decrypt(string stringToDecrypt)
        {
            string SEncryptionKey = mstrEncryptionKey;
            byte[] inputByteArray = new byte[stringToDecrypt.Length];
            // inputByteArray.Length = stringToDecrypt.Length;
            // inputByteArray.Length = stringToDecrypt.Length;
            try
            {
                key = Encoding.UTF8.GetBytes(SEncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(stringToDecrypt.Replace('@', '=').Replace("~!", "+"));
                //inputByteArray = Convert.FromBase64String(stringToDecrypt.Replace('@', '='));
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                Encoding encoding = Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
            }

        }
        #endregion

        #region TripleEncoding
        public string TripleDESEncode(string value)
        {

            string key = "a1B@c3D$";
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.IV = new byte[8];
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(key, new byte[-1 + 1]);
            des.Key = pdb.CryptDeriveKey("RC2", "MD5", 128, new byte[8]);
            MemoryStream ms = new MemoryStream((value.Length * 2) - 1);
            CryptoStream encStream = new CryptoStream(ms, des.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write);
            byte[] plainBytes = Encoding.UTF8.GetBytes(value);
            encStream.Write(plainBytes, 0, plainBytes.Length);
            encStream.FlushFinalBlock();
            byte[] encryptedBytes = new byte[(int)ms.Length - 1 + 1];
            ms.Position = 0;
            ms.Read(encryptedBytes, 0, (int)ms.Length);
            encStream.Close();
            return Convert.ToBase64String(encryptedBytes);
        }

        public string TripleDESDecode(string value)
        {

            string key = "a1B@c3D$";
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.IV = new byte[8];
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(key, new byte[-1 + 1]);
            des.Key = pdb.CryptDeriveKey("RC2", "MD5", 128, new byte[8]);
            byte[] encryptedBytes = Convert.FromBase64String(value);
            MemoryStream ms = new MemoryStream(value.Length);
            CryptoStream decStream = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            decStream.Write(encryptedBytes, 0, encryptedBytes.Length);
            decStream.FlushFinalBlock();
            byte[] plainBytes = new byte[(int)ms.Length - 1 + 1];
            ms.Position = 0;
            ms.Read(plainBytes, 0, (int)ms.Length);
            decStream.Close();
            return Encoding.UTF8.GetString(plainBytes);
        }

        #endregion

    }

}
