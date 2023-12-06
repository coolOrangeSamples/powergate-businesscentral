Import-Module powerVault
Open-VaultConnection
$authorization = "Basic " + [Convert]::ToBase64String([System.Text.Encoding]::ASCII.GetBytes("powerGate" + ":" + "c00!Orange"))

# Create a RAW MATERIAL routing link entry
$routingLink = @"
{
    "code": "RAW",
    "description": "Raw Material"
}
"@
Invoke-RestMethod -Method Post -Uri "http://localhost:7048/BC220/ODataV4/Company('CRONUS%20USA,%20Inc.')/RoutingLinks" -ContentType "application/json" -Body $routingLink -Headers @{Authorization=$authorization}


# Get all existing UOMs from Business Central and Vault
$result = Invoke-RestMethod -Method Get -Uri 'http://localhost:7048/BC220/ODataV4/Company(''CRONUS%20USA,%20Inc.'')/UnitsOfMeasures?$select=code,displayName' -Headers @{Authorization=$authorization}
$bcUoms = $result.value | Sort-Object -Property displayName
$vaultUoms = $vault.ItemService.GetAllUnitsOfMeasure()

# Create UOMs in Business Central that do exist in Vault but not in Business Central
foreach($vaultUom in $vaultUoms) {
    if ($vaultUom.Abbr -eq "EA") { # BC uses PCS instead of EA
        continue
    }

    if ($bcUoms.code -contains $vaultUom.Abbr -or $bcUoms.displayName -contains $vaultUom.UnitName) {
        continue
    }

    $json = "{ ""code"": ""$($vaultUom.Abbr)"", ""displayName"": ""$($vaultUom.UnitName)"" }"
    Invoke-RestMethod -Method Post -Uri "http://localhost:7048/BC220/ODataV4/Company('CRONUS%20USA,%20Inc.')/UnitsOfMeasures" -ContentType "application/json" -Body $json -Headers @{Authorization=$authorization}
}

# # Get all existing UOMs from Business Central and write it to the console
# $result = Invoke-RestMethod -Method Get -Uri 'http://localhost:7048/BC220/ODataV4/Company(''CRONUS%20USA,%20Inc.'')/UnitsOfMeasures?$select=code,displayName' -Headers @{Authorization=$authorization}
# $bcUoms = $result.value | Sort-Object -Property displayName
# foreach($bcUom in $bcUoms) {
#     "<Entry Key=""$($bcUom.code)"" Value=""$($bcUom.displayName)"" />"
# }