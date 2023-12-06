#==============================================================================#
# (c) 2022 coolOrange s.r.l.                                                   #
#                                                                              #
# THIS SCRIPT/CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER    #
# EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES  #
# OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, OR NON-INFRINGEMENT.   #
#==============================================================================#

# To disable the Inventor Menu 'Link ERP Item...', move this script to the %ProgramData%/coolorange/powerEvents/Events/Disabled directory

if ($processName -notin @('Inventor')) {
    return
}

Add-InventorMenuItem -Name "Link`n$erpName Item..." -Action {
    $document = $inventor.ActiveDocument

    #region Pre-Checks
    if ($document.IsModifiable -eq $fale) {
        $null = [System.Windows.MessageBox]::Show("The current document is not editable! Please re-open the file as editable for creating a link to an Item in $erpName.", "powerGate - Document not editable", "OK", "Information")
        return
    }

    if ($document.DocumentType -notin @([Inventor.DocumentTypeEnum]::kAssemblyDocumentObject, [Inventor.DocumentTypeEnum]::kPartDocumentObject)) {
        $null = [System.Windows.MessageBox]::Show("This function is available only on parts or assemblies!", "powerGate - Document type not supported", "OK", "Information")
        return
    }
    #endregion

    $xamlFile = [xml](Get-Content "$PSScriptRoot\$filePrefix.ErpItemLink.xaml")
    $window = [Windows.Markup.XamlReader]::Load( (New-Object System.Xml.XmlNodeReader $xamlFile) )
    $window.Title = "powerGate - Link $erpName Item"
    $window.FindName('Title').Content = "Search and link $erpName Item"
    $window.FindName('SearchResults').Tag = "Search $erpName Item"

    $searchCriteria = New-Object PsObject -Property @{"SearchTerm" = "" }
    $window.FindName('SearchCriteria').DataContext = $searchCriteria

    $window.FindName('ButtonSearch').Add_Click({
        $searchCriteria = $window.FindName('SearchCriteria').DataContext
        if ($null -ne $searchCriteria.SearchTerm -and $searchCriteria.SearchTerm -ne "") {
            $window.Cursor = [System.Windows.Input.Cursors]::Wait
            # TODO: Change the search fields if needed
            $results = Get-ERPObjects -EntitySet $itemEntitySet -Filter "(substringof('$($searchCriteria.SearchTerm.ToUpper())',toupper(Number)) eq true) or (substringof('$($searchCriteria.SearchTerm.ToUpper())',toupper(Title)) eq true) or (substringof('$($searchCriteria.SearchTerm.ToUpper())',toupper(Description)) eq true)  or (substringof('$($searchCriteria.SearchTerm.ToUpper())',toupper(Material)) eq true)"
            $window.FindName('SearchResults').ItemsSource = $results
            $window.FindName('SearchResults').Tag = "No $erpName Item found for '$($searchCriteria.SearchTerm)'"
            $window.FindName('SearchTerm').Cursor = $null
            $window.Cursor = $null
        }
    }.GetNewClosure())

    $window.FindName('SearchTerm').Add_KeyDown({
        if ($_.Key -eq "Enter") {
            $window.FindName('SearchTerm').Cursor = [System.Windows.Input.Cursors]::Wait
            $window.FindName('ButtonSearch').RaiseEvent((New-Object System.Windows.RoutedEventArgs([System.Windows.Controls.Button]::ClickEvent)))
        }
    }.GetNewClosure())

    $window.FindName('ButtonLink').Add_Click({
        $selectedElement = $window.FindName('SearchResults').SelectedItems[0]
		$number = $selectedElement.Number
        $oldNumber = $document.PropertySets.Item('Design Tracking Properties')['Part Number'].Value
		$answer = [System.Windows.Forms.MessageBox]::Show("To link the $erpName Item '$number' with the current file, the iProperty 'Part Number' will be changed from '$oldNumber' to '$number'.`n`nAre you shure you want to proceed?","powerGate - Confirm operation", "YesNo", "Warning", "Button1")
		if($answer -eq "Yes"){
            $document.PropertySets.Item('Design Tracking Properties')['Part Number'].Value = $selectedElement.Number
            #TODO: Write back other properties from ERP if needed. The following lines are just an example
            $document.PropertySets.Item('Inventor Summary Information')['Title'].Value = $selectedElement.Title
            $document.PropertySets.Item('Design Tracking Properties')['Description'].Value = $selectedElement.Description
			
            $window.DialogResult = $true
            $window.Close()
        }
    }.GetNewClosure())

    $null = $window.FindName('SearchTerm').Focus()
    $null = $window.ShowDialog()
}