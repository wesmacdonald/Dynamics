<#----------------------
THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
PARTICULAR PURPOSE.
#>

function Run-SolutionPackager() {
	[CmdletBinding()]
	PARAM(
		[string]$commandLineArguments
	)
	process
	{
		Start-Process "$PSScriptRoot\coretools\SolutionPackager.exe" -ArgumentList $commandLineArguments -Wait -NoNewWindow
	}
}

function ExportAndExtract-Solution() { 
[CmdletBinding()]
    PARAM(
        [string]$crmSolutionName, 
        [string]$pkgFolder)
    process
    {
        $crmSolutionFile = "$pkgFolder\$crmSolutionName.zip"
        $crmSolutionExtractPath = "$pkgFolder\$crmSolutionName"

		if (Test-Path $crmSolutionFile)
		{
        Write-Host "Deleting previously downloaded $crmSolutionFile"
			  Remove-Item $crmSolutionFile
		}
        
    # Export unmanaged solution as ZIP
    Start-Process "$PSScriptRoot\ExportSolution\ExportSolution.exe" -ArgumentList "$crmSolutionName $pkgFolder false" -Wait -NoNewWindow 

		if (Test-Path $crmSolutionFile)
		{
        Write-Host "Successfully downloaded $crmSolutionName"
		}
		else
		{
			Write-Host "No Solution found matching $crmSolutionName" -ForegroundColor Red
			return
		}

    # Extract ZIP to identify changes compared to source control repository
    Write-Host "Extracting with $solutionPackagerExe"
    Write-Host "Input (Solution): $crmSolutionFile"
    Write-Host "Output (Folder):  $crmSolutionExtractPath"

    Run-SolutionPackager "/action:Extract /zipFile:$crmSolutionFile /folder:$crmSolutionExtractPath /clobber /allowDelete:Yes /allowWrite:Yes /errorlevel:verbose"
		

    }
}

# Perform generic export and extract of the CRM Solution
ExportAndExtract-Solution -CrmSolutionName $crmSolutionName -PkgFolder $pkgFolder
