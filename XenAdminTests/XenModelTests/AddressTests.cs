using System.Collections.Generic;
using NUnit.Framework;
using XenAdmin.Core;

namespace XenAdminTests.XenModelTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class AddressTests
    {
        private readonly Dictionary<string, string> _simpleNetworks = new Dictionary<string, string>
        {
            {"0/ip", "10.71.57.53"},
            {"0/ipv4/0", "10.71.57.53"},
            {"0/ipv6/0", "fe80:0000:0000:0000:1c1a:c2ff:fe2e:c823"}
        };

        private readonly Dictionary<string, string> _compoundNetworks = new Dictionary<string, string>
        {
            {"0/ip", "192.168.0.1\n192.168.0.2"},
            {"1/ip", "192.168.0.3%n192.168.0.4"}
        };

        [Test]
        public void TestSimple()
        {
            var expected = new List<string>
            {
                "10.71.57.53",
                "fe80:0000:0000:0000:1c1a:c2ff:fe2e:c823"
            };
            var actual = Helpers.FindIpAddresses(_simpleNetworks, "0");
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void TestUnusedDevice()
        {
            var actual = Helpers.FindIpAddresses(_simpleNetworks, "1");
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void TestCompoundNetworksNewlineSeparated()
        {
            var expected = new List<string>
            {
                "192.168.0.1",
                "192.168.0.2"
            };
            var actual = Helpers.FindIpAddresses(_compoundNetworks, "0");
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void TestCompoundNetworkPercentNSeparated()
        {
            var expected = new List<string>
            {
                "192.168.0.3",
                "192.168.0.4"
            };
            var actual = Helpers.FindIpAddresses(_compoundNetworks, "1");
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void TestCompoundNetworkSkipsBlanks()
        {
            var given = new Dictionary<string, string>
            {
                {"0/ip", "10.0.0.24\n\n10.0.0.26"}
            };
            var expected = new List<string>
            {
                "10.0.0.24",
                "10.0.0.26"
            };
            var actual = Helpers.FindIpAddresses(given, "0");
            Assert.That(actual, Is.EquivalentTo(expected));
        }
    }
}
