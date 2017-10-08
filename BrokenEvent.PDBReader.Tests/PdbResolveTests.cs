using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using BrokenEvent.PDBReader.TestLib;

using NUnit.Framework;

namespace BrokenEvent.PDBReader.Tests
{
  [TestFixture]
  public class PdbResolveTests
  {
    private static PdbResolver LoadResolver()
    {
      return new PdbResolver(Path.Combine(TestContext.CurrentContext.TestDirectory, "BrokenEvent.PDBReader.TestLib.pdb"));
    }

    public static void AssertCodeLine(uint line, string file, CodeLocation location)
    {
      Assert.NotNull(location);
      Assert.AreEqual(line, location.Line);
      Assert.AreEqual(file, Path.GetFileName(location.FileName));
      Assert.AreEqual("C#", location.Language);
    }

    public static void AssertException(Exception e)
    {
      StackTrace stackTrace = new StackTrace(e, true);
      StackFrame frame = stackTrace.GetFrame(0);

      PdbResolver resolver = LoadResolver();
      MethodBase method = frame.GetMethod();
      CodeLocation location = resolver.FindLocation(method.DeclaringType.FullName, method.Name, (uint)frame.GetILOffset());

      Assert.NotNull(location);
      Assert.AreEqual(frame.GetFileName(), location.FileName);
      Assert.AreEqual(frame.GetFileLineNumber(), location.Line);
    }

    [Test]
    public void MethodStartResolve()
    {
      PdbResolver resolver = LoadResolver();

      AssertCodeLine(6, "Class1.cs", resolver.FindLocation("BrokenEvent.PDBReader.TestLib.Class1", "Method1"));
      AssertCodeLine(17, "Class1.cs", resolver.FindLocation("BrokenEvent.PDBReader.TestLib.Class1", "Method2"));
      AssertCodeLine(5, "Class2.cs", resolver.FindLocation("BrokenEvent.PDBReader.TestLib.Class2", ".ctor"));
    }

    [Test]
    public void ArithmeticExceptionResolve()
    {
      try
      {
        new Class1().Method1();
      }
      catch (Exception e)
      {
        AssertException(e);
      }
    }

    [Test]
    public void ArrayOutOfBoundsExceptionResolve()
    {
      try
      {
        new Class1().Method1();
      }
      catch (Exception e)
      {
        AssertException(e);
      }
    }

    [Test]
    public void NREResolve()
    {
      try
      {
        new Class2();
      }
      catch (Exception e)
      {
        AssertException(e);
      }
    }
  }
}
