using System;
using System.Reflection;

namespace CommandLineParser.Compatibility
{
    public static class TypeExtensions
    {
#if NET20 || NET35 || NET40
        // https://github.com/castleproject/Core/blob/netcore/src/Castle.Core/Compatibility/IntrospectionExtensions.cs
        // This allows us to use the new reflection API which separates Type and TypeInfo
        // while still supporting .NET 3.5 and 4.0. This class matches the API of the same
        // class in .NET 4.5+, and so is only needed on .NET Framework versions before that.
        //
        // Return the System.Type for now, we will probably need to create a TypeInfo class
        // which inherits from Type like .NET 4.5+ and implement the additional methods and
        // properties.
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }
#endif

        public static T GetPropertyValue<T>(this Type type, string propertyName, object target)
        {
#if (!(NET40 || NET35 || NET20))
            PropertyInfo property = GetMember(type.GetTypeInfo(), propertyName, (ti, n) => ti.GetDeclaredProperty(n));
            return (T)property.GetValue(target);
#else
            return (T)type.InvokeMember(propertyName, BindingFlags.GetProperty, null, target, null);
#endif
        }

        public static void SetPropertyValue(this Type type, string propertyName, object target, object value)
        {
#if (!(NET40 || NET35 || NET20))
            PropertyInfo property = GetMember(type.GetTypeInfo(), propertyName, (ti, n) => ti.GetDeclaredProperty(n));
            property.SetValue(target, value);
#else
            type.InvokeMember(propertyName, BindingFlags.SetProperty, null, target, new object[] { value });
#endif
        }

        public static void SetFieldValue(this Type type, string fieldName, object target, object value)
        {
#if (!(NET40 || NET35 || NET20))
            FieldInfo field = GetMember(type.GetTypeInfo(), fieldName, (ti, n) => ti.GetDeclaredField(n));
            if (field != null)
            {
                field.SetValue(target, value);
            }
            else
            {
                type.SetPropertyValue(fieldName, target, value);
            }
#else
            type.InvokeMember(fieldName, BindingFlags.SetField | BindingFlags.SetProperty, null, target, new object[] { value });
#endif
        }

        public static void InvokeMethod<T>(this Type type, string methodName, object target, T value)
        {
#if (!(NET40 || NET35 || NET20))
            MethodInfo method = GetMember(type.GetTypeInfo(), methodName, (ti, n) => ti.GetDeclaredMethod(n)); 
            method.Invoke(target, new object[] { value });
#else
            type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, target, new object[] { value });
#endif
        }

#if (!(NET40 || NET35 || NET20))
        private static T GetMember<T>(TypeInfo typeInfo, string memberName, Func<TypeInfo, string, T> getDeclaredMember)
            where T: MemberInfo
        {
            T member = getDeclaredMember(typeInfo, memberName);
            if (member != null)
                return member;
            if (typeInfo.BaseType != null)
                return GetMember(typeInfo.BaseType.GetTypeInfo(), memberName, getDeclaredMember);
            return null;
        }
#endif
    }
}