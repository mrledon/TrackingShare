using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core
{
    public static class UtilMethods
    {
        public static string CreateHashString(string Password, string Salt)
        {
            System.Security.Cryptography.SHA512Managed HashTool = new System.Security.Cryptography.SHA512Managed();
            Byte[] PasswordAsByte = System.Text.Encoding.UTF8.GetBytes(string.Concat(Password, Salt));
            Byte[] EncryptedBytes = HashTool.ComputeHash(PasswordAsByte);
            HashTool.Clear();
            return Convert.ToBase64String(EncryptedBytes);
        }
        public static T ConvertToObject<T>(this object val)
        {
            try
            {
                if (val == null || val.ToString().Length == 0)
                    return (T)Convert.ChangeType(null, typeof(T));
                if (typeof(T) == typeof(System.DateTime))
                    return (T)Convert.ChangeType(Convert.ToDateTime(val), typeof(T));

                var t = typeof(T);
                if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    if (val == null)
                        return default(T);

                    t = Nullable.GetUnderlyingType(t);
                }
                return (T)Convert.ChangeType(val, t);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public static DateTime ConvertToDateTime(this object val, string format = "")
        {
            try
            {
                if (val != null && val.ToString().Length > 0 && format != null && format.Length > 0)
                    return DateTime.ParseExact(val.ToString(), format, System.Globalization.CultureInfo.InvariantCulture);
                return val == null ? default(DateTime) : Convert.ToDateTime(val);
            }
            catch
            {
                return default(DateTime);
            }
        }
        public static DateTime? ConvertToDateTime_Null(this object val, string format = "")
        {
            try
            {
                if (val != null && val.ToString().Length > 0 && format != null && format.Length > 0)
                    return DateTime.ParseExact(val.ToString(), format, System.Globalization.CultureInfo.InvariantCulture);
                return val == null ? default(DateTime?) : Convert.ToDateTime(val);
            }
            catch
            {
                return default(DateTime?);
            }
        }
        public static Boolean? ConvertToBool(this object val)
        {
            try
            {
                return val == null ? default(Boolean?) : Convert.ToBoolean(val);
            }
            catch
            {
                return default(Boolean?);
            }
        }

        public static IEnumerable<FileSystemInfo> AllFilesAndFolders(this DirectoryInfo dir)
        {
            foreach (var f in dir.GetFiles())
                yield return f;
            foreach (var d in dir.GetDirectories())
            {
                yield return d;
                foreach (var o in AllFilesAndFolders(d))
                    yield return o;
            }
        }
    }
}
