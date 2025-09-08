#!/usr/bin/env python3
"""
Stock Technical Analysis Tool

Main script for analyzing stock data with technical indicators and candlestick patterns.
"""

import os
import sys
import json
import argparse
from datetime import datetime, timedelta
from typing import Dict, List, Optional
import pandas as pd

from data_loader import StockDataLoader
from technical_indicators import TechnicalIndicators
from candlestick_patterns import CandlestickPatterns

class StockAnalyzer:
    """Main class for stock technical analysis."""
    
    def __init__(self, data_directory: str = None):
        """
        Initialize the stock analyzer.
        
        Args:
            data_directory: Path to directory containing stock CSV files
        """
        self.data_loader = StockDataLoader(data_directory)
        self.technical_indicators = TechnicalIndicators()
        self.candlestick_patterns = CandlestickPatterns()
        
    def analyze_stock(self, file_path: str, recent_days: int = 90) -> Dict:
        """
        Perform complete technical analysis on a single stock.
        
        Args:
            file_path: Path to stock CSV file
            recent_days: Number of recent days to analyze for patterns
            
        Returns:
            Dictionary containing complete analysis results
        """
        print(f"Analyzing {os.path.basename(file_path)}...")
        
        # Load data
        df = self.data_loader.load_stock_data(file_path)
        if df is None:
            return {"error": f"Failed to load data from {file_path}"}
            
        # Extract symbol from filename
        symbol = self.data_loader.extract_symbol_from_filename(file_path)
        
        # Validate data quality
        validations = self.data_loader.validate_data_quality(df)
        if not all(validations.values()):
            print(f"Warning: Data quality issues detected for {symbol}")
            print(f"Validations: {validations}")
            
        # Get data summary
        data_summary = self.data_loader.get_data_summary(df)
        
        # Calculate technical indicators
        indicators = self.technical_indicators.calculate_all_indicators(df)
        
        # Detect candlestick patterns
        all_patterns = self.candlestick_patterns.detect_all_patterns(df)
        recent_patterns = self.candlestick_patterns.get_recent_patterns(
            all_patterns, df, recent_days
        )
        
        # Analyze pattern strength
        pattern_analysis = self.candlestick_patterns.analyze_pattern_strength(recent_patterns)
        
        # Generate interpretations
        interpretations = self.technical_indicators.interpret_indicators(
            indicators.get('current_values', {})
        )
        
        # Generate pattern summary
        pattern_summary = self.candlestick_patterns.generate_pattern_summary(
            recent_patterns, pattern_analysis
        )
        
        # Compile results
        results = {
            'symbol': symbol,
            'analysis_date': datetime.now().strftime('%Y-%m-%d %H:%M:%S'),
            'data_summary': data_summary,
            'data_quality': validations,
            'technical_indicators': indicators,
            'indicator_interpretations': interpretations,
            'candlestick_patterns': {
                'recent_patterns': recent_patterns,
                'pattern_analysis': pattern_analysis,
                'pattern_summary': pattern_summary
            },
            'recent_days_analyzed': recent_days
        }
        
        return results
        
    def analyze_multiple_stocks(self, symbols: List[str] = None, 
                              recent_days: int = 90, 
                              output_file: str = None) -> Dict:
        """
        Analyze multiple stocks.
        
        Args:
            symbols: List of stock symbols to analyze (None for all)
            recent_days: Number of recent days to analyze
            output_file: Optional file to save results
            
        Returns:
            Dictionary with results for all analyzed stocks
        """
        stock_files = self.data_loader.get_stock_files()
        
        if not stock_files:
            return {"error": "No stock data files found"}
            
        # Filter by symbols if specified
        if symbols:
            symbols_lower = [s.lower().replace('.', '_') for s in symbols]
            stock_files = [
                f for f in stock_files 
                if any(sym in os.path.basename(f).lower() for sym in symbols_lower)
            ]
            
        results = {
            'analysis_summary': {
                'total_stocks': len(stock_files),
                'analysis_date': datetime.now().strftime('%Y-%m-%d %H:%M:%S'),
                'recent_days_analyzed': recent_days
            },
            'stocks': {}
        }
        
        # Analyze each stock
        for i, file_path in enumerate(stock_files, 1):
            symbol = self.data_loader.extract_symbol_from_filename(file_path)
            print(f"[{i}/{len(stock_files)}] Analyzing {symbol}...")
            
            try:
                stock_results = self.analyze_stock(file_path, recent_days)
                results['stocks'][symbol] = stock_results
                
            except Exception as e:
                print(f"Error analyzing {symbol}: {str(e)}")
                results['stocks'][symbol] = {"error": str(e)}
                
        # Save results if requested
        if output_file:
            self.save_results(results, output_file)
            
        return results
        
    def save_results(self, results: Dict, output_file: str):
        """Save analysis results to JSON file."""
        try:
            # Convert numpy types to Python types for JSON serialization
            results_json = self._convert_numpy_types(results)
            
            with open(output_file, 'w') as f:
                json.dump(results_json, f, indent=2, default=str)
                
            print(f"Results saved to {output_file}")
            
        except Exception as e:
            print(f"Error saving results: {str(e)}")
            
    def _convert_numpy_types(self, obj):
        """Convert numpy types to Python types for JSON serialization."""
        import numpy as np
        
        if isinstance(obj, dict):
            return {key: self._convert_numpy_types(value) for key, value in obj.items()}
        elif isinstance(obj, list):
            return [self._convert_numpy_types(item) for item in obj]
        elif isinstance(obj, np.ndarray):
            return obj.tolist()
        elif isinstance(obj, (np.int64, np.int32)):
            return int(obj)
        elif isinstance(obj, (np.float64, np.float32)):
            return float(obj)
        elif pd.isna(obj):
            return None
        else:
            return obj
            
    def generate_summary_report(self, results: Dict) -> str:
        """
        Generate a human-readable summary report.
        
        Args:
            results: Results from analyze_multiple_stocks
            
        Returns:
            Formatted text report
        """
        if 'analysis_summary' not in results:
            return "Invalid results format"
            
        summary = results['analysis_summary']
        report_lines = []
        
        # Header
        report_lines.append("=" * 60)
        report_lines.append("STOCK TECHNICAL ANALYSIS SUMMARY REPORT")
        report_lines.append("=" * 60)
        report_lines.append(f"Analysis Date: {summary['analysis_date']}")
        report_lines.append(f"Stocks Analyzed: {summary['total_stocks']}")
        report_lines.append(f"Recent Days Analyzed: {summary['recent_days_analyzed']}")
        report_lines.append("")
        
        # Stocks with recent patterns
        stocks_with_patterns = []
        stocks_with_strong_signals = []
        
        for symbol, stock_data in results.get('stocks', {}).items():
            if 'error' in stock_data:
                continue
                
            patterns = stock_data.get('candlestick_patterns', {})
            pattern_analysis = patterns.get('pattern_analysis', {})
            
            if pattern_analysis.get('total_patterns', 0) > 0:
                stocks_with_patterns.append({
                    'symbol': symbol,
                    'total_patterns': pattern_analysis['total_patterns'],
                    'sentiment': pattern_analysis['sentiment'],
                    'strength_score': pattern_analysis['strength_score'],
                    'most_recent': pattern_analysis.get('most_recent', {}).get('pattern', 'N/A')
                })
                
                if pattern_analysis['strength_score'] >= 2:
                    stocks_with_strong_signals.append(stocks_with_patterns[-1])
                    
        # Sort by strength score
        stocks_with_patterns.sort(key=lambda x: x['strength_score'], reverse=True)
        
        # Strong signals section
        if stocks_with_strong_signals:
            report_lines.append("STOCKS WITH STRONG SIGNALS:")
            report_lines.append("-" * 40)
            for stock in stocks_with_strong_signals[:10]:  # Top 10
                report_lines.append(
                    f"{stock['symbol']:8} | {stock['sentiment']:8} | "
                    f"Score: {stock['strength_score']:2} | "
                    f"Recent: {stock['most_recent']}"
                )
            report_lines.append("")
            
        # All patterns section
        if stocks_with_patterns:
            report_lines.append("ALL STOCKS WITH RECENT PATTERNS:")
            report_lines.append("-" * 40)
            report_lines.append("Symbol   | Sentiment | Patterns | Recent Pattern")
            report_lines.append("-" * 40)
            
            for stock in stocks_with_patterns[:20]:  # Top 20
                report_lines.append(
                    f"{stock['symbol']:8} | {stock['sentiment']:9} | "
                    f"{stock['total_patterns']:8} | {stock['most_recent']}"
                )
                
        # Statistics
        report_lines.append("")
        report_lines.append("STATISTICS:")
        report_lines.append("-" * 20)
        report_lines.append(f"Stocks with patterns: {len(stocks_with_patterns)}")
        report_lines.append(f"Strong bullish signals: {len([s for s in stocks_with_strong_signals if s['sentiment'] == 'Bullish'])}")
        report_lines.append(f"Strong bearish signals: {len([s for s in stocks_with_strong_signals if s['sentiment'] == 'Bearish'])}")
        
        return "\n".join(report_lines)

