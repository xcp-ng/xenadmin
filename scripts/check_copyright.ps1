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

Write-Output "Started copyright check at $(Get-Date)"

$INCLUDES = @("*.cs", "*.sh", "*.wxs", "*.wxi", "*.wxl", "*.patch")
$EXCLUDES = @("*.Designer.cs")

$REPO = Get-Item "$PSScriptRoot\.." | Select-Object -ExpandProperty FullName
$files = Get-ChildItem -Recurse -File -Path $REPO -Include $INCLUDES -Exclude $EXCLUDES

[System.Collections.ArrayList]$badFiles = @()

foreach ($file in $files) {
    if ($NOISY) {
        Write-Output "Copyright check on $file"
    }

    $result = Select-String -Path $file -Pattern 'Copyright (c) Citrix Systems, Inc.' -CaseSensitive -SimpleMatch
    if ($null -eq $result) {
        $badFiles.Add($file) > $null
    }
}

$badFiles | Select-Object FullName | Write-Output

Write-Output "Finished copyright check at $(Get-Date)"
exit $badFiles.Count
