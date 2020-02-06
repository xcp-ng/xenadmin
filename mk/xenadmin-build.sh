#!/bin/bash

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

UNZIP="unzip -q -o"

mkdir_clean()
{
  rm -rf $1 && mkdir -p $1
}

ROOT="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
REPO="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SCRATCH_DIR=${ROOT}/scratch
OUTPUT_DIR=${ROOT}/output

WIX_INSTALLER_DEFAULT_GUID=65AE1345-A520-456D-8A19-2F52D43D3A09
WIX_INSTALLER_DEFAULT_VERSION=1.0.0
PRODUCT_GUID=$(uuidgen | tr [a-z] [A-Z])

source ${REPO}/Branding/branding.sh
source ${REPO}/mk/re-branding.sh

#build
MSBUILD="MSBuild.exe /nologo /m /verbosity:minimal /p:Configuration=Release /p:TargetFrameworkVersion=v4.6 /p:VisualStudioVersion=15.0"

${UNZIP} -d ${REPO}/XenOvfApi ${SCRATCH_DIR}/XenCenterOVF.zip
cd ${REPO}
$MSBUILD XenAdmin.sln
$MSBUILD /p:SolutionDir="${REPO}/XenAdmin" splash/splash.vcxproj

#prepare wix

WIX=${REPO}/WixInstaller
WIX_BIN=${WIX}/bin
WIX_SRC=${SCRATCH_DIR}/wixsrc

CANDLE="${WIX_BIN}/candle.exe -nologo"
LIT="${WIX_BIN}/lit.exe -nologo"
LIGHT="${WIX_BIN}/light.exe -nologo"

mkdir_clean ${WIX_SRC}
${UNZIP} ${SCRATCH_DIR}/wix310-debug.zip -d ${SCRATCH_DIR}/wixsrc
cp ${WIX_SRC}/src/ext/UIExtension/wixlib/CustomizeDlg.wxs ${WIX_SRC}/src/ext/UIExtension/wixlib/CustomizeStdDlg.wxs
cd ${WIX_SRC}/src/ext/UIExtension/wixlib && patch -p1 --binary < ${REPO}/mk/patches/wix_src_patch
cp -r ${WIX_SRC}/src/ext/UIExtension/wixlib ${REPO}/WixInstaller

mkdir_clean ${WIX_BIN}
${UNZIP} ${SCRATCH_DIR}/wix310-binaries.zip -d ${WIX_BIN}
touch ${REPO}/WixInstaller/PrintEula.dll

#compile_wix

chmod -R u+rx ${WIX_BIN}
cd ${WIX}
mkdir -p obj

${CANDLE} -out obj/ wixlib/WixUI_InstallDir.wxs wixlib/WixUI_FeatureTree.wxs wixlib/BrowseDlg.wxs wixlib/CancelDlg.wxs wixlib/Common.wxs wixlib/CustomizeDlg.wxs wixlib/CustomizeStdDlg.wxs wixlib/DiskCostDlg.wxs wixlib/ErrorDlg.wxs wixlib/ErrorProgressText.wxs wixlib/ExitDialog.wxs wixlib/FatalError.wxs wixlib/FilesInUse.wxs wixlib/InstallDirDlg.wxs wixlib/InvalidDirDlg.wxs wixlib/LicenseAgreementDlg.wxs wixlib/MaintenanceTypeDlg.wxs wixlib/MaintenanceWelcomeDlg.wxs wixlib/MsiRMFilesInUse.wxs wixlib/OutOfDiskDlg.wxs wixlib/OutOfRbDiskDlg.wxs wixlib/PrepareDlg.wxs wixlib/ProgressDlg.wxs wixlib/ResumeDlg.wxs wixlib/SetupTypeDlg.wxs wixlib/UserExit.wxs wixlib/VerifyReadyDlg.wxs wixlib/WaitForCostingDlg.wxs wixlib/WelcomeDlg.wxs

mkdir -p lib

