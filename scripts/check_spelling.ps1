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

Param(
    [Parameter(HelpMessage = "Whether to list the names of the examined files")]
    [switch]$NOISY
)

$ErrorActionPreference = "Stop"

Write-Host "Started spell check at $(Get-Date)"

$INCLUDES = @("*.resx")
$EXCLUDES = @("*.ja.resx", "*.zh-CN.resx")

$REPO = Get-Item "$PSScriptRoot\.." | Select-Object -ExpandProperty FullName
$files = Get-ChildItem -Recurse -File -Path $REPO -Include $INCLUDES -Exclude $EXCLUDES

foreach ($file in $files) {
    if ($file.Name -in @("InvisibleMessages.resx", "KeyMap.resx", "Resources.resx", "Branding.resx")) {
        Write-Host "Skipping $file"
        continue
    }

    if ($NOISY) {
        Write-Host "Spell check on $file"
    }

    [xml]$xml = Get-Content $file

    $found = @()
    $found = Select-Xml -Xml $xml -XPath "//data" |`
        Select-Object -ExpandProperty node |`
        Where-Object { ($_.name -like "*Text") -or ($_.name -cmatch '^[A-Z_]*$') } |`
        Select-Object -ExpandProperty Value |`
        Where-Object { $_.GetType() -eq [System.String] } |`
        Foreach-Object { $_.Replace("&", "").Replace("'", "").Replace("\n", " ") } |`
        aspell --add-wordlists="$PSScriptRoot\dictionary.txt" --list

    if ($found.Count -gt 0) {
        if (-not $NOISY) {
            Write-Warning "Found typos in $file"
        }
        $found | Sort-Object | Get-Unique
    }
}

#Remove-Item $customDic
Write-Host "Finished spell check at $(Get-Date)"
