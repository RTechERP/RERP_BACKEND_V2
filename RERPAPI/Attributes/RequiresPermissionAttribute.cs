using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace RERPAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class RequiresPermissionAttribute:Attribute
    {
        public string permission;
        public string PermissionNew;
        public RequiresPermissionAttribute(string permissionGroup = "", string permissionFunction = "")
        {
            this.permission = permissionGroup;
            PermissionNew = permissionFunction;
        }
    }
}
