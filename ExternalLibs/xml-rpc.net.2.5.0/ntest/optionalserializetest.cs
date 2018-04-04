using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Threading;
using  NUnit.Framework;
using CookComputing.XmlRpc;

namespace ntest
{
  [TestFixture]
  public class OptionalSerializeTest
  {
    struct ChildStruct
    {
      public int x;
      public ChildStruct(int num) { x = num; }
    }

    struct Struct0
    {
      public XmlRpcInt xi;
      public XmlRpcBoolean xb;
      public XmlRpcDouble xd;
      public XmlRpcDateTime xdt;
#if !FX1_0
      public int? nxi;
      public bool? nxb;
      public double? nxd;
      public DateTime? nxdt;
      public ChildStruct? nxstr;
#endif
    }

    struct Struct1
    {
      public int mi;
      public string ms;
      public bool mb;
      public double md;
      public DateTime mdt;
      public byte[] mb64;
      public int[] ma;
      public XmlRpcInt xi;
      public XmlRpcBoolean xb;
      public XmlRpcDouble xd;
      public XmlRpcDateTime xdt;
      public XmlRpcStruct xstr;
#if !FX1_0
      public int? nxi;
      public bool? nxb;
      public double? nxd;
      public DateTime? nxdt;
      public ChildStruct? nxstr;
#endif
    }

    [XmlRpcMissingMapping(MappingAction.Error)]
    struct Struct2
    {
      public int mi;
      public string ms;
      public bool mb;
      public double md;
      public DateTime mdt;
      public byte[] mb64;
      public int[] ma;
      public XmlRpcInt xi;
      public XmlRpcBoolean xb;
      public XmlRpcDouble xd;
      public XmlRpcDateTime xdt;
      public XmlRpcStruct xstr;
#if !FX1_0
      public int? nxi;
      public bool? nxb;
      public double? nxd;
      public DateTime? nxdt;
      public ChildStruct? nxstr;
#endif
    }

