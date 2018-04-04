/* 
XML-RPC.NET library
Copyright (c) 2001-2006, Charles Cook <charlescook@cookcomputing.com>

Permission is hereby granted, free of charge, to any person 
obtaining a copy of this software and associated documentation 
files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, 
publish, distribute, sublicense, and/or sell copies of the Software, 
and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be 
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
*/

namespace CookComputing.XmlRpc
{
  using System;
  using System.Diagnostics;
  using System.IO;
  using System.Text;

  public class Util
  {
    // protect constructor because static only class
    protected Util()
    {      
    }

    static public void CopyStream(Stream src, Stream dst)
    {
      byte[] buff = new byte[4096];
      while (true)
      {
        int read = src.Read(buff, 0, 4096);
        if (read == 0)
          break;
        dst.Write(buff, 0, read);
      }
    }
    
    public static Stream StringAsStream(string S)
    {
      MemoryStream mstm = new MemoryStream();
      StreamWriter sw = new StreamWriter(mstm);
      sw.Write(S);
      sw.Flush();
      mstm.Seek(0, SeekOrigin.Begin); 
      return mstm;
    }

#if (!COMPACT_FRAMEWORK)
    public static void TraceStream(Stream stm)
    {
      TextReader trdr = new StreamReader(stm, new UTF8Encoding(), true, 4096);
      String s = trdr.ReadLine();
      while (s != null)
      {
        Trace.WriteLine(s);
        s = trdr.ReadLine();
      }
    }

    public static void DumpStream(Stream stm)
    {
      TextReader trdr = new StreamReader(stm);
       String s = trdr.ReadLine();
      while (s != null)
      {
        Trace.WriteLine(s);
        s = trdr.ReadLine();
      }
    }
#endif

#if (!COMPACT_FRAMEWORK)
    public static Guid NewGuid()
    {
      return Guid.NewGuid();
    }
#else
    public static Guid NewGuid()
    {
      return OpenNETCF.GuidEx.NewGuid();
    }
#endif
  }

}
