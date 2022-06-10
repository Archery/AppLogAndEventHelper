using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Mew.Helpers;

namespace Mew {
    /// <summary>
    ///     Класс для использования пользовательских форматов
    /// </summary>
    public class Formatter {
        public delegate string ObjectFormatter(object o);

        private static volatile Formatter instance_;
        private static readonly object syncRoot_ = new object();
        private readonly Dictionary<Type, ObjectFormatter> formattingRules_;

        public static Formatter Instance {
            get {
                if (instance_ != null) return instance_;
                lock (syncRoot_) {
                    if (instance_ == null) instance_ = new Formatter();
                    }

                return instance_;
            }
        }

        private Formatter() {
            this.formattingRules_ = new Dictionary<Type, ObjectFormatter>();
            this.AddRule(typeof(string), StringF);
            this.AddRule(typeof(object), StringF);
            this.AddRule(typeof(string[]), StringArrayF);
            this.AddRule(typeof(List<string>), ListStringArrayF);
            this.AddRule(typeof(Exception), ExceptionF);
        }

        public void AddRule(Type type, ObjectFormatter f) {
            if (this.formattingRules_.ContainsKey(type)) this.formattingRules_.Remove(type);
            this.formattingRules_.Add(type, f);
        }

        public string Fo(object o) {
            if (o == null) return "null";

            var type = o.GetType();
            
            //if(type.IsArray)
            while (type != null) {
                if (this.formattingRules_.ContainsKey(type)) {
                    this.formattingRules_.TryGetValue(type, out var f);
                    Debug.Assert(f != null, "f != null");
                    return f(o);
                }

                if (type.GetInterface(nameof(IEnumerable)) != null) return IEnumerableF((IEnumerable) o);
                //if (type.IsSerializable)
                //    return o.Serialize();
                type = type.BaseType;
            }

            return o.ToString();
        }

        #region Formater

        public static string IEnumerableF(IEnumerable list, char delimiter = ',') {
            var is_dictionary = list.GetType().GetInterface(nameof(IDictionary)) != null;
            var sb = new StringBuilder($"{nameof(list)}:");

            if (list.GetType().GetInterface(nameof(IDictionary)) != null) {
                var dic = (IDictionary) list;
                if (dic.Count < 1) {
                    sb.Append(" [empty]");
                    return sb.ToString();
                }

                //var kv_element = dic.
                //var kv_type = value.GetType();
                //        object kvpKey = valueType.GetProperty("Key").GetValue(value, null);
                //    object kvpValue = valueType.GetProperty("Value").GetValue(value, null);

                foreach (var key in dic.Keys)
                    sb.Append($"{key.Fo()} => {dic[key].Fo()}" + delimiter);
                }
            else {
                foreach (var element in list)
                    sb.Append($"{element.Fo()}" + delimiter);
                }

            return sb.ToString().TrimEnd(delimiter);
        }

        private static string StringF(object o) => o.ToString();

        private static string StringArrayF(object o) {
            var to = (string[]) o;
            var result = to.Aggregate(string.Empty, (current, s) => current + "\"" + s + "\"; ");
            return result.Substring(0, result.Length - 2);
        }

        private static string ListStringArrayF(object o) {
            var to = (List<string>) o;
            return to.ToArray().Fo();
        }

        private static string ExceptionF(object o) {
            var to = (Exception) o;
            return "Exception: " + to.GetType().Name + " = \"" + to.Message + "\"";
        }

        #endregion
    }


    public static class FormatterExtension {
        public static string Fo(this object o) => Formatter.Instance.Fo(o);

        public static string FullObjectData<T>(this T o, bool public_only = true, bool is_recursive = false) {
            if (o == null) return "null";

            var type = o.GetType();
            if (!type.IsClass)
                return o.Fo();

            var sb = new StringBuilder($"\n{type.FullName}:\n");

            var fields = public_only ? type.GetFields().Where(x => x.IsPublic) : type.GetFields();
            fields = fields.OrderBy(x => x.Name);
            foreach (var field in fields) {
                sb.Append($"{field.Name}[{field.IsPublic}][{field.FieldType.Name}] = ");
                var value = field.GetValue(o);
                if (field.FieldType.IsClass && is_recursive)
                    sb.Append(value.FullObjectData(public_only, true));
                else
                    sb.Append(field.GetValue(o).Fo());
                sb.AppendLine();
            }

            var properties = public_only ? type.GetProperties().Where(x => x.CanRead) : type.GetProperties();
            properties = properties.OrderBy(x => x.Name);
            foreach (var property in properties) {
                sb.Append($"{property.Name}[{property.CanRead}][{property.PropertyType.Name}] = ");
                var value = property.GetValue(o, BindingFlags.DeclaredOnly, null, new object[0], CultureInfo.InvariantCulture);
                if (property.PropertyType.IsClass && is_recursive)
                    sb.Append(FullObjectData(value));
                else
                    sb.Append(value.Fo());
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}