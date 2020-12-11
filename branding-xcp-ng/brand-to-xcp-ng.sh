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


if [ -z "$BUILD_NUMBER" ]; then
    echo "No BUILD_NUMBER set, assume developer build: set BUILD_NUMBER to 99"
	BUILD_NUMBER=99
fi


SET_ENV_FILE="/cygdrive/c/env.sh"
if [ -f ${SET_ENV_FILE} ]; then
   . ${SET_ENV_FILE}
fi

ROOT="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
REPO="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SCRATCH_DIR=${ROOT}/scratch
OUTPUT_DIR=${ROOT}/output

# Create some dummy files and folders
mkdir -p ${REPO}/Branding/Hotfixes

# Overwrite some files
cp ${REPO}/branding-xcp-ng/HomePage/HomePage.mht ${REPO}/Branding/
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
BRANDING_PV_TOOLS=${BRANDING_COMPANY_NAME_SHORT}\ VM\ Tools

BRANDING_PRODUCT_MAJOR_VERSION=20
BRANDING_PRODUCT_MINOR_VERSION=04
BRANDING_PRODUCT_MICRO_VERSION=01
BRANDING_PRODUCT_VERSION_TEXT="${BRANDING_PRODUCT_MAJOR_VERSION}.${BRANDING_PRODUCT_MINOR_VERSION}.${BRANDING_PRODUCT_MICRO_VERSION}"


BRANDING_XC_PRODUCT_VERSION=${BRANDING_PRODUCT_MAJOR_VERSION}.${BRANDING_PRODUCT_MINOR_VERSION}.${BRANDING_PRODUCT_MICRO_VERSION}
BRANDING_XC_PRODUCT_5_6_VERSION=5.6
BRANDING_XC_PRODUCT_6_0_VERSION=6.0
BRANDING_XC_PRODUCT_6_2_VERSION=6.2
BRANDING_XC_PRODUCT_6_5_VERSION=6.5
BRANDING_XC_PRODUCT_7_0_VERSION=7.0
BRANDING_XC_PRODUCT_7_1_2_VERSION=7.1.2
BRANDING_XC_PRODUCT_8_0_VERSION=8.0


BRANDING_SEARCH=xensearch
BRANDING_UPDATE=xsupdate
BRANDING_BACKUP=xbk
BRANDING_LEGACY_PRODUCT_BRAND=XCP-ng
BRANDING_SERVER=${BRANDING_PRODUCT_BRAND}
BRANDING_COMPANY_AND_PRODUCT=${BRANDING_PRODUCT_BRAND}
BRANDING_BRAND_CONSOLE="XCP-ng Center"
BRANDING_BRAND_CONSOLE_TITLE="${BRANDING_BRAND_CONSOLE} ${BRANDING_PRODUCT_VERSION_TEXT}"
BRANDING_PERF_ALERT_MAIL_LANGUAGE_DEFAULT=en-US
BRANDING_PV_TOOLS=${BRANDING_COMPANY_NAME_SHORT}\ VM\ Tools

BRANDING_XENSERVER_UPDATE_URL="https://raw.githubusercontent.com/xcp-ng/xenadmin-updates/master/updates.xml"
BRANDING_HIDDEN_FEATURES=""
BRANDING_ADDITIONAL_FEATURES=""






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

brand_and_version_wix()
{
  sed -b -i -e "s/Version=\"0\.0\.0\.0\"/Version=\"${BRANDING_XC_PRODUCT_VERSION}.${BUILD_NUMBER}\"/g" \
      -e "s/~~XenCenter~~/${BRANDING_BRAND_CONSOLE}/g" \
      $1 
}	

rebranding_global()
{
    sed -b -i -e "s#\[BRANDING_COMPANY_NAME_LEGAL\]#${BRANDING_COMPANY_NAME_LEGAL}#g" \
        -e "s#\[Citrix\]#${BRANDING_COMPANY_NAME_SHORT}#g" \
		-e "s#\[Citrix VM Tools\]#${BRANDING_PV_TOOLS}#g" \
        -e "s#\"\[BRANDING_COPYRIGHT\]\"#${BRANDING_COPYRIGHT}#g" \
        -e "s#\"\[BRANDING_COPYRIGHT_2\]\"#${BRANDING_COPYRIGHT_2}#g" \
        -e "s#\[XenServer product\]#${BRANDING_PRODUCT_BRAND}#g" \
        -e "s#\[BRANDING_PRODUCT_VERSION\]#${BRANDING_XC_PRODUCT_VERSION}#g" \
        -e "s#\[BRANDING_PRODUCT_VERSION_TEXT\]#${BRANDING_PRODUCT_VERSION_TEXT}#g" \
        -e "s#\[BRANDING_BUILD_NUMBER\]#${BUILD_NUMBER}#g" \
        -e "s#\[xensearch\]#${BRANDING_SEARCH}#g" \
        -e "s#\[xsupdate\]#${BRANDING_UPDATE}#g" \
        -e "s#\[XenServer\]#${BRANDING_SERVER}#g" \
		-e "s#\[Citrix XenServer\]#${BRANDING_SERVER}#g" \
        -e "s#\[XenCenter\]#${BRANDING_BRAND_CONSOLE}#g" \
		-e "s#\[XenCenterTitle\]#${BRANDING_BRAND_CONSOLE_TITLE}#g" \
		-e "s#\[XenAdmin\]#${BRANDING_BRAND_CONSOLE}#g" \
		-e "s/~~XenCenter~~/${BRANDING_BRAND_CONSOLE}/g" \
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

version_brand_cpp()
{
  for file in $1
  do
    version_cpp ${file} && rebranding_global ${file}
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

#projects sign change
cd ${REPO} && /usr/bin/find -name \*.csproj -exec sed -i 's#<SignManifests>false#<SignManifests>true#' {} \;

#AssemblyInfo rebranding
version_brand_csharp "XenAdmin splash-xcp-ng CommandLib XenCenterLib XenModel XenOvfApi XenOvfTransport XenCenterVNC xe xva_verify XenServer XenServerHealthCheck"

#XenAdmin rebranding
rebranding_global ${REPO}/XenModel/BrandManager.cs
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
#rebranding_global ${REPO}/XenAdminTests/TestResources/ContextMenuBuilderTestResults.xml
#rebranding_global ${REPO}/XenAdminTests/app.config
#rebranding_global ${REPO}/XenAdminTests/TestResources/state1.treeview.serverview.xml
#rebranding_global ${REPO}/XenAdminTests/TestResources/state1.treeview.orgview.xml
#rebranding_global ${REPO}/XenAdminTests/TestResources/searchresults.xml
#rebranding_global ${REPO}/XenAdminTests/TestResources/state3.xml
#rebranding_global ${REPO}/XenAdminTests/XenAdminTests.csproj
#echo cp ${REPO}/XenAdminTests/TestResources/succeed.[xsupdate] ${REPO}/XenAdminTests/TestResources/succeed.${BRANDING_UPDATE}
#cp ${REPO}/XenAdminTests/TestResources/succeed.[xsupdate] ${REPO}/XenAdminTests/TestResources/succeed.${BRANDING_UPDATE}

#XenServerHealthCheck
#rebranding_global ${REPO}/XenServerHealthCheck/Branding.cs
#rebranding_global ${REPO}/XenServerHealthCheck/app.config

#XCP-ng Center installer
brand_and_version_wix ${REPO}/installer-xcp-ng/Product.wxs
rebranding_global ${REPO}/installer-xcp-ng/Product.wxs

set +u
