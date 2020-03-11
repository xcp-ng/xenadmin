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

# Script parameters:
# 1 Global build number
# 2 Signing node name
# 3 Sign in SBE
# 4 Self-signing certificate sha1 thumbprint
# 5 Self-signing certificate sha256 thumbprint
# 6 Timestamp server

set -exu

#TODO: remove this
SET_ENV_FILE="/cygdrive/c/env.sh"
if [ -f ${SET_ENV_FILE} ]; then
   . ${SET_ENV_FILE}
fi

UNZIP="unzip -q -o"

mkdir_clean()
{
  rm -rf $1 && mkdir -p $1
}

REPO="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SCRATCH_DIR=${REPO}/_scratch
OUTPUT_DIR=${REPO}/_output

#build
MSBUILD=MSBuild.exe
SWITCHES="/m /verbosity:minimal /p:Configuration=Release /p:TargetFrameworkVersion=v4.6 /p:VisualStudioVersion=15.0"

mkdir_clean ${SCRATCH_DIR}
mkdir_clean ${OUTPUT_DIR}

source ${REPO}/Branding/branding.sh
source ${REPO}/scripts/re-branding.sh $1

#packages sources
mkdir_clean ${SCRATCH_DIR}/SOURCES
cd ${REPO}
gitCommit=`git rev-parse HEAD`
git archive --format=zip -o "_scratch/SOURCES/xenadmin-sources.zip" ${gitCommit}
cp ${REPO}/packages/dotnet-packages-sources.zip ${SCRATCH_DIR}/SOURCES
cd ${SCRATCH_DIR}/SOURCES && zip ${BRANDING_BRAND_CONSOLE}-source.zip dotnet-packages-sources.zip xenadmin-sources.zip
cp ${SCRATCH_DIR}/SOURCES/${BRANDING_BRAND_CONSOLE}-source.zip ${OUTPUT_DIR}/${BRANDING_BRAND_CONSOLE}-source.zip

${UNZIP} -d ${SCRATCH_DIR} ${REPO}/packages/XenCenterOVF.zip
cd ${REPO} && "${MSBUILD}" ${SWITCHES} XenAdmin.sln

#sign files only if all parameters are set and non-empty
SIGN_BAT="${REPO}/scripts/sign.bat ${GLOBAL_BUILD_NUMBER} $2 $3 $4 $5 $6"
SIGN_DESCR="${BRANDING_COMPANY_NAME_SHORT} ${BRANDING_BRAND_CONSOLE}"

if [ -z "$2" ] || [ -z "$3" ] || [ -z "$4" ] || [ -z "$5" ] || [ -z "$6" ] ; then
  echo "Some signing parameters are not set; skip signing binaries"
else
  for file in XenCenterMain.exe CommandLib.dll MSTSCLib.dll XenCenterLib.dll XenCenterVNC.dll XenModel.dll XenOvf.dll XenOvfTransport.dll
  do
    cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} ${file} "${SIGN_DESCR}"
  done

  cd ${REPO}/XenAdmin/bin/Release   && ${SIGN_BAT} ${BRANDING_BRAND_CONSOLE}.exe "${SIGN_DESCR}"
  cd ${REPO}/xe/bin/Release         && ${SIGN_BAT} xe.exe "${SIGN_DESCR}"
  cd ${REPO}/xva_verify/bin/Release && ${SIGN_BAT} xva_verify.exe "${SIGN_DESCR}"

  for file in Microsoft.ReportViewer.Common.dll Microsoft.ReportViewer.ProcessingObjectModel.dll Microsoft.ReportViewer.WinForms.dll
  do
    cd ${REPO}/XenAdmin/ReportViewer && ${SIGN_BAT} ${file} "${SIGN_DESCR}"
  done

  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} CookComputing.XmlRpcV2.dll "XML-RPC.NET"
  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} Newtonsoft.Json.CH.dll "JSON.NET"
  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} log4net.dll "Log4Net"
  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} ICSharpCode.SharpZipLib.dll "SharpZipLib"
  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} DiscUtils.dll "DiscUtils"
  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} Ionic.Zip.dll "OSS"
  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} putty.exe "PuTTY"

  cd ${REPO}/XenServerHealthCheck/bin/Release && ${SIGN_BAT} XenServerHealthCheck.exe "${SIGN_DESCR}"
fi

#copy files (signed accordingly) in XenServerHealthService folder
cp ${REPO}/XenAdmin/bin/Release/{CommandLib.dll,XenCenterLib.dll,XenModel.dll,CookComputing.XmlRpcV2.dll,Newtonsoft.Json.CH.dll,log4net.dll,ICSharpCode.SharpZipLib.dll,Ionic.Zip.dll} \
  ${REPO}/XenServerHealthCheck/bin/Release

#prepare wix

WIX_BIN=${SCRATCH_DIR}/wixbin
WIX_SRC=${SCRATCH_DIR}/wixsrc
WIX=${SCRATCH_DIR}/WixInstaller

CANDLE=${WIX_BIN}/candle.exe
LIT=${WIX_BIN}/lit.exe
LIGHT=${WIX_BIN}/light.exe

