/* 
OpenDHT.NET library, based on XML-RPC.NET
Copyright (c) 2006, Michel Foucault <mmarsu@gmail.com>

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

using System;
using OpenDHTLib;

public class Program
{
    /// <summary>
    /// OpenDHTLib Test compatible with OpenDHT python scripts (http://www.opendht.org/)
    /// </summary>
    public static void Main()
	{
        string line = new string('-', 80);
        string[] res = new string[] { "Success", "Capacity", "Again" };
        OpenDHT openDHT = new OpenDHT();
        OpenDHTMessage message = new OpenDHTMessage("OpenDHT.Net-Key", string.Empty, "OpenDHT.Net-Secret", 100);

        message.Content = "OpenDHT.Net-Value";
        Console.WriteLine("Put : " + res[openDHT.Put(message)]);

        Console.WriteLine("Get : ");
        foreach (string value in openDHT.GetStringValues(message.KeyStr))
            Console.WriteLine(value);
        Console.WriteLine(line);

        message.Content = "OpenDHT.Net-Value1";
        Console.WriteLine("PutRemovable : " + res[openDHT.PutRemovable(message)]);
        message.Content = "OpenDHT.Net-Value2";
        Console.WriteLine("PutRemovable : " + res[openDHT.PutRemovable(message)]);
        Console.WriteLine(line);

        Console.WriteLine("GetDetails :");
        foreach (OpenDHTMessageDetails msg in openDHT.GetDetailsStringValues(message.KeyStr))
            Console.WriteLine(msg);
        Console.WriteLine(line);

        message.Content = "OpenDHT.Net-Value1";
        Console.WriteLine("Remove : " + res[openDHT.Rm(message)]);
        Console.WriteLine(line);

        message.Content = "OpenDHT.Net-Value2";
        message.Secret = "OpenDHT.Net-SecretFoo";
        Console.WriteLine("Remove Fails : " + res[openDHT.Rm(message)]);
        Console.WriteLine(line);

        Console.WriteLine("GetDetails :");
        foreach (OpenDHTMessageDetails msg in openDHT.GetDetailsStringValues(message.KeyStr))
            Console.WriteLine(msg);
        Console.WriteLine(line);
	}
}

