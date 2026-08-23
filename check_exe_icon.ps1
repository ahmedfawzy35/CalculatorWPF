Add-Type -AssemblyName System.Drawing

$exePath = "d:\CalculatorWPF\bin\Debug\net6.0-windows\CalculatorWPF.exe"
$outPng = "d:\CalculatorWPF\extracted_exe_icon.png"

$icon = [System.Drawing.Icon]::ExtractAssociatedIcon($exePath)
$bmp = $icon.ToBitmap()
$bmp.Save($outPng, [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
$icon.Dispose()

Write-Host "Successfully extracted embedded icon from CalculatorWPF.exe to extracted_exe_icon.png"
