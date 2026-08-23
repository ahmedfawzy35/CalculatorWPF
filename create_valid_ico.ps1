Add-Type -AssemblyName System.Drawing

$srcPath = "C:\Users\ahmed\.gemini\antigravity\brain\3601e297-f8cc-4140-a210-ad80e66c0846\blue_large_icon_preview_1786205662375.jpg"
$pngPath = "d:\CalculatorWPF\app_icon.png"
$icoPath = "d:\CalculatorWPF\app.ico"

# 1. Generate app_icon.png (512x512)
$srcImg = [System.Drawing.Image]::FromFile($srcPath)
$bmp512 = New-Object System.Drawing.Bitmap(512, 512)
$g = [System.Drawing.Graphics]::FromImage($bmp512)
$g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
$g.DrawImage($srcImg, 0, 0, 512, 512)
$bmp512.Save($pngPath, [System.Drawing.Imaging.ImageFormat]::Png)
$g.Dispose()
$bmp512.Dispose()
$srcImg.Dispose()

# 2. Build multi-resolution valid Windows ICO file (PNG encoded entries)
$sizes = @(256, 128, 64, 48, 32, 16)
$pngDataList = @()

foreach ($sz in $sizes) {
    $bmp = New-Object System.Drawing.Bitmap($sz, $sz)
    $gr = [System.Drawing.Graphics]::FromImage($bmp)
    $gr.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $gr.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $src = [System.Drawing.Image]::FromFile($pngPath)
    $gr.DrawImage($src, 0, 0, $sz, $sz)
    $src.Dispose()
    $gr.Dispose()

    $ms = New-Object System.IO.MemoryStream
    $bmp.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    $pngDataList += ,$ms.ToArray()
    $ms.Dispose()
}

$fs = [System.IO.File]::Create($icoPath)
$bw = New-Object System.IO.BinaryWriter($fs)

# ICONDIR
$bw.Write([uint16]0) # Reserved
$bw.Write([uint16]1) # Type (Icon)
$bw.Write([uint16]$sizes.Count) # Count

$dataOffset = 6 + (16 * $sizes.Count)

for ($i = 0; $i -lt $sizes.Count; $i++) {
    $sz = $sizes[$i]
    $data = $pngDataList[$i]

    $w = if ($sz -ge 256) { 0 } else { [byte]$sz }
    $h = if ($sz -ge 256) { 0 } else { [byte]$sz }

    $bw.Write([byte]$w)
    $bw.Write([byte]$h)
    $bw.Write([byte]0) # ColorCount
    $bw.Write([byte]0) # Reserved
    $bw.Write([uint16]1) # Planes
    $bw.Write([uint16]32) # BitCount
    $bw.Write([uint32]$data.Length) # BytesInRes
    $bw.Write([uint32]$dataOffset) # ImageOffset

    $dataOffset += $data.Length
}

for ($i = 0; $i -lt $sizes.Count; $i++) {
    $bw.Write($pngDataList[$i])
}

$bw.Flush()
$bw.Close()
$fs.Close()

Write-Host "Enlarged blue ICO created successfully."
