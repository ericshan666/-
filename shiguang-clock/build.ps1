param(
    [switch]$Run,
    [switch]$Preview
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
$py = 'C:\Users\lenovo\.dsh\dsh-runtimes\dsh-primary-runtime\dependencies\python\python.exe'

if (-not (Test-Path $csc)) {
    throw "找不到 .NET Framework C# 编译器：$csc"
}

Push-Location $root
try {
    if ((Test-Path $py) -and (Test-Path 'tools\make_icon.py')) {
        & $py 'tools\make_icon.py' | Out-Host
    }

    $dist = Join-Path $root 'dist'
    New-Item -ItemType Directory -Force -Path $dist | Out-Null
    $exe = Join-Path $dist '拾光时钟.exe'
    $pdb = Join-Path $dist '拾光时钟.pdb'

    $arguments = @(
        '/nologo',
        '/target:winexe',
        '/platform:anycpu',
        '/optimize+',
        '/langversion:5',
        '/codepage:65001',
        '/debug-',
        "/win32icon:$(Join-Path $root 'assets\icon.ico')",
        "/win32manifest:$(Join-Path $root 'app.manifest')",
        "/out:$exe",
        '/reference:System.dll',
        '/reference:System.Core.dll',
        '/reference:System.Drawing.dll',
        '/reference:System.Windows.Forms.dll',
        (Join-Path $root 'src\ShiguangClock.cs'),
        (Join-Path $root 'src\AssemblyInfo.cs')
    )

    & $csc $arguments
    if ($LASTEXITCODE -ne 0) {
        throw "编译失败，退出代码 $LASTEXITCODE"
    }

    if (Test-Path $pdb) { Remove-Item $pdb -Force }

    $info = Get-Item $exe
    Write-Host ''
    Write-Host ("已生成：{0}" -f $info.FullName) -ForegroundColor Green
    Write-Host ("大小：{0:N0} 字节" -f $info.Length)

    if ($Preview) {
        $previewDir = Join-Path $root 'preview'
        New-Item -ItemType Directory -Force -Path $previewDir | Out-Null
        & $exe '--preview' (Join-Path $previewDir 'dark.png')
        & $exe '--preview' (Join-Path $previewDir 'light.png') '--light'
        Write-Host ("预览截图：{0}" -f $previewDir) -ForegroundColor Green
    }

    if ($Run) {
        Start-Process -FilePath $exe
    }
}
finally {
    Pop-Location
}
