using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace ntbs_service.Helpers
{
    public static class EnumExtension
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            ?.Name ?? string.Empty;
        }
    }
}