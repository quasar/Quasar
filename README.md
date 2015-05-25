xRAT 2.0
========
[![Build status](https://ci.appveyor.com/api/projects/status/na7hitbqx8327xr9?svg=true)](https://ci.appveyor.com/project/MaxXor/xrat)

**Free, Open-Source Remote Administration Tool**

xRAT 2.0 is a fast and light-weight Remote Administration Tool coded in C# (using [.NET Framework 3.5 Client Profile](https://www.microsoft.com/en-US/download/details.aspx?id=14037)).

Requirements
---
* .NET Framework 3.5 Client Profile

Features
---
* Buffered TCP/IP stream
* Protocol Buffers
* Compressed (QuickLZ) & Encrypted (AES-128) communication
* Multi-Threaded
* UPnP Support
* No-IP.org Support
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
* Keylogger
* Reverse SOCKS5/HTTPS Proxy

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
* ~~Reverse SOCKS5/HTTPS Proxy~~
* Password Recovery (Browsers, FTP-Clients)
* Startup Persistence
* [Issues](https://github.com/MaxXor/xRAT/issues)

Contributing
---
1. Fork it
2. Create your feature branch (`git checkout -b my-new-feature`)
3. Commit your changes (`git commit -am 'Add some feature'`)
4. Push to the branch (`git push origin my-new-feature`)
5. Create new Pull Request

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
https://developers.google.com/protocol-buffers/

ResourceLib  
Copyright (c) Daniel Doubrovkine, Vestris Inc., 2008-2013  
https://github.com/dblock/resourcelib

globalmousekeyhook  
Copyright (c) 2004-2015, George Mamaladze  
https://github.com/gmamaladze/globalmousekeyhook

Thank you!
---
I really appreciate all kinds of feedback and contributions. Thanks for using and supporting xRAT 2.0!
