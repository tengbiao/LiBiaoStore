using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace TSMember.Common
{
    /// <summary>
    /// MD5加密
    /// </summary>
    public sealed class MD5Helper
    {
        /// <summary>
        /// 32位MD5算法加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <param name="time">需要加密的次数</param>
        /// <returns>加密后的字符串</returns>
        public static string Md5Encryptor32(string str, int time)
        {
            do
            {
                str = Md5Encryptor32(str);
                time--;
            } while (time > 0);
            return str;
        }
        /// <summary>
        /// 32位MD5算法加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <param name="time">需要加密的次数</param>
        /// <param name="length">加密的长度32或16</param>
        /// <returns>加密后的字符串</returns>
        public static string Md5Encryptor32(string str, int time, int length)
        {
            do
            {
                if (length == 32)
                {
                    str = Md5Encryptor32(str);
                }
                else
                {
                    str = Md5Encryptor16(str);
                }
                time--;
            } while (time > 0);
            return str;
        }
        /// <summary>
        /// 32位MD5算法加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <param name="lower">小写还是大写（默认小写）</param>
        /// <returns>加密后的字符串</returns>
        public static string Md5Encryptor32(string str, bool lower = true)
        {
            string password = "";
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            foreach (byte b in s)
                password += b.ToString("X2");
            return lower ? password.ToLower() : password.ToUpper();
        }
        /// <summary>
        /// 16位MD5算法加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <param name="lower">小写还是大写（默认小写）</param>
        /// <returns>加密后的字符串</returns>
        public static string Md5Encryptor16(string str, bool lower = true)
        {
            string password = "";
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            password = BitConverter.ToString(s, 4, 8).Replace("-", "");
            return lower ? password.ToLower() : password.ToUpper();
        }
    }
    /// <summary>
    /// DES 加解密
    /// </summary>
    public sealed class DESHelper
    {
        private const string defaultKey = "dkfja*7&^%d";
        #region Des 加解密
        /// <summary>
        /// 加密原函数
        /// </summary>
        /// <param name="pToEncrypt">加密前的明文</param>
        /// <param name="sKey">Key</param>
        /// <returns>返回加密后的密文</returns>
        public static string Encrypt(string pToEncrypt, string sKey = defaultKey)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
                des.Key = Encoding.ASCII.GetBytes(MD5Helper.Md5Encryptor16(sKey).Substring(0, 8));
                des.IV = Encoding.ASCII.GetBytes(MD5Helper.Md5Encryptor16(sKey).Substring(0, 8));
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();
                foreach (byte b in ms.ToArray())
                    ret.AppendFormat("{0:X2}", b);
                ret.ToString();
                return ret.ToString();
            }
            catch { return null; }
        }
        /// <summary>
        /// 解密原函数
        /// </summary>
        /// <param name="pToDecrypt">加密后的密文</param>
        /// <param name="sKey">Key</param>
        /// <returns>返回加密前的明文</returns>
        public static string Decrypt(string pToDecrypt, string sKey = defaultKey)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
                for (int x = 0; x < pToDecrypt.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }
                des.Key = Encoding.ASCII.GetBytes(MD5Helper.Md5Encryptor16(sKey).Substring(0, 8));
                des.IV = Encoding.ASCII.GetBytes(MD5Helper.Md5Encryptor16(sKey).Substring(0, 8));
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();
                return System.Text.Encoding.Default.GetString(ms.ToArray());
            }
            catch { return null; }
        }

        #endregion
    }
    /// <summary>
    /// Sha1
    /// </summary>
    public sealed class Sha1Helper
    {
        public static string GetSHA1(string str)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();

            //将str转换成byte[]
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] dataToHash = enc.GetBytes(str);

            //Hash运算
            byte[] dataHashed = sha.ComputeHash(dataToHash);

            //将运算结果转换成string
            string hash = BitConverter.ToString(dataHashed).Replace("-", "");

            return hash;
        }
    }
    /// <summary>
    /// 类名：RSAHelper
    /// 功能：RSA加密、解密、签名、验签
    /// </summary>
    public sealed class RSAHelper
    {
        private const string charSet = "UTF-8";

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="content">待签名字符串</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="input_charset">编码格式</param>
        /// <returns>签名后字符串</returns>
        public static string Sign(string content, string privateKey, string input_charset = charSet)
        {
            byte[] Data = Encoding.GetEncoding(input_charset).GetBytes(content);
            RSACryptoServiceProvider rsa = DecodePemPrivateKey(privateKey);
            SHA1 sh = new SHA1CryptoServiceProvider();
            byte[] signData = rsa.SignData(Data, sh);
            return Convert.ToBase64String(signData);
        }

        /// <summary>
        /// 验签
        /// </summary>
        /// <param name="content">待验签字符串</param>
        /// <param name="signedString">签名</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="input_charset">编码格式</param>
        /// <returns>true(通过)，false(不通过)</returns>
        public static bool Verify(string content, string signedString, string publicKey, string input_charset = charSet)
        {
            bool result = false;
            byte[] Data = Encoding.GetEncoding(input_charset).GetBytes(content);
            byte[] data = Convert.FromBase64String(signedString);
            RSAParameters paraPub = ConvertFromPublicKey(publicKey);
            RSACryptoServiceProvider rsaPub = new RSACryptoServiceProvider();
            rsaPub.ImportParameters(paraPub);
            SHA1 sh = new SHA1CryptoServiceProvider();
            result = rsaPub.VerifyData(Data, sh, data);
            return result;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="resData">需要加密的字符串</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="input_charset">编码格式</param>
        /// <returns>明文</returns>
        public static string EncryptData(string resData, string publicKey, string input_charset = charSet)
        {
            byte[] DataToEncrypt = Encoding.ASCII.GetBytes(resData);
            string result = Encrypt(DataToEncrypt, publicKey, input_charset);
            return result;
        }

        public static string RSAPublicKey(string path)
        {
            RsaKeyParameters publicKeyParam =
                (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(path));
            return string.Format(
                "<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned())
            );
        }

        /// <summary>
        /// rsa加密不限长度
        /// </summary>
        /// <param name="encryptInfo">明文</param>
        /// <param name="publicKey">公钥</param>
        /// <returns>密文</returns>   
        public static string RSAEncrypt(string encryptInfo, string publicKey)
        {
            try
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(encryptInfo);
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                RSAParameters para = new RSAParameters();
                rsa.FromXmlString(publicKey);
                int keySize = rsa.KeySize / 8;
                int bufferSize = keySize - 11;
                byte[] buffer = new byte[bufferSize];
                MemoryStream msInput = new MemoryStream(dataBytes);
                MemoryStream msOutput = new MemoryStream();
                int readLen = msInput.Read(buffer, 0, bufferSize);
                while (readLen > 0)
                {
                    byte[] dataToEnc = new byte[readLen];
                    Array.Copy(buffer, 0, dataToEnc, 0, readLen);
                    byte[] encData = rsa.Encrypt(dataToEnc, false);
                    msOutput.Write(encData, 0, encData.Length);
                    readLen = msInput.Read(buffer, 0, bufferSize);
                }
                msInput.Close();
                byte[] result = msOutput.ToArray();    //得到加密结果
                msOutput.Close();
                rsa.Clear();
                return Convert.ToBase64String(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="resData">加密字符串</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="input_charset">编码格式</param>
        /// <returns>明文</returns>
        public static string DecryptData(string resData, string privateKey, string input_charset = charSet)
        {
            byte[] DataToDecrypt = Convert.FromBase64String(resData);
            string result = "";
            for (int j = 0; j < DataToDecrypt.Length / 128; j++)
            {
                byte[] buf = new byte[128];
                for (int i = 0; i < 128; i++)
                {
                    buf[i] = DataToDecrypt[i + 128 * j];
                }
                result += Decrypt(buf, privateKey, input_charset);
            }
            return result;
        }

        /// <summary>
        /// RSA加密的私钥和公钥 从 /Xml/RSAKey.xml 文件中读取
        /// </summary>
        public class RSAKeyInfo
        {
            public static string PrivateKey { set; get; }
            public static string PublicKey { set; get; }
            static RSAKeyInfo()
            {
                PrivateKey = "MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBALwvY1UFE0FOi1Ah1UolVHvDQXQ4WqCsxkzwuG8TEKqw0kWKy1k8q+btyl217lIpe6uGb5L+9O1rtEgJ/2BtmVbNyzuAnJe7yo79MjIm9B60rOSB27SH+PIBI4dSYGLL4pWN6sKrkLXh+x32yS+TMcWp0RsBdQP6k8hqe+MOuNaFAgMBAAECgYBFL+sGVCJbLWR85qODaiwggI4tC5cOYddabFpyxACpsO7uEHRo76yH778qKvxfCs9kJb4ZvlEQhTu4DKzup/zq0pkwiljBTfFq5R43tScoid3Jz8Qde3nv7k2/Lwn2T65DD5ztpt/n9vKivsd40RnYTV7Lu+RyMJF07iyqsh2ItQJBAOWq2WYXnU6ceBuLSmgDwyqUhTJspeN0U16eebyDVvu19E5ODvhI+MjALAN4mJhsroapyTMmLr3yrUSOsTORCa8CQQDRwvWkUUhXB9pmhaxfIKkLs4xU3SAildELyrPtNPNRDi63bQw6Bre+zD1sW9MDXmjsXIbIlFEpgj8zjTPosFQLAkAMfVoNP0OCvueZN934Ahxe+Gy17UqoL+9Iuf2MzuewEJkUmRIfVniREkJSfgBuaZqkIB+c9HqeQdBCZjAkycJpAkEAya08Bj+MYPkQ+E9/mnrJbZG01AdDFV2b/01mTyQA6SbMrdkvzz6UPeMbD2r96BhGozKW+JT5sAIliAg0J34ZXwJAWrjleKTNN+hG2Fl/0xyZLvnWjQk9tk1VilYzaBrtgcET8emvHYPnIGZdCDJRVh4IplYE4X5pPyhsb51Bqh0OaA==";
                PublicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC8L2NVBRNBTotQIdVKJVR7w0F0OFqgrMZM8LhvExCqsNJFistZPKvm7cpdte5SKXurhm+S/vTta7RICf9gbZlWzcs7gJyXu8qO/TIyJvQetKzkgdu0h/jyASOHUmBiy+KVjerCq5C14fsd9skvkzHFqdEbAXUD+pPIanvjDrjWhQIDAQAB";
                //System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                //doc.Load(HttpContext.Current.Server.MapPath("~/Xml/RSAKey.xml"));
                //PrivateKey = doc.SelectSingleNode("RSAKey/privateKey").InnerText;
                //PublicKey = doc.SelectSingleNode("RSAKey/publicKey").InnerText;
            }
        }

        #region 内部方法

        private static string Encrypt(byte[] data, string publicKey, string input_charset = charSet)
        {
            RSACryptoServiceProvider rsa = DecodePemPublicKey(publicKey);
            SHA1 sh = new SHA1CryptoServiceProvider();
            byte[] result = rsa.Encrypt(data, false);

            return Convert.ToBase64String(result);
        }

        private static string Decrypt(byte[] data, string privateKey, string input_charset = charSet)
        {
            string result = "";
            RSACryptoServiceProvider rsa = DecodePemPrivateKey(privateKey);
            SHA1 sh = new SHA1CryptoServiceProvider();
            byte[] source = rsa.Decrypt(data, false);
            char[] asciiChars = new char[Encoding.GetEncoding(input_charset).GetCharCount(source, 0, source.Length)];
            Encoding.GetEncoding(input_charset).GetChars(source, 0, source.Length, asciiChars, 0);
            result = new string(asciiChars);
            //result = ASCIIEncoding.ASCII.GetString(source);
            return result;
        }

        private static RSACryptoServiceProvider DecodePemPublicKey(String pemstr)
        {
            byte[] pkcs8publickkey;
            pkcs8publickkey = Convert.FromBase64String(pemstr);
            if (pkcs8publickkey != null)
            {
                RSACryptoServiceProvider rsa = DecodeRSAPublicKey(pkcs8publickkey);
                return rsa;
            }
            else
                return null;
        }

        private static RSACryptoServiceProvider DecodePemPrivateKey(String pemstr)
        {
            byte[] pkcs8privatekey;
            pkcs8privatekey = Convert.FromBase64String(pemstr);
            if (pkcs8privatekey != null)
            {
                RSACryptoServiceProvider rsa = DecodePrivateKeyInfo(pkcs8privatekey);
                return rsa;
            }
            else
                return null;
        }

        private static RSACryptoServiceProvider DecodePrivateKeyInfo(byte[] pkcs8)
        {
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];

            MemoryStream mem = new MemoryStream(pkcs8);
            int lenstream = (int)mem.Length;
            BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;

            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)    //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();    //advance 2 bytes
                else
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x02)
                    return null;

                twobytes = binr.ReadUInt16();

                if (twobytes != 0x0001)
                    return null;

                seq = binr.ReadBytes(15);        //read the Sequence OID
                if (!CompareBytearrays(seq, SeqOID))    //make sure Sequence for OID is correct
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x04)    //expect an Octet string
                    return null;

                bt = binr.ReadByte();        //read next byte, or next 2 bytes is  0x81 or 0x82; otherwise bt is the byte count
                if (bt == 0x81)
                    binr.ReadByte();
                else
                    if (bt == 0x82)
                    binr.ReadUInt16();
                //------ at this stage, the remaining sequence should be the RSA private key

                byte[] rsaprivkey = binr.ReadBytes((int)(lenstream - mem.Position));
                RSACryptoServiceProvider rsacsp = DecodeRSAPrivateKey(rsaprivkey);
                return rsacsp;
            }

            catch (Exception)
            {
                return null;
            }

            finally { binr.Close(); }

        }

        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        private static RSACryptoServiceProvider DecodeRSAPublicKey(byte[] publickey)
        {
            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];
            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            MemoryStream mem = new MemoryStream(publickey);
            BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;

            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                seq = binr.ReadBytes(15);       //read the Sequence OID
                if (!CompareBytearrays(seq, SeqOID))    //make sure Sequence for OID is correct
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8203)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x00)     //expect null byte next
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                byte lowbyte = 0x00;
                byte highbyte = 0x00;

                if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
                    lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
                else if (twobytes == 0x8202)
                {
                    highbyte = binr.ReadByte(); //advance 2 bytes
                    lowbyte = binr.ReadByte();
                }
                else
                    return null;
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
                int modsize = BitConverter.ToInt32(modint, 0);

                byte firstbyte = binr.ReadByte();
                binr.BaseStream.Seek(-1, SeekOrigin.Current);

                if (firstbyte == 0x00)
                {   //if first byte (highest order) of modulus is zero, don't include it
                    binr.ReadByte();    //skip this null byte
                    modsize -= 1;   //reduce modulus buffer size by 1
                }

                byte[] modulus = binr.ReadBytes(modsize);   //read the modulus bytes

                if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data
                    return null;
                int expbytes = (int)binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
                byte[] exponent = binr.ReadBytes(expbytes);

                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAKeyInfo = new RSAParameters();
                RSAKeyInfo.Modulus = modulus;
                RSAKeyInfo.Exponent = exponent;
                RSA.ImportParameters(RSAKeyInfo);
                return RSA;
            }
            catch (Exception)
            {
                return null;
            }

            finally { binr.Close(); }

        }

        private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            // ---------  Set up stream to decode the asn.1 encoded RSA private key  ------
            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)    //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();    //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)    //version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;


                //------  all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);

                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch (Exception)
            {
                return null;
            }
            finally { binr.Close(); }
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)        //expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();    // data size in next byte
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte();    // data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;        // we already have the data size
            }

            while (binr.ReadByte() == 0x00)
            {    //remove high order zeros in data
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);        //last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }

        #endregion

        #region 解析.net 生成的Pem
        private static RSAParameters ConvertFromPublicKey(string pemFileConent)
        {

            byte[] keyData = Convert.FromBase64String(pemFileConent);
            if (keyData.Length < 162)
            {
                throw new ArgumentException("pem file content is incorrect.");
            }
            byte[] pemModulus = new byte[128];
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, 29, pemModulus, 0, 128);
            Array.Copy(keyData, 159, pemPublicExponent, 0, 3);
            RSAParameters para = new RSAParameters();
            para.Modulus = pemModulus;
            para.Exponent = pemPublicExponent;
            return para;
        }

        private static RSAParameters ConvertFromPrivateKey(string pemFileConent)
        {
            byte[] keyData = Convert.FromBase64String(pemFileConent);
            if (keyData.Length < 609)
            {
                throw new ArgumentException("pem file content is incorrect.");
            }

            int index = 11;
            byte[] pemModulus = new byte[128];
            Array.Copy(keyData, index, pemModulus, 0, 128);

            index += 128;
            index += 2;//141
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, index, pemPublicExponent, 0, 3);

            index += 3;
            index += 4;//148
            byte[] pemPrivateExponent = new byte[128];
            Array.Copy(keyData, index, pemPrivateExponent, 0, 128);

            index += 128;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//279
            byte[] pemPrime1 = new byte[64];
            Array.Copy(keyData, index, pemPrime1, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//346
            byte[] pemPrime2 = new byte[64];
            Array.Copy(keyData, index, pemPrime2, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//412/413
            byte[] pemExponent1 = new byte[64];
            Array.Copy(keyData, index, pemExponent1, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//479/480
            byte[] pemExponent2 = new byte[64];
            Array.Copy(keyData, index, pemExponent2, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//545/546
            byte[] pemCoefficient = new byte[64];
            Array.Copy(keyData, index, pemCoefficient, 0, 64);

            RSAParameters para = new RSAParameters();
            para.Modulus = pemModulus;
            para.Exponent = pemPublicExponent;
            para.D = pemPrivateExponent;
            para.P = pemPrime1;
            para.Q = pemPrime2;
            para.DP = pemExponent1;
            para.DQ = pemExponent2;
            para.InverseQ = pemCoefficient;
            return para;
        }
        #endregion
    }

}