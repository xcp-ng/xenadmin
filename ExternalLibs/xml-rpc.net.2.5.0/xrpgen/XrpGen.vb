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

Imports System
Imports System.Text
Imports System.IO
Imports System.CodeDom
Imports System.CodeDom.Compiler
Imports System.Reflection
Imports System.Globalization
Imports System.Collections

Imports CookComputing.XmlRpc

Imports Headblender.XmlRpc

Namespace Headblender.XmlRpc
    Public NotInheritable Class XrpGenOptions
        Private mInputFile As String = ""
        Private mInputTarget As String = "source"
        Private mInputLanguage As String = "cs"
        Private mInputInterfaceType As String = ""

        Private mOutputFile As String = ""
        Private mOutputTarget As String = "source"
        Private mOutputLanguage As String = "cs"

        Private mOutputImplicitAsync As Boolean = False
        Private mOutputFlattenHierarchy As Boolean = False
        Private mOutputNamespace As String = ""
        Private mOutputProxyType As String = ""

        Private mReferences As New ArrayList()

        Public Sub XrpGenOptions()
            mReferences.Add("System.dll")
        End Sub

        Public ReadOnly Property References() As ArrayList
            Get
                Return mReferences
            End Get
        End Property

        Public Property InputFile() As String
            Get
                Return mInputFile
            End Get
            Set(ByVal value As String)
                mInputFile = value
            End Set
        End Property

        Public Property InputTarget() As String
            Get
                Return mInputTarget
            End Get
            Set(ByVal value As String)
                mInputTarget = value
            End Set
        End Property

        Public Property InputLanguage() As String
            Get
                Return mInputLanguage
            End Get
            Set(ByVal value As String)
                mInputLanguage = value
            End Set
        End Property

        Public Property InputInterfaceType() As String
            Get
                Return mInputInterfaceType
            End Get
            Set(ByVal value As String)
                mInputInterfaceType = value
            End Set
        End Property

        Public Property OutputFile() As String
            Get
                Return mOutputFile
            End Get
            Set(ByVal value As String)
                mOutputFile = value
            End Set
        End Property

        Public Property OutputTarget() As String
            Get
                Return mOutputTarget
            End Get
            Set(ByVal value As String)
                mOutputTarget = value
            End Set
        End Property

        Public Property OutputLanguage() As String
            Get
                Return mOutputLanguage
            End Get
            Set(ByVal value As String)
                mOutputLanguage = value
            End Set
        End Property

        Public Property OutputImplicitAsync() As Boolean
            Get
                Return mOutputImplicitAsync
            End Get
            Set(ByVal value As Boolean)
                mOutputImplicitAsync = value
            End Set
        End Property

        Public Property OutputFlattenHierarchy() As Boolean
            Get
                Return mOutputFlattenHierarchy
            End Get
            Set(ByVal value As Boolean)
                mOutputFlattenHierarchy = value
            End Set
        End Property

        Public Property OutputNamespace() As String
            Get
                Return mOutputNamespace
            End Get
            Set(ByVal value As String)
                mOutputNamespace = value
            End Set
        End Property

        Public Property OutputProxyType() As String
            Get
                Return mOutputProxyType
            End Get
            Set(ByVal value As String)
                mOutputProxyType = value
            End Set
        End Property

        Public Sub ValidateOptions()
            ' --- input options 
            If (InputFile.Length = 0) Then
                Throw New ArgumentException( _
                    "Must specify an input filename")
            End If

            InputTarget = InputTarget.ToLower(CultureInfo.InvariantCulture)
            If (InputTarget.Length = 0) Or Not _
                ((InputTarget = "source") Or (InputTarget = "library")) Then

                Throw New ArgumentException( _
                    "Must specify either 'source' or 'library' as an input target")
            End If

            InputLanguage = InputLanguage.ToLower(CultureInfo.InvariantCulture)
            If (InputLanguage.Length = 0) Or Not _
                ((InputLanguage = "cs") Or (InputLanguage = "vb")) And _
                (InputTarget = "source") Then

                Throw New ArgumentException( _
                    "Must specify either 'CS' (for C#) or 'VB' (for VB) as an input language")
            End If

            If (InputInterfaceType.Length = 0) Or (InputInterfaceType.IndexOf(".") = -1) Then
                Throw New ArgumentException( _
                    "Must specify a full name for the input interface type")
            End If

            ' --- output options 
            If (OutputFile.Length = 0) Then
                Throw New ArgumentException( _
                    "Must specify an output filename")
            End If

            OutputTarget = OutputTarget.ToLower(CultureInfo.InvariantCulture)
            If (OutputTarget.Length = 0) Or Not _
                ((OutputTarget = "source") Or (OutputTarget = "library")) Then

                Throw New ArgumentException( _
                    "Must specify either 'source' or 'library' as an output target")
            End If

            OutputLanguage = OutputLanguage.ToLower(CultureInfo.InvariantCulture)
            If (OutputLanguage.Length = 0) Or Not _
                ((OutputLanguage = "cs") Or (OutputLanguage = "vb")) And _
                (OutputTarget = "source") Then

                Throw New ArgumentException( _
                    "Must specify either 'CS' (for C#) or 'VB' (for VB) as an output language")
            End If

            ' --- TEST
            If Not ((InputTarget = "source") And (OutputTarget = "source")) Then
                Throw New ArgumentException( _
                    "Currently, only 'source' input and output targets supported")
            End If
        End Sub

        Public Sub ParseOptions(ByVal args As String())
            Dim i As Integer
            Dim s As String
            Dim p As Integer
            Dim b As Boolean
            Dim Name As String
            Dim Value As String

            For i = 0 To args.Length - 1
                s = args(i)
                p = s.IndexOf(":")

                b = False
                Name = ""
                Value = ""
                If ((s.StartsWith("/") = True) Or (s.StartsWith("-") = True)) Then
                    If ((p > 0) And (p < s.Length - 1)) Then
                        Name = s.Substring(1, p - 1)
                        Value = s.Substring(p + 1, s.Length - p - 1)

                        Name = Name.ToLower(CultureInfo.InvariantCulture)
                        Name = Name.Trim()

                        Value = Value.Replace("""", "")
                        Value = Value.Trim()

                        If ((Name.Length > 0) And (Value.Length > 0)) Then
                            b = True
                        End If
                    End If
                End If

                If (b = True) Then
                    Select Case Name
                        Case "input", "in"
                            InputFile = Value
                        Case "output", "out"
                            OutputFile = Value
                        Case "inputlanguage", "inputlang", "inlang"
                            InputLanguage = Value.ToLower(CultureInfo.InvariantCulture)
                        Case "outputlanguage", "outputlang", "outlang"
                            OutputLanguage = Value.ToLower(CultureInfo.InvariantCulture)
                        Case "interface", "if"
                            InputInterfaceType = Value
                        Case "implicitasync", "implicit"
                            Value = Value.ToLower(CultureInfo.InvariantCulture)
                            OutputImplicitAsync = False

                            If (Value = "true") Then
                                OutputImplicitAsync = True
                            End If
                        Case "flattenhierarchy", "flatten", "flat"
                            Value = Value.ToLower(CultureInfo.InvariantCulture)
                            OutputFlattenHierarchy = False

                            If (Value = "true") Then
                                OutputFlattenHierarchy = True
                            End If
                        Case "reference", "ref"
                            If (Value.ToLower(CultureInfo.InvariantCulture).EndsWith(".dll") = False) Then
                                Value = String.Format(CultureInfo.InvariantCulture, "{0}{1}", Value, ".dll")
                            End If

                            If (References.Contains(Value) = False) Then
                                References.Add(Value)
                            Else
                                Throw New ArgumentException("Duplicate assembly reference specified")
                            End If

                        Case Else
                            Throw New ArgumentException( _
                                String.Format( _
                                CultureInfo.InvariantCulture, _
                                "Unknown command line option: {0}", Name))
                    End Select
                End If
            Next
        End Sub
    End Class

    Public NotInheritable Class XrpGen
        Public Sub New()
        End Sub

        Public Sub Generate(ByVal opts As XrpGenOptions)
            Generate(opts, New String() {"<< none specified >>"})
        End Sub

        Public Sub Generate(ByVal opts As XrpGenOptions, ByVal args As String())
            opts.ValidateOptions()

            ' --- create language compiler and generator

            ' input source code
            Dim xcp As CodeDomProvider = Nothing

            Select Case opts.InputLanguage.ToLower(CultureInfo.InvariantCulture)
                Case "cs"
                    xcp = New Microsoft.CSharp.CSharpCodeProvider()
                Case "vb"
                    xcp = New Microsoft.VisualBasic.VBCodeProvider()
                Case Else
                    Throw New Exception("Bad input language provider")
            End Select

            ' output source code
            Dim xcg As ICodeGenerator = Nothing

            Select Case opts.OutputLanguage.ToLower(CultureInfo.InvariantCulture)
                Case "cs"
                    Dim x As New Microsoft.CSharp.CSharpCodeProvider()
                    xcg = x.CreateGenerator()
                Case "vb"
                    Dim x As New Microsoft.VisualBasic.VBCodeProvider()
                    xcg = x.CreateGenerator()
                Case Else
                    Throw New Exception("Bad output language generator")
            End Select

            ' --- set compiler options

            Dim csc As ICodeCompiler = xcp.CreateCompiler()
            Dim csp As New CompilerParameters()

            csp.GenerateExecutable = False
            csp.GenerateInMemory = True

            '!csp.TempFiles.KeepFiles = True

            Dim ro As Object = Nothing
            For Each ro In opts.References
                csp.ReferencedAssemblies.Add(CType(ro, String))
            Next

            ' --- compile the input source

            Dim csr As CompilerResults = csc.CompileAssemblyFromFile(csp, opts.InputFile)

            If (csr.Errors.Count > 0) Then
                Dim eb As New StringBuilder()

                eb.AppendFormat(CultureInfo.InvariantCulture, "Errors encountered compiling source file:{0}", Environment.NewLine)
                eb.AppendFormat(CultureInfo.InvariantCulture, Environment.NewLine)

                Dim j As Integer

                For j = 0 To csr.Errors.Count - 1
                    eb.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}", csr.Errors(j).ToString(), Environment.NewLine)
                Next

                Throw New ArgumentException(eb.ToString())
            End If

            ' --- generate the proxy

            ' get the assembly just compiled
            Dim a As [Assembly] = csr.CompiledAssembly

            ' get the correct type from the assembly
            Dim t As Type = a.GetType(opts.InputInterfaceType, True)

            ' set proxy gen options
            Dim xo As New XmlRpcProxyCodeGenOptions( _
                opts.OutputNamespace, _
                opts.OutputProxyType, _
                opts.OutputImplicitAsync, _
                opts.OutputFlattenHierarchy)

            ' get a compuleunit for the proxy
            Dim ccu As CodeCompileUnit = XmlRpcProxyCodeGen.CreateCodeCompileUnit( _
                t, _
                xcg, _
                xo)

            ' add some custom comments to the code
            ' (code is always in the first namespace)
            ccu.Namespaces(0).Comments.Add(New CodeCommentStatement("# # # # # #"))
            ccu.Namespaces(0).Comments.Add(New CodeCommentStatement( _
                String.Format( _
                CultureInfo.InvariantCulture, _
                "XrpGen generated file, created {0}", _
                DateTime.Now.ToString(CultureInfo.InvariantCulture))))
            ccu.Namespaces(0).Comments.Add(New CodeCommentStatement(""))
            ccu.Namespaces(0).Comments.Add(New CodeCommentStatement("Command line options:"))

            Dim sb As New StringBuilder()
            Dim i As Integer

            For i = 0 To args.Length - 1
                sb.AppendFormat(CultureInfo.InvariantCulture, "{0} ", args(i))
            Next
            ccu.Namespaces(0).Comments.Add(New CodeCommentStatement(sb.ToString()))

            ccu.Namespaces(0).Comments.Add(New CodeCommentStatement("# # # # # #"))
            ccu.Namespaces(0).Comments.Add(New CodeCommentStatement(""))

            ' set code generator options
            Dim cgo As New CodeGeneratorOptions()

            cgo.BlankLinesBetweenMembers = True
            cgo.BracingStyle = "C"

            ' create source code string from compileunit
            Dim sw As New StringWriter(CultureInfo.InvariantCulture)

            xcg.GenerateCodeFromCompileUnit(ccu, sw, cgo)

            ' write the proxy source to the output file
            Dim s As String = sw.ToString()

            Dim ow As New StreamWriter(opts.OutputFile, False)

            ow.Write(s)
            ow.Close()
        End Sub
    End Class
End Namespace
