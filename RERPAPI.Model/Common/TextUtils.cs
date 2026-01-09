using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RERPAPI.Model.Common
{
    public static class TextUtils
    {

        public static DateTime MinDate { get
            {
                return new DateTime(1900, 1, 1, 0, 0, 0);
            }
        }

        public static DateTime MaxDate
        {
            get
            {
                return new DateTime(9999, 1, 1, 0, 0, 0);
            }
        }

        public static string ToString(object x)
        {
            try
            {
                if (x != null)
                {
                    return x.ToString().Trim();
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Chuyển giá trị sang kiểu integer
        /// </summary>
        /// <param name="x">giá trị cần chuyển</param>
        /// <returns></returns>
        public static int ToInt32(object? x)
        {
            try
            {
                //return Convert.ToInt32(x);

                if (x == null) return 0;

                var str = x.ToString(); // StringValues → string
                return int.TryParse(str, out var result) ? result : 0;
            }
            catch
            {
                return 0;
            }
        }
        public static float ToFloat(object x)
        {
            try
            {
                return Convert.ToSingle(x);
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// Chuyển giá trị sang kiểu long
        /// </summary>
        /// <param name="x">giá trị cần chuyển</param>
        /// <returns></returns>
        public static Int64 ToInt64(object x)
        {
            try
            {
                return Convert.ToInt64(x);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Chuyển giá trị sang kiểu decimal
        /// </summary>
        /// <param name="x">giá trị cần chuyển</param>
        /// <returns></returns>
        //public static Decimal ToDecimal(object x)
        //{
        //    try
        //    {
        //        return Convert.ToDecimal(x);
        //    }
        //    catch
        //    {
        //        return 0;
        //    }
        //}

        public static decimal ToDecimal(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0m;

            try
            {
                // 🔹 Case JSON
                if (value is JsonElement je)
                {
                    return je.ValueKind switch
                    {
                        JsonValueKind.Number => je.GetDecimal(),

                        JsonValueKind.String =>
                            decimal.TryParse(je.GetString(), out var d)
                                ? d
                                : 0m,

                        _ => 0m
                    };
                }

                // 🔹 Case bình thường
                return Convert.ToDecimal(value);
            }
            catch
            {
                return 0m;
            }
        }

        /// <summary>
        /// Chuyển giá trị sang kiểu double
        /// </summary>
        /// <param name="x">giá trị cần chuyển</param>
        /// <returns></returns>
        public static Double ToDouble(object x)
        {
            try
            {
                return Convert.ToDouble(x);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Chuyển giá trị sang kiểu bool
        /// </summary>
        /// <param name="x">giá trị cần chuyển</param>
        /// <returns></returns>
        public static bool ToBoolean(object x)
        {
            try
            {
                return Convert.ToBoolean(x);
            }
            catch
            {
                return false;
            }
        }

    }
}
