
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
        static void fail() => throw new Exception();
        public static void True(bool b)
        {
            if (!b) fail();
        }
        public static void True(bool b, string msg)
        {
            if (!b) fail();
        }
        public static void False(bool b)
        {
            if (b) fail();
        }
        public static void Null(object b)
        {
            if (b != null) fail();
        }
        public static void NotNull(object b)
        {
            if (b == null) fail();
        }
        public static void Equal<T>(T expected, T actual)
            where T : class
        {
            if (!Object.ReferenceEquals(expected, actual)) fail();
        }
        public static void Equal(byte expected, byte actual)
        {
            if (expected != actual) fail();
        }
        public static void Equal(int expected, byte actual)
        {
            if (expected != actual) fail();
        }
        public static void Equal(char expected, char actual)
        {
            if (expected != actual) fail();
        }
        public static void Equal(int expected, int actual)
        {
            if (expected != actual) fail();
        }
        public static void Equal(int expected, int? actual)
        {
            if (expected != actual.Value) fail();
        }
        public static void Equal(long expected, long actual)
        {
            if (expected != actual) fail();
        }
        public static void Equal(long expected, long? actual)
        {
            if (expected != actual.Value) fail();
        }
        public static void Equal(double expected, double actual)
        {
            if (expected != actual) fail();
        }
        public static void Equal(double expected, double? actual)
        {
            if (expected != actual.Value) fail();
        }
        public static void Equal(string expected, string actual)
        {
            if (expected != actual) fail();
        }
        public static void Single<T>(IEnumerable<T> e)
        {
            if (e.Count() != 1) throw new Exception();
        }
    }

    public static class Run
    {
        static void w(string s)
        {
            System.Console.WriteLine("{0}", s);
        }

        public static int AllTestsIn(Assembly a)
        {
            var pass = 0;
            var fail = 0;
            foreach (var t in a.GetTypes())
            {
                var ma = t.GetMethods()
                        .Where(m => m.GetCustomAttribute(typeof(FactAttribute)) != null)
                        .ToArray();
                if (ma.Length > 0)
                {
                    object inst = Activator.CreateInstance(t);
                    foreach (var m in ma)
                    {
                        w($"{m.Name}");
                        try
                        {
                            m.Invoke(inst, null);
                            w("    pass");
                            pass++;
                        }
                        catch (Exception e)
                        {
                            w($"    {e}");
                            fail++;
                        }
                    }
                }
            }
            w($"pass: {pass}  fail: {fail}");
            return fail;
        }
        public static int AllTestsInCurrentAssembly()
        {
            return AllTestsIn(System.Reflection.Assembly.GetExecutingAssembly());
        }
    }
}

