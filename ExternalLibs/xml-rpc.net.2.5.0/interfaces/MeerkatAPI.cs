                                                                                                                                                                                                                                                               
using CookComputing.XmlRpc;

namespace CookComputing.Meerkat
{

  public struct Category
  {
    public int id;
    public string title;
  }

  public struct Channel
  {
    public int id;
    public string title;
  }

  [XmlRpcMissingMapping(MappingAction.Ignore)]
  public struct Recipe
  {
    // search criteria
    public XmlRpcInt channel;
    public XmlRpcInt category;
    public XmlRpcInt item;
    public string search;
    public string search_what;
    public string time_period;
    public XmlRpcInt profile;
    public XmlRpcInt mob;
    public string url;
    // display recipes
    public int ids;
    public int descriptions;
    public int categories;
    public int channels;
    public int dates;
    public int dc;
    public XmlRpcInt num_items;
  }

  [XmlRpcMissingMapping(MappingAction.Ignore)]
  public struct Result
  {
    [XmlRpcMissingMapping(MappingAction.Error)]
    public string title;
    [XmlRpcMissingMapping(MappingAction.Error)]
    public string link;
    public string description;
    public string dc_creator;
    public string dc_subject;
    public string dc_publisher;
    public string dc_date;
    public string dc_format;
    public string dc_language;
    public string dc_rights;
  }

  [XmlRpcUrl("http://www.oreillynet.com/meerkat/xml-rpc/server.php")]
  public interface IMeerkat
  {
    [XmlRpcMethod("meerkat.getCategories")]
    Category[] GetCategories();
    [XmlRpcMethod("meerkat.getChannels")]
    Channel[] GetChannels();
    [XmlRpcMethod("meerkat.getChannelsByCategory")]
    Channel[] GetChannelsByCategory(int id);
    [XmlRpcMethod("meerkat.getItems")]
    Result[] GetItems(Recipe recipe);
  }

}

