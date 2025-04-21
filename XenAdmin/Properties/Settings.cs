
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Xml.Serialization;
using XenAdmin.Core;

namespace XenAdmin.Properties
{


    public class Settings : INotifyPropertyChanged
    {
        private static string _settingsPath;
            

        private static Settings _default;
        private bool _toolbarsEnabled = true;
        private string[] _serverHistory = Array.Empty<string>();
        private bool _saveSession = true;
        private bool _localSRsVisible = true;
        private bool _defaultTemplatesVisible = false;
        private bool _userTemplatesVisible = true;
        private Size _windowSize = new System.Drawing.Size(1024, 769);
        private Point _windowLocation = new System.Drawing.Point(150, 20);
        private bool _requirePass = false;
        private bool _doUpgrade = true;
        private int _fullScreenShortcutKey = 3;
        private bool _windowsShortcuts = true;
        private bool _receiveSoundFromRdp = true;
        private bool _autoSwitchToRdp = true;
        private bool _preserveScaleWhenUndocked = true;
        private int _proxySetting = 0;
        private string _proxyAddress = string.Empty;
        private int _proxyPort = 80;
        private bool _preserveScaleWhenSwitchBackToVnc = true;
        private int _connectionTimeout = 20000;
        private int _httpTimeout = 40000;
        private bool _showHiddenVMs = false;
        private bool _clipboardAndPrinterRedirection = true;
        private int _dockShortcutKey = 1;
        private string[] _serverAddressList = Array.Empty<string>();
        private bool _connectToServerConsole = true;
        private bool _warnUnrecognizedCertificate = false;
        private string[] _knownServers = Array.Empty<string>();
        private bool _warnChangedCertificate = true;
        private bool _allowXenCenterUpdates = true;
        private bool _seenAllowUpdatesDialog = false;
        private bool _fillAreaUnderGraphs = false;
        private string _defaultSearch = null;
        private bool _loadPlugins = true;
        private string[] _cslgCredentials = Array.Empty<string>();
        private string _serverStatusPath;
        private bool _rollingUpgradeWizardShowFirstPage = false;
        private string _applicationVersion = string.Empty;
        private int _uncaptureShortcutKey = 0;
        private bool _drFailoverWizardShowFirstPage = true;
        private bool _pinConnectionBar = true;
        private bool _showAboutDialog = true;
        private bool _provideProxyAuthentication = false;
        private string _proxyUsername = string.Empty;
        private string _proxyPassword = string.Empty;
        private bool _bypassProxyForServers = false;
        private int _proxyAuthenticationMethod = 1;
        private bool _doNotConfirmDismissAlerts = false;
        private bool _doNotConfirmDismissEvents = false;
        private bool _doNotConfirmDismissUpdates = false;
        private string _helpLastUsed = string.Empty;
        private bool _ejectSharedIsoOnUpdate = false;
        private bool _showUpdatesByServer = false;
        private bool _rememberLastSelectedTab = true;
        private AutoCompleteStringCollection _vMwareServerHistory = new System.Windows.Forms.AutoCompleteStringCollection();
        private bool _conversionClientUseSsl = true;
        private bool _ignoreOvfValidationWarnings = false;
        private bool _remindChangePassword = true;
        private SshConsole _customSshConsole = SshConsole.None;
        private string _puttyLocation = string.Empty;
        private string _openSshLocation = string.Empty;
        private string[] _serverList = Array.Empty<string>();
        private string[] _disabledPlugins = Array.Empty<string>();
        private string[] _ignoreFirstRunWizards = Array.Empty<string>();
        private bool _showTimestampsInUpdatesLog = true;
        private string _fileServiceUsername = string.Empty;
        private string _fileServiceClientId = string.Empty;
        private bool _allowXenServerUpdates = false;
        private bool _allowPatchesUpdates = false;
        private bool _seenAllowCfuUpdatesDialog = false;
        private string _windowState = "Normal";

        public static Settings Default
        {
            get => _default;
            internal set => _default = value;
        }

        public bool ToolbarsEnabled
        {
            get => _toolbarsEnabled;
            set
            {
                if (value == _toolbarsEnabled) return;
                _toolbarsEnabled = value;
                OnPropertyChanged();
            }
        }

