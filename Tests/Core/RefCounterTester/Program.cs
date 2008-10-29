// The contents of this file are public domain.
// You may use them as you wish.
//
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AgateLib;
using AgateLib.Core;
using AgateLib.Utility;

namespace RefCounterTester
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Ref<TestClass> myref = new Ref<TestClass>(new TestClass());
            Ref<TestClass> newref = new Ref<TestClass>(myref);


            newref.Dispose();
            myref.Dispose();

            Console.ReadKey(false);
        }

        class TestClass : IDisposable
        {
            public TestClass()
            {
                System.Console.WriteLine("TestClass Created.");
            }
            ~TestClass()
            {
                System.Console.WriteLine("TestClass Destroyed.");
            }

            public void Dispose()
            {
                System.Console.WriteLine("TestClass Disposed.");
            }
        }
    }
}