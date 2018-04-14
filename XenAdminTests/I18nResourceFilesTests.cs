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

using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace XenAdminTests
{
	[TestFixture, Category(TestCategories.UICategoryA)]
	public class ResourceFilesTests
	{
		/// <summary>
		/// Assemblies to check
		/// </summary>
		private string[] m_assemblyIds = { "XenCenterMain", "XenModel", "XenOvf", "XenOvfTransport" };

	    /// <summary>
	    /// Available cultures (japanese and simplified chinese at the moment)
	    /// </summary>
	    private string[] m_locales = {"ja", "zh-CN"};

	    private string[] m_excludeFromCheck = {"XenAdmin.Help.HelpManager"};

		/// <summary>
		/// Test checks all resx files in the project have their i18n counterparts in place
		/// </summary>
        [Test]
        public void TestEnsureI18nFilesInPlace()
        {
            var xenAdminTests = Assembly.GetExecutingAssembly();
            var assemblies = GetAssembliesRecursively(xenAdminTests);

            Assert.IsNotEmpty(assemblies, "Assemblies to check are found");
            var missingSb = new StringBuilder();
            var extraSb = new StringBuilder();

            foreach (var assembly in assemblies)
            {
                List<string> defaultResx = new List<string>(assembly.GetManifestResourceNames().Where(resource => resource.EndsWith("resources")));

                foreach (string locale in m_locales)
                {
                    CultureInfo cultureInfo = new CultureInfo(locale);
                    Assembly localeDll = assembly.GetSatelliteAssembly(cultureInfo);
                    List<string> localeResx = new List<string>(localeDll.GetManifestResourceNames());

                    foreach (string def in defaultResx)
                    {
                        var name = def.Substring(0, def.Length - ".resources".Length);
                        var exclude = m_excludeFromCheck.Contains(name);

                        string localName = string.Format("{0}.{1}.resources", name, locale);
                        var localized = localeResx.Contains(localName);

                        if (localized && exclude)
                           extraSb.AppendLine(localName);
                        else if (!localized && !exclude)
                            missingSb.AppendLine(localName);
                    }
                }
            }

            Assert.That(missingSb.ToString(), Is.Null.Or.Empty); //Missing resources detected.
            Assert.That(extraSb.ToString(), Is.Null.Or.Empty); //"Unecessary resources detected"
        }

	    #region Auxiliary private methods

		/// <summary>
		/// Some of the assemblies we need to check may not be in the manifest of the assembly we're
		/// looking at, but are referenced by assemblies in the manifest. This method retrieves them.
		/// </summary>
		/// <param name="startAssembly"></param>
		/// <returns></returns>
		private List<Assembly> GetAssembliesRecursively(Assembly startAssembly)
		{
			var temp = new List<Assembly>();

			AssemblyName[] assemblyNames = startAssembly.GetReferencedAssemblies();

			var assemblyNamesToCheck = from name in assemblyNames
									   where m_assemblyIds.Contains(name.Name)
									   select name;

			foreach (var assemblyName in assemblyNamesToCheck)
			{
				var assembly = Assembly.Load(assemblyName);

				if (!temp.Contains(assembly))
				{
					temp.Add(assembly);

					var secAssemblies = GetAssembliesRecursively(assembly);

					foreach (var secAssembly in secAssemblies)
					{
						if (!temp.Contains(secAssembly))
							temp.Add(secAssembly);
					}
				}
			}

			return temp;
		}

		#endregion
	}
}

