# PowerShell Scripts for Stock Data Management

## Compare_tickers.ps1

Enhanced script developed to synchronize stock ticker data between multiple CSV screener files and the Moove database.

### What it does:
- **Multi-file processing**: Reads all `*_screener_*` CSV files in bulk from NASDAQ, NYSE, and AMEX
- **Database comparison**: Connects to SQL Server database and compares tickers between CSV files and `[dbo].[Symbol]` table
- **Smart categorization**: Identifies new tickers, existing tickers, and orphaned tickers
- **SQL generation**: Creates three separate SQL files for database maintenance

### Output Files:
1. **insert_new_symbols.sql** - INSERT statements for new tickers with full column mapping:
   - `[Ticker], [Name], [Exchange], [Sector], [Industry], [QuoteType], [Country], [IPOYear], [MarketCap]`
   - QuoteType defaults to 'S' (Stock)

2. **update_existing_symbols.sql** - UPDATE statements for existing tickers:
   - Updates `[Country], [IPOYear], [MarketCap], [QuoteType]` fields

3. **orphaned.sql** - UPDATE statements for orphaned tickers:
   - Sets `[Enabled] = 0` for tickers in database but not in any CSV file

### Features:
- Handles duplicate tickers across multiple files
- Proper NULL value handling and SQL escaping
- Market Cap parsing (removes commas, validates numeric values)
- Comprehensive logging and summary reporting
- Preview of generated SQL statements

### Usage:
```powershell
.\Compare_tickers.ps1
```

### Data Sources:
Initial CSV data retrieved from:
- https://www.nasdaq.com/market-activity/stocks/screener?page=1&rows_per_page=25

## TODO:
- [ ] Implement automated retrieval of screener data from NASDAQ API endpoints:
  - https://api.nasdaq.com/api/screener/stocks?tableonly=false&limit=25&exchange=NYSE&country=united_states&download=true
  - https://api.nasdaq.com/api/screener/stocks?tableonly=false&limit=25&exchange=AMEX&country=united_states&download=true
  - https://api.nasdaq.com/api/screener/stocks?tableonly=false&limit=25&exchange=NASDAQ&country=united_states&download=true