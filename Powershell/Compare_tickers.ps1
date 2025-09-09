# Enhanced PowerShell script to compare multiple screener CSV files with database and generate INSERT/UPDATE statements

# Connection string
$connectionString = "Data Source=DESKTOP-O691E9E\SQLEXPRESS;Initial Catalog=Moove;Integrated Security=true;Connect Timeout=60;Encrypt=True;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"

# Find and read all screener CSV files
Write-Host "Finding screener CSV files..."
$screenerFiles = Get-ChildItem -Path "." -Name "*_screener_*"
Write-Host "Found $($screenerFiles.Count) screener files: $($screenerFiles -join ', ')"

# Read all CSV files and combine data
$allCsvData = @()
foreach ($file in $screenerFiles) {
    Write-Host "Reading $file..."
    $csvData = Import-Csv -Path $file
    Write-Host "  - Found $($csvData.Count) rows"
    $allCsvData += $csvData
}

Write-Host "Total CSV records: $($allCsvData.Count)"

# Connect to database and get existing tickers with their data
Write-Host "Connecting to database..."
$connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
$connection.Open()

$query = "SELECT [Ticker], [Name] FROM [dbo].[Symbol]"
$command = New-Object System.Data.SqlClient.SqlCommand($query, $connection)
$reader = $command.ExecuteReader()

$dbTickers = @{}
$dbTickersList = @()
while ($reader.Read()) {
    $ticker = $reader["Ticker"].ToString()
    $name = $reader["Name"].ToString()
    $dbTickers[$ticker] = $name
    $dbTickersList += $ticker
}
$reader.Close()
$connection.Close()

Write-Host "Found $($dbTickersList.Count) tickers in database"

# Get unique CSV tickers (remove duplicates)
$csvTickers = $allCsvData | Select-Object -Property Symbol -Unique | Select-Object -ExpandProperty Symbol
$uniqueCsvData = @{}
foreach ($row in $allCsvData) {
    if (-not $uniqueCsvData.ContainsKey($row.Symbol)) {
        $uniqueCsvData[$row.Symbol] = $row
    }
}

Write-Host "Unique CSV tickers: $($csvTickers.Count)"

# Compare and categorize tickers
$newTickers = $csvTickers | Where-Object { $_ -notin $dbTickersList }
$existingTickers = $csvTickers | Where-Object { $_ -in $dbTickersList }
$orphanedTickers = $dbTickersList | Where-Object { $_ -notin $csvTickers }

Write-Host "New tickers to INSERT: $($newTickers.Count)"
Write-Host "Existing tickers to UPDATE: $($existingTickers.Count)"
Write-Host "Orphaned tickers (in DB but not in CSV): $($orphanedTickers.Count)"

