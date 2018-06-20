using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Microsoft.Cci;
using Microsoft.Cci.Pdb;

namespace BrokenEvent.PDBReader
{
  /// <summary>
  /// Entry point of the PDB reader lib. Used to resolve code file:line from class and method names.
  /// </summary>
  public class PdbResolver
  {
    private PdbInfo info;
    private readonly Dictionary<string, Dictionary<string, PdbMethod>> functionsMap = new Dictionary<string, Dictionary<string, PdbMethod>>();

    /// <summary>
    /// Creates instance of PdbResolver from stream.
    /// </summary>
    /// <param name="stream">Stream to load symbol data from</param>
    public PdbResolver(Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof(stream));
      
      LoadFunctionsInfo(stream);
    }

    /// <summary>
    /// Creates instance of PdbResolver from file.
    /// </summary>
    /// <param name="filename">File to load symbol data from</param>
    public PdbResolver(string filename)
    {
      if (filename == null)
        throw new ArgumentNullException(nameof(filename));

      using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
        LoadFunctionsInfo(stream);
    }

    /// <summary>
    /// Age of the PDB file is used to match the PDB against the PE binary.
    /// </summary>
    public int PdbAge
    {
      get { return info.Age; }
    }

    /// <summary>
    /// GUID of the PDB file is used to match the PDB against the PE binary.
    /// </summary>
    public Guid PdbGuid
    {
      get { return info.Guid; }
    }

    /// <summary>
    /// Source server data information.
    /// </summary>
    public string SourceServerData
    {
      get { return info.SourceServerData; }
    }

    private void LoadFunctionsInfo(Stream stream)
    {
      info = PdbFile.LoadFunctions(stream);

      foreach (PdbFunction function in info.Functions)
      {
        Dictionary<string, PdbMethod> moduleMap;
        if (!functionsMap.TryGetValue(function.module, out moduleMap))
        {
          moduleMap = new Dictionary<string, PdbMethod>();
          functionsMap.Add(function.module, moduleMap);
        }

        moduleMap[function.name] = new PdbMethod(function);
      }
    }

    private PdbMethod FindFunction(string classname, string methodName)
    {
      Dictionary<string, PdbMethod> moduleMap;
      if (!functionsMap.TryGetValue(classname, out moduleMap))
        return null;

      PdbMethod result;
      return moduleMap.TryGetValue(methodName, out result) ? result : null;
    }

    /// <summary>
    /// Finds a location of method first line
    /// </summary>
    /// <param name="classname">Class name with namespace (i.e. "MyNamespace.MyClass")</param>
    /// <param name="methodName">Name of method (i.e. "MyMethod")</param>
    /// <returns>Code location or null if not found.</returns>
    /// <example>
    /// <code>CodeLocation location = FindLocation("BrokenEvent.PDBReader.PdbResolver", "FindLocation")</code>
    /// </example>
    public CodeLocation FindLocation(string classname, string methodName)
    {
      return FindLocation(classname, methodName, 0);
    }

    /// <summary>
    /// Finds a location of code.
    /// </summary>
    /// <param name="classname">Class name with namespace (i.e. "MyNamespace.MyClass")</param>
    /// <param name="methodName">Name of method (i.e. "MyMethod")</param>
    /// <param name="ilOffset">IL offset of code to get line for. See <see cref="StackFrame.GetILOffset"/></param>
    /// <returns>Code location or null if not found.</returns>
    /// <example>
    /// <code>CodeLocation location = FindLocation("BrokenEvent.PDBReader.PdbResolver", "FindLocation", 10)</code>
    /// </example>
    public CodeLocation FindLocation(string classname, string methodName, uint ilOffset)
    {
      if (classname == null)
        throw new ArgumentNullException(nameof(classname));
      if (methodName == null)
        throw new ArgumentNullException(nameof(methodName));

      PdbMethod function = FindFunction(classname, methodName);
      if (function == null)
        return null;

      PdbMethod.CodeBlock codeBlock = function.FindBlock(ilOffset);
      if (codeBlock == null)
        return null;

      return new CodeLocation(codeBlock.Filename, codeBlock.LineNumber, codeBlock.Language);
    }
  }
}
