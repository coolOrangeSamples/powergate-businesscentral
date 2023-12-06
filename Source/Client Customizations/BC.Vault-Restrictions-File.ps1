#==============================================================================#
# (c) 2022 coolOrange s.r.l.                                                   #
#                                                                              #
# THIS SCRIPT/CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER    #
# EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES  #
# OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, OR NON-INFRINGEMENT.   #
#==============================================================================#

# To disable the ERP checks on lifecycle state change, move this script to the %ProgramData%/coolorange/powerEvents/Events/Disabled directory

if ($processName -notin @('Connectivity.VaultPro')) {
	return
}

Register-VaultEvent -EventName UpdateFileStates_Restrictions -Action {
	param($files = @())	

    foreach ($file in $files) {
		$lifecycleState = Get-VaultLifecycleState -LifecycleDefinition $file._NewLifeCycleDefinition -State $file._NewState
		if ($lifecycleState.ReleasedState) {
			$erpItem = Get-ERPObject -EntitySet $itemEntitySet -Keys @{ Number = $file._PartNumber.ToUpper() } #TODO: Business Central converts Item Number to upper case
			if (-not $erpItem) {
				Add-VaultRestriction -EntityName ($file._Name) -Message "Item '$($file._PartNumber)' doesn't exist in $erpName. Create the Item in $erpName first and then release the file again."
			}
			if ($erpItem -and $erpItem.Blocked) {
				Add-VaultRestriction -EntityName ($file._Name) -Message "Item '$($file._PartNumber)' is blocked in $erpName and must not be used."
			}
        }
    }
}