        public string[] ServerHistory
        {
            get => _serverHistory;
            set
            {
                if (Equals(value, _serverHistory)) return;
                _serverHistory = value;
                OnPropertyChanged();
            }
        }

        public bool SaveSession
        {
            get => _saveSession;
            set
            {
                if (value == _saveSession) return;
                _saveSession = value;
                OnPropertyChanged();
            }
        }

        public bool LocalSRsVisible
        {
            get => _localSRsVisible;
            set
            {
                if (value == _localSRsVisible) return;
                _localSRsVisible = value;
                OnPropertyChanged();
            }
        }

        public bool DefaultTemplatesVisible
        {
            get => _defaultTemplatesVisible;
            set
            {
                if (value == _defaultTemplatesVisible) return;
                _defaultTemplatesVisible = value;
                OnPropertyChanged();
            }
        }

        public bool UserTemplatesVisible
        {
            get => _userTemplatesVisible;
            set
            {
                if (value == _userTemplatesVisible) return;
                _userTemplatesVisible = value;
                OnPropertyChanged();
            }
        }

        public global::System.Drawing.Size WindowSize
        {
            get => _windowSize;
            set
            {
                if (value.Equals(_windowSize)) return;
                _windowSize = value;
                OnPropertyChanged();
            }
        }

        public global::System.Drawing.Point WindowLocation
        {
            get => _windowLocation;
            set
            {
                if (value.Equals(_windowLocation)) return;
                _windowLocation = value;
                OnPropertyChanged();
            }
        }

        public bool RequirePass
        {
            get => _requirePass;
            set
            {
                if (value == _requirePass) return;
                _requirePass = value;
                OnPropertyChanged();
            }
        }

        public bool DoUpgrade
        {
            get => _doUpgrade;
            set
            {
                if (value == _doUpgrade) return;
                _doUpgrade = value;
                OnPropertyChanged();
            }
        }

        public int FullScreenShortcutKey
        {
            get => _fullScreenShortcutKey;
            set
            {
                if (value == _fullScreenShortcutKey) return;
                _fullScreenShortcutKey = value;
                OnPropertyChanged();
            }
        }

        public bool WindowsShortcuts
        {
            get => _windowsShortcuts;
            set
            {
                if (value == _windowsShortcuts) return;
                _windowsShortcuts = value;
                OnPropertyChanged();
            }
        }

        public bool ReceiveSoundFromRDP
        {
            get => _receiveSoundFromRdp;
            set
            {
                if (value == _receiveSoundFromRdp) return;
                _receiveSoundFromRdp = value;
                OnPropertyChanged();
            }
        }

        public bool AutoSwitchToRDP
        {
            get => _autoSwitchToRdp;
            set
            {
                if (value == _autoSwitchToRdp) return;
                _autoSwitchToRdp = value;
                OnPropertyChanged();
            }
        }

        public bool PreserveScaleWhenUndocked
        {
            get => _preserveScaleWhenUndocked;
            set
            {
                if (value == _preserveScaleWhenUndocked) return;
                _preserveScaleWhenUndocked = value;
                OnPropertyChanged();
            }
        }

        public int ProxySetting
        {
            get => _proxySetting;
            set
            {
                if (value == _proxySetting) return;
                _proxySetting = value;
                OnPropertyChanged();
            }
        }

        public string ProxyAddress
        {
            get => _proxyAddress;
            set
            {
                if (value == _proxyAddress) return;
                _proxyAddress = value;
                OnPropertyChanged();
            }
        }

        public int ProxyPort
        {
            get => _proxyPort;
            set
            {
                if (value == _proxyPort) return;
                _proxyPort = value;
                OnPropertyChanged();
            }
        }

        public bool PreserveScaleWhenSwitchBackToVNC
        {
            get => _preserveScaleWhenSwitchBackToVnc;
            set
            {
                if (value == _preserveScaleWhenSwitchBackToVnc) return;
                _preserveScaleWhenSwitchBackToVnc = value;
                OnPropertyChanged();
            }
        }

        public int ConnectionTimeout
        {
            get => _connectionTimeout;
            set
            {
                if (value == _connectionTimeout) return;
                _connectionTimeout = value;
                OnPropertyChanged();
            }
        }

        public int HttpTimeout
        {
            get => _httpTimeout;
            set
            {
                if (value == _httpTimeout) return;
                _httpTimeout = value;
                OnPropertyChanged();
            }
        }

