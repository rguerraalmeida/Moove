# Requires PowerShell 7 or later for ForEach-Object -Parallel

# Directory containing the files
$directory = ".\historical-data"
$today = (Get-Date).Date
$daysToConsiderRecent = 7 # Days to consider data as recent
$recentDateThreshold = $today.AddDays(-$daysToConsiderRecent) # Calculate recent date threshold
$outputFile = ".\stock_summary.csv"

# Check if the directory exists
if (Test-Path -Path $directory) {
    # Get all CSV files in the directory
    $files = Get-ChildItem -Path $directory -Filter "*-historical-data.csv"

    # Process each file in parallel
    $outputData = $files | ForEach-Object -Parallel {
        $file = $_
        $stockData = Import-Csv -Path $file.FullName

        # Check the last record's date
        $lastRecord = $stockData[-1]
        $lastDate = [datetime]$lastRecord.Date
        $isRecent = $lastDate -ge $using:recentDateThreshold

        if (-not $isRecent) {
            return
        }

        $ath = 0
        $athDate = $null

        foreach ($record in $stockData) {
            $close = $record.Close -as [double]
            if ($null -ne $close -and $close -gt $ath) {
                $ath = $close
                $athDate = [datetime]$record.Date
            }
        }

        $currentPrice = $lastRecord.Close -as [double]
        $percentageDiff = ($ath - $currentPrice) / $ath * 100
        $daysSinceATH = ($using:today - $athDate).Days

        [PSCustomObject]@{
            Ticker = $file.BaseName.Split('-')[0].ToUpper()
            CurrentPrice = $currentPrice.ToString('N2')
            ATH = $ath.ToString('N2')
            ATHDate = $athDate
            PercentageDiff = $percentageDiff.ToString('N2')
            IsRecent = $isRecent
            DaysSinceATH = $daysSinceATH
        }
    } -ThrottleLimit 10

    # Export the data to a CSV file
    $outputData | Export-Csv -Path $outputFile -NoTypeInformation

    Write-Host "Data exported to $outputFile"
} else {
    Write-Host "Directory not found: $directory"
}
