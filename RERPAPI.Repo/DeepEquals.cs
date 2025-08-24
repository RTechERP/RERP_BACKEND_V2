using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo
{
    public static class DeepEquals
    {
        public static bool AreEqual<T>(T obj1, T obj2)
        {
            if (obj1 == null && obj2 == null) return true;
            if (obj1 == null || obj2 == null) return false;
            var type = typeof(T);
            var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var prop in properties)
            {
                var value1 = prop.GetValue(obj1);
                var value2 = prop.GetValue(obj2);
                if (value1 == null && value2 == null) continue;
                if (value1 == null || value2 == null) return false;
                if (!value1.Equals(value2)) return false;
            }
            return true;
        }
        /// <summary>
        /// So sánh 2 object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="another"></param>
        /// <returns></returns>
        public static object DeepEqual(object obj, object another)
        {

            if (ReferenceEquals(obj, another)) return true;
            if ((obj == null) || (another == null)) return false;
            //So sánh class của 2 object, nếu khác nhau thì trả fail
            if (obj.GetType() != another.GetType()) return false;

            //Lấy toàn bộ các properties của obj
            //sau đó so sánh giá trị của từng property

            List<string> propertyNames = new List<string>();
            foreach (var property in obj.GetType().GetProperties())
            {
                var objValue = (property.GetValue(obj) ?? "").ToString();
                var anotherValue = (property.GetValue(another) ?? "").ToString();

                if (!objValue.Equals(anotherValue))
                {
                    propertyNames.Add(property.Name);
                }
            }
            return new
            {
                equal = !(propertyNames.Count > 0),
                property = propertyNames
            };
        }
    }
}
