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

# help script to download hotfixes to local dev environment
# NOTE: do not remove the Requires directive

#Requires -Version 3.0

Param(
    [Parameter(Mandatory = $true, HelpMessage = "Artifactory domain (e.g. artifactory.domain.com)")]
    [String]$DOMAIN,

    [Parameter(HelpMessage = "Whether to download source packages")]
    [switch]$SOURCES
)

$DOMAIN = $DOMAIN.Trim()
$SCRIPT_DIR = Get-Item "$PSScriptRoot\" | select -ExpandProperty FullName

$HOTFIX_DIR = "$PSScriptRoot\..\Branding\Hotfixes"
if (-not (Test-Path $HOTFIX_DIR)) {
    $HOTFIX_DIR = New-Item $HOTFIX_DIR -ItemType Directory | select -ExpandProperty FullName
}

$HOTFIX_MAP = Get-Content "$SCRIPT_DIR\hotfix-map.json" | ConvertFrom-Json

foreach ($dep in $HOTFIX_MAP.files) {
    $pattern = "https://$DOMAIN/" + $dep.pattern
    $filename = Split-Path $pattern -leaf

    $download = $false

    if ($filename -like "*.xsupdate") {
        $download = $true
    }
    elseif ($filename -like "*-sources.iso") {
        $download = $SOURCES
    }
    elseif ($filename -like "*.iso") {
        $download = $true
    }
    elseif ($filename -like "*-src-pkgs.tar") {
        $download = $SOURCES
    }

    if ($download) {
        Invoke-WebRequest -Uri $pattern -Method Get -OutFile "$HOTFIX_DIR\$filename"
    }
}
