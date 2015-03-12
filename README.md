xRAT 2.0
========
**Free, Open-Source Remote Administration Tool**

xRAT 2.0 is a fast and light-weight Remote Administration Tool coded in C# (using .NET Framework 2.0).

Features
---
* Buffered TCP/IP stream
* Protocol Buffers
* Compressed (LZ4) & Encrypted (AES-128) communication
* Multi-Threaded
* UPnP Support
* Custom social engineering tactic to elevate Admin privileges (betabot's trick)
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

Compiling
---
Open the project in Visual Studio and click build, or use one of the batch files included in the root directory.

| Batch file        | Description
| ----------------- |:-------------
| build-debug.bat   | Builds the application using the debug configuration (for testing)
| build-release.bat | Builds the application using the release configuration  (for publishing)

Building a client
---
| Build configuration         | Description
| ----------------------------|:-------------
| debug configuration         | The pre-defined [Settings.cs](/Client/Config/Settings.cs) will be used. The client builder does not work in this configuration. You can execute the client directly with the specified settings.
| release configuration       | Use the client builder to build your client otherwise it is going to crash.

ToDo
---
* ~~Startup Manager~~
* Socks5 Proxy
* Password Stealer (Browsers, FTP-Clients)
* Keylogger
* Startup Persistence

License
---
See LICENSE file for details.

Donate
---
BTC: `1EWgMfBw1fUSWMfat9oY8t8qRjCRiMEbET`

Credits
---
Protocol Buffers - Google's data interchange format  
Copyright 2008 Google Inc.  
https://code.google.com/apis/protocolbuffers/

ResourceLib  
Copyright (c) Daniel Doubrovkine, Vestris Inc., 2008-2013  
https://github.com/dblock/resourcelib
