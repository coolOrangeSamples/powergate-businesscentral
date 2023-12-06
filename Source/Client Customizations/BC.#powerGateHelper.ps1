#==============================================================================#
# (c) 2022 coolOrange s.r.l.                                                   #
#                                                                              #
# THIS SCRIPT/CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER    #
# EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES  #
# OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, OR NON-INFRINGEMENT.   #
#==============================================================================#

if ($processName -notin @('Connectivity.VaultPro', 'Inventor')) {
    return
}

#Remark: this function is used to identify differences between the current ERP Item as fetched from the ERP sytstem and the new
#        ERP Item, computed based on the mapping from the Vault entity (see powerGateMappingFiles.ps1 and powerGAteMappingItems.ps1)
function GetErpObjectDifferences($erpItem, $newErpItem, $fieldsToExclude = @()) {

    $differences = @()
    $properties = $erpItem | Get-Member -MemberType Properties | Select-Object Name
    foreach ($property in $properties) {
        if ($property.Name -eq "_Keys" -or $property.Name -eq "_Properties") { continue }
        if ($fieldsToExclude -contains $property.Name) { continue }

        if ([string]$erpItem.$($property.Name) -ne [string]$newErpItem.$($property.Name)) {
            if (([string]$erpItem.$($property.Name)).Length -gt 20) {
                $currentValue = ([string]$erpItem.$($property.Name)).Substring(0, 20).Trim() + "..."
            }
            else {
                $currentValue = ([string]$erpItem.$($property.Name))
            }
            if (([string]$newErpItem.$($property.Name)).Length -gt 20) {
                $newValue = ([string]$newErpItem.$($property.Name)).Substring(0, 20).Trim() + "..."
            }
            else {
                $newValue = ([string]$newErpItem.$($property.Name))
            }

            $differences += "$($property.Name): $currentValue -> $newValue"
        }
    }

    return $differences
}

#Remark: this function is used to display an Image in a WPF control (see Vault-Tab-Item-ErpItem.ps1 and Vault-Tab-File-ErpItem.ps1)
function GetImageFromBase64String($thumbnail) {
    if (-not $thumbnail) { return $null }
    $bmp = [System.Drawing.Bitmap]::FromStream((New-Object System.IO.MemoryStream (@(, [System.Convert]::FromBase64String($thumbnail)))))
    $memory = New-Object System.IO.MemoryStream
    $null = $bmp.Save($memory, [System.Drawing.Imaging.ImageFormat]::Png)
    $memory.Position = 0
    $img = New-Object System.Windows.Media.Imaging.BitmapImage
    $img.BeginInit()
    $img.StreamSource = $memory
    $img.CacheOption = [System.Windows.Media.Imaging.BitmapCacheOption]::OnLoad
    $img.EndInit()
    $img.Freeze()
    $memory.Close()
    $memory.Dispose()
    return $img
}