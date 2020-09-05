using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mono.Unix;

namespace EchoSevenUtility
{
    public static class Unix
    {
        public static void DropRoot(String username, ILogger<BackgroundService> logger)
        {
            Mono.Unix.UnixUserInfo unixUser = null;
            try
            {
                unixUser = new UnixUserInfo(username);
                logger.LogDebug("Target user ID is: " + unixUser.UserId);

                if (Mono.Unix.Native.Syscall.getuid() == 0)
                {
                    // we're root, we need to drop privileges
                    Mono.Unix.Native.Syscall.setuid((uint) unixUser.UserId);
                    logger.LogDebug("Dropping root privileges for process. Setting process uid to " + unixUser.UserId);
                }
                else
                {
                    logger.LogDebug("We're not root, we cannot change user id.");
                }
            }
            catch (ArgumentException e)
            {
                logger.LogCritical("User " + username + " does not exist. Cannot change user id. Exiting.");
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                logger.LogCritical(e.Message);
                Environment.Exit(1);
            }
        }
    }
}