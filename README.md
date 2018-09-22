# QuasarRAT

[![Build status](https://ci.appveyor.com/api/projects/status/5857hfy6r1ltb5f2?svg=true)](https://ci.appveyor.com/project/MaxXor/quasarrat) [![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

**Free, Open-Source Remote Administration Tool for Windows**

Quasar is a fast and light-weight remote administration tool coded in C#. Providing high stability and an easy-to-use user interface, Quasar is the perfect remote administration solution for you.

## Features
* TCP network stream (IPv4 & IPv6 support)
* Fast network serialization (NetSerializer)
* Compressed (QuickLZ) & Encrypted (AES-128) communication
* Multi-Threaded
* UPnP Support
* No-Ip.com Support
* Visit Website (hidden & visible)
* Show Messagebox
* Task Manager
* File Manager
* Startup Manager
* Remote Desktop
* Remote Shell
* Download & Execute
* Upload & Execute
* System Information
* Computer Commands (Restart, Shutdown, Standby)
* Keylogger (Unicode Support)
* Reverse Proxy (SOCKS5)
* Password Recovery (Common Browsers and FTP Clients)
* Registry Editor

## Supported runtimes and operating systems
* .NET Framework 4.0 Client Profile or higher ([Download](https://www.microsoft.com/en-us/download/details.aspx?id=24872))
* Supported operating systems (32- and 64-bit)
  * Windows XP SP3
  * Windows Server 2003
  * Windows Vista
  * Windows Server 2008
  * Windows 7
  * Windows Server 2012
  * Windows 8/8.1
  * Windows 10

## Compiling
Open the project in Visual Studio 2017+ and click build. See below which build configuration to choose.

## Building a client
| Build configuration         | Usage scenario | Description
| ----------------------------|----------------|--------------
| Debug configuration         | Testing        | The pre-defined [Settings.cs](/Client/Config/Settings.cs) will be used, so edit this file before compiling the client. You can execute the client directly with the specified settings.
| Release configuration       | Live use       | Start `Quasar.exe` and use the client builder.

## Contributing
See [CONTRIBUTING.md](CONTRIBUTING.md)

## Roadmap
See [ROADMAP.md](ROADMAP.md)

## License
See [LICENSE](LICENSE)

## Third-party libraries
protobuf-net  
Copyright (c) 2008 Marc Gravell  
https://github.com/mgravell/protobuf-net

ResourceLib  
Copyright (c) 2008-2016 Daniel Doubrovkine, Vestris Inc.  
https://github.com/dblock/resourcelib

GlobalMouseKeyHook  
Copyright (c) 2004-2018 George Mamaladze  
https://github.com/gmamaladze/globalmousekeyhook

Mono.Cecil  
Copyright (c) 2008 - 2015 Jb Evain  
Copyright (c) 2008 - 2011 Novell, Inc.  
https://github.com/jbevain/cecil

Mono.Nat  
Copyright (c) 2006 Alan McGovern  
Copyright (c) 2007 Ben Motmans  
https://github.com/nterry/Mono.Nat

## Thank you!
I really appreciate all kinds of feedback and contributions. Thanks for using and supporting Quasar!
