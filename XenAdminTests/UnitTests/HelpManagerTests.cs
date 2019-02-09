/* Copyright (c) Citrix Systems, Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using NUnit.Framework;
using XenAdmin.Help;

namespace XenAdminTests.UnitTests
{
    //[TestFixture, Category(TestCategories.Unit)]
    //public class HelpManagerTests
    //{
    //    //As per current InvisibleMessages.HELP_URL.
    //    private const string HelpUrl =
    //        "http://docs.citrix.com/{0}/xencenter/current-release/{1}.html?utm_campaign={2}&utm_medium={3}&utm_source={4}";
    //    private const string HelpUrlUpper = "UPPER_HELP_URL/{0}/{1}/{2}/{3}/{4}";

    //    //As per InvisibleMessages.LOCALE.
    //    private const string Locale = "en-US";

    //    private const string Campaign = "7.9.50.6753";
    //    private const string Medium = "ui_link";

    //    //As per Messages.XENCENTER.
    //    private const string Source = "XenCenter";

    //    [TestCase(null, HelpUrl, Locale, Campaign, Medium, Source,
    //        Result = "http://docs.citrix.com/en-us/xencenter/current-release/index.html?utm_campaign=7_9_50_6753&utm_medium=ui_link&utm_source=xencenter",
    //        Description = "No topic defaults to index page")]
    //    [TestCase(null, HelpUrl, "ja-JP", Campaign, Medium, Source,
    //        Result = "http://docs.citrix.com/ja-jp/xencenter/current-release/index.html?utm_campaign=7_9_50_6753&utm_medium=ui_link&utm_source=xencenter",
    //        Description = "Japanese locale is handled correctly")]
    //    [TestCase("tabs", HelpUrl, Locale, Campaign, Medium, Source,
    //        Result = "http://docs.citrix.com/en-us/xencenter/current-release/tabs.html?utm_campaign=7_9_50_6753&utm_medium=ui_link&utm_source=xencenter",
    //        Description = "Given topic is reflected in result")]
    //    [TestCase(null, HelpUrl, Locale, "10.0.0.9999", Medium, Source,
    //        Result = "http://docs.citrix.com/en-us/xencenter/current-release/index.html?utm_campaign=10_0_0_9999&utm_medium=ui_link&utm_source=xencenter",
    //        Description = "Given campaign version is reflected in result")]
    //    [TestCase("UPPER_TOPIC", HelpUrlUpper, "UPPER_LOCALE", "UPPER_CAMPAIGN", "UPPER_MEDIUM", "UPPER_SOURCE",
    //        Result = "upper_help_url/upper_locale/upper_topic/upper_campaign/upper_medium/upper_source",
    //        Description = "Ensure the URL is in lower case")]
    //    public string TestProduceUrl(string topicId, string helpUrl, string locale, string campaign, string medium, string source)
    //    {
    //        return HelpManager.ProduceUrl(topicId, helpUrl, locale, campaign, medium, source);
    //    }
    //}
}
