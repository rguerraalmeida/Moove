# Stock Technical Analysis Tool

A Python tool for analyzing stock data with technical indicators and candlestick pattern detection.

## Features

- Read and process historical stock data from CSV files
- Calculate comprehensive technical indicators using TA-Lib
- Detect candlestick patterns
- Recent pattern analysis (up to 90 days ago)
- Export analysis results

## Installation

1. Install Python dependencies:
```bash
pip install -r requirements.txt
```

2. Install TA-Lib:
   - Windows: Download from https://www.lfd.uci.edu/~gohlke/pythonlibs/#ta-lib
   - Linux/Mac: `pip install TA-Lib` (may require compilation)

## Usage

```python
from stock_analyzer import StockAnalyzer

# Initialize analyzer
analyzer = StockAnalyzer()

# Analyze a single stock (default: 90 days lookback)
results = analyzer.analyze_stock('aapl-historical-data.csv')

# Analyze with custom lookback period
results = analyzer.analyze_stock('aapl-historical-data.csv', recent_days=30)

# Analyze multiple stocks
analyzer.analyze_all_stocks('../Powershell/historical-data/')
```

## Data Format

Expected CSV format:
```
Date,Open,High,Low,Close,Adj Close,Volume
2023-01-01,150.00,155.00,149.00,154.50,154.50,1000000
```