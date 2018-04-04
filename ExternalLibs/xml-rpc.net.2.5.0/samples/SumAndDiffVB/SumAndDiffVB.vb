Imports CookComputing.XmlRpc

Public Structure SumAndDiffValue
    Public sum As Integer
    Public difference As Integer
End Structure

<XmlRpcUrl("http://www.cookcomputing.com/sumAndDiff.rem")> _
Public Interface SumAndDiffItf
    Inherits IXmlRpcProxy
    <XmlRpcMethod("sample.sumAndDifference")> _
    Function SumAndDifference(ByVal x As Integer, _
                              ByVal y As Integer) _
                              As SumAndDiffValue
End Interface

Module SumAndDiffVB
    Sub Main()
        Dim proxy As SumAndDiffItf
        proxy = CType(XmlRpcProxyGen.Create(GetType(SumAndDiffItf)), SumAndDiffItf)
        Dim ret As SumAndDiffValue
        ret = proxy.SumAndDifference(2, 3)
        Console.WriteLine("sum = {0}  diff = {1}", ret.sum, ret.difference)
    End Sub
End Module