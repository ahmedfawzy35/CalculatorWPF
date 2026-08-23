Add-Type -AssemblyName System.Drawing
$bmp = New-Object System.Drawing.Bitmap("d:\CalculatorWPF\app_icon.png")
$hIcon = $bmp.GetHicon()
$icon = [System.Drawing.Icon]::FromHandle($hIcon)
$fs = [System.IO.File]::Create("d:\CalculatorWPF\app.ico")
$icon.Save($fs)
$fs.Close()
$bmp.Dispose()
