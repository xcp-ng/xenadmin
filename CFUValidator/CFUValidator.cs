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

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CFUValidator.CommandLineOptions;
using CFUValidator.OutputDecorators;
using CFUValidator.Updates;
using CFUValidator.Validators;
using Moq;
using XenAdmin;
using XenAdmin.Alerts;
using XenAdmin.Core;
using XenAdminTests;
using XenAPI;

namespace CFUValidator
{
    public class CFUValidationException : Exception
    {
        public CFUValidationException(string message) : base(message){}
    }

    public class CFUValidator
    {
        private readonly MockObjectManager mom = new MockObjectManager();
        private readonly XmlRetrieverFactory xmlFactory = new XmlRetrieverFactory();
        private const string id = "id";
        private string XmlLocation { get; set; }
        private string ServerVersion { get; set; }
        private OptionUsage UrlOrFile { get; set; }
        private List<string> InstalledHotfixes { get; set; }
        private bool CheckHotfixContents{ get; set; }

        public CFUValidator(OptionUsage urlOrFile, string xmlLocation, string serverVersion,
                            List<string> installedHotfixes, bool checkHotfixContents)
        {
            if(urlOrFile != OptionUsage.File && urlOrFile != OptionUsage.Url)
                throw new ArgumentException("urlOrFile option should be either File or Url");
            
            mom.CreateNewConnection(id);
            ConnectionsManager.XenConnections.AddRange(mom.AllConnections);
            XmlLocation = xmlLocation;
            ServerVersion = serverVersion;
            InstalledHotfixes = installedHotfixes;
            UrlOrFile = urlOrFile;
            CheckHotfixContents = checkHotfixContents;
        }

        public void Run()
        {
            List<XenServerPatch> xenServerPatches; 
            List<XenServerVersion> xenServerVersions;
            List<XenCenterVersion> xenCenterVersions;
           
            Status = "Getting check for updates XML from " + XmlLocation + "...";
            ReadCheckForUpdatesXML(out xenServerPatches, out xenServerVersions, out xenCenterVersions);

            List<string> versionToCheck = GetVersionToCheck(xenServerVersions);

            foreach (string ver in versionToCheck)
            {
                ServerVersion = ver;
                RunTestsForGivenServerVersion(xenServerVersions, xenServerPatches, xenCenterVersions);
            }
            
        }


        private List<string> GetVersionToCheck(List<XenServerVersion> xenServerVersions)
        {
            if(ServerVersion == CFUCommandLineOptionManager.AllVersions)
                return xenServerVersions.ConvertAll(i => i.Version.ToString()).Distinct().ToList();
            
            return new List<string>{ServerVersion};
        }

        private void RunTestsForGivenServerVersion(List<XenServerVersion> xenServerVersions, 
                                                   List<XenServerPatch> xenServerPatches,
                                                   List<XenCenterVersion> xenCenterVersions)
        {
            CheckProvidedVersionNumber(xenServerVersions);
            
            Status = String.Format("Generating server {0} mock-ups...", ServerVersion);
            SetupMocks(xenServerPatches, xenServerVersions);

            Status = "Determining XenCenter update required...";
            var xcupdateAlerts = XenAdmin.Core.Updates.NewXenCenterUpdateAlerts(xenCenterVersions, new Version(ServerVersion));

            Status = "Determining XenServer update required...";
            var updateAlerts = XenAdmin.Core.Updates.NewXenServerVersionAlerts(xenServerVersions).Where(alert => !alert.CanIgnore).ToList();

            HfxEligibilityValidator hfxEligibilityValidator = new HfxEligibilityValidator(xenServerVersions);
            Status = "Running hotfix eligibility check...";
            RunHfxEligibilityValidator(hfxEligibilityValidator);

            Status = "Determining patches required...";
            var patchAlerts = XenAdmin.Core.Updates.NewXenServerPatchAlerts(xenServerVersions, xenServerPatches).Where(alert => !alert.CanIgnore).ToList();

            //Build patch checks list
            List<AlertFeatureValidator> validators = new List<AlertFeatureValidator>
                                                         {
                                                             new CorePatchDetailsValidator(patchAlerts),
                                                             new PatchURLValidator(patchAlerts),
                                                             new ZipContentsValidator(patchAlerts)
                                                         };

            Status = "Running patch check(s), this may take some time...";
            RunValidators(validators);
            
            Status = "Generating summary...";
            GeneratePatchSummary(patchAlerts, validators, hfxEligibilityValidator, updateAlerts, xcupdateAlerts);
        }

        private void CheckProvidedVersionNumber(List<XenServerVersion> xenServerVersions)
        {
            if (!xenServerVersions.Any(v => v.Version.ToString() == ServerVersion))
            {
                StringBuilder sb = new StringBuilder("\nAvailable versions are:\n");
                xenServerVersions.ConvertAll(i=>i.Version.ToString()).Distinct().ToList().ForEach(v=>sb.AppendLine(v));
                throw new CFUValidationException("Could not find the version in the check for updates file: " + ServerVersion + sb);
            }
        }

        public string Output { get; private set; }

        #region Status event code
        public delegate void StatusChangedHandler(object sender, EventArgs e);

        public event StatusChangedHandler StatusChanged;

        protected virtual void OnStatusChanged()
        {
            if (StatusChanged != null)
                StatusChanged(Status, EventArgs.Empty);
        }

