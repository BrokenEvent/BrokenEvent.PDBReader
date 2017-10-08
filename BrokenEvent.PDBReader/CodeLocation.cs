namespace BrokenEvent.PDBReader
{
  /// <summary>
  /// Location in the source files
  /// </summary>
  public class CodeLocation
  {
    internal CodeLocation(string fileName, uint line, string language)
    {
      FileName = fileName;
      Line = line;
      Language = language;
    }

    /// <summary>
    /// Gets the filename of where the found code is.
    /// </summary>
    public string FileName { get; private set; }

    /// <summary>
    /// Gets the zero-based line index in file.
    /// </summary>
    /// <remarks>Accuracy depends on search arguments. It may be method start or line inside of the method.</remarks>
    public uint Line { get; private set; }

    /// <summary>
    /// Gets the language of code file
    /// </summary>
    public string Language { get; private set; }

    /// <inheritdoc />
    public override string ToString()
    {
      return FileName + ":" + Line;
    }
  }
}
