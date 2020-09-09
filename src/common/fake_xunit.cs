
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

    public static class Assert
    {
        public static int count;

        public static void True(bool b)
        {
            count++;
            if (!b) throw new Exception("expected True");
        }
        public static void True(bool b, string msg)
        {
            count++;
            if (!b) throw new Exception($"expected True: {msg}");
        }
        public static void False(bool b)
        {
            count++;
            if (b) throw new Exception("expected False");
        }
        public static void Null(object b)
        {
            count++;
            if (b != null) throw new Exception("expected Null");
        }
        public static void NotNull(object b)
        {
            count++;
            if (b == null) throw new Exception("expected NotNull");
        }
        public static void Contains(string sub, string actual)
        {
            count++;
            if (!actual.Contains(sub)) throw new Exception($"expected that {actual} contains {sub}");
        }
        public static void DoesNotContain(string sub, string actual)
        {
            count++;
            if (actual.Contains(sub)) throw new Exception($"expected that {actual} not contain {sub}");
        }
        public static void Equal<T>(T expected, T actual)
            where T : class
        {
            count++;
            if (!Object.ReferenceEquals(expected, actual)) throw new Exception($"expected ReferenceEquals {expected} but actual {actual}");
        }
        public static void Equal(byte expected, byte actual)
        {
            count++;
            if (expected != actual) throw new Exception($"expected {expected} but actual {actual}");
        }
        public static void Equal(int expected, byte actual)
        {
            count++;
            if (expected != actual) throw new Exception($"expected {expected} but actual {actual}");
        }
        public static void Equal(char expected, char actual)
        {
            count++;
            if (expected != actual) throw new Exception($"expected {expected} but actual {actual}");
        }
        public static void Equal(int expected, int actual)
        {
            count++;
            if (expected != actual) throw new Exception($"expected {expected} but actual {actual}");
        }
        public static void Equal(int expected, int? actual)
        {
            count++;
            if (expected != actual.Value) throw new Exception($"expected {expected} but actual {actual}");
        }
        public static void Equal(long expected, long actual)
        {
            count++;
            if (expected != actual) throw new Exception($"expected {expected} but actual {actual}");
        }
        public static void Equal(long expected, long? actual)
        {
            count++;
            if (expected != actual.Value) throw new Exception($"expected {expected} but actual {actual}");
        }
        public static void Equal(double expected, double actual)
        {
            count++;
            if (expected != actual) throw new Exception($"expected {expected} but actual {actual}");
        }
        public static void Equal(double expected, double? actual)
        {
            count++;
            if (expected != actual.Value) throw new Exception($"expected {expected} but actual {actual}");
        }
        public static void Equal(string expected, string actual)
        {
            count++;
            if (expected != actual) throw new Exception($"expected len {expected.Length} actual len {actual.Length} expected {expected} actual {actual}");
        }
        public static void Single<T>(IEnumerable<T> e)
        {
            count++;
            var enumerableCount = e.Count();
            if (enumerableCount != 1) throw new Exception($"expected enumerable to contain a single element but contains {enumerableCount} elements");
        }
        public static T Throws<T>(Action a)
            where T : System.Exception
        {
            count++;
            try
            {
                a();
            }
            catch (T e)
            {
                return e;
            }
            throw new Exception($"expected throw {typeof(T).Name}");
        }
    }

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
                            Assert.count = 0;
                            var sw = System.Diagnostics.Stopwatch.StartNew();
                            m.Invoke(inst, null);
                            sw.Stop();
                            var elapsed = (long)(sw.ElapsedMilliseconds);
                            wn($"pass ({Assert.count,4} asserts, {elapsed,5} ms)");
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

