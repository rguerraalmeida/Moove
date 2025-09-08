import pandas as pd
import numpy as np
import talib
from typing import Dict, List, Tuple
from datetime import datetime, timedelta

class CandlestickPatterns:
    """Detect and analyze candlestick patterns using TA-Lib."""
    
    def __init__(self):
        self.pattern_functions = self._get_pattern_functions()
        
    def _get_pattern_functions(self) -> Dict[str, callable]:
        """Get all available candlestick pattern functions from TA-Lib."""
        return {
            # Reversal Patterns
            'HAMMER': talib.CDLHAMMER,
            'INVERTED_HAMMER': talib.CDLINVERTEDHAMMER,
            'HANGING_MAN': talib.CDLHANGINGMAN,
            'SHOOTING_STAR': talib.CDLSHOOTINGSTAR,
            'DOJI': talib.CDLDOJI,
            'DOJI_STAR': talib.CDLDOJISTAR,
            'DRAGONFLY_DOJI': talib.CDLDRAGONFLYDOJI,
            'GRAVESTONE_DOJI': talib.CDLGRAVESTONEDOJI,
            'LONG_LEGGED_DOJI': talib.CDLLONGLEGGEDDOJI,
            'MORNING_STAR': talib.CDLMORNINGSTAR,
            'EVENING_STAR': talib.CDLEVENINGSTAR,
            'MORNING_DOJI_STAR': talib.CDLMORNINGDOJISTAR,
            'EVENING_DOJI_STAR': talib.CDLEVENINGDOJISTAR,
            'PIERCING_LINE': talib.CDLPIERCING,
            'DARK_CLOUD_COVER': talib.CDLDARKCLOUDCOVER,
            'ENGULFING': talib.CDLENGULFING,
            'HARAMI': talib.CDLHARAMI,
            'HARAMI_CROSS': talib.CDLHARAMICROSS,
            'SPINNING_TOP': talib.CDLSPINNINGTOP,
            
            # Continuation Patterns  
            'THREE_WHITE_SOLDIERS': talib.CDL3WHITESOLDIERS,
            'THREE_BLACK_CROWS': talib.CDL3BLACKCROWS,
            'RISE_FALL_THREE_METHODS': talib.CDLRISEFALL3METHODS,
            'THREE_INSIDE': talib.CDL3INSIDE,
            'THREE_OUTSIDE': talib.CDL3OUTSIDE,
            'THREE_LINE_STRIKE': talib.CDL3LINESTRIKE,
            'UPSIDE_GAP_TWO_CROWS': talib.CDLUPSIDEGAP2CROWS,
            'ON_NECK': talib.CDLONNECK,
            'IN_NECK': talib.CDLINNECK,
            'THRUSTING': talib.CDLTHRUSTING,
            
            # Other Patterns
            'MARUBOZU': talib.CDLMARUBOZU,
            'CLOSING_MARUBOZU': talib.CDLCLOSINGMARUBOZU,
            'HIGH_WAVE': talib.CDLHIGHWAVE,
            'BELT_HOLD': talib.CDLBELTHOLD,
            'BREAKAWAY': talib.CDLBREAKAWAY,
            'CONCEALING_BABY_SWALLOW': talib.CDLCONCEALBABYSWALL,
            'COUNTERATTACK': talib.CDLCOUNTERATTACK,
            'GAP_SIDE_SIDE_WHITE': talib.CDLGAPSIDESIDEWHITE,
            'HOMING_PIGEON': talib.CDLHOMINGPIGEON,
            'KICKING': talib.CDLKICKING,
            'LADDER_BOTTOM': talib.CDLLADDERBOTTOM,
            'LONG_LINE': talib.CDLLONGLINE,
            'MATCHING_LOW': talib.CDLMATCHINGLOW,
            'RICKSHAW_MAN': talib.CDLRICKSHAWMAN,
            'SEPARATING_LINES': talib.CDLSEPARATINGLINES,
            'SHORT_LINE': talib.CDLSHORTLINE,
            'STALLED_PATTERN': talib.CDLSTALLEDPATTERN,
            'STICK_SANDWICH': talib.CDLSTICKSANDWICH,
            'TAKURI': talib.CDLTAKURI,
            'TASUKI_GAP': talib.CDLTASUKIGAP,
            'TRISTAR': talib.CDLTRISTAR,
            'UNIQUE_THREE_RIVER': talib.CDLUNIQUE3RIVER,
            'ABANDONED_BABY': talib.CDLABANDONEDBABY,
            'ADVANCE_BLOCK': talib.CDLADVANCEBLOCK,
            'TWO_CROWS': talib.CDL2CROWS,
            'THREE_STARS_IN_SOUTH': talib.CDL3STARSINSOUTH,
            'IDENTICAL_THREE_CROWS': talib.CDLIDENTICAL3CROWS,
            'KICKING_BY_LENGTH': talib.CDLKICKINGBYLENGTH,
            'HIKKAKE': talib.CDLHIKKAKE,
            'HIKKAKE_MOD': talib.CDLHIKKAKEMOD,
            'MAT_HOLD': talib.CDLMATHOLD,
            'XSIDE_GAP_THREE_METHODS': talib.CDLXSIDEGAP3METHODS
        }
        
    def detect_all_patterns(self, df: pd.DataFrame) -> Dict:
        """
        Detect all candlestick patterns in the given data.
        
        Args:
            df: DataFrame with OHLC data
            
        Returns:
            Dictionary containing pattern detection results
        """
        if len(df) < 5:  # Need minimum data for pattern detection
            return {"error": "Insufficient data for pattern detection"}
            
        open_prices = df['Open'].values
        high_prices = df['High'].values
        low_prices = df['Low'].values
        close_prices = df['Close'].values
        
        patterns = {}
        
        for pattern_name, pattern_func in self.pattern_functions.items():
            try:
                result = pattern_func(open_prices, high_prices, low_prices, close_prices)
                patterns[pattern_name] = result
            except Exception as e:
                patterns[pattern_name + '_error'] = str(e)
                
        return patterns
        
    def get_recent_patterns(self, patterns: Dict, df: pd.DataFrame, days: int = 90) -> Dict:
        """
        Get patterns that occurred in the last N days.
        
        Args:
            patterns: Dictionary of pattern arrays from detect_all_patterns
            df: Original DataFrame with dates
            days: Number of recent days to check
            
        Returns:
            Dictionary with recent pattern occurrences
        """
        if df.empty or len(df) < days:
            lookback_days = len(df)
        else:
            lookback_days = days
            
        recent_patterns = {
            'bullish': [],
            'bearish': [],
            'neutral': [],
            'dates': df['Date'].tail(lookback_days).tolist(),
            'days_analyzed': lookback_days
        }
        
        for pattern_name, pattern_values in patterns.items():
            if isinstance(pattern_values, np.ndarray):
                # Check last N days for pattern occurrences
                recent_values = pattern_values[-lookback_days:]
                
                for i, value in enumerate(recent_values):
                    if value != 0:  # Pattern detected
                        date_index = len(df) - lookback_days + i
                        pattern_date = df.iloc[date_index]['Date']
                        days_ago = (df['Date'].iloc[-1] - pattern_date).days
                        
                        pattern_info = {
                            'pattern': pattern_name,
                            'date': pattern_date.strftime('%Y-%m-%d'),
                            'days_ago': days_ago,
                            'strength': int(value),  # TA-Lib returns -100, 100, etc.
                            'candle_data': self._get_candle_data(df, date_index)
                        }
                        
                        # Categorize pattern by strength/direction
                        if value > 0:
                            recent_patterns['bullish'].append(pattern_info)
                        elif value < 0:
                            recent_patterns['bearish'].append(pattern_info)
                        else:
                            recent_patterns['neutral'].append(pattern_info)
                            
        # Sort by date (most recent first)
        for category in ['bullish', 'bearish', 'neutral']:
            recent_patterns[category] = sorted(
                recent_patterns[category], 
                key=lambda x: x['days_ago']
            )
            
        return recent_patterns
        
    def _get_candle_data(self, df: pd.DataFrame, index: int) -> Dict:
        """Get OHLCV data for a specific candle."""
        if index >= len(df):
            return {}
            
        row = df.iloc[index]
        return {
            'open': float(row['Open']),
            'high': float(row['High']),
            'low': float(row['Low']),
            'close': float(row['Close']),
            'volume': int(row['Volume']) if 'Volume' in row else 0
        }
        
    def get_pattern_descriptions(self) -> Dict[str, str]:
        """Get descriptions for each candlestick pattern."""
        descriptions = {
            'HAMMER': 'Bullish reversal pattern with small body and long lower shadow',
            'INVERTED_HAMMER': 'Potential bullish reversal with small body and long upper shadow',
            'HANGING_MAN': 'Bearish reversal pattern appearing at top of uptrend',
            'SHOOTING_STAR': 'Bearish reversal with small body and long upper shadow',
            'DOJI': 'Indecision pattern where open and close are nearly equal',
            'DOJI_STAR': 'Gap doji indicating potential reversal',
            'DRAGONFLY_DOJI': 'Bullish reversal doji with long lower shadow',
            'GRAVESTONE_DOJI': 'Bearish reversal doji with long upper shadow',
            'LONG_LEGGED_DOJI': 'High indecision with long shadows on both sides',
            'MORNING_STAR': 'Three-candle bullish reversal pattern',
            'EVENING_STAR': 'Three-candle bearish reversal pattern',
            'MORNING_DOJI_STAR': 'Bullish reversal with doji in middle',
            'EVENING_DOJI_STAR': 'Bearish reversal with doji in middle',
            'PIERCING_LINE': 'Two-candle bullish reversal pattern',
            'DARK_CLOUD_COVER': 'Two-candle bearish reversal pattern',
            'ENGULFING': 'Strong reversal where second candle engulfs first',
            'HARAMI': 'Reversal pattern where second candle is inside first',
            'HARAMI_CROSS': 'Harami pattern with doji as second candle',
            'SPINNING_TOP': 'Small body with long shadows indicating indecision',
            'THREE_WHITE_SOLDIERS': 'Strong bullish continuation with three advancing candles',
            'THREE_BLACK_CROWS': 'Strong bearish continuation with three declining candles',
            'RISE_FALL_THREE_METHODS': 'Three methods continuation pattern',
            'THREE_INSIDE': 'Three inside reversal pattern',
            'THREE_OUTSIDE': 'Three outside reversal pattern',
            'THREE_LINE_STRIKE': 'Four-candle continuation pattern',
            'MARUBOZU': 'Strong directional movement with no shadows',
            'CLOSING_MARUBOZU': 'Marubozu with shadow only on opening side',
            'BELT_HOLD': 'Strong reversal signal',
            'HIGH_WAVE': 'High volatility and indecision pattern',
            'ABANDONED_BABY': 'Rare three-candle reversal with gaps',
            'ADVANCE_BLOCK': 'Bearish pattern with three advancing candles',
            'TWO_CROWS': 'Bearish reversal pattern',
            'HIKKAKE': 'Breakout failure pattern',
            'TAKURI': 'Bullish reversal dragonfly doji variant'
        }
        return descriptions
        
    def analyze_pattern_strength(self, recent_patterns: Dict) -> Dict:
        """
        Analyze the overall pattern strength and market sentiment.
        
        Args:
            recent_patterns: Output from get_recent_patterns
            
        Returns:
            Dictionary with pattern analysis
        """
        analysis = {
            'total_patterns': 0,
            'bullish_count': len(recent_patterns.get('bullish', [])),
            'bearish_count': len(recent_patterns.get('bearish', [])),
            'neutral_count': len(recent_patterns.get('neutral', [])),
            'sentiment': 'Neutral',
            'strength_score': 0,
            'most_recent': None,
            'strongest_signal': None
        }
        
        analysis['total_patterns'] = (analysis['bullish_count'] + 
                                    analysis['bearish_count'] + 
                                    analysis['neutral_count'])
        
        if analysis['total_patterns'] == 0:
            return analysis
            
        # Calculate sentiment
        if analysis['bullish_count'] > analysis['bearish_count']:
            analysis['sentiment'] = 'Bullish'
            analysis['strength_score'] = analysis['bullish_count'] - analysis['bearish_count']
        elif analysis['bearish_count'] > analysis['bullish_count']:
            analysis['sentiment'] = 'Bearish'
            analysis['strength_score'] = analysis['bearish_count'] - analysis['bullish_count']
            
        # Find most recent pattern
        all_patterns = (recent_patterns.get('bullish', []) + 
                       recent_patterns.get('bearish', []) + 
                       recent_patterns.get('neutral', []))
        
        if all_patterns:
            analysis['most_recent'] = min(all_patterns, key=lambda x: x['days_ago'])
            
        # Find strongest signal (highest absolute strength value)
        strongest = None
        max_strength = 0
        
        for pattern in all_patterns:
            abs_strength = abs(pattern['strength'])
            if abs_strength > max_strength:
                max_strength = abs_strength
                strongest = pattern
                
        analysis['strongest_signal'] = strongest
        
        return analysis
        
    def generate_pattern_summary(self, recent_patterns: Dict, analysis: Dict) -> str:
        """
        Generate a human-readable summary of recent patterns.
        
        Args:
            recent_patterns: Output from get_recent_patterns
            analysis: Output from analyze_pattern_strength
            
        Returns:
            Text summary of patterns
        """
        if analysis['total_patterns'] == 0:
            return "No significant candlestick patterns detected in the recent period."
            
        summary_lines = []
        summary_lines.append(f"Pattern Analysis Summary ({analysis['total_patterns']} patterns detected):")
        summary_lines.append(f"Overall Sentiment: {analysis['sentiment']}")
        
        if analysis['most_recent']:
            most_recent = analysis['most_recent']
            summary_lines.append(f"Most Recent: {most_recent['pattern']} ({most_recent['days_ago']} days ago)")
            
        if analysis['strongest_signal']:
            strongest = analysis['strongest_signal']
            signal_type = "Bullish" if strongest['strength'] > 0 else "Bearish"
            summary_lines.append(f"Strongest Signal: {strongest['pattern']} ({signal_type}, strength: {strongest['strength']})")
            
        # Add recent bullish patterns
        if recent_patterns.get('bullish'):
            summary_lines.append("\nRecent Bullish Patterns:")
            for pattern in recent_patterns['bullish'][:3]:  # Show top 3
                summary_lines.append(f"  • {pattern['pattern']} - {pattern['days_ago']} days ago (strength: {pattern['strength']})")
                
        # Add recent bearish patterns
        if recent_patterns.get('bearish'):
            summary_lines.append("\nRecent Bearish Patterns:")
            for pattern in recent_patterns['bearish'][:3]:  # Show top 3
                summary_lines.append(f"  • {pattern['pattern']} - {pattern['days_ago']} days ago (strength: {pattern['strength']})")
                
        return "\n".join(summary_lines)