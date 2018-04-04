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

  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
  public class XmlRpcServiceAttribute : Attribute
  {
    public XmlRpcServiceAttribute()
    {
    }

    public bool AutoDocumentation
    {    
        get { return autoDocumentation; }
        set { autoDocumentation = value; }
    }

    public bool AutoDocVersion
    {    
      get { return autoDocVersion; }
      set { autoDocVersion = value; }
    }

    public string Description 
    {
      get { return description; }
      set { description = value; }
    }

    public int Indentation 
    {
      get { return indentation; }
      set { indentation = value; }
    }

    public bool Introspection
    {
      get { return introspection; }
      set { introspection = value; }
    }

    public string Name 
    {
      get { return name; }
      set { name = value; }
    }

    public bool UseIndentation
    {
      get { return useIndentation; }
      set { useIndentation = value; }
    }

    public bool UseIntTag
    {
      get { return useIntTag; }
      set { useIntTag = value; }
    }

    public bool UseStringTag
    {
      get { return useStringTag; }
      set { useStringTag = value; }
    }

    public string XmlEncoding
    {
      get { return xmlEncoding; }
      set { xmlEncoding = value; }
    }

    public override string ToString()
    {
      string value = "Description : " + description;
      return value;
    }
 
    private string description = "";
    private string xmlEncoding = null;
    private int indentation = 2;
    private bool introspection = false;
    private bool autoDocumentation = true;
    private bool autoDocVersion = true;
    private string name = "";
    private bool useStringTag = true;
    private bool useIndentation = true;
    private bool useIntTag = false;
  }
}