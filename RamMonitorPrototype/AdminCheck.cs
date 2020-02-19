
namespace RamMonitorPrototype
{
    
    
    // https://stackoverflow.com/a/44565550/155077
    public class AdminCheck
    {
        
        // [System.Runtime.InteropServices.DllImport("shell32.dll", SetLastError=true)]
        // [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        // static extern bool IsUserAnAdmin(); // which more generically tells you whether user is running under elevated rights.
        
        
        public static bool IsCurrentUserAdmin(bool checkCurrentRole)
        {
            bool isElevated = false;

            using (System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent())
            {
                if (checkCurrentRole)
                {
                    // Even if the user is defined in the Admin group, UAC defines 2 roles: one user and one admin. 
                    // IsInRole consider the current default role as user, thus will return false!
                    // Will consider the admin role only if the app is explicitly run as admin!
                    System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
                    isElevated = principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
                }
                else
                {
                    // read all roles for the current identity name, asking ActiveDirectory
                    isElevated = IsAdministratorNoCache(identity.Name);
                }
            } // End Using identity 
            
            return isElevated;
        } // End Function IsCurrentUserAdmin
        
        
        public static bool IsCurrentUserAdmin()
        {
            return IsCurrentUserAdmin(true);
        } // End Function IsCurrentUserAdmin 
        
        
        /// <summary>
        /// Determines whether the specified user is an administrator.
        /// </summary>
        /// <param name="username">The user name.</param>
        /// <returns>
        ///   <c>true</c> if the specified user is an administrator; otherwise, <c>false</c>.
        /// </returns>
        /// <seealso href="https://ayende.com/blog/158401/are-you-an-administrator"/>
        private static bool IsAdministratorNoCache(string username)
        {
            System.DirectoryServices.AccountManagement.PrincipalContext ctx;
            try
            {
                // System.DirectoryServices.ActiveDirectory.Domain d = System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain();
                // string domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                
                try
                {
                    ctx = new System.DirectoryServices.AccountManagement.PrincipalContext(
                        System.DirectoryServices.AccountManagement.ContextType.Domain
                    );
                }
                catch (System.DirectoryServices.AccountManagement.PrincipalServerDownException)
                {
                    // can't access domain, check local machine instead 
                    ctx = new System.DirectoryServices.AccountManagement.PrincipalContext(System.DirectoryServices.AccountManagement.ContextType.Machine);
                }
            }
            catch (System.DirectoryServices.ActiveDirectory.ActiveDirectoryObjectNotFoundException)
            {
                // not in a domain
                ctx = new System.DirectoryServices.AccountManagement.PrincipalContext(System.DirectoryServices.AccountManagement.ContextType.Machine);
            }

            System.DirectoryServices.AccountManagement.UserPrincipal up = System.DirectoryServices.AccountManagement.UserPrincipal.FindByIdentity(ctx, username);
            if (up != null)
            {
                System.DirectoryServices.AccountManagement.PrincipalSearchResult<System.DirectoryServices.AccountManagement.Principal> authGroups = up.GetAuthorizationGroups();

                foreach (System.DirectoryServices.AccountManagement.Principal principal in authGroups)
                {
                    if (
                        principal.Sid.IsWellKnown(System.Security.Principal.WellKnownSidType.BuiltinAdministratorsSid) ||
                        principal.Sid.IsWellKnown(System.Security.Principal.WellKnownSidType.AccountDomainAdminsSid) ||
                        principal.Sid.IsWellKnown(System.Security.Principal.WellKnownSidType.AccountAdministratorSid) ||
                        principal.Sid.IsWellKnown(System.Security.Principal.WellKnownSidType.AccountEnterpriseAdminsSid)
                    )
                        return true;
                } // Next principal 
                
            } // End if (up != null) 
            
            return false;
        } // End Function IsAdministratorNoCache 
        
        
    }
    
    
}