        private string status;
        private string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnStatusChanged();
            }
        } 
        #endregion

        private void RunValidators(List<AlertFeatureValidator> validators)
        {
            int count = 1;
            foreach (AlertFeatureValidator validator in validators)
            {
                if (validator is ZipContentsValidator && !CheckHotfixContents)
                    continue;

                Status = count++ + ". " + validator.Description + "..."; 

                validator.StatusChanged += validator_StatusChanged;
                validator.Validate();
                validator.StatusChanged -= validator_StatusChanged;
            }
            Status = "Validator checks complete";
        }

        private void RunHfxEligibilityValidator(HfxEligibilityValidator validator)
        {
            validator.Validate();
            Status = "Hotfix Eligibility Validator check complete";
        }

        private void validator_StatusChanged(object sender, EventArgs e)
        {
            Status = sender as string;
        }

        private void GeneratePatchSummary(List<XenServerPatchAlert> alerts, List<AlertFeatureValidator> validators, HfxEligibilityValidator hfxEligibilityValidator,
                                          List<XenServerVersionAlert> updateAlerts, List<XenCenterUpdateAlert> xcupdateAlerts)
        {
            OuputComponent oc = new OutputTextOuputComponent(XmlLocation, ServerVersion);
            XenCenterUpdateDecorator xcud = new XenCenterUpdateDecorator(oc, xcupdateAlerts);
            XenServerUpdateDecorator xsud = new XenServerUpdateDecorator(xcud, updateAlerts);
            HfxEligibilityValidatorDecorator hevd = new HfxEligibilityValidatorDecorator(xsud, hfxEligibilityValidator, "Hotfix eligibility check:");
            PatchAlertDecorator pad = new PatchAlertDecorator(hevd, alerts);
            AlertFeatureValidatorDecorator afdCoreFields = new AlertFeatureValidatorDecorator(pad,
                                                                                              validators.First(v => v is CorePatchDetailsValidator),
                                                                                              "Core fields in patch checks:");
            AlertFeatureValidatorDecorator afdPatchUrl = new AlertFeatureValidatorDecorator(afdCoreFields, 
                                                                                            validators.First(v => v is PatchURLValidator),
                                                                                            "Required patch URL checks:");
            AlertFeatureValidatorDecorator afdZipContents = new AlertFeatureValidatorDecorator(afdPatchUrl,
                                                                                               validators.First(v => v is ZipContentsValidator),
                                                                                               "Required patch zip content checks:");

            if (CheckHotfixContents)
                Output = afdZipContents.Generate().Insert(0, Output).ToString();
            else
                Output = afdPatchUrl.Generate().Insert(0, Output).ToString();

        }

        private void ReadCheckForUpdatesXML(out List<XenServerPatch> patches, out List<XenServerVersion> versions, out List<XenCenterVersion> xcVersions)
        {
            ICheckForUpdatesXMLSource checkForUpdates = xmlFactory.GetAction(UrlOrFile, XmlLocation);
            checkForUpdates.RunAsync();
            
            ConsoleSpinner spinner = new ConsoleSpinner();
            while(!checkForUpdates.IsCompleted)
            {
                spinner.Turn(checkForUpdates.PercentComplete);
            }

            if (checkForUpdates.ErrorRaised != null)
                throw checkForUpdates.ErrorRaised;

            patches = checkForUpdates.XenServerPatches;
            versions = checkForUpdates.XenServerVersions;
            xcVersions = checkForUpdates.XenCenterVersions;
        }

        private void SetupMocks(List<XenServerPatch> xenServerPatches, List<XenServerVersion> xenServerVersions)
        {
            Mock<Host> master = mom.NewXenObject<Host>(id);
            Mock<Pool> pool = mom.NewXenObject<Pool>(id);
            XenRef<Host> masterRef = new XenRef<Host>("ref");
            pool.Setup(p => p.master).Returns(masterRef);
            pool.Setup(p => p.other_config).Returns(new Dictionary<string, string>());
            mom.MockCacheFor(id).Setup(c => c.Resolve(It.IsAny<XenRef<Pool>>())).Returns(pool.Object);
            mom.MockConnectionFor(id).Setup(c => c.Resolve(masterRef)).Returns(master.Object);
            mom.MockConnectionFor(id).Setup(c => c.IsConnected).Returns(true);
            master.Setup(h => h.software_version).Returns(new Dictionary<string, string>());
            master.Setup(h => h.ProductVersion()).Returns(ServerVersion);
            master.Setup(h => h.AppliedPatches()).Returns(GenerateMockPoolPatches(xenServerPatches));
            
            //Currently build number will be referenced first so if it's present hook it up
            string buildNumber = xenServerVersions.First(v => v.Version.ToString() == ServerVersion).BuildNumber;
            master.Setup(h=>h.BuildNumberRaw()).Returns(buildNumber);
        }

        private List<Pool_patch> GenerateMockPoolPatches(List<XenServerPatch> xenServerPatches)
        {
            List<Pool_patch> patches = new List<Pool_patch>();

            foreach (string installedHotfix in InstalledHotfixes)
            {
                string hotfix = installedHotfix;
                XenServerPatch match = xenServerPatches.Find(m => m.Name.Contains(hotfix));

                if(match == null)
                    throw new CFUValidationException("No patch could be found in the XML matching " + hotfix);

                Mock<Pool_patch> pp = mom.NewXenObject<Pool_patch>(id);
                pp.Setup(p => p.uuid).Returns(match.Uuid);
                patches.Add(pp.Object);
            }
            
            return patches;
        }

    }
}
