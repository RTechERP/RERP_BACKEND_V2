using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace RERPAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class RequiresPermissionAttribute:Attribute
    {
        public string permission;
        public RequiresPermissionAttribute(string permission)
        {
            this.permission = permission;
        }
    }
}
