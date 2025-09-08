import pandas as pd
import numpy as np
import talib
from typing import Dict, Optional, Tuple
from datetime import datetime, timedelta

class TechnicalIndicators:
    """Calculate technical indicators using TA-Lib."""
    
    def __init__(self):
        self.indicators = {}
        
    def calculate_all_indicators(self, df: pd.DataFrame) -> Dict:
        """
        Calculate all technical indicators for the given stock data.
        
        Args:
            df: DataFrame with OHLCV data
            
        Returns:
            Dictionary containing all calculated indicators
        """
        if len(df) < 20:
            return {"error": "Insufficient data points for technical analysis"}
            
        # Convert to numpy arrays for TA-Lib
        open_prices = df['Open'].values
        high_prices = df['High'].values
        low_prices = df['Low'].values
        close_prices = df['Close'].values
        volume = df['Volume'].values
        
        indicators = {}
        
        # Trend Indicators
        indicators['trend'] = self._calculate_trend_indicators(
            open_prices, high_prices, low_prices, close_prices, volume
        )
        
        # Momentum Indicators
        indicators['momentum'] = self._calculate_momentum_indicators(
            high_prices, low_prices, close_prices, volume
        )
        
        # Volatility Indicators
        indicators['volatility'] = self._calculate_volatility_indicators(
            high_prices, low_prices, close_prices
        )
        
        # Volume Indicators
        indicators['volume'] = self._calculate_volume_indicators(
            high_prices, low_prices, close_prices, volume
        )
        
        # Support/Resistance Levels
        indicators['levels'] = self._calculate_support_resistance(
            high_prices, low_prices, close_prices
        )
        
        # Add recent values (last value of each indicator)
        indicators['current_values'] = self._get_current_values(indicators)
        
        return indicators
        
    def _calculate_trend_indicators(self, open_p, high_p, low_p, close_p, volume) -> Dict:
        """Calculate trend-following indicators."""
        indicators = {}
        
        try:
            # Moving Averages
            indicators['SMA_5'] = talib.SMA(close_p, timeperiod=5)
            indicators['SMA_10'] = talib.SMA(close_p, timeperiod=10)
            indicators['SMA_20'] = talib.SMA(close_p, timeperiod=20)
            indicators['SMA_50'] = talib.SMA(close_p, timeperiod=50)
            indicators['SMA_200'] = talib.SMA(close_p, timeperiod=200)
            
            indicators['EMA_5'] = talib.EMA(close_p, timeperiod=5)
            indicators['EMA_10'] = talib.EMA(close_p, timeperiod=10)
            indicators['EMA_20'] = talib.EMA(close_p, timeperiod=20)
            indicators['EMA_50'] = talib.EMA(close_p, timeperiod=50)
            
            # MACD
            macd, signal, histogram = talib.MACD(close_p, fastperiod=12, slowperiod=26, signalperiod=9)
            indicators['MACD'] = macd
            indicators['MACD_Signal'] = signal
            indicators['MACD_Histogram'] = histogram
            
            # ADX (Average Directional Index)
            indicators['ADX'] = talib.ADX(high_p, low_p, close_p, timeperiod=14)
            indicators['PLUS_DI'] = talib.PLUS_DI(high_p, low_p, close_p, timeperiod=14)
            indicators['MINUS_DI'] = talib.MINUS_DI(high_p, low_p, close_p, timeperiod=14)
            
            # Parabolic SAR
            indicators['SAR'] = talib.SAR(high_p, low_p, acceleration=0.02, maximum=0.2)
            
            # Ichimoku Components
            indicators['Tenkan'] = self._calculate_ichimoku_tenkan(high_p, low_p)
            indicators['Kijun'] = self._calculate_ichimoku_kijun(high_p, low_p)
            
        except Exception as e:
            indicators['error'] = f"Error calculating trend indicators: {str(e)}"
            
        return indicators
        
    def _calculate_momentum_indicators(self, high_p, low_p, close_p, volume) -> Dict:
        """Calculate momentum indicators."""
        indicators = {}
        
        try:
            # RSI
            indicators['RSI_14'] = talib.RSI(close_p, timeperiod=14)
            indicators['RSI_21'] = talib.RSI(close_p, timeperiod=21)
            
            # Stochastic
            slowk, slowd = talib.STOCH(high_p, low_p, close_p, 
                                     fastk_period=14, slowk_period=3, slowd_period=3)
            indicators['Stoch_K'] = slowk
            indicators['Stoch_D'] = slowd
            
            # Williams %R
            indicators['Williams_R'] = talib.WILLR(high_p, low_p, close_p, timeperiod=14)
            
            # CCI (Commodity Channel Index)
            indicators['CCI'] = talib.CCI(high_p, low_p, close_p, timeperiod=14)
            
            # ROC (Rate of Change)
            indicators['ROC'] = talib.ROC(close_p, timeperiod=10)
            
            # MFI (Money Flow Index)
            indicators['MFI'] = talib.MFI(high_p, low_p, close_p, volume, timeperiod=14)
            
            # Ultimate Oscillator
            indicators['UO'] = talib.ULTOSC(high_p, low_p, close_p, 
                                          timeperiod1=7, timeperiod2=14, timeperiod3=28)
            
        except Exception as e:
            indicators['error'] = f"Error calculating momentum indicators: {str(e)}"
            
        return indicators
        
    def _calculate_volatility_indicators(self, high_p, low_p, close_p) -> Dict:
        """Calculate volatility indicators."""
        indicators = {}
        
        try:
            # Bollinger Bands
            bb_upper, bb_middle, bb_lower = talib.BBANDS(close_p, timeperiod=20, 
                                                        nbdevup=2, nbdevdn=2, matype=0)
            indicators['BB_Upper'] = bb_upper
            indicators['BB_Middle'] = bb_middle
            indicators['BB_Lower'] = bb_lower
            
            # Calculate BB Width with division by zero protection
            bb_width = np.full_like(bb_upper, np.nan)
            valid_middle = bb_middle != 0
            bb_width[valid_middle] = (bb_upper[valid_middle] - bb_lower[valid_middle]) / bb_middle[valid_middle] * 100
            indicators['BB_Width'] = bb_width
            
            # Calculate BB Position with division by zero protection
            bb_position = np.full_like(bb_upper, np.nan)
            band_diff = bb_upper - bb_lower
            valid_diff = band_diff != 0
            bb_position[valid_diff] = (close_p[valid_diff] - bb_lower[valid_diff]) / band_diff[valid_diff] * 100
            indicators['BB_Position'] = bb_position
            
            # Average True Range
            indicators['ATR'] = talib.ATR(high_p, low_p, close_p, timeperiod=14)
            
            # True Range
            indicators['TRANGE'] = talib.TRANGE(high_p, low_p, close_p)
            
            # Keltner Channels (approximation using EMA and ATR)
            ema_20 = talib.EMA(close_p, timeperiod=20)
            atr_10 = talib.ATR(high_p, low_p, close_p, timeperiod=10)
            
            # Handle potential NaN values in ATR
            atr_safe = np.where(np.isnan(atr_10), 0, atr_10)
            indicators['KC_Upper'] = ema_20 + (2 * atr_safe)
            indicators['KC_Lower'] = ema_20 - (2 * atr_safe)
            indicators['KC_Middle'] = ema_20
            
        except Exception as e:
            indicators['error'] = f"Error calculating volatility indicators: {str(e)}"
            
        return indicators
        
    def _calculate_volume_indicators(self, high_p, low_p, close_p, volume) -> Dict:
        """Calculate volume-based indicators."""
        indicators = {}
        
        try:
            # On-Balance Volume
            indicators['OBV'] = talib.OBV(close_p, volume)
            
            # Volume SMA
            indicators['Volume_SMA_10'] = talib.SMA(volume, timeperiod=10)
            indicators['Volume_SMA_20'] = talib.SMA(volume, timeperiod=20)
            
            # Accumulation/Distribution Line
            indicators['AD'] = talib.AD(high_p, low_p, close_p, volume)
            
            # Chaikin A/D Oscillator
            indicators['ADOSC'] = talib.ADOSC(high_p, low_p, close_p, volume, 
                                            fastperiod=3, slowperiod=10)
            
        except Exception as e:
            indicators['error'] = f"Error calculating volume indicators: {str(e)}"
            
        return indicators
        
    def _calculate_support_resistance(self, high_p, low_p, close_p) -> Dict:
        """Calculate support and resistance levels."""
        levels = {}
        
        try:
            # Pivot Points (traditional)
            pivot = (high_p[-1] + low_p[-1] + close_p[-1]) / 3
            levels['Pivot'] = pivot
            levels['R1'] = 2 * pivot - low_p[-1]
            levels['R2'] = pivot + (high_p[-1] - low_p[-1])
            levels['R3'] = high_p[-1] + 2 * (pivot - low_p[-1])
            levels['S1'] = 2 * pivot - high_p[-1]
            levels['S2'] = pivot - (high_p[-1] - low_p[-1])
            levels['S3'] = low_p[-1] - 2 * (high_p[-1] - pivot)
            
            # Recent highs and lows (last 20 periods)
            recent_periods = min(20, len(high_p))
            recent_high = np.max(high_p[-recent_periods:])
            recent_low = np.min(low_p[-recent_periods:])
            
            levels['Recent_High_20'] = recent_high
            levels['Recent_Low_20'] = recent_low
            
        except Exception as e:
            levels['error'] = f"Error calculating support/resistance levels: {str(e)}"
            
        return levels
        
    def _calculate_ichimoku_tenkan(self, high_p, low_p, period=9):
        """Calculate Ichimoku Tenkan-sen (Conversion Line)."""
        tenkan = []
        for i in range(len(high_p)):
            if i < period - 1:
                tenkan.append(np.nan)
            else:
                period_high = np.max(high_p[i-period+1:i+1])
                period_low = np.min(low_p[i-period+1:i+1])
                tenkan.append((period_high + period_low) / 2)
        return np.array(tenkan)
        
    def _calculate_ichimoku_kijun(self, high_p, low_p, period=26):
        """Calculate Ichimoku Kijun-sen (Base Line)."""
        kijun = []
        for i in range(len(high_p)):
            if i < period - 1:
                kijun.append(np.nan)
            else:
                period_high = np.max(high_p[i-period+1:i+1])
                period_low = np.min(low_p[i-period+1:i+1])
                kijun.append((period_high + period_low) / 2)
        return np.array(kijun)
        
    def _get_current_values(self, indicators) -> Dict:
        """Extract the most recent (current) values from all indicators."""
        current = {}
        
        for category, category_indicators in indicators.items():
            if category == 'current_values':
                continue
                
            current[category] = {}
            for name, values in category_indicators.items():
                if isinstance(values, np.ndarray) and len(values) > 0:
                    # Get the last non-NaN value
                    last_val = values[~np.isnan(values)]
                    if len(last_val) > 0:
                        current[category][name] = float(last_val[-1])
                    else:
                        current[category][name] = None
                elif isinstance(values, (int, float)):
                    current[category][name] = float(values)
                else:
                    current[category][name] = values
                    
        return current
        
    def interpret_indicators(self, current_values: Dict) -> Dict[str, str]:
        """
        Provide basic interpretation of indicator values.
        
        Args:
            current_values: Dictionary of current indicator values
            
        Returns:
            Dictionary with interpretations
        """
        interpretations = {}
        
        try:
            # RSI interpretation
            if 'momentum' in current_values and 'RSI_14' in current_values['momentum']:
                rsi = current_values['momentum']['RSI_14']
                if rsi:
                    if rsi > 70:
                        interpretations['RSI'] = "Overbought (>70)"
                    elif rsi < 30:
                        interpretations['RSI'] = "Oversold (<30)"
                    else:
                        interpretations['RSI'] = f"Neutral ({rsi:.1f})"
                        
            # Bollinger Bands interpretation
            if 'volatility' in current_values and 'BB_Position' in current_values['volatility']:
                bb_pos = current_values['volatility']['BB_Position']
                if bb_pos:
                    if bb_pos > 80:
                        interpretations['Bollinger_Bands'] = "Near upper band (potential overbought)"
                    elif bb_pos < 20:
                        interpretations['Bollinger_Bands'] = "Near lower band (potential oversold)"
                    else:
                        interpretations['Bollinger_Bands'] = f"Middle range ({bb_pos:.1f}%)"
                        
            # MACD interpretation
            if 'trend' in current_values:
                macd = current_values['trend'].get('MACD')
                signal = current_values['trend'].get('MACD_Signal')
                if macd and signal:
                    if macd > signal:
                        interpretations['MACD'] = "Bullish (above signal line)"
                    else:
                        interpretations['MACD'] = "Bearish (below signal line)"
                        
        except Exception as e:
            interpretations['error'] = f"Error interpreting indicators: {str(e)}"
            
        return interpretations