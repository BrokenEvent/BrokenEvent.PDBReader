namespace BrokenEvent.PDBReader.TestLib
{
  public class Class2
  {
    public Class2()
    {
      string str = null;
      // exception here, at line 15
      int l = str.Length;
    }
  }
}