mkdir_clean ${WIX_BIN} && ${UNZIP} ${REPO}/packages/wix311-binaries.zip -d ${WIX_BIN}
mkdir_clean ${WIX_SRC} && ${UNZIP} ${REPO}/packages/wix311-debug.zip -d ${WIX_SRC}
cp -r ${REPO}/WixInstaller ${SCRATCH_DIR}/
cp -r ${WIX_SRC}/src/ext/UIExtension/wixlib ${WIX}/
cd ${WIX}/wixlib && cp CustomizeDlg.wxs CustomizeStdDlg.wxs
cd ${WIX}/wixlib && patch -p1 --binary < ${WIX}/wix_src.patch
touch ${WIX}/PrintEula.dll

#compile_wix
chmod -R u+rx ${WIX_BIN}
cd ${WIX} && mkdir -p obj lib
RepoRoot=$(cygpath -w ${REPO}) ${CANDLE} -out obj/ @candleList.txt
${LIT} -out lib/WixUI_InstallDir.wixlib @litList.txt

locale_id() {
  case "$1" in
    "ja-jp") echo 1041 ;;
    "zh-cn") echo 2052 ;;
    "zh-tw") echo 1028 ;;
    *)       echo 1033 ;; #en-us
  esac
}

if [ "XenCenter" != "${BRANDING_BRAND_CONSOLE}" ] ; then
  cd ${WIX} && mv XenCenter.wxs ${BRANDING_BRAND_CONSOLE}.wxs
fi

#for each locale create an msi containing all resources

for locale in en-us ja-jp zh-cn
do
  if [ "${locale}" = "en-us" ] ; then
    name=${BRANDING_BRAND_CONSOLE}
  else
    name=${BRANDING_BRAND_CONSOLE}.${locale}
  fi

  cd ${WIX}
  mkdir -p obj${name} out${name}

  WixLangId=$(locale_id ${locale} | tr -d [:space:]) RepoRoot=$(cygpath -w ${REPO}) \
    ${CANDLE} -ext WiXNetFxExtension -out obj${name}/ ${BRANDING_BRAND_CONSOLE}.wxs branding.wxs

  ${LIGHT} -ext WiXNetFxExtension -out out${name}/${name}.msi \
          -loc wixlib/wixui_${locale}.wxl -loc ${locale}.wxl \
          obj${name}/${BRANDING_BRAND_CONSOLE}.wixobj obj${name}/branding.wixobj lib/WixUI_InstallDir.wixlib

  cp ${WIX}/out${name}/${name}.msi ${WIX}
done

cd ${WIX} && cp ${BRANDING_BRAND_CONSOLE}.msi ${BRANDING_BRAND_CONSOLE}.zh-tw.msi
cd ${WIX} && cscript CodePageChange.vbs ZH-TW ${BRANDING_BRAND_CONSOLE}.zh-tw.msi

#create localised mst files and then embed them into the combined msi

for locale in ja-jp zh-cn zh-tw ; do
  cd ${WIX} && \
    wscript msidiff.js ${BRANDING_BRAND_CONSOLE}.msi ${BRANDING_BRAND_CONSOLE}.${locale}.msi ${locale}.mst && \
    wscript WiSubStg.vbs ${BRANDING_BRAND_CONSOLE}.msi ${locale}.mst $(locale_id ${locale} | tr -d [:space:])
done

#copy and sign the combined installer

if [ -z "$2" ] || [ -z "$3" ] || [ -z "$4" ] || [ -z "$5" ] || [ -z "$6" ] ; then
  echo "Some signing parameters are not set; skip signing installer"
else
  cd ${WIX} && chmod a+rw ${BRANDING_BRAND_CONSOLE}.msi && ${SIGN_BAT} ${BRANDING_BRAND_CONSOLE}.msi "${SIGN_DESCR}"
fi

cp ${WIX}/${BRANDING_BRAND_CONSOLE}.msi ${OUTPUT_DIR}

#build the tests
echo "INFO: Build the tests..."
cd ${REPO}/XenAdminTests && "${MSBUILD}" ${SWITCHES}
cp ${REPO}/XenAdmin/ReportViewer/* ${REPO}/XenAdminTests/bin/Release/
cd ${REPO}/XenAdminTests/bin/ && tar -czf ${OUTPUT_DIR}/XenAdminTests.tgz ./Release
cd ${REPO}/XenAdmin/TestResources && tar -cf ${OUTPUT_DIR}/${BRANDING_BRAND_CONSOLE}TestResources.tar *

#include cfu validator binary in output directory
cd ${REPO}/CFUValidator/bin/Release && zip CFUValidator.zip ./{*.dll,CFUValidator.exe,XenCenterMain.exe}
cp ${REPO}/CFUValidator/bin/Release/CFUValidator.zip ${OUTPUT_DIR}/CFUValidator.zip

#now package the pdbs
cp ${REPO}/packages/*.pdb ${OUTPUT_DIR}

cp ${REPO}/XenAdmin/bin/Release/{CommandLib.pdb,${BRANDING_BRAND_CONSOLE}.pdb,XenCenterLib.pdb,XenCenterMain.pdb,XenCenterVNC.pdb,XenModel.pdb,XenOvf.pdb,XenOvfTransport.pdb} \
   ${REPO}/xe/bin/Release/xe.pdb \
   ${REPO}/xva_verify/bin/Release/xva_verify.pdb \
   ${REPO}/XenServerHealthCheck/bin/Release/XenServerHealthCheck.pdb \
   ${OUTPUT_DIR}

cd ${OUTPUT_DIR} && tar cjf ${BRANDING_BRAND_CONSOLE}.Symbols.tar.bz2 --remove-files *.pdb

set +u