param($installPath, $toolsPath, $package, $project)

. (join-path $toolsPath 'props.ps1')
. (join-path $toolsPath 'funcs.ps1')

function say($x)
{
    write-host "ABCpdf> $x"
}

$installBin = join-path $installPath 'bin'
$installLib = join-path $installPath (join-path 'lib' 'net40')

install-nativedlls $project $package $installBin $nativeDlls

say "Thank you for installing!"
say ""
say "For documentation and example projects, please download the installer bundle from the ABCpdf download site."

cp (join-path $installLib 'ABCpdf.dll') $installBin
$pdfSettingsExe = join-path $installBin 'PDFSettings.exe'

if([System.Environment]::UserInteractive)
{
    & $pdfSettingsExe "/Register"
}
else
{
    say "Run $pdfSettingsExe to check your license information"
}
