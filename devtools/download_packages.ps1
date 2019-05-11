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

# help script to download third party binaries to local dev environment
# NOTE: do not remove the Requires directive

#Requires -Version 3.0

Param(
    [Parameter(Mandatory=$true, HelpMessage ="Artifactory domain (e.g. artifactory.domain.com)")]
    [String]$DOMAIN,

    [Parameter(HelpMessage ="Whether to download symbols (*.pdb files)")]
    [switch]$SYMBOLS
)

$DOMAIN = $DOMAIN.Trim()
$PACKAGE_DIR = Get-Item "$PSScriptRoot\..\packages" | select -ExpandProperty FullName
$MK_DIR = Get-Item "$PSScriptRoot\..\mk" | select -ExpandProperty FullName

#dotnet packages

$BUILD_LOCATION = (Get-Content "$PACKAGE_DIR\DOTNET_BUILD_LOCATION").Trim()
$DEPS_MAP = Get-Content "$MK_DIR\deps-map.json" `
            | foreach {$_ -replace '@REMOTE_DOTNET@',"$BUILD_LOCATION"} `
            | ConvertFrom-Json

foreach($dep in $DEPS_MAP.files) {
    $pattern = "https://$DOMAIN/" + $dep.pattern
    $filename = Split-Path $pattern -leaf

    if (($filename -eq "putty.exe") -or ($filename -like "*.dll")) {
        Invoke-WebRequest -Uri $pattern -Method Get -OutFile "$PACKAGE_DIR\$filename"

        if ($SYMBOLS) {
            $symbolfile = [IO.Path]::GetFileNameWithoutExtension($filename) + ".pdb"
            Invoke-WebRequest -Uri $pattern -Method Get -OutFile "$PACKAGE_DIR\$symbolfile"
        }
    }
}
