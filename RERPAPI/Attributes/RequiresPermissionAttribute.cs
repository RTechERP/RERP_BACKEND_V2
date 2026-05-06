using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace RERPAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class RequiresPermissionAttribute:Attribute
    {
        public string PermissionGroup;
        public string PermissionFunction;
        public RequiresPermissionAttribute(string permissionGroup = "", string permissionFunction = "")
        {
            PermissionGroup = permissionGroup;
            PermissionFunction = permissionFunction;
        }
    }
}
