if ($args.Count -eq 1) {
    $path = $args[0];
} else {
    Write-Output "Please specify the project directory containing: vmware-downloader.sln";
}

function CheckDotnet {
    try {
        & dotnet --info > $null 2>&1;
        if ($LASTEXITCODE -eq 0) {
            Write-Output "Found dotnet installation, continue build..";
        }
    } catch {
        Write-Output "Could not found dotnet installation. Please install dotnet first";
        Write-Output "dotnet version 8.0 can be installed from here: https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-8.0.406-windows-x64-installer";
        Exit -1;
    }   
}

function BuildProgram {
    try {
        Write-Output "Request build..";
        & dotnet publish $path;
    } catch {
        Write-Output "Error: $_";
        Exit -1;
    }
}

Write-Output "Build path set to: $path";

CheckDotnet;
BuildProgram;

Write-Output "Finished";
