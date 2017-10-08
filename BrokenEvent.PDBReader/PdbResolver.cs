using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Cci;
using Microsoft.Cci.Pdb;

namespace BrokenEvent.PDBReader
{
  public class PdbResolver
  {
    private PdbInfo info;
    private readonly Dictionary<string, Dictionary<string, PdbMethod>> functionsMap = new Dictionary<string, Dictionary<string, PdbMethod>>();

    public PdbResolver(Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof(stream));
      
      LoadFunctionsInfo(stream);
    }

    public PdbResolver(string filename)
    {
      if (filename == null)
        throw new ArgumentNullException(nameof(filename));

      using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
        LoadFunctionsInfo(stream);
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

    public CodeLocation FindLocation(string classname, string methodName)
    {
      return FindLocation(classname, methodName, 0);
    }

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
