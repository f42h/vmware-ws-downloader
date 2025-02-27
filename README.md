# vmware-ws-downloader

### Description

```
Download VMware Workstation Pro without the need for an account. Specify what is needed and this simple .NET8.0 based CLI application will download it for you easily. 
```

### External Libraries used by this project

- [HtmlAgilityPack](https://html-agility-pack.net/)

### Setup

- To be able to build this program, download the [dotnet, version 8.0](https://dotnet.microsoft.com/en-us/download) framework 
- [Download](https://github.com/f42h/vmware-ws-downloader/archive/refs/heads/main.zip) the repository
- Unzip, fireup a console and go to the projects root directory
- Build the downloader by using `build.ps1` in the `tools` directory

```
.\tools\build.ps1 .\vmware-downloader 
```

- Parse to: `vmware-downloader\vmware-downloader\bin\Release\net8.0` and execute `vmware-downloader.exe`

### Download Process

- The first section asks for the VMware workstation version
- In this case we want the latest version: 17.6.2

#### `Note:` Everything will be configured by index

```
VMWare Workstation Downloader

#######################################

0) 12.0.0
1) 12.0.1
2) 12.1.0
3) 12.1.1
...
50) 17.5.1
51) 17.5.2
52) 17.6.0
53) 17.6.1
54) 17.6.2

Version> 54

#######################################

0) 24409262

Nr> 0
```

- In the next step select the `target OS`

```
#######################################

0) linux
1) windows

OS> 1
```

- Select `core` to show the available files

```
#######################################

0) VMware-workstation-17.6.2-24409262.exe.tar
1) metadata.xml.gz

Package> 0
```

- Now the setup is completed and a prompt will show up asking to start the download
- Simply type `y` and the download will start 

```
Start the download for VMware-workstation-17.6.2-24409262.exe.tar?
n/N = back to start
y/Y = download

> y

Starting download..

File saved to: ..\vmware-downloader\vmware-downloader\bin\Release\net8.0\output\VMware-workstation-17.6.2-24409262.exe.tar

Press any key to continue..
```

- This process can be repeated by simply pressing enter
#### Move to the output directory, unpack the installer and all is ready to install VMware Workstation on the target machine