def main():
    """Main function for command-line usage."""
    parser = argparse.ArgumentParser(description='Stock Technical Analysis Tool')
    parser.add_argument('--data-dir', '-d', type=str, 
                       default='../Powershell/historical-data/',
                       help='Directory containing stock CSV files')
    parser.add_argument('--symbol', '-s', type=str,
                       help='Single stock symbol to analyze')
    parser.add_argument('--symbols', '-S', type=str, nargs='+',
                       help='Multiple stock symbols to analyze')
    parser.add_argument('--recent-days', '-r', type=int, default=90,
                       help='Number of recent days to analyze for patterns')
    parser.add_argument('--output', '-o', type=str,
                       help='Output file for JSON results')
    parser.add_argument('--summary', '-sum', action='store_true',
                       help='Generate summary report')
    parser.add_argument('--all', '-a', action='store_true',
                       help='Analyze all stocks in directory')
    
    args = parser.parse_args()
    
    # Initialize analyzer
    analyzer = StockAnalyzer(args.data_dir)
    
    # Single stock analysis
    if args.symbol:
        # Find the file for this symbol
        stock_files = analyzer.data_loader.get_stock_files()
        symbol_file = None
        
        for file_path in stock_files:
            file_symbol = analyzer.data_loader.extract_symbol_from_filename(file_path)
            if file_symbol.upper() == args.symbol.upper():
                symbol_file = file_path
                break
                
        if symbol_file:
            results = analyzer.analyze_stock(symbol_file, args.recent_days)
            
            if args.output:
                analyzer.save_results({args.symbol: results}, args.output)
            else:
                # Print key results
                print(f"\n{args.symbol} Analysis Results:")
                print("-" * 40)
                
                # Pattern summary
                pattern_summary = results.get('candlestick_patterns', {}).get('pattern_summary', '')
                if pattern_summary:
                    print(pattern_summary)
                    
                # Current indicator values
                current_values = results.get('technical_indicators', {}).get('current_values', {})
                if current_values:
                    print(f"\nCurrent Technical Indicators:")
                    for category, indicators in current_values.items():
                        if isinstance(indicators, dict):
                            print(f"  {category.upper()}:")
                            for name, value in indicators.items():
                                if value is not None and not isinstance(value, str):
                                    print(f"    {name}: {value:.2f}")
        else:
            print(f"Stock symbol '{args.symbol}' not found in data directory")
            
    # Multiple stocks analysis
    elif args.symbols or args.all:
        symbols = args.symbols if args.symbols else None
        results = analyzer.analyze_multiple_stocks(symbols, args.recent_days, args.output)
        
        if args.summary:
            summary_report = analyzer.generate_summary_report(results)
            print(summary_report)
            
    else:
        parser.print_help()

if __name__ == '__main__':
    main()