#QuasarRAT Changelog

##Quasar v1.1.0.0 [30.08.2015]
* **Changed Target Framework to .NET Framework 4.0 Client Profile**
* Added deletion of ZoneIdentifier file when installing
* Improved Client installation error handling
* Improved Client HardwareID generation
* Improved Client-Server handshake
* Support detection of multiple AVs, Firewalls, GPUs, CPUs
* Fixed Builder Profile not saving correctly Installation Subfolder
* Fixed Builder not validating input correctly
* Fixed Builder creating Client with empty list of hosts
* Fixed Settings Password not hashed when pressing 'Start listening'
* Fixed Reverse Proxy using always wrong port
* Fixed Server throwing NullReferenceException when closing and no Clients connected
* Fixed Client reporting wrong uptime on systems with uptime longer than 49.7 days
* Fixed Client installation path empty on Windows XP 32-bit in some scenarios
* Fixed Client installation to system directory failing on 64-bit OS
* Fixed Client uninstallation not working when file is marked as read-only
* Fixed Client crashing after update on first start in some scenarios
* Fixed Client crashing when list of hosts is empty (Client exits now)
* Fixed Client not reconnecting when Server uses different password
* Fixed Client registry access
* Removed Statistics window, will be remade in a later version

##Quasar v1.0.0.0 [22.08.2015]
* **xRAT is now Quasar**
* Added Password Recovery (Common Browsers and FTP Clients)
* Added Server compatiblity with Mono (Server now runs on Linux with Mono installed)
 * Client Builder works also on Linux/Mono
* Added ability to upload batch files
* Added Client support for multiple hosts
* Added maximum simultaneous file downloads/uploads (current max: 2)
* Fixed Remote Shell redirecting of standard output not working after redirecting error output
* Fixed Remote Shell not displaying unicode characters correctly
* Fixed Remote Desktop crash when changing screen resolution
* Fixed File Manager would refresh directory when double-clicking files
* Improved support for Windows 8 and above
* Improved Remote Desktop (Speed, Full Mouse and Keyboard support)
* Improved File Manager (Show name of drive, current path, upload files)
* Improved UPnP support
* Improved Geo IP support
* Improved Builder UI
* Switched from Protobuff to NetSerializer
* Lots of under the hood changes for stability and performance
