# EchoSeven

EchoSeven is a RFC 862 echo server implementation in .Net Core with Systemd integration. The networking implementation is built on [NetCoreServer](https://github.com/chronoxor/NetCoreServer). EchoSeven is a multi-threaded, high performance echo server implementation.

There are many RFC 862 implementations, why create another one? For me, this was a learning experience and a vehicle to explore if .Net Core can be used to create daemon programs for Unix - Linux in particular, as it's the only supported Unix server operating system supported by .Net Core at the moment.

EchoSeven makes use of the .Net Core worker template, and the recently added Systemd support. It also leverages appsettings.json for configuration and uses libraries from Mono to make libc system calls to drop root privileges after binding TCP and UDP port 7 to a socket.

I plan on adding the CI/CD automation to build a Debian package of EchoSeven. In other words, EchoSeven's primary purpose is not to provide yet another echo server implementation but a more extended and complete template for anyone wanting to build network daemons for Linux written in C# and using .Net Core.

If you plan on using an echo server that does a little more than what RFC 862 describes, this code base can also be a great starting point for you, allowing you to add whatever functionality you have in mind. I have my own ideas already...

If you came here looking for a plain old RFC 862 echo service, just use inetd or other standalone echo servers that already ship with your Linux distributions package management. Of course, you may also use EchoSeven.