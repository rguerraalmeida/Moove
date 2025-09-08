#!/usr/bin/env python3
"""
Example usage of the Stock Technical Analysis Tool

This script demonstrates how to use the various components of the stock analyzer.
"""

from stock_analyzer import StockAnalyzer
from data_loader import StockDataLoader
from technical_indicators import TechnicalIndicators
from candlestick_patterns import CandlestickPatterns

def example_single_stock_analysis():
    """Example: Analyze a single stock (AAPL)"""
    print("=" * 60)
    print("EXAMPLE 1: Single Stock Analysis (AAPL)")
    print("=" * 60)
    
    # Initialize analyzer
    analyzer = StockAnalyzer('../Powershell/historical-data/')
    
    # Analyze AAPL
    stock_files = analyzer.data_loader.get_stock_files()
    aapl_file = None
    
    for file in stock_files:
        if 'aapl' in file.lower():
            aapl_file = file
            break
            
    if aapl_file:
        results = analyzer.analyze_stock(aapl_file, recent_days=90)
        
        # Display results
        print(f"Symbol: {results['symbol']}")
        print(f"Analysis Date: {results['analysis_date']}")
        print(f"Data Range: {results['data_summary']['date_range']['start']} to {results['data_summary']['date_range']['end']}")
        print(f"Total Records: {results['data_summary']['total_records']}")
        print(f"Latest Close: ${results['data_summary']['price_stats']['latest_close']:.2f}")
        
        # Technical indicators
        print(f"\nKey Technical Indicators:")
        current_values = results['technical_indicators']['current_values']
        
        # RSI
        if 'momentum' in current_values and 'RSI_14' in current_values['momentum']:
            rsi = current_values['momentum']['RSI_14']
            print(f"  RSI (14): {rsi:.2f}")
            
        # MACD
        if 'trend' in current_values and 'MACD' in current_values['trend']:
            macd = current_values['trend']['MACD']
            macd_signal = current_values['trend']['MACD_Signal']
            if macd is not None and macd_signal is not None:
                print(f"  MACD: {macd:.4f}, Signal: {macd_signal:.4f}")
            else:
                print(f"  MACD: {macd}, Signal: {macd_signal}")
            
        # Bollinger Bands
        if 'volatility' in current_values:
            bb_upper = current_values['volatility'].get('BB_Upper')
            bb_lower = current_values['volatility'].get('BB_Lower')
            bb_pos = current_values['volatility'].get('BB_Position')
            if bb_upper and bb_lower:
                print(f"  Bollinger Bands: Upper ${bb_upper:.2f}, Lower ${bb_lower:.2f}, Position {bb_pos:.1f}%")
        
        # Candlestick patterns
        print(f"\nCandlestick Pattern Summary:")
        pattern_summary = results['candlestick_patterns']['pattern_summary']
        print(pattern_summary)
        
    else:
        print("AAPL data file not found")

def example_multiple_stocks_analysis():
    """Example: Analyze multiple specific stocks"""
    print("\n" + "=" * 60)
    print("EXAMPLE 2: Multiple Stocks Analysis")
    print("=" * 60)
    
    # Initialize analyzer
    analyzer = StockAnalyzer('../Powershell/historical-data/')
    
    # Analyze specific stocks
    symbols = ['AAPL', 'MSFT', 'GOOGL', 'AMZN', 'TSLA']
    results = analyzer.analyze_multiple_stocks(symbols, recent_days=90)
    
    print(f"Analyzed {results['analysis_summary']['total_stocks']} stocks")
    
    # Show summary for each stock
    for symbol, stock_data in results['stocks'].items():
        if 'error' in stock_data:
            print(f"{symbol}: Error - {stock_data['error']}")
            continue
            
        pattern_analysis = stock_data['candlestick_patterns']['pattern_analysis']
        current_close = stock_data['data_summary']['price_stats']['latest_close']
        
        print(f"\n{symbol}:")
        print(f"  Current Price: ${current_close:.2f}")
        print(f"  Patterns Found: {pattern_analysis['total_patterns']}")
        print(f"  Sentiment: {pattern_analysis['sentiment']}")
        
        if pattern_analysis['most_recent']:
            most_recent = pattern_analysis['most_recent']
            print(f"  Most Recent Pattern: {most_recent['pattern']} ({most_recent['days_ago']} days ago)")

