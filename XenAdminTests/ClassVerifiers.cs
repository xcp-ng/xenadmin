/* Copyright (c) Citrix Systems, Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace XenAdminTests
{
    internal static class ClassVerifiers
    {
        /// <summary>
        /// Use this when you want to test that some expected data has already been set in a class
        /// </summary>
        /// <param name="classToVerify">A class with getters and setters to verify</param>
        /// <param name="expected">A struct of expected data content with members having the same name and return type as the getters and setters of the class</param>
        public static void VerifyGetters(object classToVerify, object expected)
        {
            FieldInfo[] fields = expected.GetType().GetFields();

            foreach (FieldInfo fieldInfo in fields)
            {
                string fieldName = fieldInfo.Name;
                object expectedValue = fieldInfo.GetValue(expected);
                var getter = classToVerify.GetType().GetProperty(fieldName);

                Assert.NotNull(getter, $"Could not find getter {fieldName}");

                object actualValue = getter.GetValue(classToVerify);
                if (expectedValue is string)
                    actualValue = actualValue.ToString();

                Assert.AreEqual(expectedValue, actualValue, $"Mismatched Property {fieldName}");
            }
        }

        /// <summary>
        /// Use this when you want to test that some data is set and a set of different data is retrieved afterwards.
        /// This is useful when your class manipulates the underlying data before returning/storing it
        /// </summary>
        /// <param name="classToVerify">A class with getters and setters to verify</param>
        /// <param name="expected">A struct of expected data content with members having the same name
        /// and return type as the getters and setters of the class</param>
        /// <param name="toSet">A struct of data to set with members having the same name and return
        /// type as the getters and setters of the class</param>
        public static void VerifySettersAndGetters(object classToVerify, object expected, object toSet = null)
        {
            if (toSet == null)
                toSet = expected;

            FieldInfo[] fields = expected.GetType().GetFields();
            foreach (FieldInfo fieldInfo in fields)
            {
                object valueToSet = fieldInfo.GetValue(toSet);
                string fieldName = fieldInfo.Name;

                PropertyInfo setter = classToVerify.GetType().GetProperty(fieldName);
                setter.SetValue(classToVerify, valueToSet, null);

                object expectedValue = fieldInfo.GetValue(expected);
                PropertyInfo getter = classToVerify.GetType().GetProperty(fieldName);
                Assert.AreEqual(expectedValue, getter.GetValue(classToVerify, null), "Property mismatched: " + fieldName);
            }
        }

        /// <summary>
        /// Use this class to go through a class with the constructed type and 
        /// verify there are methods that return the expected type and the
        /// return value is not null
        /// </summary>
        /// <param name="typeToValidate"></param>
        /// <param name="returnTypeForMethod">Type that the static method will check for as a return type</param>
        public static void VerifyStaticMethodReturn(Type typeToValidate, object returnTypeForMethod)
        {
            Type returnType = returnTypeForMethod as Type;
            MethodInfo[] methods = typeToValidate.GetMethods();

            Assert.AreNotEqual(0, methods.Length, "No methods found returning " + returnType);

            int staticMethodCount = 0;
            foreach (MethodInfo methodInfo in methods)
            {
                if (methodInfo.ReturnType == returnType && methodInfo.IsStatic)
                {
                    Assert.IsNotNull(methodInfo.Invoke(null, null), "Null value returned for " + methodInfo.Name);
                    staticMethodCount++;
                }
            }

            Assert.AreNotEqual(0, staticMethodCount, "Static method call count should not be zero");
        }

        /// <summary>
        /// Verify the number of properties on the provided class are as expected
        /// </summary>
        /// <param name="classToVerify">A class with getters and setters to verify</param>
        /// <param name="expectedNumberOfProperties">expected number of properties</param>
        public static void VerifyPropertyCounter(object classToVerify, object expectedNumberOfProperties)
        {
            int propertyCount = classToVerify.GetType().GetProperties().Count();
            Assert.AreEqual((int)expectedNumberOfProperties, propertyCount, "Number of properties on the class tested");
        }
    }
}