${LIT} -out lib/WixUI_InstallDir.wixlib obj/WixUI_InstallDir.wixobj obj/WixUI_FeatureTree.wixobj obj/BrowseDlg.wixobj obj/CancelDlg.wixobj obj/Common.wixobj obj/CustomizeDlg.wixobj obj/CustomizeStdDlg.wixobj obj/DiskCostDlg.wixobj obj/ErrorDlg.wixobj obj/ErrorProgressText.wixobj obj/ExitDialog.wixobj obj/FatalError.wixobj obj/FilesInUse.wixobj obj/InstallDirDlg.wixobj obj/InvalidDirDlg.wixobj obj/LicenseAgreementDlg.wixobj obj/MaintenanceTypeDlg.wixobj obj/MaintenanceWelcomeDlg.wixobj obj/MsiRMFilesInUse.wixobj obj/OutOfDiskDlg.wixobj obj/OutOfRbDiskDlg.wixobj obj/PrepareDlg.wixobj obj/ProgressDlg.wixobj obj/ResumeDlg.wixobj obj/SetupTypeDlg.wixobj obj/UserExit.wixobj obj/VerifyReadyDlg.wixobj obj/WaitForCostingDlg.wixobj obj/WelcomeDlg.wixobj

#create mui wxs file
cd ${WIX} && patch --binary --output XenCenter.l10n.wxs XenCenter.wxs XenCenter.l10n.diff

#version installers
version_installer()
{
  sed -e "s/${WIX_INSTALLER_DEFAULT_GUID}/${PRODUCT_GUID}/g" \
      -e "s/${WIX_INSTALLER_DEFAULT_VERSION}/${BRANDING_XC_PRODUCT_VERSION}/g" \
      $1 > $1.tmp
  mv -f $1.tmp $1
}

version_installer ${WIX}/XenCenter.wxs
version_installer ${WIX}/XenCenter.l10n.wxs

echo "INFO: Collecting unsigned files..."
mkdir_clean ${OUTPUT_DIR}/${BRANDING_BRAND_CONSOLE}Unsigned
cp -R ${REPO}/* ${OUTPUT_DIR}/${BRANDING_BRAND_CONSOLE}Unsigned
cd ${OUTPUT_DIR} && zip -q -r -m ${BRANDING_BRAND_CONSOLE}Unsigned.zip ${BRANDING_BRAND_CONSOLE}Unsigned
echo "Unsigned artifacts archived"

#build and sign the installers
. ${REPO}/mk/build-installers.sh

#build the tests
echo "INFO: Build the tests..."
cd ${REPO}/XenAdminTests && $MSBUILD
cp ${REPO}/XenAdmin/ReportViewer/* ${REPO}/XenAdminTests/bin/Release/
cd ${REPO}/XenAdminTests/bin/ && tar -czf XenAdminTests.tgz ./Release
cd ${REPO}/XenAdmin/TestResources && tar -cf ${OUTPUT_DIR}/XenCenterTestResources.tar *
cp ${REPO}/XenAdminTests/bin/XenAdminTests.tgz ${OUTPUT_DIR}/XenAdminTests.tgz

#include cfu validator binary in output directory
cd ${REPO}/CFUValidator/bin/Release && zip CFUValidator.zip ./{*.dll,CFUValidator.exe,XenCenterMain.exe}
cp ${REPO}/CFUValidator/bin/Release/CFUValidator.zip ${OUTPUT_DIR}/CFUValidator.zip

#now package the pdbs

cp ${REPO}/XenAdmin/bin/Release/{CommandLib.pdb,${BRANDING_BRAND_CONSOLE}.pdb,XenCenterLib.pdb,XenCenterMain.pdb,XenCenterVNC.pdb,XenModel.pdb,XenOvf.pdb,XenOvfTransport.pdb} \
   ${REPO}/xe/bin/Release/xe.pdb \
   ${REPO}/xva_verify/bin/Release/xva_verify.pdb \
   ${REPO}/XenServerHealthCheck/bin/Release/XenServerHealthCheck.pdb \
   ${OUTPUT_DIR}

cd ${OUTPUT_DIR} && tar cjf XenCenter.Symbols.tar.bz2 --remove-files *.pdb

echo "INFO:	Build phase succeeded at "
date

set +u
