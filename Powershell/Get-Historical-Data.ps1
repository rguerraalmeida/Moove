# Define the path to the CSV file
$csvPath = ".\nasdaq_screener.csv"
# Import the CSV file into a PowerShell object
$nasdaqData = Import-Csv -Path $csvPath
# Create an object to store errors
$errorsList = @()
$errorDate = (Get-Date).Date
# Get today's date at 23:59
$todayAt2359 = Get-Date -Hour 23 -Minute 59 -Second 0
# Convert to Unix timestamp
$unixTimestamp = [int][double]::Parse((New-TimeSpan -Start (Get-Date "1970-01-01") -End $todayAt2359).TotalSeconds)
# Define a custom user agent
$userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36"
# Iterate through each row in the CSV file
foreach ($ticker in $nasdaqData) {
    try {
        #Write-Host "Symbol: $($ticker.Symbol), Name: $($ticker.Name), Country: $($ticker.Country), Sector: $($ticker.Sector), Industry: $($ticker.Industry), Market Cap: $($ticker.'Market Cap')"
        $url = "https://query1.finance.yahoo.com/v7/finance/download/$($ticker.Symbol)?period1=631152000&period2=$unixTimestamp&interval=1d&events=history&includeAdjustedClose=true"
        # Use Invoke-WebRequest to download the data
        $response = Invoke-WebRequest -Uri $url -UserAgent $userAgent
        # Convert the ticker to lowercase and replace special characters with a dash
        $cleanTicker = $ticker.Symbol.ToLower() -replace '[^a-zA-Z0-9]', '_'
        # Filename for the CSV
        $filename = ".\historical-data\$cleanTicker-historical-data.csv"
        # Save the content to a CSV file
        $response.Content | Out-File $filename
        # Output the filename
        #Write-Host "Data saved to $filename"
    } catch {
        # Create an error object and add it to the list
        $errorObject = [PSCustomObject]@{
            Ticker     = $ticker.Symbol
            Company    = $ticker.Name
            Exception  = $_.Exception.Message
        }
        $errorsList += $errorObject
    }
}

# Create the final object with date and errors
$errorReport = [PSCustomObject]@{
    Date   = $errorDate
    Errors = $errorsList
}
# Convert the object to JSON
$jsonErrorReport = $errorReport | ConvertTo-Json
# Output the JSON to a file
$jsonErrorReport | Out-File -FilePath ".\error_report.json"
# Optionally, display the JSON in the console
Write-Host $jsonErrorReport