using CookComputing.XmlRpc;

struct SumAndDiffValue 
{
  public int sum; 
  public int difference; 
}


[XmlRpcService(AutoDocVersion=false)]
class SumAndDiffService : XmlRpcService
{ 
  [XmlRpcMethod("sample.sumAndDifference")] 
  public SumAndDiffValue SumAndDifference(int x, int y) 
  { 
    SumAndDiffValue ret; 
    ret.sum = x + y; 
    ret.difference = x - y; 
    return ret; 
  } 
}
