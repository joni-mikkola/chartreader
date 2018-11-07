# chartreader

<img src="https://github.com/joni-mikkola/chartreader/blob/master/screenshots/screenshot.png" alt="Chart reader">

### Idea
This was a small project I did while back, when I wanted learn to read charts and specifically chart patterns, usually known as technical analysis.
I had plenty of stock data, but no means of studying it, so then I came up with this idea. 

This very simple app can read charts and 1 min chart is provided for Apple(AAPL) stock for one day.
The project also includes simple implementation of RSI and SMA indicators. There is also one custom strategy(MyStrategy) included with the project
which identifies between bullish and bearish engulfing patterns. Obviously this is not a real strategy, but merely an example of how one can create custom strategies with this code.

### Usage:
1. The sample AAPL chart is loaded by default and few first candlesticks are shown
2. The aspiring trader now reads the chart observing market forces and makes estimation of the trend
3. Now pressing space will reveal next candlestick, OR pressing up/down will store your prediction and percentage of correct guesses is shown

### Using Autotrade:
In MyStrategy class there is implementation of very naive and simple strategy, which gives 'rating' to each of the candlesticks and based on these ratings Autotrade will make trades.
Once Autotrade has finished, the application shows percentage of correct predictions.

## Disclaimer: This strategy is naive and real technical analysis goes far beyond this, but this may help some aspiring traders in their journey.

## License: GNU General Public License V3