def example_pattern_screening():
    """Example: Screen for stocks with specific patterns"""
    print("\n" + "=" * 60)
    print("EXAMPLE 3: Pattern Screening")
    print("=" * 60)
    
    # Initialize analyzer
    analyzer = StockAnalyzer('../Powershell/historical-data/')
    
    # Get all stock files (limit to first 20 for example)
    stock_files = analyzer.data_loader.get_stock_files()[:20]
    
    bullish_stocks = []
    bearish_stocks = []
    
    for file_path in stock_files:
        symbol = analyzer.data_loader.extract_symbol_from_filename(file_path)
        
        try:
            results = analyzer.analyze_stock(file_path, recent_days=90)
            pattern_analysis = results['candlestick_patterns']['pattern_analysis']
            
            if pattern_analysis['sentiment'] == 'Bullish' and pattern_analysis['strength_score'] >= 2:
                bullish_stocks.append({
                    'symbol': symbol,
                    'strength': pattern_analysis['strength_score'],
                    'patterns': pattern_analysis['total_patterns']
                })
            elif pattern_analysis['sentiment'] == 'Bearish' and pattern_analysis['strength_score'] >= 2:
                bearish_stocks.append({
                    'symbol': symbol,
                    'strength': pattern_analysis['strength_score'],
                    'patterns': pattern_analysis['total_patterns']
                })
                
        except Exception as e:
            print(f"Error analyzing {symbol}: {str(e)}")
            
    # Sort by strength
    bullish_stocks.sort(key=lambda x: x['strength'], reverse=True)
    bearish_stocks.sort(key=lambda x: x['strength'], reverse=True)
    
    print("Strong Bullish Signals:")
    for stock in bullish_stocks:
        print(f"  {stock['symbol']}: Strength {stock['strength']}, {stock['patterns']} patterns")
        
    print("\nStrong Bearish Signals:")
    for stock in bearish_stocks:
        print(f"  {stock['symbol']}: Strength {stock['strength']}, {stock['patterns']} patterns")

def example_custom_analysis():
    """Example: Custom analysis using individual components"""
    print("\n" + "=" * 60)
    print("EXAMPLE 4: Custom Analysis Components")
    print("=" * 60)
    
    # Load data manually
    data_loader = StockDataLoader('../Powershell/historical-data/')
    stock_files = data_loader.get_stock_files()
    
    if stock_files:
        # Load first available stock
        df = data_loader.load_stock_data(stock_files[0])
        symbol = data_loader.extract_symbol_from_filename(stock_files[0])
        
        if df is not None:
            print(f"Custom analysis for {symbol}")
            
            # Technical indicators only
            tech_indicators = TechnicalIndicators()
            indicators = tech_indicators.calculate_all_indicators(df)
            
            # Print specific indicators
            current = indicators.get('current_values', {})
            if 'momentum' in current:
                print("Momentum Indicators:")
                for name, value in current['momentum'].items():
                    if value is not None and isinstance(value, (int, float)):
                        print(f"  {name}: {value:.4f}")
                    elif value is not None:
                        print(f"  {name}: {value}")
                        
            # Candlestick patterns only
            candle_patterns = CandlestickPatterns()
            all_patterns = candle_patterns.detect_all_patterns(df)
            recent_patterns = candle_patterns.get_recent_patterns(all_patterns, df, 90)
            
            print(f"\nRecent Bullish Patterns ({len(recent_patterns['bullish'])}):")
            for pattern in recent_patterns['bullish']:
                print(f"  {pattern['pattern']} - {pattern['days_ago']} days ago")
                
            print(f"\nRecent Bearish Patterns ({len(recent_patterns['bearish'])}):")
            for pattern in recent_patterns['bearish']:
                print(f"  {pattern['pattern']} - {pattern['days_ago']} days ago")

if __name__ == '__main__':
    """Run all examples"""
    try:
        example_single_stock_analysis()
        example_multiple_stocks_analysis()
        example_pattern_screening()
        example_custom_analysis()
        
        print("\n" + "=" * 60)
        print("All examples completed successfully!")
        print("=" * 60)
        
    except Exception as e:
        print(f"Error running examples: {str(e)}")
        print("Make sure you have installed all requirements and have stock data available.")