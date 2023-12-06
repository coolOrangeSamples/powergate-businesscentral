# path to sample data csv
$demoItems = Import-Csv -Path "Business Central Demo Items.csv" -Delimiter ";"

Open-VaultConnection
Connect-ERP -Service "http://localhost:8080/coolOrange/BusinessCentral"

foreach($demoItem in $demoItems) {
    $item = New-ERPObject -EntityType 'Item'
    $item.Number = $demoItem.Number
    $item.Title = $demoItem.Title
    $item.Description = $demoItem.Description
    $item.Material = $demoItem.Material
    $item.UnitOfMeasure = "PCS"

    Add-ErpObject -EntitySet 'Items' -Properties $item
}

#Delete:
# foreach($demoItem in $demoItems) {
#     $i = Invoke-RestMethod -Method Get -Uri "http://localhost:7048/BC220/ODataV4/Company('CRONUS%20USA,%20Inc.')/ItemCards('$($material.Number.ToUpper())')" -ContentType "application/json" -Headers @{Authorization=$authorization}
#     Invoke-RestMethod -Method Delete -Uri "http://localhost:7048/BC220/ODataV4/Company('CRONUS%20USA,%20Inc.')/ItemCards('$($material.Number.ToUpper())')" -ContentType "application/json" -Headers @{Authorization=$authorization; IfMatch=$i.'@odata.etag'}
# }