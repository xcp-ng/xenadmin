                                                                                                                                                                                                                                                               
using System;
using CookComputing.XmlRpc;

namespace CookComputing.MovableType
{
  public struct Category
  {
    public string categoryId;
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string categoryName;
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public bool isPrimary;
  }

  public struct PostTitle
  {
    [XmlRpcMember(Description="This is in the timezone of the weblog blogid.")]
    public DateTime created;
    public string postid;
    public string userid;
    public string title;
  }

  public struct TrackbackPing
  {
    [XmlRpcMember(Description="The title of the entry sent in the ping.")]
    public string pingTitle;
    [XmlRpcMember(Description="The URL of the entry.")]
    public string pingURL;
    [XmlRpcMember(Description="The IP address of the host that sent the ping.")]
    public string pingIP;
  }

  public interface IMovableType
  {
    [XmlRpcMethod("mt.getCategoryList",
       Description="Returns a list of all categories defined in the weblog.")]
    [return: XmlRpcReturnValue(
       Description="The isPrimary member of each Category structs is not used.")]
    Category[] getCategoryList(
      string blogid,
      string username,
      string password);

    [XmlRpcMethod("mt.getPostCategories",
       Description="Returns a list of all categories to which the post is "
       + "assigned.")]
    Category[] getPostCategories(
      string postid,
      string username,
      string password);

    [XmlRpcMethod("mt.getRecentPostTitles",
       Description="Returns a bandwidth-friendly list of the most recent "
       + "posts in the system.")]
    PostTitle[] getRecentPostTitles(
      string blogid,
      string username,
      string password,
      int numberOfPosts);

    [XmlRpcMethod("mt.getTrackbackPings",
       Description="Retrieve the list of TrackBack pings posted to a "
       + "particular entry. This could be used to programmatically "
       + "retrieve the list of pings for a particular entry, then "
       + "iterate through each of those pings doing the same, until "
       + "one has built up a graph of the web of entries referencing "
       + "one another on a particular topic.")]
    TrackbackPing[] getTrackbackPings(
      string postid);

    [XmlRpcMethod("mt.publishPost",
       Description="Publish (rebuild) all of the static files related "
       + "to an entry from your weblog. Equivalent to saving an entry "
       + "in the system (but without the ping).")]
    [return: XmlRpcReturnValue(Description="Always returns true.")]
    bool publishPost(
      string postid,
      string username,
      string password);

    [XmlRpcMethod("mt.setPostCategories",
       Description="Sets the categories for a post.")]
    [return: XmlRpcReturnValue(Description="Always returns true.")]
    bool setPostCategories(
      string postid,
      string username,
      string password,
      [XmlRpcParameter(
         Description="categoryName not required in Category struct.")]
      Category[] categories);

    [XmlRpcMethod("mt.supportedMethods",
       Description="The method names supported by the server.")]
    [return: XmlRpcReturnValue(
               Description="The method names supported by the server.")]
    string[] supportedMethods();
  }
}


