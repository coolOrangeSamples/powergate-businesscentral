#==============================================================================#
# (c) 2023 coolOrange s.r.l.                                                   #
#                                                                              #
# THIS SCRIPT/CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER    #
# EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES  #
# OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, OR NON-INFRINGEMENT.   #
#==============================================================================#

if ($processName -notin @('Connectivity.VaultPro', 'Inventor')) {
	return
}

#Remark: The name of this file starts with '#' as it needs to be loaded before all the other scripts to provide the ERP name for Add-VaultTab, Add-VaultMenuItem, and Add-InventorMenuItem

#region Settings
#TODO: Set the display Name of the ERP system. This name will be displayed in the powerGate UI
$global:erpName = "Business Central"

#TODO: Set the prefix for the powerGate files. All files will be named like this: <filePrefix>.<filename>.<extension>
$global:filePrefix = "BC"

#TODO: Change EntitySet names if needed. EntityType names must be changed in *powerGateMappingFiles.ps1 and *powerGateMappingItems.ps1 and will be adjustable in future in the new powerGate Configuration Manager
$global:itemEntitySet = "Items"
$global:bomHeaderEntitySet = "BomHeaders"
$global:bomRowEntitySet = "BomRows"
#endregion


# Connect to powerGate Server on Vault login
Register-VaultEvent -EventName LoginVault_Post -Action {
	Disconnect-ERP
	
	#TODO: Set the URL of the powerGate Server
	$connected = Connect-ERP -Service "http://localhost:8080/coolOrange/BusinessCentral"
	if(-not $connected) { return }

	$sampleERPServices = (Get-ERPServices) | Where-Object {$_.Url.LocalPath -like '/PGS/ERP/*'}
	$sampleERPServices | Foreach-Object { Disconnect-ERP -Service $_.Url }
}