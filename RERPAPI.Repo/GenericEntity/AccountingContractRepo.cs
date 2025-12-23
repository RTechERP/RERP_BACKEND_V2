using RERPAPI.Model.Common;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class AccountingContractRepo : GenericRepo<AccountingContract>
    {
       
        public AccountingContractRepo(CurrentUser currentUser) : base(currentUser)
        {
        }

        public CompareResult DeepEquals(object oldObj, object newObj)
        {
            var result = new CompareResult();

            if (oldObj == null && newObj == null)
            {
                result.Equal = true;
                return result;
            }

            if (oldObj == null || newObj == null || oldObj.GetType() != newObj.GetType())
            {
                result.Equal = false;
                return result;
            }

            foreach (var prop in oldObj.GetType().GetProperties())
            {
                var oldVal = TextUtils.ToString(prop.GetValue(oldObj));
                var newVal = TextUtils.ToString(prop.GetValue(newObj));

                if (!oldVal.Equals(newVal))
                    result.ChangedProperties.Add(prop.Name);
            }

            result.Equal = result.ChangedProperties.Count == 0;
            return result;
        }


        //public object DeepEquals(object obj, object another)
        //{

        //    if (ReferenceEquals(obj, another)) return true;
        //    if ((obj == null) || (another == null)) return false;
        //    //So sánh class của 2 object, nếu khác nhau thì trả fail
        //    if (obj.GetType() != another.GetType()) return false;

        //    //var result = new
        //    //{
        //    //    equal = true,
        //    //    property = ""
        //    //};
        //    //Lấy toàn bộ các properties của obj
        //    //sau đó so sánh giá trị của từng property

        //    List<string> propertyNames = new List<string>();
        //    foreach (var property in obj.GetType().GetProperties())
        //    {
        //        var objValue = TextUtils.ToString(property.GetValue(obj));
        //        var anotherValue = TextUtils.ToString(property.GetValue(another));

        //        //var typeObj = obj.GetType().GetProperty(property.Name);
        //        //var typeAnother = another.GetType().GetProperty(property.Name);
        //        //if (typeObj == typeAnother)
        //        //{
        //        //    var objValue = property.GetValue(obj);
        //        //    var anotherValue = property.GetValue(another);

        //        //}

        //        if (!objValue.Equals(anotherValue))
        //        {
        //            propertyNames.Add(property.Name);
        //        }
        //    }

        //    return new
        //    {
        //        equal = !(propertyNames.Count > 0),
        //        property = propertyNames
        //    };
        //}

        public class CompareResult
        {
            public bool Equal { get; set; }
            public List<string> ChangedProperties { get; set; } = new();
        }

    }
}
