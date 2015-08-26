#QuasarRAT Changelog

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
