# Define file paths
$errorReportPath = ".\error_report.json"
# Check if the error report file exists and the date is today
if (Test-Path $errorReportPath) {
    $errorReport = Get-Content $errorReportPath | ConvertFrom-Json
    $errorDate = [DateTime]::Parse($errorReport.Date)
    $today = (Get-Date).Date

    if ($errorDate -eq $today) {
        # Iterate through each error in the report and retry the download
        foreach ($error in $errorReport.Errors) {
            try {
                # Construct the URL for downloading the data
                $url = "https://query1.finance.yahoo.com/v7/finance/download/$($error.Ticker)?period1=631152000&period2=$unixTimestamp&interval=1d&events=history&includeAdjustedClose=true"

                # Use Invoke-WebRequest to download the data
                $response = Invoke-WebRequest -Uri $url

                # Save the data to a file
                $cleanTicker = $error.Ticker.ToLower() -replace '[^a-zA-Z0-9]', '_'
                $filename = ".\historical-data\$cleanTicker-historical-data.csv"
                $response.Content | Out-File $filename
                Write-Host "Data successfully downloaded for $($error.Ticker)"
            } catch {
                # Add the error to a new error list if the retry also fails
                $errorObject = [PSCustomObject]@{
                    Ticker     = $error.Ticker
                    Company    = $error.Company
                    Exception  = $_.Exception.Message
                }
                $errorsList += $errorObject
                Write-Host "Error retrying download for $($error.Ticker): $_"
            }
        }
        
        # Update the error report with any new errors
        $newErrorReport = [PSCustomObject]@{
            Date   = $today
            Errors = $errorsList
        }
        $newErrorReport | ConvertTo-Json | Out-File -FilePath $errorReportPath
    } else {
        Write-Host "The error report is not from today. Exiting script."
        exit
    }
} else {
    Write-Host "No error report found. Exiting script."
    exit
}
