using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blockcore.Platform
{
    public static class Extensions
    {
        public static List<Type> GetAllTypesImplementingOpenGenericType(this Assembly assembly, Type openGenericType)
        {
            var list = from x in assembly.GetTypes()
                   from z in x.GetInterfaces()
                   let y = x.BaseType
                   where
                   (y != null && y.IsGenericType &&
                   openGenericType.IsAssignableFrom(y.GetGenericTypeDefinition())) ||
                   (z.IsGenericType &&
                   openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition()))
                   select x;

            return list.ToList();
        }

        public static List<Type> GetGenericTypes<T>(this Assembly assembly)
        {
            return assembly.GetGenericTypes(typeof(T));
        }

        public static List<Type> GetGenericTypes(this Assembly assembly, Type compareType)
        {
            List<Type> ret = new List<Type>();
            foreach (var type in assembly.DefinedTypes)
            {
                if (type.GetInterface(compareType.Name) != null)
                {
                    ret.Add(type);
                }
            }
            return ret;
        }

        public static bool ImplementsFirstInterface(this Type type, Type ifaceType)
        {
            Type[] intf = ifaceType.GetInterfaces();

            if (intf.Length == 0)
            {
                return false;
            }

            if (intf[0] == type)
            {
                return true;
            }

            return false;
        }

        public static bool ImplementsInterface(this Type type, Type ifaceType)
        {
            Type[] intf = ifaceType.GetInterfaces();

            for (int i = 0; i < intf.Length; i++)
            {
                if (intf[i] == type)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns only types that has the supplied interface as the first interface returned.
        /// This only validates on the first interface, if multiple, only first is checked.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static List<Type> GetTypesImplementing<T>(this Assembly assembly)
        {
            return assembly.GetTypesImplementing(typeof(T));
        }

        public static List<Type> GetTypesImplementing(this Assembly assembly, Type compareType)
        {
            List<Type> ret = new List<Type>();
            foreach (var type in assembly.DefinedTypes)
            {
                if (type.IsInterface || type.IsGenericType)
                {
                    continue;
                }

                if (compareType.ImplementsFirstInterface(type) && compareType != type)
                {
                    ret.Add(type);
                }
            }
            return ret;
        }

        public static List<Type> GetTypesAssignableFrom<T>(this Assembly assembly)
        {
            return assembly.GetTypesAssignableFrom(typeof(T));
        }

        public static List<Type> GetTypesAssignableFrom(this Assembly assembly, Type compareType)
        {
            List<Type> ret = new List<Type>();
            foreach (var type in assembly.DefinedTypes)
            {
                if (compareType.IsAssignableFrom(type) && compareType != type)
                {
                    ret.Add(type);
                }
            }
            return ret;
        }

        public static List<Type> GetTypesFromAttribute<T>(this Assembly assembly)
        {
            return assembly.GetTypesFromAttribute(typeof(T));
        }

        public static List<Type> GetTypesFromAttribute(this Assembly assembly, Type compareType)
        {
            List<Type> ret = new List<Type>();
            foreach (var type in assembly.DefinedTypes)
            {
                if (type.GetCustomAttribute(compareType) != null)
                {
                    ret.Add(type);
                }
            }
            return ret;
        }
    }
}
