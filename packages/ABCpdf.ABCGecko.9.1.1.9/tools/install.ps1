param($installPath, $toolsPath, $package, $project)

. (join-path $toolsPath 'props.ps1')
. (join-path $toolsPath 'funcs.ps1')

$installBin = join-path $installPath 'bin'
install-nativedlls $project $installBin $nativeDlls

function say($x)
{
    if([Environment]::UserInteractive)
    {
        [System.Windows.Forms.MessageBox]::Show($x, "ABCGecko")
    }
    
    else
    {
        write-host "ABCGecko> $x"
    }
}

if([Environment]::UserInteractive)
{
    add-type -AssemblyName System.Windows.Forms
}

say "Finished! Please deploy the XULRunner21_0 folder to your output directory manually."
