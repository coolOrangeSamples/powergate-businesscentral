# Download Business Central 2023 Release Wave 1 on-premises from https://www.microsoft.com/en-us/download/details.aspx?id=105113
# Use the US version (Dynamics.365.BC.55195.US.DVD.zip)

# Code must run as "Administrator"
Import-Module -name "C:\Program Files\Microsoft Dynamics 365 Business Central\220\Service\navadmintool.ps1"

# path to BC app (for item attributes and item record links)
$app = "COOLORANGE_PowerGate_1.0.0.0.app"


$InstanceName = "BC220"
Get-NAVApplication -ServerInstance $InstanceName
Get-NAVCompany -ServerInstance $InstanceName
Get-NAVServerUser -ServerInstance $InstanceName

$username = "powerGate"
$password = "c00!Orange"
New-NAVServerUser $InstanceName -UserName $username -Password (ConvertTo-SecureString $password -AsPlainText -Force) #-WindowsAccount $(whoami)
New-NAVServerUserPermissionSet $InstanceName -UserName $username -PermissionSetId SUPER

# OData
New-NAVWebService -ServerInstance $InstanceName -ServiceName ItemAttributes -ObjectType Page -ObjectId 7500 -Published 1
New-NAVWebService -ServerInstance $InstanceName -ServiceName ItemCards -ObjectType Page -ObjectId 30 -Published 1
New-NAVWebService -ServerInstance $InstanceName -ServiceName AssemblyBOMs -ObjectType Page -ObjectId 36 -Published 1
New-NAVWebService -ServerInstance $InstanceName -ServiceName ProductionBOMs -ObjectType Page -ObjectId 99000786 -Published 1
New-NAVWebService -ServerInstance $InstanceName -ServiceName ProductionBOMLines -ObjectType Page -ObjectId 99000788 -Published 1
New-NAVWebService -ServerInstance $InstanceName -ServiceName RoutingLinks -ObjectType Page -ObjectId 99000798 -Published 1

# OData - APIV2
New-NAVWebService -ServerInstance $InstanceName -ServiceName Items -ObjectType Page -ObjectId 30008 -Published 1
New-NAVWebService -ServerInstance $InstanceName -ServiceName DocumentAttachments -ObjectType Page -ObjectId 30080 -Published 1
New-NAVWebService -ServerInstance $InstanceName -ServiceName ItemCategories -ObjectType Page -ObjectId 30025 -Published 1
New-NAVWebService -ServerInstance $InstanceName -ServiceName UnitsOfMeasures -ObjectType Page -ObjectId 30030 -Published 1
New-NAVWebService -ServerInstance $InstanceName -ServiceName TaxGroups -ObjectType Page -ObjectId 30015 -Published 1
New-NAVWebService -ServerInstance $InstanceName -ServiceName InventoryPostingGroups -ObjectType Page -ObjectId 30096 -Published 1
New-NAVWebService -ServerInstance $InstanceName -ServiceName GeneralProductPostingGroups -ObjectType Page -ObjectId 30079 -Published 1
New-NAVWebService -ServerInstance $InstanceName -ServiceName Vendors -ObjectType Page -ObjectId 30010 -Published 1


Set-NAVServerConfiguration -ServerInstance $InstanceName -KeyName ClientServicesCredentialType -KeyValue NavUserPassword

$navSettingsFile = "C:\inetpub\wwwroot\BC220\navsettings.json"
$a = Get-Content $navSettingsFile -raw | ConvertFrom-Json
$a.NAVWebSettings.ClientServicesCredentialType = "NavUserPassword"
$a | ConvertTo-Json -depth 32| Set-Content $navSettingsFile

Start-Process "iisreset.exe" -NoNewWindow -Wait

Set-NAVServerConfiguration $InstanceName -KeyName SOAPServicesEnabled -KeyValue $false
Set-NAVServerConfiguration $InstanceName -KeyName ODataServicesEnabled -KeyValue $true
Set-NAVServerConfiguration $InstanceName -KeyName ApiServicesEnabled -KeyValue $true
Set-NAVServerConfiguration $InstanceName -KeyName DeveloperServicesEnabled -KeyValue $true

Restart-NAVServerInstance $InstanceName

$app = "C:\Users\christian\Desktop\ItemAttributeMapping\COOLORANGE_PowerGate_1.0.0.0.app"
#Remove-NAVWebService -ServerInstance $InstanceName -ServiceName ItemAttributes -ObjectType CodeUnit -Force
#Remove-NAVWebService -ServerInstance $InstanceName -ServiceName ItemRecordLinks -ObjectType CodeUnit -Force
#Uninstall-NAVApp -ServerInstance $InstanceName -Path $app
#Unpublish-NAVApp -ServerInstance $InstanceName -Path $app
#Get-NAVAppInfo -ServerInstance $InstanceName -Name 'PowerGate' | Uninstall-NAVApp
#Get-NAVAppInfo -ServerInstance $InstanceName -Name 'PowerGate' | Unpublish-NAVApp

Publish-NAVApp -ServerInstance $InstanceName -Path $app -SkipVerification

# WAIT a minute for the app to be published
Start-Sleep -Seconds 60
# If this fails, install the app manually in Business Central

Install-NAVApp -ServerInstance $InstanceName -Path $app
New-NAVWebService -ServerInstance $InstanceName -ServiceName ItemAttributes -ObjectType CodeUnit -ObjectId 50149 -Published 1
New-NAVWebService -ServerInstance $InstanceName -ServiceName ItemRecordLinks -ObjectType CodeUnit -ObjectId 50150 -Published 1
