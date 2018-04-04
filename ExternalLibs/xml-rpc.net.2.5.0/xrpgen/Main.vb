'
'XML-RPC.NET proxy class code generator
'Copyright (c) 2003, Joe Bork <joe@headblender.com>
'
'Permission is hereby granted, free of charge, to any person 
'obtaining a copy of this software and associated documentation 
'files (the "Software"), to deal in the Software without restriction, 
'including without limitation the rights to use, copy, modify, merge, 
'publish, distribute, sublicense, and/or sell copies of the Software, 
'and to permit persons to whom the Software is furnished to do so, 
'subject to the following conditions:
'
'The above copyright notice and this permission notice shall be 
'included in all copies or substantial portions of the Software.
'
'THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
'EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
'OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
'NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
'HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
'WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
'OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
'DEALINGS IN THE SOFTWARE.
'

Option Strict On
Option Explicit On 


Imports Headblender.XmlRpc

Module Main
    Private Sub ShowBanner()
        System.Console.WriteLine("XML-RPC.NET proxy class code generator v1.0.1")
        System.Console.WriteLine("Copyright (c) 2003, Joe Bork <joe@headblender.com>")
        System.Console.WriteLine()
    End Sub

    Private Sub ShowUsage()
        Console.WriteLine("Usage: xrpgen <options>")
        Console.WriteLine("")
        Console.WriteLine("/input:<filename>        * Input source file defining the interface to proxy")
        Console.WriteLine("")
        Console.WriteLine("/output:<filename>       * Output file to write proxy wrapper source to")
        Console.WriteLine("")
        Console.WriteLine("/inputlanguage:<cs|vb>   Specify the input language, either CS (default) or VB")
        Console.WriteLine("")
        Console.WriteLine("/outputlanguage:<cs|vb>  Specify the output language (CS default)")
        Console.WriteLine("")
        Console.WriteLine("/interface:<full.type>   * Specify the full typename of the interface to proxy")
        Console.WriteLine("")
        Console.WriteLine("/implicit:<true|false>   Indicate whether to implicitly generate ")
        Console.WriteLine("                         asynchronous proxy methods (default is False)")
        Console.WriteLine("")
        Console.WriteLine("/reference:<assembly>    Include a reference to <assembly> for compilation;")
        Console.WriteLine("                         multiple references may be added using multiple ")
        Console.WriteLine("                         '/reference' options")
        Console.WriteLine("")
        Console.WriteLine("/flatten:<true|false>    Indicate whether to recursively flatten the interface")
        Console.WriteLine("                         inheritance hierarchy for the root interface ")
        Console.WriteLine("                         (default False)")
        Console.WriteLine("")
        Console.WriteLine("(Note that options denoted by '*' are required)")
        Console.WriteLine()
    End Sub

    Sub Main(ByVal args As String())
        Dim opts As New XrpGenOptions()
        Dim gen As New XrpGen()

        ShowBanner()

        If (args.Length = 0) Then
            ShowUsage()
            Return
        End If

        Try
            opts.ParseOptions(args)
            opts.ValidateOptions()

            gen.Generate(opts, args)
        Catch ex As ArgumentException
            Console.WriteLine(ex.Message)
            Return
        End Try
    End Sub
End Module
