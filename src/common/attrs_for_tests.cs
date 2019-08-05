
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class OrderAttribute : Attribute
{
    public int Value { get; private set; }
    public OrderAttribute(int n)
    {
        Value = n;
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TestAttribute : Attribute
{
}


