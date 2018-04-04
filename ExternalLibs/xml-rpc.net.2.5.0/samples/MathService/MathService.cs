namespace MathService
{
  using System;
  using System.Threading;
  using CookComputing.XmlRpc;

  [XmlRpcService(
    Name="Test Math Service",
    Description="This is a sample XML-RPC service illustrating method calls with simple parameters and return type.",
    AutoDocumentation=true)]
  public class MathService : XmlRpcService, IMath
  {
    public int Add(int A,int B)
    {
      return A + B;
    }

    public int Subtract(int A, int B)
    {
      return A - B;
    }

    public int Multiply(int A, int B)
    {
      return A * B;
    }

    public int Divide(int A, int B)
    {
      if (B == 0)
      {
        throw new XmlRpcFaultException(1001, "Divide by zero");
      }
      return A/B;
    }
    
    public SumAndDiff SumAndDifference(int A, int B)
    {
      SumAndDiff ret;
      ret.difference = A - B;
      ret.sum = A + B;
      return ret;
    }

    public void NoOp()
    {
    }
  }
}
