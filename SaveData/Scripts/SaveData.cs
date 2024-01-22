using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;

class Data<T>
{
    public string DeviceID;
    public T obj;
    public Data(T obj0)
    {
        DeviceID = SystemInfo.deviceUniqueIdentifier;
        obj = obj0;
    }
}


public class SD
{
    public static bool Exists(string key)
    {
        string HashKey = EncryptStringUsingSHA256(key);
        string HashFile = EncryptStringUsingMD5(key);

        if (File.Exists(Application.persistentDataPath + "/" + HashFile) && PlayerPrefs.HasKey(HashKey))
        {
            return true;
        }
        return false;
    }

    public static void Delete(string key)
    {
        string HashKey = EncryptStringUsingSHA256(key);
        string HashFile = EncryptStringUsingMD5(key);

        if (File.Exists(Application.persistentDataPath + "/" + HashFile))
        {
            File.Delete(Application.persistentDataPath + "/" + HashFile);
        }
        if (PlayerPrefs.HasKey(HashKey))
        {
            PlayerPrefs.DeleteKey(HashKey);
        }
    }

    public static T Load<T>(string key)
    {
        string text = "{}";

        string HashKey = EncryptStringUsingSHA256(key);
        string HashFile = EncryptStringUsingMD5(key);

        if (File.Exists(Application.persistentDataPath + "/" + HashFile) && PlayerPrefs.HasKey(HashKey))
        {
            FileStream dataStream;

            byte[] ByteKey = Convert.FromBase64String(PlayerPrefs.GetString(HashKey));

            dataStream = new FileStream(Application.persistentDataPath + "/" + HashFile, FileMode.Open);

            Aes oAes = Aes.Create();

            byte[] outputIV = new byte[oAes.IV.Length];

            dataStream.Read(outputIV, 0, outputIV.Length);

            CryptoStream oStream = new CryptoStream(
                   dataStream,
                   oAes.CreateDecryptor(ByteKey, outputIV),
                   CryptoStreamMode.Read);

            StreamReader reader = new StreamReader(oStream);

            text = reader.ReadToEnd();

            reader.Close();
        }
        Data<T> data = JsonConvert.DeserializeObject<Data<T>>(text);
        if(data.DeviceID == SystemInfo.deviceUniqueIdentifier)
        {
            return data.obj;
        }
        else
        {
            Delete(key);
        }
        text = "{}";
        return JsonConvert.DeserializeObject<T>(text);
    }

    public static void Save<T>(string key, T obj)
    {
        Aes iAes = Aes.Create();

        byte[] ByteKey = GenerateRandomKey(iAes.KeySize / 8);

        string HashKey = EncryptStringUsingSHA256(key);
        string HashFile = EncryptStringUsingMD5(key);

        PlayerPrefs.SetString(HashKey, Convert.ToBase64String(ByteKey));

        iAes.Key = ByteKey;

        FileStream dataStream;

        dataStream = new FileStream(Application.persistentDataPath + "/" + HashFile, FileMode.Create);

        byte[] inputIV = iAes.IV;

        dataStream.Write(inputIV, 0, inputIV.Length);

        CryptoStream iStream = new CryptoStream(
                dataStream,
                iAes.CreateEncryptor(iAes.Key, iAes.IV),
                CryptoStreamMode.Write);

        StreamWriter sWriter = new StreamWriter(iStream);

        Data<T> data = new Data<T>(obj);

        string jsonString = JsonConvert.SerializeObject(data);

        sWriter.Write(jsonString);

        sWriter.Close();

        iStream.Close();

        dataStream.Close();
    }

    static byte[] GenerateRandomKey(int keySizeInBytes)
    {
        using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
        {
            byte[] randomKey = new byte[keySizeInBytes];
            rngCsp.GetBytes(randomKey);
            return randomKey;
        }
    }
    static string EncryptStringUsingMD5(string inputString)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(inputString));
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }

    static string EncryptStringUsingSHA256(string inputString)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputString));
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }
}