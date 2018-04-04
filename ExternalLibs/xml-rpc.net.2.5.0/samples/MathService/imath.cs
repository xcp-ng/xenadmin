using System;
using CookComputing.XmlRpc;

namespace MathService
{
  public interface IMath 
  {
    [XmlRpcMethod("math.Add", 
       Description="Add two integers and return the result")]
    [return: XmlRpcReturnValue(Description="A plus B.")]
    int Add(
      [XmlRpcParameter(Description="first number")]
      int A, 
      [XmlRpcParameter(Description="second number")]
      int B);

    [XmlRpcMethod("math.Subtract", 
       Description="Subtract one integer from another and return the result")]
    [return: XmlRpcReturnValue(Description="A minus B.")]
    int Subtract(int A, int B);

    [XmlRpcMethod("math.Multiply", 
       Description="Multiply two integers and return the result")]
    [return: XmlRpcReturnValue(Description="A multiplied by B.")]
    int Multiply(int A, int B);

    [XmlRpcMethod("math.Divide", 
       Description="Divide one integer by another and return the result.")]
    [return: XmlRpcReturnValue(Description="A divided by B. Returns Fault Response on divide by zero.")]
    int Divide(int A, int B);
    
    [XmlRpcMethod("math.SumAndDifference",
       Description="Calculate sum and difference of two integers.")]
    [return: XmlRpcReturnValue(Description="A struct containing two members, sum and difference.")]    
    SumAndDiff SumAndDifference(int A, int B);

    [XmlRpcMethod("math.NoOp",
       Description="No op method returning 'void'.",
       Hidden=true)]
    [return: XmlRpcReturnValue(Description="Returns empty string value as 'void'")]    
    void NoOp();
  }
  
  public struct SumAndDiff 
  {
    public int sum; 
    public int difference; 
  }
}
