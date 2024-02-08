# powergate-businesscentral
[![Build Status](https://dev.azure.com/CoolOrange/LABS/_apis/build/status%2FcoolOrangeLabs.powergate-businesscentral?branchName=main)](https://dev.azure.com/CoolOrange/LABS/_build/latest?definitionId=201&branchName=main)

[![Windows](https://img.shields.io/badge/Platform-Windows-lightgray.svg)](https://www.microsoft.com/en-us/windows/)
[![PowerShell](https://img.shields.io/badge/PowerShell-5-blue.svg)](https://microsoft.com/PowerShell/)
[![.NET](https://img.shields.io/badge/.NET%20Framework-4.8-blue.svg)](https://dotnet.microsoft.com/)

[![powerGate](https://img.shields.io/badge/coolOrange%20powerGate-24-orange.svg)](https://www.coolorange.com/powerGate)
[![powerGate Server](https://img.shields.io/badge/coolOrange%20powerJobs-24-orange.svg)](https://www.coolorange.com/powerJobs)

## Disclaimer

THE SAMPLE CODE ON THIS REPOSITORY IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, OR NON-INFRINGEMENT.

THE USAGE OF THIS SAMPLE IS AT YOUR OWN RISK AND **THERE IS NO SUPPORT** RELATED TO IT.

## Description

powerGate Server Plugin that connects Autodesk Vault with Microsoft Dynamics Business Central (Cloud and On-Prem). Supports Business Central 2023 Release Wave 1 and later versions.

Business Central 2023 Release Wave 1 (On-Prem) can be downloaded here: [https://www.microsoft.com/en-us/download/details.aspx?id=105113](https://www.microsoft.com/en-us/download/details.aspx?id=105113)

## Installation

### powerGate Server and powerGate Plugin Installation
Install [powerGate Server](https://www.coolorange.com/download) and the [Business Central plugin](https://github.com/coolOrangeLabs/powergate-businesscentral/releases) for powerGate Server on a server machine. For small and mid size installations, this is typically the Vault ADMS Server.

### powerGate Client Installation
Install [powerGate Client Collection](https://www.coolorange.com/download) to all Vault Clients, download all files from the "Source" - "[Client Customization](https://github.com/coolOrangeLabs/powergate-businesscentral/tree/main/Source/Client%20Customizations)" directory of this repository and copy them to "*C:\ProgramData\coolOrange\Client Customizations*" on all client machines. Make sure the files are not blocked by the Operating System after the download. In case they are blocked, [unblock](https://support.coolorange.com/kb/how-to-unblock-files) all files.

### Business Central Application Installation

In Business Central, go to "Administration" - "General" - "System" - "Extension Management" and upload and install the [COOLORANGE\_PowerGate\_1.0.0.0.app](https://github.com/coolOrangeLabs/powergate-businesscentral/blob/main/COOLORANGE_PowerGate_1.0.0.0.app) Application. In the "Extension Management" (Installed Extensions), click on "Manage" - "Upload Extension...", select the .app file and deploy the extension. Once uploaded, find the extension in the "Extension Management" windows and make sure it is installed and deployed, 

This application installs the code units "50149" and "50150" in Business Central. These code units provide web services for "Item Attributes" and "Item Record Links".

## Configuration

### Business Central Web Services

In Business Central, go to "Administration" - "System Administration" - "Services" - "Web Services" and activate the following web services:

| Object Type | Object ID | Object Name  | Service Name  | 
|:---|---:|:---|:---|
| Page | 30 | Item Card | ***ItemCards*** |
| Page	| 7500 | Item Attributes | ***ItemAttributes*** |
| Page	| 30008 | APIV2 - Items | 	***Items*** |
| Page	| 30010 | APIV2 - Vendors | ***Vendors*** |
| Page	| 30025 | APIV2 - Item Categories | 	***ItemCategories*** |
| Page	| 30030 | APIV2 - Units of Measure | 	***UnitsOfMeasures*** |
| Page	| 30079 | APIV2 - Gen. Prod. Post. Group | 	***GeneralProductPostingGroups*** |
| Page	| 30080 | APIV2 - Document Attachments | 	***DocumentAttachments*** |
| Page	| 30096| APIV2 - Inventory Post. Group | 	***InventoryPostingGroups*** |
| Codeunit | 50149 | ItemAttributes | ***ItemAttributes*** |
| Codeunit| 50150 | ItemRecordLinks | ***ItemRecordLinks*** |
| Page	| 99000786 | Production BOM | ***ProductionBOMs*** |
| Page	| 99000788 | Lines | ***ProductionBOMLines*** |
| Page	| 99000798 | Routing Links | ***RoutingLinks*** |

Make sure, the web services are "Published" and that the ***Service Name*** exactly matches, otherwise the plugin will terminate with errors.

### Business Central Authentication (Cloud)
For a Business Central Cloud authentication, a Service-to-Service (S2S) Authentication must be setup. Follow the [official documentation](https://learn.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/automation-apis-using-s2s-authentication) to setup a S2S OAuth connection. Pictured step-by-step instructions can also be found here: [https://thatnavguy.com/d365-business-central-setup-oauth2-authentication/](https://thatnavguy.com/d365-business-central-setup-oauth2-authentication/)

### Business Central Authentication (On-Prem)
For an authentication with Business Central that is installed on-prem, the "NavUserPassword" [credential type](https://learn.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/users-credential-types) must be enabled. This can be setup in the navsettings.json file of the on-prem Business Central installation.


### powerGate Plugin Settings
On the powerGate Server, open the file "*C:\ProgramData\coolOrange\powerGateServer\Plugins\BusinessCentral\BusinessCentralPlugin.dll.config*" in a text editor and modify the following settings:


#### Business Central general settings

```xml
<BusinessCentral>
	<!-- Business Central Company name -->
	<add key="Company" value="CRONUS USA, Inc." />

	<!-- Business Central Auth Type. "OAuth" for cloud, "Basic" for on-prem installations -->
	<add key="AuthType" value="OAuth" />

	<!-- Validates the Configuration on startup by checking the configured settings against Business Central -->
	<!-- Set to 'False' for productive use! -->
	<add key="EnableStartupCheck" value="False" />
</BusinessCentral>`
```
##### Company
Go to "Settings" - "Company Information", copy the company name and paste it to the "value" attribute. This must match, because this value gets used when calling the Business Central web services.

##### AuthType
Use "OAuth" when Business Central is in the cloud, otherwise "Basic".

##### EnableStartupCheck
Only used for debugging purposes. Value must be "False".

#### Settings for Business Central Cloud
```xml
<BusinessCentral.OAuth>
	<!-- Business Central OData Base Url: e.g. https://api.businesscentral.dynamics.com/v2.0/[TenantId]/Sandbox/ODataV4) -->
	<add key="BaseUrl" value="https://api.businesscentral.dynamics.com/v2.0/00000000-0000-0000-0000-000000000000/Sandbox/ODataV4" />

	<!-- Business Central TenantId -->
	<add key="TenantId" value="00000000-0000-0000-0000-000000000000" />

	<!-- Business Central ClientId -->
	<add key="ClientId" value="[yourClientID]" />

	<!-- Business Central Client Secret -->
	<add key="ClientSecret" value="[yourClient Secret]" />
</BusinessCentral.OAuth>
```

##### BaseUrl
Provide the web service base URL until and including "ODataV4"

##### TenantID
The Tenant ID a GUID and part of the URL. E.g. https://businesscentral.dynamics.com/**00000000-0000-0000-0000-000000000000**/Sandbox

##### ClientId and ClientSecret
See "Configuration" - "Business Central Authentication (Cloud)" above for more information.

#### Settings for Business Central On-Prem
```xml
	<BusinessCentral.BasicAuth>
		<!-- Business Central OData Base Url: e.g. http://servername:7048/BC220/ODataV4) -->
		<add key="BaseUrl" value="http://mybcservername:7048/BC220/ODataV4" />
		
		<!-- Business Central Username for Basic Auth -->
		<add key="Username" value="powerGate" />
		
		<!-- Business Central Username for Basic Auth -->
		<add key="Password" value="c00!Orange" />
	</BusinessCentral.BasicAuth>
```

##### BaseUrl
Provide the web service base URL until and including "ODataV4"

##### Username
The user name in Business Central on-prem to be used by the web services ("NavUserPassword").

##### Password
The password for the user in Business Central on-prem to be used by the web services ("NavUserPassword").

#### Business Central default values
```xml
	<BusinessCentral.Settings>
		<!-- Default 'Type' used to create a new Business Central Item. Cannot be empty and must be either 'Inventory', 'Service' or 'Non-Inventory'. -->
		<add key="Default_Item_Type" value="Inventory" />

		<!-- Default 'Item Category Code' used to create a new Business Central Item. Can be empty. -->
		<add key="Default_Item_Category_Code" value="MISC" />

		<!-- Default 'Type' used to create a new Business Central Item. Can be empty. -->
		<add key="Default_Inventory_Posting_Group" value="FINISHED" />

		<!-- Default 'Gen. Prod. Posting Group' used to create a new Business Central Item. Can be empty. -->
		<add key="Default_General_Product_Posting_Group" value="MANUFACT" />

		<!-- Value that indicates if an Item is flagged as Make or Buy. If the BC Item's 'Gen. Prod. Posting Group' field has this value, the Vault UI shows 'Make', otherwise is shows 'Buy' -->
		<add key="General_Product_Posting_Group_Make_Indicator" value="MANUFACT" />

		<!-- Name of the Attribute (Item/Details/Item Attributes) for the Description. Must exist as a Attribute in BC. Cannot be empty. -->
		<add key="Item_Attribute_Description" value="Material Description" />
		
		<!-- Name of the Attribute (Item/Details/Item Attributes) for the Material. Must exist as a Attribute in BC. Cannot be empty. -->
		<add key="Item_Attribute_Material" value="Material (Surface)" />

		<!-- Name of the Link (Item/Attachments/Links) for a Vault Thin Client Link. Cannot be empty. -->
		<add key="Item_Link_ThinClient" value="Vault Thin Client" />
		
		<!-- Name of the Link (Item/Attachments/Links) for a Vault Thick Client Link. Cannot be empty. -->
		<add key="Item_Link_ThickClient" value="Vault Explorer" />

		<!-- Code of the Routing Link of BOM rows that identify the row as Raw Material. Must be setup in Business Central -->
		<add key="Routing_Link_RawMaterial" value="900" />
	</BusinessCentral.Settings>`
```
To adjust the behavior of the powerGate Server plugin, these settings can be used to specify how items are created.

### powerGate Client Customizations Settings

On the Vault Client machines, in the directory "*C:\ProgramData\coolOrange\Client Customizations*" find the following files and edit them with a text editor:

#### BC.#powerGate.ps1
Change the URL to the powerGate Server on line 34

#### BC.#powerGateConfiguration.xml
Change the mapping of Unit of Measures between Vault and Business Central in this XML file. Entry=Business Central UOM; Value=Vault UOM

#### BC.#powerGateMappingFiles.ps1
Change the property mapping between Vault Files and Business Central items and Production BOMs in this file (PowerShell syntax).

#### BC.#powerGateMappingItems.ps1
Change the property mapping between Vault Items and Business Central items and Production BOMs in this file (PowerShell syntax).


## Product Documentation

[coolOrange powerGate](https://www.coolorange.com/wiki/doku.php?id=powergate)  
[coolOrange powerGate Server](https://www.coolorange.com/wiki/doku.php?id=powergateserver)

## Author
coolOrange s.r.l.  

![coolOrange](https://i.ibb.co/NmnmjDT/Logo-CO-Full-colore-RGB-short-Payoff.png)


![]()
