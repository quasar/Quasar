# Quasar Roadmap

## Network communication layer abstraction
At the moment the command handling is tightly coupled to the WinForms. This should be decoupled and the WinForms when opened have to register callbacks in the CommandHandler. Use dedicated command handler where the WinForms register their callbacks to get notified when a command is received. This also flattens the way for a plugin system in the future.

## Add HTTP API to the server
With a new HTTP JSON API it will be possible to create a web interface for Quasar. Remote desktop could also work via websockets.

## Switch serialization to Protocol Buffers (protobuf)
Protocol Buffers is a well supported serialization format which makes it easier to implement clients in different programming languages. Additionally it provides versioning information of serialized data which may get useful with different releases of Quasar communicating with each other.

## Transparent communication protocol
To open up the way for Quasar clients in different programming languages the communication protocol needs to be clearly specified and documented.

## Use TLS with client certificates instead of password for authentication
TLS replaces the self-made encryption of the network communication. The server should create a certificate at the first startup and use that to authentificate itself to new clients. When building a new client, a client certificate will be created and signed by the server. The server will only allow client certificates which are signed by the server certificate.

## Command line (CLI) version of the server
It would be quite handy if the Quasar server could be a .NET Core CLI program which can run as a daemon in the background of Windows, MacOS or Linux.

## Use NuGet or gitmodules for referenced libraries
NuGet/gitmodules helps updating the referenced libraries in an easy way instead of using a binary.

## Refactor graphical user interface (GUI) of the server
WPF as GUI framework would drastically improve rendering performance of the remote desktop with the hardware accelerated rendering. WPF uses DirectX if available unlike WinForms which stick to rendering the controls in the message loop of the window.

## Use a shared library for classes that both server and client have in common
This removes a lot of duplicated code which is error prone once someone forgets to change it in one place.

## Remove features such as password recovery
Features which could get abused too easily should get removed from Quasar.

## Allow different types of clients (permissioned clients)
Allow clients with higher privileges (i.e. ability to administrate other lower privileged clients) connect to the server. This change would allow administrators to manage clients from their own computers with a lightweight client without having to run the server.

## Allow client to be installed as Windows service
Currently the client is installed on a per-user basis and this makes it unflexible to remotly manage the machine when the user is not logged in. it also requires the client to be installed for every account who uses the machine. Machines which are used by multiple users would greatly benefit when Quasar could be run as a Windows service.

## Basic client GUI
Add a basic GUI to the client to allow the user at any time to check the status if a specific feature is active. Additionally it can show a notification when the client gets installed or when someone requests permission to use remote desktop (similar to teamviewer).