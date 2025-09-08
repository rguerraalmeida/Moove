#!/usr/bin/env python3
"""
Quick test script to verify the stock analyzer is working properly.
"""

import os
import sys
from stock_analyzer import StockAnalyzer

def test_basic_functionality():
    """Test basic functionality of the stock analyzer."""
    print("Testing Stock Analyzer...")
    print("=" * 40)
    
    try:
        # Initialize analyzer
        analyzer = StockAnalyzer('../Powershell/historical-data/')
        
        # Get available stock files
        stock_files = analyzer.data_loader.get_stock_files()
        print(f"Found {len(stock_files)} stock data files")
        
        if not stock_files:
            print("No stock data files found. Please check the data directory.")
            return False
            
        # Test with first available stock
        test_file = stock_files[0]
        symbol = analyzer.data_loader.extract_symbol_from_filename(test_file)
        
        print(f"Testing with {symbol} ({os.path.basename(test_file)})")
        
        # Perform analysis
        results = analyzer.analyze_stock(test_file, recent_days=5)
        
        # Check results
        if 'error' in results:
            print(f"Error in analysis: {results['error']}")
            return False
            
        # Verify key components
        required_sections = ['technical_indicators', 'candlestick_patterns', 'data_summary']
        for section in required_sections:
            if section not in results:
                print(f"Missing section: {section}")
                return False
                
        # Check technical indicators
        indicators = results['technical_indicators']
        if 'current_values' not in indicators:
            print("Missing current values in technical indicators")
            return False
            
        current_values = indicators['current_values']
        print(f"Technical indicator categories: {list(current_values.keys())}")
        
        # Check candlestick patterns
        patterns = results['candlestick_patterns']
        pattern_analysis = patterns.get('pattern_analysis', {})
        
        print(f"Pattern analysis: {pattern_analysis.get('total_patterns', 0)} patterns found")
        print(f"Sentiment: {pattern_analysis.get('sentiment', 'Unknown')}")
        
        # Show some current indicator values
        print("\nSample Technical Indicators:")
        if 'momentum' in current_values:
            rsi = current_values['momentum'].get('RSI_14')
            if rsi:
                print(f"  RSI (14): {rsi:.2f}")
                
        if 'trend' in current_values:
            sma_20 = current_values['trend'].get('SMA_20')
            if sma_20:
                print(f"  SMA (20): {sma_20:.2f}")
                
        print("\nTest completed successfully!")
        return True
        
    except ImportError as e:
        print(f"Import error: {e}")
        print("Please install required packages: pip install -r requirements.txt")
        return False
        
    except Exception as e:
        print(f"Unexpected error: {e}")
        return False

def test_pattern_detection():
    """Test candlestick pattern detection specifically."""
    print("\nTesting Candlestick Pattern Detection...")
    print("=" * 40)
    
    try:
        from candlestick_patterns import CandlestickPatterns
        from data_loader import StockDataLoader
        
        # Load data
        loader = StockDataLoader('../Powershell/historical-data/')
        stock_files = loader.get_stock_files()
        
        if not stock_files:
            print("No stock files available for testing")
            return False
            
        df = loader.load_stock_data(stock_files[0])
        if df is None:
            print("Failed to load stock data")
            return False
            
        # Test pattern detection
        pattern_detector = CandlestickPatterns()
        patterns = pattern_detector.detect_all_patterns(df)
        
        print(f"Detected patterns for {len(df)} candles")
        
        # Get recent patterns
        recent = pattern_detector.get_recent_patterns(patterns, df, 7)
        
        total_recent = (len(recent.get('bullish', [])) + 
                       len(recent.get('bearish', [])) + 
                       len(recent.get('neutral', [])))
        
        print(f"Recent patterns (7 days): {total_recent}")
        print(f"  Bullish: {len(recent.get('bullish', []))}")
        print(f"  Bearish: {len(recent.get('bearish', []))}")
        print(f"  Neutral: {len(recent.get('neutral', []))}")
        
        # Show a few pattern examples
        if recent.get('bullish'):
            print(f"Example bullish pattern: {recent['bullish'][0]['pattern']}")
            
        if recent.get('bearish'):
            print(f"Example bearish pattern: {recent['bearish'][0]['pattern']}")
            
        print("Pattern detection test completed!")
        return True
        
    except Exception as e:
        print(f"Pattern detection test failed: {e}")
        return False

if __name__ == '__main__':
    print("Stock Analyzer Test Suite")
    print("=" * 50)
    
    success = True
    
    # Run tests
    success &= test_basic_functionality()
    success &= test_pattern_detection()
    
    print("\n" + "=" * 50)
    if success:
        print("[PASS] All tests passed! Stock analyzer is working correctly.")
    else:
        print("[FAIL] Some tests failed. Please check the error messages above.")
        sys.exit(1)