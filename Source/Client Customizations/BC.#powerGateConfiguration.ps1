#==============================================================================#
# (c) 2022 coolOrange s.r.l.                                                   #
#                                                                              #
# THIS SCRIPT/CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER    #
# EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES  #
# OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, OR NON-INFRINGEMENT.   #
#==============================================================================#

# To disable the ToolsMenu items for showing the configuration, move this script to the %ProgramData%/coolorange/powerEvents/Events/Disabled directory

if ($processName -notin @('Connectivity.VaultPro', 'Inventor')) {
    return
}

#Remark: The configuration functions are going to be replaced by the new powerGate Configuration Manager
#https://youtrack.coolorange.com/youtrack/issue/PG-1391/Configurable-ERP-Item-Creation-from-Vault-Files-Items-via-Field-Mappings

$powerGateErpTabconfiguration = "$PSScriptRoot\$filePrefix.#powerGateConfiguration.xml"

Add-VaultMenuItem -Location ToolsMenu -Name "powerGate - Open $erpName Configuration..." -Action {

    if(Test-Path $powerGateErpTabconfiguration) {
        Start-Process -FilePath explorer.exe -ArgumentList "/select, ""$powerGateErpTabconfiguration"""
    }
}

function GetPowerGateConfiguration($section) {
    Write-Host "Retrieving configuration for section: $section"

    if(-not (Test-Path $powerGateErpTabconfiguration)) {
        Write-Host "Configuration could not be retrieved from: '$powerGateErpTabconfiguration'"
        return
    }

    $configuration = [xml](Get-Content $powerGateErpTabconfiguration) 
    if ($null -eq $configuration -or $configuration.HasChildNodes -eq $false) {
        return
    }
    $configEntries = Select-Xml -xml $configuration -XPath "//$section"
    return @($configEntries.Node.ChildNodes | Sort-Object -Property value)
}

