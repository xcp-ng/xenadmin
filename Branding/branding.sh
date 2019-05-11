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

#==============================================================
#Micro version override - please keep at the top of the script
#==============================================================
#Set and uncomment this to override the 3rd value of the product number 
#normally fetched from branding
#
#PRODUCT_MICRO_VERSION_OVERRIDE=<My override value here>

# bring versions from the server branding repo
ROOT="$( cd -P "$( dirname "${BASH_SOURCE[0]}" )/../.." && pwd )"
OUTPUT_DIR=${ROOT}/output

cd ${REPO}/Branding/Hotfixes

for hfx in RPU003
do
  if [ -d "${hfx}" ]; then
    latest=$(ls ${hfx} | /usr/bin/sort -n | tail -n 1)
    echo "INFO: Latest version of ${hfx} hotfix is $latest"
    cp ${hfx}/$latest/${hfx}.xsupdate ${hfx}.xsupdate
  fi
done

for hfx in RPU004
do
  if [ -d "${hfx}" ]; then
    latest=$(ls ${hfx} | /usr/bin/sort -n | tail -n 1)
    echo "INFO: Latest version of ${hfx} hotfix is $latest"
    cp ${hfx}/$latest/${hfx}.iso ${hfx}.iso
  fi
done


TOPLEVEL_VERSIONS=${ROOT}/branding.git/xenserver/toplevel-versions
TOPLEVEL_BRANDING=${ROOT}/branding.git/xenserver/toplevel-branding

BRANDING_COMPANY_NAME_LEGAL=$(cat ${TOPLEVEL_BRANDING} | grep -F "COMPANY_NAME_LEGAL := " | sed -e 's/COMPANY_NAME_LEGAL := //g')
BRANDING_COMPANY_NAME_SHORT=$(cat ${TOPLEVEL_BRANDING} | grep -F "COMPANY_NAME_SHORT := " | sed -e 's/COMPANY_NAME_SHORT := //g')
BRANDING_COPYRIGHT=\"Copyright\ ©\ ${BRANDING_COMPANY_NAME_LEGAL}\"
BRANDING_COPYRIGHT_2=\"Copyright\ \\\\251\ ${BRANDING_COMPANY_NAME_LEGAL}\"
BRANDING_PRODUCT_BRAND=$(cat ${TOPLEVEL_BRANDING} | grep "^PRODUCT_BRAND := " | sed -e 's/PRODUCT_BRAND := //g')
BRANDING_COMPANY_URL=www.$(cat ${TOPLEVEL_BRANDING} | grep -F "COMPANY_DOMAIN := " | sed -e 's/COMPANY_DOMAIN := //g')
BRANDING_PRODUCT_VERSION_TEXT=$(cat ${TOPLEVEL_VERSIONS} | grep -F "PRODUCT_VERSION_TEXT := " | sed -e 's/PRODUCT_VERSION_TEXT := //g')
BRANDING_PRODUCT_MAJOR_VERSION=$(cat ${TOPLEVEL_VERSIONS} | grep -F "PRODUCT_MAJOR_VERSION := " | sed -e 's/PRODUCT_MAJOR_VERSION := //g')
BRANDING_PRODUCT_MINOR_VERSION=$(cat ${TOPLEVEL_VERSIONS} | grep -F "PRODUCT_MINOR_VERSION := " | sed -e 's/PRODUCT_MINOR_VERSION := //g')
BRANDING_SEARCH=xensearch
BRANDING_UPDATE=xsupdate
BRANDING_BACKUP=xbk
BRANDING_LEGACY_PRODUCT_BRAND=XenServer
BRANDING_SERVER=${BRANDING_PRODUCT_BRAND}
BRANDING_COMPANY_AND_PRODUCT=${BRANDING_PRODUCT_BRAND}
BRANDING_BRAND_CONSOLE=$(cat ${TOPLEVEL_BRANDING} | grep -F "BRAND_CONSOLE := " | sed -e 's/BRAND_CONSOLE := //g')
BRANDING_PERF_ALERT_MAIL_LANGUAGE_DEFAULT=en-US
BRANDING_PV_TOOLS=${BRANDING_COMPANY_NAME_SHORT}\ VM\ Tools

# Check for the micro version override and use it if present otherwise use the one from branding
if [ -n "${PRODUCT_MICRO_VERSION_OVERRIDE+x}" ]; then
  BRANDING_PRODUCT_MICRO_VERSION=${PRODUCT_MICRO_VERSION_OVERRIDE}
  echo Using override for micro product number of: ${BRANDING_PRODUCT_MICRO_VERSION}
else
  BRANDING_PRODUCT_MICRO_VERSION=$(cat ${TOPLEVEL_VERSIONS} | grep -F "PRODUCT_MICRO_VERSION := " | sed -e 's/PRODUCT_MICRO_VERSION := //g')
fi

BRANDING_XC_PRODUCT_VERSION=${BRANDING_PRODUCT_MAJOR_VERSION}.${BRANDING_PRODUCT_MINOR_VERSION}.${BRANDING_PRODUCT_MICRO_VERSION}
BRANDING_XC_PRODUCT_5_6_VERSION=5.6
BRANDING_XC_PRODUCT_6_0_VERSION=6.0
BRANDING_XC_PRODUCT_6_2_VERSION=6.2
BRANDING_XC_PRODUCT_6_5_VERSION=6.5
BRANDING_XC_PRODUCT_7_0_VERSION=7.0
BRANDING_XC_PRODUCT_7_1_2_VERSION=7.1.2
BRANDING_XC_PRODUCT_8_0_VERSION=8.0
BRANDING_XENSERVER_UPDATE_URL="https://updates.xensource.com/XenServer/updates.xml"
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
