using System.Collections.Generic;

using Microsoft.Cci;
using Microsoft.Cci.Pdb;

namespace BrokenEvent.PDBReader
{
  internal class PdbMethod
  {
    private readonly string name;
    private readonly string module;
    private readonly List<CodeBlock> blocks = new List<CodeBlock>();

    public PdbMethod(PdbFunction function)
    {
      name = function.name;
      module = function.module;

      if (function.lines == null)
        return;

      foreach (PdbLines line in function.lines)
      {
        string language;
        if (!PdbSourceDocument.sourceLanguageGuidToName.TryGetValue(line.file.language, out language))
          language = "n/a";

        foreach (PdbLine l in line.lines)
          if (l.lineBegin != 0xFEEFEE)
            blocks.Add(new CodeBlock(line.file.name, l.lineBegin, language, l.offset));
      }
      blocks.Sort(BlocksComparison);
    }

    private static int BlocksComparison(CodeBlock a, CodeBlock b)
    {
      return (int)a.IlOffset - (int)b.IlOffset;
    }

    public string Name
    {
      get { return name; }
    }

    public string Module
    {
      get { return module; }
    }

    public CodeBlock FindBlock(uint ilOffset)
    {
      if (blocks.Count == 0)
        return null;

      if (ilOffset == 0)
        return blocks[0];

      for(int i = 0; i < blocks.Count; i++)
        if (blocks[i].IlOffset > ilOffset)
          return blocks[i - 1];

      return blocks[blocks.Count - 1];
    }

    public override string ToString()
    {
      return module + "." + name;
    }

    public class CodeBlock
    {
      public readonly string Filename;
      public readonly uint LineNumber;
      public readonly string Language;
      public uint IlOffset;

      public CodeBlock(string filename, uint lineNumber, string language, uint ilOffset)
      {
        Filename = filename;
        LineNumber = lineNumber;
        Language = language;
        IlOffset = ilOffset;
      }

      public override string ToString()
      {
        return $"{Filename}:{LineNumber}#IL{IlOffset}";
      }
    }
  }
}
