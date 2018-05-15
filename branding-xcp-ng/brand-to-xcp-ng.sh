#!/bin/sh

# Copyright (c) Citrix Systems, Inc. 
# All rights reserved.
# 
# Redistribution and use in source and binary forms, 
# with or without modification, are permitted provided 
# that the following conditions are met: 
# 
# *   Redistributions of source code must retain the above 
#     copyright notice, this list of conditions and the 
#     following disclaimer. 
# *   Redistributions in binary form must reproduce the above 
#     copyright notice, this list of conditions and the 
#     following disclaimer in the documentation and/or other 
#     materials provided with the distribution. 
# 
# THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
# CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
# INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
# MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
# DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
# CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
# SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
# BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
# SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
# INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
# WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
# NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
# OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
# SUCH DAMAGE.

set -ex

SET_ENV_FILE="/cygdrive/c/env.sh"
if [ -f ${SET_ENV_FILE} ]; then
   . ${SET_ENV_FILE}
fi

ROOT="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
REPO="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SCRATCH_DIR=${ROOT}/scratch
OUTPUT_DIR=${ROOT}/output

WIX_INSTALLER_DEFAULT_GUID=65AE1345-A520-456D-8A19-2F52D43D3A09
WIX_INSTALLER_DEFAULT_VERSION=1.0.0
PRODUCT_GUID=$(uuidgen | tr [a-z] [A-Z])

# Create some dummy files and folders
mkdir -p ${REPO}/Branding/Hotfixes

