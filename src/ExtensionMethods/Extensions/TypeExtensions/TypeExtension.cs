using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extensions.TypeExtensions {
    public static class TypeExtension {

        public static bool IsNullable(this Type t) {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static Type GetCoreType(this Type t) {
            if (t == null || !IsNullable(t)) return t;
            return !t.IsValueType ? t : Nullable.GetUnderlyingType(t);
        }

    }
    
}
