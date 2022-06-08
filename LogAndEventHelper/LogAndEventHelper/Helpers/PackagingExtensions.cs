using System;
using System.IO;
//using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Mew.AppLogAndEventHelper.Helpers
{
    public static class PackagingExtensions
    {
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        public static string Base64Encode(this string utf8_string, Encoding encoding = null)
        {
            encoding = encoding ?? DefaultEncoding;
            var to_encode_as_bytes = encoding.GetBytes(utf8_string);
            var return_value = Convert.ToBase64String(to_encode_as_bytes);
            return return_value;
        }

        public static string Base64Decode(this string base64_string, Encoding encoding = null)
        {
            if (!IsBase64String(base64_string))
                return base64_string;

            encoding = encoding ?? DefaultEncoding;

            var encoded_data_as_bytes = FromBase64String_Safe(base64_string);
            var return_value = encoding.GetString(encoded_data_as_bytes);
            return return_value;
        }

        public static byte[] Base64Decode_Array(this string base64_string)
        {
            if (!IsBase64String(base64_string))
                return Encoding.UTF8.GetBytes(base64_string);
            
            var encoded_data_as_bytes = FromBase64String_Safe(base64_string);
            return encoded_data_as_bytes;
        }

        //Regex.IsMatch(s, "^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$");
        public static bool IsBase64String(this string s) => Regex.IsMatch(s, "^([A-Za-z0-9+/=]+)$");

        private static byte[] FromBase64String_Safe(this string input)
        {
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, new FromBase64Transform(), CryptoStreamMode.Write))
            using (var tr = new StreamWriter(cs))
            {
                tr.Write(input);
                tr.Flush();
                return ms.ToArray();
            }
        }

        //public static string Serialize<T>(this T obj, Encoding encoding = null)
        //{
        //    if (obj == null)
        //        return "null";

        //    encoding = encoding ?? DefaultEncoding;
        //    var serializer = new DataContractJsonSerializer(obj.GetType());
        //    var ms = new MemoryStream();
        //    serializer.WriteObject(ms, obj);
        //    var ret_val = encoding.GetString(ms.ToArray());
        //    return ret_val;
        //}

        //public static T Deserialize<T>(this string json, Encoding encoding = null)
        //{
        //    encoding = encoding ?? DefaultEncoding;
        //    var obj = Activator.CreateInstance<T>();
        //    var ms = new MemoryStream(encoding.GetBytes(json));
        //    var serializer = new DataContractJsonSerializer(obj.GetType());
        //    obj = (T) serializer.ReadObject(ms);
        //    ms.Close();
        //    return obj;
        //}

        public static string GetMD5HashFromFile(this FileInfo file_info)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(file_info.FullName))
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
            }
        }
    }
    //public class SerializeToString {
    //    public override string ToString() => this.Serialize();
    //}
}