        public bool ShowHiddenVMs
        {
            get => _showHiddenVMs;
            set
            {
                if (value == _showHiddenVMs) return;
                _showHiddenVMs = value;
                OnPropertyChanged();
            }
        }

        public bool ClipboardAndPrinterRedirection
        {
            get => _clipboardAndPrinterRedirection;
            set
            {
                if (value == _clipboardAndPrinterRedirection) return;
                _clipboardAndPrinterRedirection = value;
                OnPropertyChanged();
            }
        }

        public int DockShortcutKey
        {
            get => _dockShortcutKey;
            set
            {
                if (value == _dockShortcutKey) return;
                _dockShortcutKey = value;
                OnPropertyChanged();
            }
        }

        public string[] ServerAddressList
        {
            get => _serverAddressList;
            set
            {
                if (Equals(value, _serverAddressList)) return;
                _serverAddressList = value;
                OnPropertyChanged();
            }
        }

        public bool ConnectToServerConsole
        {
            get => _connectToServerConsole;
            set
            {
                if (value == _connectToServerConsole) return;
                _connectToServerConsole = value;
                OnPropertyChanged();
            }
        }

        public bool WarnUnrecognizedCertificate
        {
            get => _warnUnrecognizedCertificate;
            set
            {
                if (value == _warnUnrecognizedCertificate) return;
                _warnUnrecognizedCertificate = value;
                OnPropertyChanged();
            }
        }

        public string[] KnownServers
        {
            get => _knownServers;
            set
            {
                if (Equals(value, _knownServers)) return;
                _knownServers = value;
                OnPropertyChanged();
            }
        }

        public bool WarnChangedCertificate
        {
            get => _warnChangedCertificate;
            set
            {
                if (value == _warnChangedCertificate) return;
                _warnChangedCertificate = value;
                OnPropertyChanged();
            }
        }

        public bool AllowXenCenterUpdates
        {
            get => _allowXenCenterUpdates;
            set
            {
                if (value == _allowXenCenterUpdates) return;
                _allowXenCenterUpdates = value;
                OnPropertyChanged();
            }
        }

        public bool SeenAllowUpdatesDialog
        {
            get => _seenAllowUpdatesDialog;
            set
            {
                if (value == _seenAllowUpdatesDialog) return;
                _seenAllowUpdatesDialog = value;
                OnPropertyChanged();
            }
        }

        public bool FillAreaUnderGraphs
        {
            get => _fillAreaUnderGraphs;
            set
            {
                if (value == _fillAreaUnderGraphs) return;
                _fillAreaUnderGraphs = value;
                OnPropertyChanged();
            }
        }

        public string DefaultSearch
        {
            get => _defaultSearch;
            set
            {
                if (value == _defaultSearch) return;
                _defaultSearch = value;
                OnPropertyChanged();
            }
        }

        public bool LoadPlugins
        {
            get => _loadPlugins;
            set
            {
                if (value == _loadPlugins) return;
                _loadPlugins = value;
                OnPropertyChanged();
            }
        }

        public string[] CslgCredentials
        {
            get => _cslgCredentials;
            set
            {
                if (Equals(value, _cslgCredentials)) return;
                _cslgCredentials = value;
                OnPropertyChanged();
            }
        }

        public string ServerStatusPath
        {
            get => _serverStatusPath;
            set
            {
                if (value == _serverStatusPath) return;
                _serverStatusPath = value;
                OnPropertyChanged();
            }
        }

        public bool RollingUpgradeWizardShowFirstPage
        {
            get => _rollingUpgradeWizardShowFirstPage;
            set
            {
                if (value == _rollingUpgradeWizardShowFirstPage) return;
                _rollingUpgradeWizardShowFirstPage = value;
                OnPropertyChanged();
            }
        }

        public string ApplicationVersion
        {
            get => _applicationVersion;
            set
            {
                if (value == _applicationVersion) return;
                _applicationVersion = value;
                OnPropertyChanged();
            }
        }

        public int UncaptureShortcutKey
        {
            get => _uncaptureShortcutKey;
            set
            {
                if (value == _uncaptureShortcutKey) return;
                _uncaptureShortcutKey = value;
                OnPropertyChanged();
            }
        }