# Enable the user to automatically create all required Vault UDPs including mappings to Inventor iProperties
# TODO: This is for evaluation purposes only and must be removed for productive environments
Add-VaultMenuItem -Location ToolsMenu -Name "powerGate - Create Vault Properties" -Action {
    $propertyDefinitions = $vault.PropertyService.GetPropertyDefinitionsByEntityClassId($null)
    $numberPropDef = $propertyDefinitions | Where-Object {$_.DispName -eq "Raw Material Number"}
    if (-not $numberPropDef) {
        $ctntSrcPropDef = [Autodesk.Connectivity.WebServices.CtntSrcPropDef]::new()
        $ctntSrcPropDef.CtntSrcId = 4
        $ctntSrcPropDef.DispName = "Raw Material Number"
        $ctntSrcPropDef.Moniker = "Raw Material Number!{D5CDD505-2E9C-101B-9397-08002B2CF9AE}!nvarchar"
        $ctntSrcPropDef.MapDirection = [Autodesk.Connectivity.WebServices.AllowedMappingDirection]::ReadAndWrite
        $ctntSrcPropDef.CanCreateNew = $true
        $ctntSrcPropDef.Classification = [Autodesk.Connectivity.WebServices.Classification]::None
        $ctntSrcPropDef.Typ = [Autodesk.Connectivity.WebServices.DataType]::String

        $cfgFile = [Autodesk.Connectivity.WebServices.EntClassCtntSrcPropCfg]::new()
        $cfgFile.EntClassId = "FILE"
        $cfgFile.CtntSrcPropDefArray = @($ctntSrcPropDef)
        $cfgFile.MapTypArray = @([Autodesk.Connectivity.WebServices.MappingType]::Constant)
        $cfgFile.PriorityArray = @(1)
        $cfgFile.MapDirectionArray = @([Autodesk.Connectivity.WebServices.MappingDirection]::Read)
        $cfgFile.CanCreateNewArray = @($true)

        $cfgItem = [Autodesk.Connectivity.WebServices.EntClassCtntSrcPropCfg]::new()
        $cfgItem.EntClassId = "ITEM"
        $cfgItem.CtntSrcPropDefArray = @($ctntSrcPropDef)
        $cfgItem.MapTypArray = @([Autodesk.Connectivity.WebServices.MappingType]::Constant)
        $cfgItem.PriorityArray = @(2)
        $cfgItem.MapDirectionArray = @([Autodesk.Connectivity.WebServices.MappingDirection]::Read)
        $cfgItem.CanCreateNewArray = @($true)

        try {
            $null = $vault.PropertyService.AddPropertyDefinition(
                [guid]::NewGuid(),
                "Raw Material Number",
                [Autodesk.Connectivity.WebServices.DataType]::String,
                $true,
                $true,
                $null,
                @("FILE", "ITEM"),
                @($cfgFile, $cfgItem),
                $null,
                @()
            )            
        }
        catch {
            $null = [System.Windows.Forms.MessageBox]::Show("Error while creating Property Definition 'Raw Material Quantity'!", "powerGate - Create Vault Properties", "OK", "Error")
            return
        }

    }

    $quantityPropDef = $propertyDefinitions | Where-Object {$_.DispName -eq "Raw Material Quantity"}
    if (-not $quantityPropDef) {
        $ctntSrcPropDef = [Autodesk.Connectivity.WebServices.CtntSrcPropDef]::new()
        $ctntSrcPropDef.CtntSrcId = 4
        $ctntSrcPropDef.DispName = "Raw Material Quantity"
        $ctntSrcPropDef.Moniker = "Raw Material Quantity!{D5CDD505-2E9C-101B-9397-08002B2CF9AE}!float(53)"
        $ctntSrcPropDef.MapDirection = [Autodesk.Connectivity.WebServices.AllowedMappingDirection]::ReadAndWrite
        $ctntSrcPropDef.CanCreateNew = $true
        $ctntSrcPropDef.Classification = [Autodesk.Connectivity.WebServices.Classification]::None
        $ctntSrcPropDef.Typ = [Autodesk.Connectivity.WebServices.DataType]::String

        $cfgFile = [Autodesk.Connectivity.WebServices.EntClassCtntSrcPropCfg]::new()
        $cfgFile.EntClassId = "FILE"
        $cfgFile.CtntSrcPropDefArray = @($ctntSrcPropDef)
        $cfgFile.MapTypArray = @([Autodesk.Connectivity.WebServices.MappingType]::Constant)
        $cfgFile.PriorityArray = @(1)
        $cfgFile.MapDirectionArray = @([Autodesk.Connectivity.WebServices.MappingDirection]::Read)
        $cfgFile.CanCreateNewArray = @($true)

        $cfgItem = [Autodesk.Connectivity.WebServices.EntClassCtntSrcPropCfg]::new()
        $cfgItem.EntClassId = "ITEM"
        $cfgItem.CtntSrcPropDefArray = @($ctntSrcPropDef)
        $cfgItem.MapTypArray = @([Autodesk.Connectivity.WebServices.MappingType]::Constant)
        $cfgItem.PriorityArray = @(2)
        $cfgItem.MapDirectionArray = @([Autodesk.Connectivity.WebServices.MappingDirection]::Read)
        $cfgItem.CanCreateNewArray = @($true)

        try {
            $null = $vault.PropertyService.AddPropertyDefinition(
                [guid]::NewGuid(),
                "Raw Material Quantity",
                [Autodesk.Connectivity.WebServices.DataType]::String,
                $true,
                $true,
                $null,
                @("FILE", "ITEM"),
                @($cfgFile, $cfgItem),
                $null,
                @()
            )            
        }
        catch {
            $null = [System.Windows.Forms.MessageBox]::Show("Error while creating Property Definition 'Raw Material Quantity'!", "powerGate - Create Vault Properties", "OK", "Error")
            return
        }
    }

    if (-not $numberPropDef -or -not $quantityPropDef) {
        $null = [System.Windows.Forms.MessageBox]::Show("Property Definitions 'Raw Material Number' and 'Raw Material Quantity' have been created!", "powerGate - Create Vault Properties", "OK", "Information")
        return
    }

    $null = [System.Windows.Forms.MessageBox]::Show("All required Property Definitions already exist in Vault!", "powerGate - Create Vault Properties", "OK", "Information")
}