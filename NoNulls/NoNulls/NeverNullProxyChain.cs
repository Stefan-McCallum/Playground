using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;

namespace NoNulls
{
    #region Primitive Checker

    /*
     * Code to check primitive types, taken from stack overflow user
     * Ronnie Overby: http://stackoverflow.com/a/15578098/310196
     */

    internal static class PrimitiveTypes
    {
        public static readonly Type[] List;

        static PrimitiveTypes()
        {
            var types = new[]
                        {
                            typeof (Enum),
                            typeof (String),
                            typeof (Char),

                            typeof (Boolean),
                            typeof (Byte),
                            typeof (Int16),
                            typeof (Int32),
                            typeof (Int64),
                            typeof (Single),
                            typeof (Double),
                            typeof (Decimal),

                            typeof (SByte),
                            typeof (UInt16),
                            typeof (UInt32),
                            typeof (UInt64),

                            typeof (DateTime),
                            typeof (DateTimeOffset),
                            typeof (TimeSpan),
                        };


            var nullTypes = from t in types
                            where t.IsValueType
                            select typeof(Nullable<>).MakeGenericType(t);

            List = types.Concat(nullTypes).ToArray();
        }

        public static bool Test(Type type)
        {
            if (List.Any(x => x.IsAssignableFrom(type)))
                return true;

            var nut = Nullable.GetUnderlyingType(type);
            return nut != null && nut.IsEnum;
        }
    }

    #endregion

    #region Marker interface to unbox proxy

    public interface IUnBoxProxy
    {
        object Value { get; }
    }

    #endregion

    #region Interceptor

    public class NeverNullInterceptor : IInterceptor
    {
        private object Source { get; set; }

        public NeverNullInterceptor(object source)
        {
            Source = source;
        }

        public void Intercept(IInvocation invocation)
        {
            try
            {
                if (invocation.Method.DeclaringType == typeof(IUnBoxProxy))
                {
                    invocation.ReturnValue = Source;
                    return;
                }

                invocation.Proceed();

                var d = Convert.ChangeType(invocation.ReturnValue, invocation.Method.ReturnType);

                if (!PrimitiveTypes.Test(invocation.Method.ReturnType))
                {
                    invocation.ReturnValue = invocation.ReturnValue == null
                                                 ? ProxyExtensions.NeverNullProxy(invocation.Method.ReturnType)
                                                 : ProxyExtensions.NeverNull(d, invocation.Method.ReturnType);
                }
            }
            catch (Exception ex)
            {
                invocation.ReturnValue = null;
            }
        }
    }

    #endregion

    #region Proxy extensions to create never null proxies

    public static class ProxyExtensions
    {
        private static ProxyGenerator _generator = new ProxyGenerator();

        public static object NeverNullProxy(Type t)
        {
            return _generator.CreateClassProxy(t, new[] { typeof(IUnBoxProxy) }, new NeverNullInterceptor(null));
        }

        public static object NeverNull(object source, Type type)
        {
            return _generator.CreateClassProxyWithTarget(type, new[] { typeof(IUnBoxProxy) }, source,
                                                         new NeverNullInterceptor(source));
        }

        public static T NeverNull<T>(this T source) where T : class
        {
            return
                (T)
                _generator.CreateClassProxyWithTarget(typeof(T), new[] { typeof(IUnBoxProxy) }, source,
                                                      new NeverNullInterceptor(source));
        }

        public static T Final<T>(this T source)
        {
            var proxy = (source as IUnBoxProxy);
            if (proxy == null)
            {
                return source;
            }

            return (T)proxy.Value;
        }
    }

    #endregion

}
