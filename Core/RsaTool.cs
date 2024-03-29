﻿using System;
using System.IO;
using Microsoft.Win32;
using System.Security.Cryptography;

namespace Core {
    public class RsaTool {
        /// <summary>
        /// Generates Public and Private Keys if doesn't exist.
        /// </summary>
        public void GenerateKeys() {
            using (var rsa = new RSACryptoServiceProvider(2048)) {
                rsa.PersistKeyInCsp = false;

                var pubKey = rsa.ExportParameters(false);
                var privKey = rsa.ExportParameters(true);

                string pubKeyString; {
                    var sw = new System.IO.StringWriter();
                    var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                    xs.Serialize(sw, pubKey);
                    pubKeyString = sw.ToString();
                }

                string privKeyString; {
                    var sw = new System.IO.StringWriter();
                    var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                    xs.Serialize(sw, privKey);
                    privKeyString = sw.ToString();
                }

                SaveFileDialog PrivateSfd = new SaveFileDialog();
                PrivateSfd.FileName = "private";
                PrivateSfd.DefaultExt = ".key";
                PrivateSfd.Title = "Choose the path for your private.key file";
                if (PrivateSfd.ShowDialog() == true) {
                    File.WriteAllText(PrivateSfd.FileName, privKeyString);
                }

                SaveFileDialog PublicSfd = new SaveFileDialog();
                PublicSfd.FileName = "public";
                PublicSfd.DefaultExt = ".cert"; 
                PublicSfd.Title = "Choose the path for your public.cert file";
                if (PublicSfd.ShowDialog() == true) {
                    File.WriteAllText(PublicSfd.FileName, pubKeyString);
                }
                
            }
        }

        /// <summary>
        /// Reads Public Key Generated
        /// </summary>
        /// <param name="publicKeyFile">Public Key File Path</param>
        /// <returns>Public Key in XML Format</returns>
        public RSAParameters ReadPublicKey(string publicKeyFile) {
            string pubKeyString = File.ReadAllText(publicKeyFile);
            var sr = new StringReader(pubKeyString);
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            return (RSAParameters)xs.Deserialize(sr);
        }

        /// <summary>
        /// Reads Private Key Generated
        /// </summary>
        /// <param name="privateKeyFile">Private Key File Path</param>
        /// <returns>Private Key in XML Format</returns>
        public RSAParameters ReadPrivateKey(string privateKeyFile) {
            string privKeyString = File.ReadAllText(privateKeyFile);
            var sr = new StringReader(privKeyString);
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            return (RSAParameters)xs.Deserialize(sr);
        }

        /// <summary>
        /// Encrypts a String
        /// </summary>
        /// <param name="publicKeyFile">Public Key File Path</param>
        /// <param name="textToEncrypt">Text to be Encrypted</param>
        /// <returns>Encrypted String</returns>
        public string Encrypt(string publicKeyFile, string textToEncrypt) {
            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(ReadPublicKey(publicKeyFile));
            var bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(textToEncrypt);
            var bytesCypherText = csp.Encrypt(bytesPlainTextData, true);
            return Convert.ToBase64String(bytesCypherText);
        }

        /// <summary>
        /// Decrypts a String
        /// </summary>
        /// <param name="privateKeyFile">Private Key File Path to be Used</param>
        /// <param name="textToDecrypt">Text to be Decrypted</param>
        /// <returns>Decrypted Text</returns>
        public string Decrypt(string privateKeyFile, string textToDecrypt) {
            var bytesCypherText = Convert.FromBase64String(textToDecrypt);
            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(ReadPrivateKey(privateKeyFile));
            var bytesPlainTextData = csp.Decrypt(bytesCypherText, true);
            return System.Text.Encoding.Unicode.GetString(bytesPlainTextData);
        }
    }
}
