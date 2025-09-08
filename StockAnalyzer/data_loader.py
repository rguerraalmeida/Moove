import pandas as pd
import numpy as np
from datetime import datetime, timedelta
import os
import glob
from typing import Optional, Dict, List

class StockDataLoader:
    """Handles loading and preprocessing of stock data from CSV files."""
    
    def __init__(self, data_directory: str = None):
        self.data_directory = data_directory or "../Powershell/historical-data/"
        
    def load_stock_data(self, file_path: str) -> Optional[pd.DataFrame]:
        """
        Load stock data from a CSV file.
        
        Args:
            file_path: Path to the CSV file
            
        Returns:
            DataFrame with processed stock data or None if error
        """
        try:
            df = pd.read_csv(file_path)
            
            # Ensure required columns exist
            required_cols = ['Date', 'Open', 'High', 'Low', 'Close', 'Volume']
            missing_cols = [col for col in required_cols if col not in df.columns]
            if missing_cols:
                print(f"Missing columns in {file_path}: {missing_cols}")
                return None
                
            # Convert Date column to datetime
            df['Date'] = pd.to_datetime(df['Date'])
            
            # Sort by date
            df = df.sort_values('Date').reset_index(drop=True)
            
            # Remove any rows with NaN values in OHLCV columns
            df = df.dropna(subset=['Open', 'High', 'Low', 'Close', 'Volume'])
            
            # Ensure numeric types
            numeric_cols = ['Open', 'High', 'Low', 'Close', 'Volume']
            for col in numeric_cols:
                df[col] = pd.to_numeric(df[col], errors='coerce')
                
            # Remove any rows that became NaN after conversion
            df = df.dropna(subset=numeric_cols)
            
            # Clean OHLC data anomalies
            df = self._clean_ohlc_data(df)
            
            # Use Adj Close if available, otherwise use Close
            if 'Adj Close' in df.columns:
                df['Adj_Close'] = pd.to_numeric(df['Adj Close'], errors='coerce')
                # Fill NaN in Adj_Close with Close values
                df['Adj_Close'] = df['Adj_Close'].fillna(df['Close'])
            else:
                df['Adj_Close'] = df['Close']
                
            return df
            
        except Exception as e:
            print(f"Error loading {file_path}: {str(e)}")
            return None
            
    def get_recent_data(self, df: pd.DataFrame, days: int = 90) -> pd.DataFrame:
        """
        Get the most recent N days of data.
        
        Args:
            df: Stock data DataFrame
            days: Number of recent days to include
            
        Returns:
            DataFrame with recent data
        """
        if len(df) <= days:
            return df
        return df.tail(days).copy()
        
    def get_stock_files(self) -> List[str]:
        """
        Get list of all stock CSV files in the data directory.
        
        Returns:
            List of file paths
        """
        pattern = os.path.join(self.data_directory, "*-historical-data.csv")
        return glob.glob(pattern)
        
    def extract_symbol_from_filename(self, file_path: str) -> str:
        """
        Extract stock symbol from filename.
        
        Args:
            file_path: Path to CSV file
            
        Returns:
            Stock symbol in uppercase
        """
        filename = os.path.basename(file_path)
        # Remove "-historical-data.csv" suffix and convert to uppercase
        symbol = filename.replace("-historical-data.csv", "").replace("_", ".").upper()
        return symbol
        
    def validate_data_quality(self, df: pd.DataFrame) -> Dict[str, bool]:
        """
        Validate data quality for technical analysis.
        
        Args:
            df: Stock data DataFrame
            
        Returns:
            Dictionary with validation results
        """
        validations = {
            'has_minimum_rows': len(df) >= 20,  # Need at least 20 days for most indicators
            'no_missing_ohlcv': not df[['Open', 'High', 'Low', 'Close', 'Volume']].isnull().any().any(),
            'valid_ohlc_relationships': self._check_ohlc_relationships(df),
            'positive_values': (df[['Open', 'High', 'Low', 'Close']] > 0).all().all(),
            'chronological_order': df['Date'].is_monotonic_increasing
        }
        
        return validations
        
    def _check_ohlc_relationships(self, df: pd.DataFrame) -> bool:
        """Check if OHLC relationships are valid (High >= Low, etc.)"""
        try:
            # High should be >= Low
            high_low_valid = (df['High'] >= df['Low']).all()
            
            # Close should be between Low and High
            close_valid = ((df['Close'] >= df['Low']) & (df['Close'] <= df['High'])).all()
            
            # Open should be between Low and High
            open_valid = ((df['Open'] >= df['Low']) & (df['Open'] <= df['High'])).all()
            
            return high_low_valid and close_valid and open_valid
            
        except Exception:
            return False
            
    def get_data_summary(self, df: pd.DataFrame) -> Dict:
        """
        Get summary statistics for the stock data.
        
        Args:
            df: Stock data DataFrame
            
        Returns:
            Dictionary with summary statistics
        """
        if df.empty:
            return {}
            
        summary = {
            'total_records': len(df),
            'date_range': {
                'start': df['Date'].min().strftime('%Y-%m-%d'),
                'end': df['Date'].max().strftime('%Y-%m-%d')
            },
            'price_stats': {
                'latest_close': float(df['Close'].iloc[-1]),
                'min_close': float(df['Close'].min()),
                'max_close': float(df['Close'].max()),
                'avg_volume': float(df['Volume'].mean())
            }
        }
        
        return summary
        
    def _clean_ohlc_data(self, df: pd.DataFrame) -> pd.DataFrame:
        """
        Clean OHLC data anomalies where Open/Close are outside High/Low range.
        
        Args:
            df: DataFrame with OHLC data
            
        Returns:
            Cleaned DataFrame
        """
        df_clean = df.copy()
        
        # Find rows where OHLC relationships are invalid
        invalid_rows = (
            (df_clean['High'] < df_clean['Low']) |
            (df_clean['Open'] > df_clean['High']) |
            (df_clean['Open'] < df_clean['Low']) |
            (df_clean['Close'] > df_clean['High']) |
            (df_clean['Close'] < df_clean['Low'])
        )
        
        if invalid_rows.any():
            print(f"Warning: Found {invalid_rows.sum()} rows with invalid OHLC relationships")
            
            # Fix invalid rows by adjusting High/Low to accommodate Open/Close
            for idx in df_clean[invalid_rows].index:
                row = df_clean.loc[idx]
                
                # Calculate what High and Low should be to include Open and Close
                values = [row['Open'], row['High'], row['Low'], row['Close']]
                corrected_high = max(values)
                corrected_low = min(values)
                
                df_clean.loc[idx, 'High'] = corrected_high
                df_clean.loc[idx, 'Low'] = corrected_low
                
        return df_clean