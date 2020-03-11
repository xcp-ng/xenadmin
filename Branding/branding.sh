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

TOPLEVEL_VERSIONS=${ROOT}/branding.git/xenserver/toplevel-versions
TOPLEVEL_BRANDING=${ROOT}/branding.git/xenserver/toplevel-branding

BRANDING_COMPANY_NAME_LEGAL=$(cat ${TOPLEVEL_BRANDING} | grep -F "COMPANY_NAME_LEGAL := " | sed -e 's/COMPANY_NAME_LEGAL := //g')
BRANDING_COMPANY_NAME_SHORT=$(cat ${TOPLEVEL_BRANDING} | grep -F "COMPANY_NAME_SHORT := " | sed -e 's/COMPANY_NAME_SHORT := //g')
BRANDING_PRODUCT_BRAND=$(cat ${TOPLEVEL_BRANDING} | grep "^PRODUCT_BRAND := " | sed -e 's/PRODUCT_BRAND := //g')
BRANDING_COMPANY_URL=www.$(cat ${TOPLEVEL_BRANDING} | grep -F "COMPANY_DOMAIN := " | sed -e 's/COMPANY_DOMAIN := //g')
BRANDING_PRODUCT_VERSION_TEXT=$(cat ${TOPLEVEL_VERSIONS} | grep -F "PRODUCT_VERSION_TEXT := " | sed -e 's/PRODUCT_VERSION_TEXT := //g')
BRANDING_PRODUCT_MAJOR_VERSION=$(cat ${TOPLEVEL_VERSIONS} | grep -F "PRODUCT_MAJOR_VERSION := " | sed -e 's/PRODUCT_MAJOR_VERSION := //g')
BRANDING_PRODUCT_MINOR_VERSION=$(cat ${TOPLEVEL_VERSIONS} | grep -F "PRODUCT_MINOR_VERSION := " | sed -e 's/PRODUCT_MINOR_VERSION := //g')
BRANDING_SERVER=${BRANDING_PRODUCT_BRAND}
BRANDING_COMPANY_AND_PRODUCT=${BRANDING_PRODUCT_BRAND}
BRANDING_BRAND_CONSOLE=$(cat ${TOPLEVEL_BRANDING} | grep -F "BRAND_CONSOLE := " | sed -e 's/BRAND_CONSOLE := //g')
BRANDING_PV_TOOLS=${BRANDING_COMPANY_NAME_SHORT}\ VM\ Tools

# Check for the micro version override and use it if present otherwise use the one from branding
if [ -n "${PRODUCT_MICRO_VERSION_OVERRIDE+x}" ]; then
  BRANDING_PRODUCT_MICRO_VERSION=${PRODUCT_MICRO_VERSION_OVERRIDE}
  echo Using override for micro product number of: ${BRANDING_PRODUCT_MICRO_VERSION}
else
  BRANDING_PRODUCT_MICRO_VERSION=$(cat ${TOPLEVEL_VERSIONS} | grep -F "PRODUCT_MICRO_VERSION := " | sed -e 's/PRODUCT_MICRO_VERSION := //g')
fi

BRANDING_XC_PRODUCT_VERSION=${BRANDING_PRODUCT_MAJOR_VERSION}.${BRANDING_PRODUCT_MINOR_VERSION}.${BRANDING_PRODUCT_MICRO_VERSION}
