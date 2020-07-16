# Quasar Roadmap

## Web API
A web API (e.g. REST-like) allows to interact with the clients in more flexible ways and can be used to build a web interface for Quasar.

## Transparent communication protocol
To open up the way for Quasar clients in different programming languages the communication protocol needs to be clearly specified and documented.

## Cross-platform support
A long-term goal is to support operating systems such as MacOS and Linux. The new .NET Core framework will help achieve this goal.

## Command line (CLI) version of the server
It should be possible to use the server as a simple CLI tool to accept and forward (proxy) connections to other servers.

## GUI overhaul
The GUI needs to be reworked in a more modern way, such as WPF or a web-based interface. WPF as GUI framework would drastically improve rendering performance of the remote desktop with the hardware accelerated rendering, similar to a web-based GUI depending on the used browser.

## Allow different types of clients (permissioned clients)
Allow clients with higher privileges (i.e. ability to administrate other lower privileged clients) connect to the server. This change would allow administrators to manage clients from their own computers with a lightweight client without having to run the server.

## Allow client installation as Windows service
Currently the client is installed on a per-user basis and this makes it unflexible to remotly manage the machine when the user is not logged in. it also requires the client to be installed for every account who uses the machine. Machines which are used by multiple users would greatly benefit when Quasar could be run as a Windows service.

## Basic client GUI
Add a basic GUI to the client to allow the user at any time to check the status if a specific feature is active. Additionally it can show a notification when the client gets installed or when someone requests permission to use remote desktop (similar to teamviewer).
