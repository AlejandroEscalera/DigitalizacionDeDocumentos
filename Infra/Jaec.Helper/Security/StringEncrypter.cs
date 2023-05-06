using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace System.Security;

public static class StringEncrypter
{
    // Esta clave secreta se utiliza para cifrar y descifrar la cadena.
    // Asegúrate de cambiarla por una clave secreta segura.
    const string C_STR_COMPLEMENTO = "12324/11/1972089";
    private static readonly byte[] key = Encoding.UTF8.GetBytes(((new DriveInfo("C:").VolumeLabel??"") + Environment.GetEnvironmentVariable("COMPUTERNAME").ReverseString() + C_STR_COMPLEMENTO)[..15]); // "EstaEsUnaClaveSecreta"

    public static string ReverseString(this string? stringAInvertir)
    {
        if (string.IsNullOrEmpty(stringAInvertir))
        {
            stringAInvertir = C_STR_COMPLEMENTO;
        }
        char[] charArray = stringAInvertir.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    public static SecureString ToSecureString(this string seguro)
    {
        SecureString ssPass = new();
        foreach (char c in seguro.ToCharArray())
        {
            ssPass.AppendChar(c);

        }
        return ssPass;
    }

    private static string FromSecureString(this SecureString stringSeguro)
    {
        return new NetworkCredential("", stringSeguro).Password;
    }

    public static SecureString Encrypt(this SecureString textoSeguro)
    {
        string text = textoSeguro.FromSecureString();

        // Convierte el texto en una matriz de bytes.
        byte[] textBytes = Encoding.UTF8.GetBytes(text);

        // Crea un objeto de cifrado AES.
        using Aes aes = Aes.Create();
        // Configura la clave y el vector de inicialización.
        aes.Key = key;
        aes.IV = new byte[aes.BlockSize / 8];

        // Crea un objeto de cifrado.
        ICryptoTransform encryptor = aes.CreateEncryptor();

        // Cifra la matriz de bytes.
        byte[] encryptedBytes = encryptor.TransformFinalBlock(textBytes, 0, textBytes.Length);

        // Convierte la matriz de bytes cifrada en una cadena base64.
        return Convert.ToBase64String(encryptedBytes).ToSecureString();
    }

    public static SecureString Decrypt(this SecureString textoSeguro)
    {
        string encryptedText = textoSeguro.FromSecureString();
        // Convierte la cadena base64 en una matriz de bytes cifrada.
        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

        // Crea un objeto de cifrado AES.
        using Aes aes = Aes.Create();
        // Configura la clave y el vector de inicialización.
        aes.Key = key;
        aes.IV = new byte[aes.BlockSize / 8];

        // Crea un objeto de descifrado.
        ICryptoTransform decryptor = aes.CreateDecryptor();

        // Descifra la matriz de bytes cifrada.
        byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

        // Convierte la matriz de bytes descifrada en una cadena.
        return Encoding.UTF8.GetString(decryptedBytes).ToSecureString();
    }
}