# Generate INSERT statements for new tickers
Write-Host "Generating INSERT statements..."
$insertStatements = @()
foreach ($ticker in $newTickers) {
    $csvRow = $uniqueCsvData[$ticker]
    
    # Clean and escape single quotes, handle NULL values
    $name = if ($csvRow.Name) { ($csvRow.Name -replace "'", "''") } else { "NULL" }
    $exchange = if ($csvRow.Exchange) { ($csvRow.Exchange -replace "'", "''") } else { "''" }
    $sector = if ($csvRow.Sector) { ($csvRow.Sector -replace "'", "''") } else { "NULL" }
    $industry = if ($csvRow.Industry) { ($csvRow.Industry -replace "'", "''") } else { "NULL" }
    $quoteType = "'S'"
    $country = if ($csvRow.Country) { "'$($csvRow.Country -replace "'", "''")'" } else { "NULL" }
    $ipoYear = if ($csvRow.'IPO Year' -and $csvRow.'IPO Year' -ne '') { $csvRow.'IPO Year' } else { "NULL" }
    $marketCap = if ($csvRow.'Market Cap' -and $csvRow.'Market Cap' -ne '') { 
        # Remove commas and convert to numeric if possible
        $mcValue = $csvRow.'Market Cap' -replace '[,$]', ''
        if ($mcValue -match '^\d+\.?\d*$') { $mcValue } else { "NULL" }
    } else { "NULL" }
    
    $insertStatement = @"
INSERT INTO [dbo].[Symbol] ([Ticker], [Name], [Exchange], [Sector], [Industry], [QuoteType], [Country], [IPOYear], [MarketCap])
VALUES ('$ticker', '$name', '$exchange', '$sector', '$industry', $quoteType, $country, $ipoYear, $marketCap);
"@
    
    $insertStatements += $insertStatement
}

# Generate UPDATE statements for existing tickers
Write-Host "Generating UPDATE statements..."
$updateStatements = @()
foreach ($ticker in $existingTickers) {
    $csvRow = $uniqueCsvData[$ticker]
    
    # Clean and prepare update values
    $country = if ($csvRow.Country) { "'$($csvRow.Country -replace "'", "''")'" } else { "NULL" }
    $ipoYear = if ($csvRow.'IPO Year' -and $csvRow.'IPO Year' -ne '') { $csvRow.'IPO Year' } else { "NULL" }
    $marketCap = if ($csvRow.'Market Cap' -and $csvRow.'Market Cap' -ne '') {
        $mcValue = $csvRow.'Market Cap' -replace '[,$]', ''
        if ($mcValue -match '^\d+\.?\d*$') { $mcValue } else { "NULL" }
    } else { "NULL" }
    
    $updateStatement = @"
UPDATE [dbo].[Symbol] 
SET [Country] = $country, [IPOYear] = $ipoYear, [MarketCap] = $marketCap, [QuoteType] = 'S'
WHERE [Ticker] = '$ticker';
"@
    
    $updateStatements += $updateStatement
}

# Generate orphaned tickers UPDATE statements
Write-Host "Generating orphaned tickers UPDATE statements..."
$orphanedStatements = @()
foreach ($ticker in $orphanedTickers) {
    $orphanedStatement = @"
UPDATE [dbo].[Symbol] 
SET [Enabled] = 0
WHERE [Ticker] = '$ticker';
"@
    $orphanedStatements += $orphanedStatement
}

# Write all output files
$insertFile = "insert_new_symbols.sql"
$updateFile = "update_existing_symbols.sql"
$orphanedFile = "orphaned.sql"

if ($insertStatements.Count -gt 0) {
    $insertStatements | Out-File -FilePath $insertFile -Encoding UTF8
    Write-Host "Generated $($insertStatements.Count) INSERT statements -> $insertFile"
} else {
    Write-Host "No new symbols to insert"
}

if ($updateStatements.Count -gt 0) {
    $updateStatements | Out-File -FilePath $updateFile -Encoding UTF8
    Write-Host "Generated $($updateStatements.Count) UPDATE statements -> $updateFile"
} else {
    Write-Host "No existing symbols to update"
}

if ($orphanedStatements.Count -gt 0) {
    $orphanedStatements | Out-File -FilePath $orphanedFile -Encoding UTF8
    Write-Host "Generated $($orphanedStatements.Count) orphaned UPDATE statements -> $orphanedFile"
} else {
    Write-Host "No orphaned symbols found"
}

# Display summary
Write-Host "`n=== SUMMARY ==="
Write-Host "Total screener files processed: $($screenerFiles.Count)"
Write-Host "Total unique CSV tickers: $($csvTickers.Count)"
Write-Host "Database tickers: $($dbTickersList.Count)"
Write-Host "New tickers (INSERT): $($newTickers.Count)"
Write-Host "Existing tickers (UPDATE): $($existingTickers.Count)"
Write-Host "Orphaned tickers (DB only): $($orphanedTickers.Count)"

# Display first few statements as preview
if ($insertStatements.Count -gt 0) {
    Write-Host "`nFirst 2 INSERT statements:"
    $insertStatements | Select-Object -First 2 | ForEach-Object { Write-Host $_ -ForegroundColor Green }
}

if ($updateStatements.Count -gt 0) {
    Write-Host "`nFirst 2 UPDATE statements:"
    $updateStatements | Select-Object -First 2 | ForEach-Object { Write-Host $_ -ForegroundColor Yellow }
}