# Overwrite some files
cp ${REPO}/branding-xcp-ng/HomePage/HomePage.mht ${REPO}/XenAdmin/
cp ${REPO}/branding-xcp-ng/Images/* ${REPO}/Branding/Images/


#######################################################################
#
#		Branding/branding.sh
#
#######################################################################

BRANDING_COMPANY_NAME_LEGAL="XCP New Generation"
BRANDING_COMPANY_NAME_SHORT=XCP-ng
BRANDING_COPYRIGHT=\"Copyright\ ©\ ${BRANDING_COMPANY_NAME_LEGAL}\"
BRANDING_COPYRIGHT_2=\"Copyright\ \\\\251\ ${BRANDING_COMPANY_NAME_LEGAL}\"
BRANDING_PRODUCT_BRAND=XCP-ng
BRANDING_COMPANY_URL=xcp-ng.org

BRANDING_PRODUCT_VERSION_TEXT=7.4.2-RC3
BRANDING_PRODUCT_MAJOR_VERSION=7
BRANDING_PRODUCT_MINOR_VERSION=4
BRANDING_PRODUCT_MICRO_VERSION=2

BRANDING_SEARCH=xensearch
BRANDING_UPDATE=xsupdate
BRANDING_BACKUP=xbk
BRANDING_SERVER=${BRANDING_PRODUCT_BRAND}
BRANDING_BRAND_CONSOLE="XCP-ng Center"

BRANDING_PERF_ALERT_MAIL_LANGUAGE_DEFAULT=en-US


BRANDING_XC_PRODUCT_VERSION=${BRANDING_PRODUCT_MAJOR_VERSION}.${BRANDING_PRODUCT_MINOR_VERSION}.${BRANDING_PRODUCT_MICRO_VERSION}
BRANDING_XC_PRODUCT_5_6_VERSION=5.6
BRANDING_XC_PRODUCT_6_0_VERSION=6.0
BRANDING_XC_PRODUCT_6_2_VERSION=6.2
BRANDING_XC_PRODUCT_6_5_VERSION=6.5
BRANDING_XC_PRODUCT_7_0_VERSION=7.0
BRANDING_XENSERVER_UPDATE_URL="https://raw.githubusercontent.com/xcp-ng/xenadmin/master/updates.xml"
BRANDING_HIDDEN_FEATURES=""
BRANDING_ADDITIONAL_FEATURES=""

#GUID
BRANDING_XENCENTER_UPGRADE_CODE_GUID=EA0EF50F-5CC6-452B-B09F-3F5EC564899D
BRANDING_JA_RESOURCES_GUID=D3ADD803-AF0B-4787-AC29-C6387FFF403B
BRANDING_SC_RESOURCES_GUID=381e9319-f0c4-4c69-a1c2-0a2fc725bd19
BRANDING_REPORT_VIEWER_GUID=D01090B9-1988-4ab4-B48A-D0B6161FAA48
BRANDING_MAIN_EXECUTABLE_GUID=64FEF765-7593-4612-8D4D-EE81CF704DEB
BRANDING_TEST_RESOURCES_GUID=FA8D4F56-A94A-467c-9E6B-F3DC26F95B1E
BRANDING_EXTERNAL_TOOLS_GUID=D5FC0252-C97B-46e7-9633-A6B68EDB6654
BRANDING_SCHEMAS_FILES_GUID=E2186CD8-5064-4414-8AD7-E4495B6A3204
BRANDING_REGISTRY_ENTRIES_GUID=193BAE1F-F2AE-4451-94DC-4B105DB5179C
BRANDING_APPLICAION_SHOTCUT_GUID=6B875059-26BC-4fa7-ACB7-0B9A4E4665CA
BRANDING_README_FILE_GUID=47427a60-4064-4fdb-878d-04309a0fd9ce
BRANDING_XSUPDATE_FILE_GUID=1cfbf607-cc80-4bf8-b2fc-37e69c872316
BRANDING_HEALTH_CHECK_GUID=9D686BFC-B4FD-435F-AC74-0ACE29425095


#######################################################################
#
#		mk/re-branding.sh
#
#######################################################################

if [ -z "$BUILD_NUMBER" ]; then
    echo "Need to set BUILD_NUMBER"
    exit 1
fi


version_cpp()
{
  num=$(echo "${BRANDING_XC_PRODUCT_VERSION}.${BUILD_NUMBER}" | sed 's/\./, /g')
  sed -b -i -e "s/1,0,0,1/${num}/g" \
      -e "s/1, 0, 0, 1/${num}/g" \
      -e "s/@BUILD_NUMBER@/${BUILD_NUMBER}/g" \
      $1 
}

version_csharp()
{
  sed -b -i -e "s/0\.0\.0\.0/${BRANDING_XC_PRODUCT_VERSION}.${BUILD_NUMBER}/g" \
	  -e "s/0000/${BRANDING_XC_PRODUCT_VERSION}.${BUILD_NUMBER}/g" \
      $1 
}

rebranding_global()
{
    sed -b -i -e "s#\[BRANDING_COMPANY_NAME_LEGAL\]#${BRANDING_COMPANY_NAME_LEGAL}#g" \
        -e "s#\[Citrix\]#${BRANDING_COMPANY_NAME_SHORT}#g" \
        -e "s#\"\[BRANDING_COPYRIGHT\]\"#${BRANDING_COPYRIGHT}#g" \
        -e "s#\"\[BRANDING_COPYRIGHT_2\]\"#${BRANDING_COPYRIGHT_2}#g" \
        -e "s#\[XenServer product\]#${BRANDING_PRODUCT_BRAND}#g" \
        -e "s#\[BRANDING_PRODUCT_VERSION\]#${BRANDING_XC_PRODUCT_VERSION}#g" \
        -e "s#\[BRANDING_PRODUCT_VERSION_TEXT\]#${BRANDING_PRODUCT_VERSION_TEXT}#g" \
        -e "s#\[BRANDING_BUILD_NUMBER\]#${BUILD_NUMBER}#g" \
        -e "s#\[xensearch\]#${BRANDING_SEARCH}#g" \
        -e "s#\[xsupdate\]#${BRANDING_UPDATE}#g" \
        -e "s#\[XenServer\]#${BRANDING_SERVER}#g" \
        -e "s#\[XenCenter\]#${BRANDING_BRAND_CONSOLE}#g" \
        -e "s#\[xbk\]#${BRANDING_BACKUP}#g" \
        -e "s#\[BRANDING_VERSION_5_6\]#${BRANDING_XC_PRODUCT_5_6_VERSION}#g" \
        -e "s#\[BRANDING_VERSION_6_0\]#${BRANDING_XC_PRODUCT_6_0_VERSION}#g" \
        -e "s#\[BRANDING_VERSION_6_2\]#${BRANDING_XC_PRODUCT_6_2_VERSION}#g" \
        -e "s#\[BRANDING_VERSION_6_5\]#${BRANDING_XC_PRODUCT_6_5_VERSION}#g" \
        -e "s#\[BRANDING_VERSION_7_0\]#${BRANDING_XC_PRODUCT_7_0_VERSION}#g" \
        -e "s#\[BRANDING_XENSERVER_UPDATE_URL\]#${BRANDING_XENSERVER_UPDATE_URL}#g" \
        -e "s#\[BRANDING_PERF_ALERT_MAIL_LANGUAGE_DEFAULT\]#${BRANDING_PERF_ALERT_MAIL_LANGUAGE_DEFAULT}#g" \
        $1    
}

rebranding_features()
{
  sed -b -i -e "s#\[BRANDING_HIDDEN_FEATURES\]#${BRANDING_HIDDEN_FEATURES}#g" \
	  -e "s#\[BRANDING_ADDITIONAL_FEATURES\]#${BRANDING_ADDITIONAL_FEATURES}#g" \
	  $1   
}

rebranding_GUID()
{
  sed -b -i \
      -e "s#\[BRANDING_XENCENTER_UPGRADE_CODE_GUID\]#${BRANDING_XENCENTER_UPGRADE_CODE_GUID}#g" \
      -e "s#\[BRANDING_JA_RESOURCES_GUID\]#${BRANDING_JA_RESOURCES_GUID}#g" \
      -e "s#\[BRANDING_SC_RESOURCES_GUID\]#${BRANDING_SC_RESOURCES_GUID}#g" \
      -e "s#\[BRANDING_REPORT_VIEWER_GUID\]#${BRANDING_REPORT_VIEWER_GUID}#g" \
      -e "s#\[BRANDING_MAIN_EXECUTABLE_GUID\]#${BRANDING_MAIN_EXECUTABLE_GUID}#g" \
      -e "s#\[BRANDING_TEST_RESOURCES_GUID\]#${BRANDING_TEST_RESOURCES_GUID}#g" \
      -e "s#\[BRANDING_EXTERNAL_TOOLS_GUID\]#${BRANDING_EXTERNAL_TOOLS_GUID}#g" \
      -e "s#\[BRANDING_SCHEMAS_FILES_GUID\]#${BRANDING_SCHEMAS_FILES_GUID}#g" \
      -e "s#\[BRANDING_REGISTRY_ENTRIES_GUID\]#${BRANDING_REGISTRY_ENTRIES_GUID}#g" \
      -e "s#\[BRANDING_APPLICAION_SHOTCUT_GUID\]#${BRANDING_APPLICAION_SHOTCUT_GUID}#g" \
      -e "s#\[BRANDING_README_FILE_GUID\]#${BRANDING_README_FILE_GUID}#g" \
      -e "s#\[BRANDING_XSUPDATE_FILE_GUID\]#${BRANDING_XSUPDATE_FILE_GUID}#g" \
      -e "s#\[BRANDING_HEALTH_CHECK_GUID\]#${BRANDING_HEALTH_CHECK_GUID}#g" \
      $1   
}

version_brand_cpp()
{
  for file in $1
  do
    version_cpp ${file} && rebranding_global ${file}
  done
}

branding_wxs()
{
  for file in $1
  do
    rebranding_global ${file} && rebranding_features ${file} && rebranding_GUID ${file}
  done
}

version_brand_csharp()
{
  for projectName in $1
  do
    assemblyInfo=${REPO}/${projectName}/Properties/AssemblyInfo.cs
    version_csharp ${assemblyInfo} && rebranding_global ${assemblyInfo}
  done
}

RESX_rebranding()
{
  for resx in $1
  do
    rebranding_global ${resx}.resx
    rebranding_global ${resx}.zh-CN.resx
    rebranding_global ${resx}.ja.resx
  done  
}

#splace rebranding
version_brand_cpp "${REPO}/splash/splash.rc ${REPO}/splash/main.cpp ${REPO}/splash/splash.vcproj ${REPO}/splash/splash.vcxproj  ${REPO}/splash/util.cpp"

#projects sign change
cd ${REPO} && /usr/bin/find -name \*.csproj -exec sed -i 's#<SignManifests>false#<SignManifests>true#' {} \;

#AssemblyInfo rebranding
version_brand_csharp "XenAdmin CommandLib XenCenterLib XenModel XenOvfApi XenOvfTransport XenCenterVNC xe xva_verify XenServer XenServerHealthCheck"

#XenAdmin rebranding
rebranding_global ${REPO}/XenAdmin/Branding.cs
#XenAdmin controls
XENADMIN_RESXS=$(/usr/bin/find ${REPO}/XenAdmin -name \*.resx)
for XENADMIN_RESX in ${XENADMIN_RESXS}
do
    rebranding_global ${XENADMIN_RESX}
done
#xenadmin resouces
RESX_rebranding "${REPO}/XenAdmin/Properties/Resources"
rebranding_global ${REPO}/XenAdmin/app.config

#XenModel rebranding
RESX_rebranding "${REPO}/XenModel/Messages ${REPO}/XenModel/InvisibleMessages ${REPO}/XenModel/FriendlyNames ${REPO}/XenModel/XenAPI/FriendlyErrorNames"

#XenOvfApi rebranding
RESX_rebranding "${REPO}/XenOvfApi/Messages ${REPO}/XenOvfApi/Content"
rebranding_global ${REPO}/XenOvfApi/app.config

#XenOvfTransport XenOvfTransport
RESX_rebranding ${REPO}/XenOvfTransport/Messages
rebranding_global ${REPO}/XenOvfTransport/app.config

#XenAdminTests
rebranding_global ${REPO}/XenAdminTests/TestResources/ContextMenuBuilderTestResults.xml
rebranding_global ${REPO}/XenAdminTests/app.config
rebranding_global ${REPO}/XenAdminTests/TestResources/state1.treeview.serverview.xml
rebranding_global ${REPO}/XenAdminTests/TestResources/state1.treeview.orgview.xml
rebranding_global ${REPO}/XenAdminTests/TestResources/searchresults.xml
rebranding_global ${REPO}/XenAdminTests/TestResources/state3.xml
rebranding_global ${REPO}/XenAdminTests/XenAdminTests.csproj
echo cp ${REPO}/XenAdminTests/TestResources/succeed.[xsupdate] ${REPO}/XenAdminTests/TestResources/succeed.${BRANDING_UPDATE}
cp ${REPO}/XenAdminTests/TestResources/succeed.[xsupdate] ${REPO}/XenAdminTests/TestResources/succeed.${BRANDING_UPDATE}

#XenServerHealthCheck
rebranding_global ${REPO}/XenServerHealthCheck/Branding.cs
rebranding_global ${REPO}/XenServerHealthCheck/app.config



set +u

