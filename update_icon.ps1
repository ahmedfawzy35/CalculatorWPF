Add-Type -AssemblyName System.Drawing
$imgPath = "C:\Users\ahmed\.gemini\antigravity\brain\3601e297-f8cc-4140-a210-ad80e66c0846\app_icon_preview_1786205172683.jpg"
$pngPath = "d:\CalculatorWPF\app_icon.png"
$icoPath = "d:\CalculatorWPF\app.ico"

$img = [System.Drawing.Image]::FromFile($imgPath)
$bmp = New-Object System.Drawing.Bitmap(512, 512)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$g.DrawImage($img, 0, 0, 512, 512)
$bmp.Save($pngPath, [System.Drawing.Imaging.ImageFormat]::Png)
$g.Dispose()
$bmp.Dispose()
$img.Dispose()

$bmp2 = New-Object System.Drawing.Bitmap($pngPath)
$hIcon = $bmp2.GetHicon()
$icon = [System.Drawing.Icon]::FromHandle($hIcon)
$fs = [System.IO.File]::Create($icoPath)
$icon.Save($fs)
$fs.Close()
$bmp2.Dispose()
Write-Host "Icons updated successfully."
