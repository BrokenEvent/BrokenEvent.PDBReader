namespace BrokenEvent.PDBReader.TestLib
{
  public class Class1
  {
    public void Method1()
    { // here are first line
      int a = 1;
      int b = 2;
      int c = 3;
      int d = c + b + a;

      // exception here, at line 13
      int e = 1 / (d - 6);
    }

    public void Method2()
    { // here are first line
      int[] array = { 1, 2, 3, 4 };

      // exception here, at line 23
      for (int i = 0; i <= array.Length; i++)
        array[i]++;
    }
  }
}