    struct Struct3
    {
      [XmlRpcMissingMapping(MappingAction.Error)]
      public int mi;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public string ms;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public bool mb;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public double md;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public DateTime mdt;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public byte[] mb64;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public int[] ma;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public XmlRpcInt xi;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public XmlRpcBoolean xb;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public XmlRpcDouble xd;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public XmlRpcDateTime xdt;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public XmlRpcStruct xstr;
#if !FX1_0
      [XmlRpcMissingMapping(MappingAction.Error)]
      public int? nxi;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public bool? nxb;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public double? nxd;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public DateTime? nxdt;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public ChildStruct? nxstr;
#endif
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    struct Struct4
    {
      [XmlRpcMissingMapping(MappingAction.Error)]
      public int mi;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public string ms;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public bool mb;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public double md;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public DateTime mdt;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public byte[] mb64;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public int[] ma;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public XmlRpcInt xi;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public XmlRpcBoolean xb;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public XmlRpcDouble xd;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public XmlRpcDateTime xdt;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public XmlRpcStruct xstr;
#if !FX1_0
      [XmlRpcMissingMapping(MappingAction.Error)]
      public int? nxi;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public bool? nxb;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public double? nxd;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public DateTime? nxdt;
      [XmlRpcMissingMapping(MappingAction.Error)]
      public ChildStruct? nxstr;
#endif
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    struct Struct5
    {
      public int mi;
      public string ms;
      public bool mb;
      public double md;
      public DateTime mdt;
      public byte[] mb64;
      public int[] ma;
      public XmlRpcInt xi;
      public XmlRpcBoolean xb;
      public XmlRpcDouble xd;
      public XmlRpcDateTime xdt;
      public XmlRpcStruct xstr;
#if !FX1_0
      public int? nxi;
      public bool? nxb;
      public double? nxd;
      public DateTime? nxdt;
      public ChildStruct? nxstr;
#endif
    }

    struct Struct6
    {
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public int mi;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public string ms;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public bool mb;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public double md;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public DateTime mdt;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public byte[] mb64;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public int[] ma;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public XmlRpcInt xi;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public XmlRpcBoolean xb;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public XmlRpcDouble xd;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public XmlRpcDateTime xdt;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public XmlRpcStruct xstr;
#if !FX1_0
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public int? nxi;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public bool? nxb;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public double? nxd;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public DateTime? nxdt;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public ChildStruct? nxstr;
#endif
    }

    [XmlRpcMissingMapping(MappingAction.Error)]
    struct Struct7
    {
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public int mi;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public string ms;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public bool mb;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public double md;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public DateTime mdt;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public byte[] mb64;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public int[] ma;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public XmlRpcInt xi;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public XmlRpcBoolean xb;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public XmlRpcDouble xd;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public XmlRpcDateTime xdt;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public XmlRpcStruct xstr;
#if !FX1_0
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public int? nxi;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public bool? nxb;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public double? nxd;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public DateTime? nxdt;
      [XmlRpcMissingMapping(MappingAction.Ignore)]
      public ChildStruct? nxstr;
#endif
    }

    //-------------------------------------------------------------------------/
    [Test]
    public void Struct0_AllExist()
    {
      Struct0 strout = new Struct0();
      strout.xi = 1234;
      strout.xb = true;
      strout.xd = 1234.567;
      strout.xdt = new DateTime(2006, 8, 9, 10, 11, 13);
#if !FX1_0
      strout.nxi = 5678;
      strout.nxb = true;
      strout.nxd = 2345.678;
      strout.nxdt = new DateTime(2007, 9, 10, 11, 12, 14);
      strout.nxstr = new ChildStruct(567);
#endif
      XmlDocument xdoc = Utils.Serialize("Struct0_AllExist",
        strout, Encoding.UTF8, MappingAction.Error);
      Type parsedType, parsedArrayType;
      object obj = Utils.Parse(xdoc, typeof(Struct0), MappingAction.Error,
        out parsedType, out parsedArrayType);
      Assert.IsInstanceOfType(typeof(Struct0), obj);
      Struct0 strin = (Struct0)obj;
      Assert.AreEqual(strout.xi, strin.xi);
      Assert.AreEqual(strout.xb, strin.xb);
      Assert.AreEqual(strout.xd, strin.xd);
      Assert.AreEqual(strout.xdt, strin.xdt);
#if !FX1_0
      Assert.AreEqual(strout.nxi, strin.nxi);
      Assert.AreEqual(strout.nxb, strin.nxb);
      Assert.AreEqual(strout.nxd, strin.nxd);
      Assert.AreEqual(strout.nxdt, strin.nxdt);
      Assert.AreEqual(((ChildStruct)strout.nxstr).x, ((ChildStruct)strin.nxstr).x);
#endif
    }

    //-------------------------------------------------------------------------/
    [Test]
    [ExpectedException(typeof(XmlRpcMappingSerializeException))]
    public void Struct1_AllMissing_ErrorDefault()
    {
      XmlDocument xdoc = Utils.Serialize("Struct1_AllMissing_ErrorDefault",
        new Struct1(),
        Encoding.UTF8, MappingAction.Error);
    }

    [Test]
    public void Struct1_AllMissing_IgnoreDefault()
    {
      XmlDocument xdoc = Utils.Serialize("Struct1_AllMissing_IgnoreDefault",
        new Struct1(),
        Encoding.UTF8, MappingAction.Ignore);
    }

    //-------------------------------------------------------------------------/
    [Test]
    [ExpectedException(typeof(XmlRpcMappingSerializeException))]
    public void Struct2_AllMissing_ErrorError()
    {
      XmlDocument xdoc = Utils.Serialize(
        "Struct2_AllMissing_ErrorError",
        new Struct2(),
        Encoding.UTF8, MappingAction.Error);
    }

    [Test]
    [ExpectedException(typeof(XmlRpcMappingSerializeException))]
    public void Struct2_AllMissing_IgnoreError()
    {
      XmlDocument xdoc = Utils.Serialize(
        "Struct2_AllMissing_IgnoreError",
        new Struct2(),
        Encoding.UTF8, MappingAction.Ignore);
    }

    //-------------------------------------------------------------------------/
    [Test]
    [ExpectedException(typeof(XmlRpcMappingSerializeException))]
    public void Struct3_AllMissing_ErrorDefaultError()
    {
      XmlDocument xdoc = Utils.Serialize(
        "Struct3_AllMissing_ErrorDefaultError",
        new Struct3(),
        Encoding.UTF8, MappingAction.Error);
    }

    [Test]
    [ExpectedException(typeof(XmlRpcMappingSerializeException))]
    public void Struct3_AllMissing_IgnoreDefaultError()
    {
      XmlDocument xdoc = Utils.Serialize(
        "Struct3_AllMissing_IgnoreDefaultError",
        new Struct3(),
        Encoding.UTF8, MappingAction.Ignore);
    }

    //-------------------------------------------------------------------------/
    [Test]
    [ExpectedException(typeof(XmlRpcMappingSerializeException))]
    public void Struct4_AllMissing_ErrorIgnoreError()
    {
      XmlDocument xdoc = Utils.Serialize(
        "Struct4_AllMissing_ErrorIgnoreError",
        new Struct4(),
        Encoding.UTF8, MappingAction.Error);
    }

    [Test]
    [ExpectedException(typeof(XmlRpcMappingSerializeException))]
    public void Struct4_AllMissing_IgnoreIgnoreError()
    {
      XmlDocument xdoc = Utils.Serialize(
        "Struct4_AllMissing_IgnoreIgnoreError",
        new Struct4(),
        Encoding.UTF8, MappingAction.Ignore);
    }

    //-------------------------------------------------------------------------/
    [Test]
    public void Struct5_AllMissing_ErrorIgnoreDefault()
    {
      XmlDocument xdoc = Utils.Serialize(
        "Struct5_AllMissing_ErrorIgnoreDefault",
        new Struct5(),
        Encoding.UTF8, MappingAction.Error);
    }

    [Test]
    public void Struct5_AllMissing_IgnoreIgnoreDefault()
    {
      XmlDocument xdoc = Utils.Serialize(
        "Struct5_AllMissing_IgnoreIgnoreDefault",
        new Struct5(),
        Encoding.UTF8, MappingAction.Ignore);
    }

    //-------------------------------------------------------------------------/
    [Test]
    public void Struct6_AllMissing_ErrorDefaultIgnore()
    {
      XmlDocument xdoc = Utils.Serialize(
        "Struct6_AllMissing_ErrorDefaultIgnore",
        new Struct6(),
        Encoding.UTF8, MappingAction.Error);
    }

    [Test]
    public void Struct6_AllMissing_IgnoreDefaultIgnore()
    {
      XmlDocument xdoc = Utils.Serialize(
        "Struct6_AllMissing_IgnoreDefaultIgnore",
        new Struct6(),
        Encoding.UTF8, MappingAction.Ignore);
    }

    //-------------------------------------------------------------------------/
    [Test]
    public void Struct7_AllMissing_ErrorErrorIgnore()
    {
      XmlDocument xdoc = Utils.Serialize(
        "Struct7_AllMissing_ErrorErrorIgnore",
        new Struct7(),
        Encoding.UTF8, MappingAction.Error);
    }

    [Test]
    public void Struct7_AllMissing_IgnoreErrorIgnore()
    {
      XmlDocument xdoc = Utils.Serialize(
        "Struct7_AllMissing_IgnoreErrorIgnore",
        new Struct7(),
        Encoding.UTF8, MappingAction.Ignore);
    }
  }
}