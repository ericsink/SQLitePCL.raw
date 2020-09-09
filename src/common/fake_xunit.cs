
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xunit
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class FactAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CollectionDefinitionAttribute : Attribute
    {
        public CollectionDefinitionAttribute(string name) { }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class CollectionAttribute : Attribute
    {
        public CollectionAttribute(string name) { }
    }

    public interface ICollectionFixture<TFixture> where TFixture : class { }

    public static class Run
    {
        static void w(string s)
        {
            System.Console.Write("{0}", s);
        }
        static void wn(string s)
        {
            System.Console.WriteLine("{0}", s);
        }

        class compare_test_classes : IComparer<Type>
        {
            public int Compare(Type t1, Type t2)
            {
                var order1 = t1.GetCustomAttribute<OrderAttribute>();
                var order2 = t2.GetCustomAttribute<OrderAttribute>();
                if (order1 != null)
                {
                    if (order2 != null)
                    {
                        if (order1.Value < order2.Value)
                        {
                            return -1;
                        }
                        else if (order2.Value < order1.Value)
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    if (order2 != null)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        class compare_test_methods : IComparer<MethodInfo>
        {
            public int Compare(MethodInfo m1, MethodInfo m2)
            {
                var order1 = m1.GetCustomAttribute<OrderAttribute>();
                var order2 = m2.GetCustomAttribute<OrderAttribute>();
                if (order1 != null)
                {
                    if (order2 != null)
                    {
                        if (order1.Value < order2.Value)
                        {
                            return -1;
                        }
                        else if (order2.Value < order1.Value)
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    if (order2 != null)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        static bool IsTest(MethodInfo m)
        {
            return
                (m.GetCustomAttribute(typeof(FactAttribute)) != null)
                || (m.GetCustomAttribute(typeof(TestAttribute)) != null)
                ;
        }

        public static int AllTestsIn(Assembly a)
        {
            var pass = 0;
            var fail = 0;
            var a_types =
                a.GetTypes()
                    .Where(t => t.GetMethods().Where(m => IsTest(m)).Any())
                    .OrderBy(t => t, new compare_test_classes())
                    .ToArray()
                    ;
            foreach (var t in a_types)
            {
                var ma = t.GetMethods()
                        .Where(m => IsTest(m))
                        .OrderBy(m => m, new compare_test_methods())
                        .ToArray();
                if (ma.Length > 0)
                {
                    object inst = Activator.CreateInstance(t);
                    foreach (var m in ma)
                    {
                        w($"{m.Name,-40} -- ");
                        try
                        {
                            var sw = System.Diagnostics.Stopwatch.StartNew();
                            m.Invoke(inst, null);
                            sw.Stop();
                            var elapsed = (long)(sw.ElapsedMilliseconds);
                            wn($"pass ({elapsed,5} ms)");
                            pass++;
                        }
                        catch (Exception e)
                        {
                            wn("FAIL");
                            wn($"{e}");
                            fail++;
                        }
                    }
                }
            }
            wn($"pass: {pass}  fail: {fail}");
            return fail;
        }
        public static int AllTestsInCurrentAssembly()
        {
            return AllTestsIn(System.Reflection.Assembly.GetExecutingAssembly());
        }
    }
}

