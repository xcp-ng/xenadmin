
XenCenter
=========

This repository contains the source code for XenCenter.

XenCenter is a Windows-based management tool for XenServer environments
which enables users to manage and monitor XenServer hosts and resource pools,
and to deploy, monitor, manage and migrate virtual machines.

XenCenter is written mostly in C#.

Contributions
-------------

The preferable way to contribute patches is to fork the repository on Github and
then submit a pull request. If for some reason you can't use Github to submit a
pull request, then you may send your patch for review to the

License
-------

This code is licensed under the BSD 2-Clause license. Please see the
[LICENSE](LICENSE) file for more information.

How to build XenCenter
----------------------

To build XenCenter, you need

* the source from xenadmin repository
* Visual Studio 2017 Community

(these should Visual Studio restore automatically per NuGet)
* Newtonsoft.Json.dll
* DiscUtils.dll
* ICSharpCode.SharpZipLib.dll
* Ionic.Zip.dll
* log4net.dll

You have to add those two libraries yourself (ZIP files in ExternalLibs folder):

* CookComputing.XmlRpcV2.dll (Extract zip file, dll is allready in folder xml-rpc.net.2.5.0\bin)
* 


(Only for testing purposes) you also need NUnit libraries 

* nunit.framework.dll
* Moq.dll

which can be obtained from <http://www.nunit.org/>.
