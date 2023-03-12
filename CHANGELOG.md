# Quasar Changelog

## Quasar v1.4.1 [12.03.2023]
* Added missing WOW64 subsystem autostart locations
* Fixed file transfers of files larger than 2 GB
* Fixed file transfers of empty files
* Fixed browser credentials recovery
* Fixed race condition on shutdown
* Fixed IP Geolocation
* Fixed opening remote shell sessions on non-system drives
* Fixed incorrectly set file attributes on client installations
* Fixed sorting of listview columns with numbers
* Updated dependencies

## Quasar v1.4.0 [05.06.2020]
* **Changed target framework to .NET Framework 4.5.2**
* **Changed license to MIT**
* Changed message serializer to Protobuf
* Changed versioning scheme to Semantic Versioning (https://semver.org/)
* Added attended/unattended client modes
* Added TLS 1.2 as transport encryption
* Added UTC timestamps to log files
* Added dependencies as NuGet packages
* Updated dependencies
* Updated message processing in client and server
* Updated mouse and keyboard input to SendInput API
* Fixed file transfer vulnerbilities ([#623](https://github.com/quasar/Quasar/issues/623))
* Lots of under the hood changes for an upcoming plugin system

## Quasar v1.3.0.0 [28.09.2016]
* Added Registry Editor
* Added Remote Webcam
* Added Windows DPI scaling support
* Added IPv6 support
* Added ability to elevate Client
* Added full Unicode support
* Added Remote TCP Connections Viewer
* Added option to hide sub directory of installation path
* Improved cryptography
* Fixed XSS vulnerability in Keylogger Logs
* Fixed Remote Messagebox having wrong icon
* Fixed FileZilla Recovery base64 decoding
* Fixed UPnP discovery freezing in some cases
* Fixed IP Geolocation
* Fixed Client loses Administrator privileges on restart
* Some minor improvements

## Quasar v1.2.0.0 [12.10.2015]
* Added Client restart on unhandled exceptions
* Added additional settings to Keylogger (set/hide log-directory)
* Added encrypted Keylogger logs
* Improved Client Builder
* Improved System Information
* Improved File Manager behaviour when loading directories with many files
* Improved Remote Shell (scrolls now correctly to the bottom when new text received)
* Improved compatibility with many connected clients (1k+)
* Improved AES encryption/decryption speed (if available, makes use of hardware accelerated AES)
* Fixed Client not setting file attribute correctly on startup
* Fixed Remote Desktop lagging with mouse input and maximized window
* Some minor improvements

## Quasar v1.1.0.0 [30.08.2015]
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

## Quasar v1.0.0.0 [22.08.2015]
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
