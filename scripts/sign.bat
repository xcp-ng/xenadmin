@echo off
rem Copyright (c) Citrix Systems, Inc.
rem All rights reserved.
rem
rem Redistribution and use in source and binary forms,
rem with or without modification, are permitted provided
rem that the following conditions are met:
rem
rem *   Redistributions of source code must retain the above
rem     copyright notice, this list of conditions and the
rem     following disclaimer.
rem *   Redistributions in binary form must reproduce the above
rem     copyright notice, this list of conditions and the
rem     following disclaimer in the documentation and/or other
rem     materials provided with the distribution.
rem
rem THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
rem CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
rem INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
rem MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
rem DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
rem CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
rem SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
rem BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
rem SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
rem INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
rem WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
rem NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
rem OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
rem SUCH DAMAGE.

rem Script parameters:
rem 1 Global build number
rem 2 Signing node name
rem 3 Sign in SBE
rem 4 Self-signing certificate sha1 thumbprint
rem 5 Self-signing certificate sha256 thumbprint
rem 6 Timestamp server
rem 7 File to be signed
rem 8 Description

rem reset error level
cmd /c "exit /b 0"

set global_build_number=%1
set worker=%~2
set sbe=%~3
set thumb_sha1=%4
set thumb_sha256=%5
set timestamp_server=%~6
set descr=%~8
set thefile=%~7

if /I "%~x7"==".msi" (
  set cross_sign=no
) else (
  set cross_sign=yes
)

set CTXSIGN=C:\ctxsign2\ctxsign.exe

if "%sbe%"=="true" (
  echo "Signing in SBE"

  echo %CTXSIGN%  --authorise --workerID %worker% --orchID %worker% --jobID XenServerWindowsLegacyPVTools_signing ^
    --task XenCenter-%global_build_number% --debug

  date /t && time /t
  %CTXSIGN%  --authorise --workerID %worker% --orchID %worker% --jobID XenServerWindowsLegacyPVTools_signing ^
    --task XenCenter-%global_build_number% --debug > out.txt

  echo OUTPUT FROM CTXSIGN --AUTHORISE:
  type out.txt
  echo.
  echo OUTPUT ENDS

  set /p CCSS_TICKET= < out.txt

  echo %CTXSIGN% --sign --key XenServer.NET_KEY --cross-sign --pagehashes yes --type Authenticode ^
      --description "%descr%" "%thefile%"

  date /t && time /t
  %CTXSIGN% --sign --key XenServer.NET_KEY --cross-sign --pagehashes yes --type Authenticode ^
      --description "%descr%" "%thefile%"

  if "%cross_sign%"=="yes" (
    date /t && time /t
    %CTXSIGN% --sign --authenticode-append --authenticode-SHA256 --key XenServerSHA256.NET_KEY ^
      --cross-sign --pagehashes yes "%thefile%"
  )
  %CTXSIGN% --end

) else (
  echo "Self signing"

  if /I "%cross_sign%" == "yes" (
    signtool sign -v -sm -sha1 %thumb_sha1% -d "%descr%" -t %timestamp_server% "%thefile%"
    signtool sign -v -sm -as -sha1 %thumb_sha256% -d "%descr%" -tr %timestamp_server% -td sha256 "%thefile%"
  ) else (
    signtool sign -v -sm -sha1 %thumb_sha1% -d "%descr%" -tr %timestamp_server% -td sha256 "%thefile%"
  )
)