        public bool DRFailoverWizardShowFirstPage
        {
            get => _drFailoverWizardShowFirstPage;
            set
            {
                if (value == _drFailoverWizardShowFirstPage) return;
                _drFailoverWizardShowFirstPage = value;
                OnPropertyChanged();
            }
        }

        public bool PinConnectionBar
        {
            get => _pinConnectionBar;
            set
            {
                if (value == _pinConnectionBar) return;
                _pinConnectionBar = value;
                OnPropertyChanged();
            }
        }

        public bool ShowAboutDialog
        {
            get => _showAboutDialog;
            set
            {
                if (value == _showAboutDialog) return;
                _showAboutDialog = value;
                OnPropertyChanged();
            }
        }

        public bool ProvideProxyAuthentication
        {
            get => _provideProxyAuthentication;
            set
            {
                if (value == _provideProxyAuthentication) return;
                _provideProxyAuthentication = value;
                OnPropertyChanged();
            }
        }

        public string ProxyUsername
        {
            get => _proxyUsername;
            set
            {
                if (value == _proxyUsername) return;
                _proxyUsername = value;
                OnPropertyChanged();
            }
        }

        public string ProxyPassword
        {
            get => _proxyPassword;
            set
            {
                if (value == _proxyPassword) return;
                _proxyPassword = value;
                OnPropertyChanged();
            }
        }

        public bool BypassProxyForServers
        {
            get => _bypassProxyForServers;
            set
            {
                if (value == _bypassProxyForServers) return;
                _bypassProxyForServers = value;
                OnPropertyChanged();
            }
        }

        public int ProxyAuthenticationMethod
        {
            get => _proxyAuthenticationMethod;
            set
            {
                if (value == _proxyAuthenticationMethod) return;
                _proxyAuthenticationMethod = value;
                OnPropertyChanged();
            }
        }

        public bool DoNotConfirmDismissAlerts
        {
            get => _doNotConfirmDismissAlerts;
            set
            {
                if (value == _doNotConfirmDismissAlerts) return;
                _doNotConfirmDismissAlerts = value;
                OnPropertyChanged();
            }
        }

        public bool DoNotConfirmDismissEvents
        {
            get => _doNotConfirmDismissEvents;
            set
            {
                if (value == _doNotConfirmDismissEvents) return;
                _doNotConfirmDismissEvents = value;
                OnPropertyChanged();
            }
        }

        public bool DoNotConfirmDismissUpdates
        {
            get => _doNotConfirmDismissUpdates;
            set
            {
                if (value == _doNotConfirmDismissUpdates) return;
                _doNotConfirmDismissUpdates = value;
                OnPropertyChanged();
            }
        }

        public string HelpLastUsed
        {
            get => _helpLastUsed;
            set
            {
                if (value == _helpLastUsed) return;
                _helpLastUsed = value;
                OnPropertyChanged();
            }
        }

        public bool EjectSharedIsoOnUpdate
        {
            get => _ejectSharedIsoOnUpdate;
            set
            {
                if (value == _ejectSharedIsoOnUpdate) return;
                _ejectSharedIsoOnUpdate = value;
                OnPropertyChanged();
            }
        }

        public bool ShowUpdatesByServer
        {
            get => _showUpdatesByServer;
            set
            {
                if (value == _showUpdatesByServer) return;
                _showUpdatesByServer = value;
                OnPropertyChanged();
            }
        }

        public bool RememberLastSelectedTab
        {
            get => _rememberLastSelectedTab;
            set
            {
                if (value == _rememberLastSelectedTab) return;
                _rememberLastSelectedTab = value;
                OnPropertyChanged();
            }
        }

        public global::System.Windows.Forms.AutoCompleteStringCollection VMwareServerHistory
        {
            get => _vMwareServerHistory;
            set
            {
                if (Equals(value, _vMwareServerHistory)) return;
                _vMwareServerHistory = value;
                OnPropertyChanged();
            }
        }

        public bool ConversionClientUseSsl
        {
            get => _conversionClientUseSsl;
            set
            {
                if (value == _conversionClientUseSsl) return;
                _conversionClientUseSsl = value;
                OnPropertyChanged();
            }
        }

        public bool IgnoreOvfValidationWarnings
        {
            get => _ignoreOvfValidationWarnings;
            set
            {
                if (value == _ignoreOvfValidationWarnings) return;
                _ignoreOvfValidationWarnings = value;
                OnPropertyChanged();
            }
        }

