using CookComputing.XmlRpc;

public struct StateStructRequest
{
  public int state1;
  public int state2;
  public int state3;
}

public interface IStateName
{
  [XmlRpcMethod]
  string GetStateName(int stateNumber);

  [XmlRpcMethod]
  string GetStateNames(StateStructRequest request);
}