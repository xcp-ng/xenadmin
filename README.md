XCP-ng Console
==============

This repository contains the source code for XCP-ng Console.

XCP-ng Console is a Windows-based management tool for XCP-ng and Citrix® XenServer® environments
which enables users to manage and monitor XCP-ng and Citrix® XenServer® hosts and resource pools,
and to deploy, monitor, manage and migrate virtual machines.

XCP-ng Console is written mostly in C#.

Contributions
-------------

The preferable way to contribute patches is to fork the repository on Github and
then submit a pull request. Also have a look at https://xcp-ng.org/forum.

License
-------

This code is licensed under the BSD 2-Clause license. Please see the
[LICENSE](LICENSE) file for more information.


Developer Build
---------------

You need:

* Source of this repository
* Source of dotnet-packages (https://github.com/borzel/dotnet-packages)
* Visual Studio Community 2017
* Cygwin

You should:

* Add `C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin` to PATH-Variable (buildscripts need to use MSBuild.exe from this path)

### Build dotnet-packages (3rd Party Libraries)


1.) Open cygwin-Konsole

2.) `cd dotnet-packages` (root folder)

3.) `./build.sh`

4.) The libraries should now be in `_build\output`


### Build


1.) Copy content of `dotnet-packages\_build\output\dotnet46` to `<path-to-repo>\packages`

2.) (optional XCP-ng branding) Open cygwin-Konsole, execute `<path-to-repo>/branding-xcp-ng/brand-to-xcp-ng.sh`

2.) Open `XenAdmin.sln`

3.) Build and enjoy