        public bool RemindChangePassword
        {
            get => _remindChangePassword;
            set
            {
                if (value == _remindChangePassword) return;
                _remindChangePassword = value;
                OnPropertyChanged();
            }
        }

        public global::XenAdmin.SshConsole CustomSshConsole
        {
            get => _customSshConsole;
            set
            {
                if (value == _customSshConsole) return;
                _customSshConsole = value;
                OnPropertyChanged();
            }
        }

        public string PuttyLocation
        {
            get => _puttyLocation;
            set
            {
                if (value == _puttyLocation) return;
                _puttyLocation = value;
                OnPropertyChanged();
            }
        }

        public string OpenSSHLocation
        {
            get => _openSshLocation;
            set
            {
                if (value == _openSshLocation) return;
                _openSshLocation = value;
                OnPropertyChanged();
            }
        }

        public string[] ServerList
        {
            get => _serverList;
            set
            {
                if (Equals(value, _serverList)) return;
                _serverList = value;
                OnPropertyChanged();
            }
        }

        public string[] DisabledPlugins
        {
            get => _disabledPlugins;
            set
            {
                if (Equals(value, _disabledPlugins)) return;
                _disabledPlugins = value;
                OnPropertyChanged();
            }
        }

        public string[] IgnoreFirstRunWizards
        {
            get => _ignoreFirstRunWizards;
            set
            {
                if (Equals(value, _ignoreFirstRunWizards)) return;
                _ignoreFirstRunWizards = value;
                OnPropertyChanged();
            }
        }

        public bool ShowTimestampsInUpdatesLog
        {
            get => _showTimestampsInUpdatesLog;
            set
            {
                if (value == _showTimestampsInUpdatesLog) return;
                _showTimestampsInUpdatesLog = value;
                OnPropertyChanged();
            }
        }

        public string FileServiceUsername
        {
            get => _fileServiceUsername;
            set
            {
                if (value == _fileServiceUsername) return;
                _fileServiceUsername = value;
                OnPropertyChanged();
            }
        }

        public string FileServiceClientId
        {
            get => _fileServiceClientId;
            set
            {
                if (value == _fileServiceClientId) return;
                _fileServiceClientId = value;
                OnPropertyChanged();
            }
        }

        public bool AllowXenServerUpdates
        {
            get => _allowXenServerUpdates;
            set
            {
                if (value == _allowXenServerUpdates) return;
                _allowXenServerUpdates = value;
                OnPropertyChanged();
            }
        }

        public bool AllowPatchesUpdates
        {
            get => _allowPatchesUpdates;
            set
            {
                if (value == _allowPatchesUpdates) return;
                _allowPatchesUpdates = value;
                OnPropertyChanged();
            }
        }

        public bool SeenAllowCfuUpdatesDialog
        {
            get => _seenAllowCfuUpdatesDialog;
            set
            {
                if (value == _seenAllowCfuUpdatesDialog) return;
                _seenAllowCfuUpdatesDialog = value;
                OnPropertyChanged();
            }
        }

        public string WindowState
        {
            get => _windowState;
            set
            {
                if (value == _windowState) return;
                _windowState = value;
                OnPropertyChanged();
            }
        }

        private static string _settingsRootFolder;

        public static string SettingsPath
        {
            get => _settingsRootFolder;
            set
            {
                _settingsRootFolder = value;
                _settingsPath = Path.Combine(_settingsRootFolder, BrandManager.ProductBrand, BrandManager.BrandConsole, "Settings.xml");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event SettingChangingEventHandler SettingChanging;

        internal static void Load()
        {
            if (!File.Exists(_settingsPath))
            {
                Default = new Settings();
                return;
            }

            using (var fs = new FileStream(_settingsPath, FileMode.Open))
            {
                XmlSerializer x = new XmlSerializer(typeof(Settings));
                Default = (Settings)x.Deserialize(fs);
            }
        }

        internal void Save()
        {
            using (var fs = new FileStream(_settingsPath, FileMode.Create))
            {
                XmlSerializer x = new XmlSerializer(typeof(Settings));
                x.Serialize(fs, Default);
            }
        }

        internal void Upgrade()
        {
        }
    }
}
