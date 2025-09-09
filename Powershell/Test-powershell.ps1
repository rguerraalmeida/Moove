$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
$session.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36"
Invoke-WebRequest -UseBasicParsing -Uri "https://api.nasdaq.com/api/screener/stocks?tableonly=false&limit=25&exchange=NYSE" `
-WebSession $session `
-Headers @{
"authority"="api.nasdaq.com"
  "method"="GET"
  "path"="/api/screener/stocks?tableonly=false&limit=25&exchange=NYSE"
  "scheme"="https"
  "accept"="application/json, text/plain, */*"
  "accept-encoding"="gzip, deflate, br, zstd"
  "accept-language"="en-US,en;q=0.9"
  "origin"="https://www.nasdaq.com"
  "priority"="u=1, i"
  "referer"="https://www.nasdaq.com/"
  "sec-ch-ua"="`"Not;A=Brand`";v=`"99`", `"Google Chrome`";v=`"139`", `"Chromium`";v=`"139`""
  "sec-ch-ua-mobile"="?0"
  "sec-ch-ua-platform"="`"Windows`""
  "sec-fetch-dest"="empty"
  "sec-fetch-mode"="cors"
  "sec-fetch-site"="same-site"
}