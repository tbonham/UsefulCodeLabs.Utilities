#region MITLincense
//MIT License
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,:
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
#endregion

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;

namespace UsefulCodeLabs.Utilities
{
    public static class DirectoryManager
    {
        #region DirectoryInformation
        /// <summary>
        ///
        /// </summary>
        /// <param name="Path">Directory path</param>
        /// <param name="Name">Directory Name</param>
        /// <return>Returns DataTable</return>
        public static DataTable DirectoryInfo(String Path)
        {
            DirectoryInfo di = new DirectoryInfo(Path);
            DataTable dt = new DataTable();
            dt.Columns.Add("Path");
            dt.Columns.Add("Name");
            dt.Columns.Add("CreateTime");
            dt.Columns.Add("LastAccessTime");
            dt.Rows.Add(Path, "", di.CreationTime, di.LastAccessTime);
            return dt;
        }
        public static DataTable DirectoryInfo(String Path, String Name)
        {
            String dir = BuildDirectory(Path, Name);
            DirectoryInfo di = new DirectoryInfo(dir);
            DataTable dt = new DataTable();
            dt.Columns.Add("Path");
            dt.Columns.Add("Name");
            dt.Columns.Add("CreateTime");
            dt.Columns.Add("LastAccessTime");
            dt.Rows.Add(Path, Name, di.CreationTime, di.LastAccessTime);
            return dt;
        }
        /// <summary>
        /// Checks to see if the directory is empty.
        /// </summary>
        /// <param name="Path">Directory path</param>
        /// <param name="Name">Directory Name</param>
        /// <return>True or False</return>
        public static Boolean isDirectoryEmpty(String Path)
        {
            return !Directory.EnumerateFileSystemEntries(Path).Any();
        }
        public static Boolean isDirectoryEmpty(String Path, String Name)
        {
            String dir = BuildDirectory(Path, Name);
            return !Directory.EnumerateFileSystemEntries(dir).Any();
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="Path">Directory path</param>
        /// <param name="Name">Directory Name</param>
        /// <return>Returns owners name</return>
        public static String whoIsOwner(String Path, String Name)
        {
            String dir = BuildDirectory(Path, Name);
            String Value = null;
            if (IsElevated)
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                DirectorySecurity dsec = di.GetAccessControl(AccessControlSections.Owner);
                IdentityReference identityReference = dsec.GetOwner(typeof(SecurityIdentifier));
                NTAccount ntAccount = identityReference.Translate(typeof(NTAccount)) as NTAccount;
                Value = ntAccount.Value;
            }
            else
            {
                Value = "Must be run with Elevation";
            }
            return Value;
        }
        #endregion

        #region DirectoryTask
        /// <summary>
        ///
        /// </summary>
        /// <param name="Path">Directory path</param>
        /// <param name="Name">Directory Name</param>
        /// <return>True or False</return>
        public static Boolean CreateDirectory(String Path, String Name)
        {
            if (OperatingSystem.IsWindows())
            {
                String strDirectory = BuildDirectory(Path, Name);
                if (Directory.Exists(strDirectory))
                {
                    return false;
                }
                Directory.CreateDirectory(strDirectory);
                if (Directory.Exists(strDirectory))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="Path">Directory path</param>
        /// <param name="Name">Directory Name</param>
        /// <return>True or False</return>
        public static void DeleteDirectory(String Path, String Name)
        {

            String strDirectory = BuildDirectory(Path, Name);
            try
            {
                Directory.Delete(strDirectory);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="Path">Directory path</param>
        /// <param name="Name">Directory Name</param>
        /// <return>True or False</return>
        private static Boolean RenameDirectory(String Path, String Name, String newName)
        {
            return false;
        }
        #endregion

        #region DirectoryPermissions
        /// <summary>
        ///
        /// </summary>
        /// <param name="Path">Directory path</param>
        /// <param name="Name">Directory Name</param>
        /// <return>True or False</return>
        public static void EnableDirectoryInheritance(String Path, String Name)
        {

            String strDirectory = BuildDirectory(Path, Name);
            DirectoryInfo di = new DirectoryInfo(strDirectory);
            DirectorySecurity dirSec = di.GetAccessControl();
            dirSec.SetAccessRuleProtection(false, false);
            di.SetAccessControl(dirSec);

        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="Path">Directory path</param>
        /// <param name="Name">Directory Name</param>
        /// <return>True or False</return>
        public static void DisableDirectoryInheritance(String Path, String Name)
        {
            String strDirectory = BuildDirectory(Path, Name);
            DirectoryInfo di = new DirectoryInfo(strDirectory);
            DirectorySecurity dirSec = di.GetAccessControl();
            dirSec.SetAccessRuleProtection(true, true);
            di.SetAccessControl(dirSec);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="Path">Directory path</param>
        /// <param name="Name">Directory Name</param>
        /// <return>True or False</return>
        public static Boolean isInheritance(String Path)
        {
            String strDirectory = Path;
            DirectoryInfo di = new DirectoryInfo(strDirectory);
            DirectorySecurity dirSec = di.GetAccessControl();
            var isInheritanceEnabled = !dirSec.AreAccessRulesProtected;
            return isInheritanceEnabled;
        }
        public static Boolean isInheritance(String Path, String Name)
        {
            String strDirectory = BuildDirectory(Path, Name);
            DirectoryInfo di = new DirectoryInfo(strDirectory);
            DirectorySecurity dirSec = di.GetAccessControl();
            var isInheritanceEnabled = !dirSec.AreAccessRulesProtected;
            return isInheritanceEnabled;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="Path">Directory path</param>
        /// <param name="Name">Directory Name</param>
        /// <param name="Username">Username</param>
        /// <return>True or False</return>
        public static void SetFullPermission(String Path, String Name, String Username)
        {
            String strDirectory = BuildDirectory(Path, Name);
            DirectoryInfo di = new DirectoryInfo(strDirectory);
            DirectorySecurity dirSecurity = di.GetAccessControl();
            dirSecurity.AddAccessRule(new FileSystemAccessRule(Username,
                FileSystemRights.FullControl | FileSystemRights.Synchronize,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.None, AccessControlType.Allow
            ));
            di.SetAccessControl(dirSecurity);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="Path">Directory path</param>
        /// <param name="Name">Directory Name</param>
        /// <param name="Username">Username</param>
        /// <return>True or False</return>
        public static void SetModifyPermission(String Path, String Name, String Username)
        {
            String strDirectory = BuildDirectory(Path, Name);
            DirectoryInfo di = new DirectoryInfo(strDirectory);
            DirectorySecurity dirSecurity = di.GetAccessControl();
            dirSecurity.AddAccessRule(new FileSystemAccessRule(Username,
                FileSystemRights.Modify | FileSystemRights.Synchronize,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.None, AccessControlType.Allow
            ));
            di.SetAccessControl(dirSecurity);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="Path">Directory path</param>
        /// <param name="Name">Directory Name</param>
        /// <param name="Username">Username</param>
        /// <return>True or False</return>
        public static void SetWritePermission(String Path, String Name, String Username)
        {
            String strDirectory = BuildDirectory(Path, Name);
            DirectoryInfo di = new DirectoryInfo(strDirectory);
            DirectorySecurity dirSecurity = di.GetAccessControl();
            dirSecurity.AddAccessRule(new FileSystemAccessRule(Username,
                FileSystemRights.Write | FileSystemRights.Synchronize,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.None, AccessControlType.Allow
            ));
            di.SetAccessControl(dirSecurity);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="Path">Directory path</param>
        /// <param name="Name">Directory Name</param>
        /// <param name="Username">Username</param>
        /// <return>True or False</return>
        public static void SetReadPermission(String Path, String Name, String Username)
        {
            String strDirectory = BuildDirectory(Path, Name);
            DirectoryInfo di = new DirectoryInfo(strDirectory);
            DirectorySecurity dirSecurity = di.GetAccessControl();
            dirSecurity.AddAccessRule(new FileSystemAccessRule(Username,
                FileSystemRights.Read | FileSystemRights.Synchronize,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.None, AccessControlType.Allow
            ));
            di.SetAccessControl(dirSecurity);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="Path">Directory path</param>
        /// <param name="Name">Directory Name</param>
        /// <param name="Username">Username</param>
        /// <return>True or False</return>
        public static void SetReadExecutePermission(String Path, String Name, String Username)
        {
            String strDirectory = BuildDirectory(Path, Name);
            DirectoryInfo di = new DirectoryInfo(strDirectory);
            DirectorySecurity dirSecurity = di.GetAccessControl();
            dirSecurity.AddAccessRule(new FileSystemAccessRule(Username,
                FileSystemRights.ReadAndExecute | FileSystemRights.Synchronize,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.None, AccessControlType.Allow
            ));
            di.SetAccessControl(dirSecurity);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="Path">Directory path</param>
        /// <param name="Name">Directory Name</param>
        /// <param name="Username">Username</param>
        /// <return>True or False</return>
        public static void SetListFolderContents(String Path, String Name, String Username)
        {
            String strDirectory = BuildDirectory(Path, Name);
            DirectoryInfo di = new DirectoryInfo(strDirectory);
            DirectorySecurity dirSecurity = di.GetAccessControl();
            dirSecurity.AddAccessRule(new FileSystemAccessRule(Username,
                FileSystemRights.ListDirectory | FileSystemRights.Synchronize,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.None, AccessControlType.Allow
            ));
            di.SetAccessControl(dirSecurity);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="Path">Directory path</param>
        /// <param name="Name">Directory Name</param>
        /// <param name="Username">Username</param>
        /// <return>True or False</return>
        public static void RemovePermissions(String Path, String Name, String Username)
        {
            String strDirectory = BuildDirectory(Path, Name);
            DirectoryInfo dirinfo = new DirectoryInfo(strDirectory);
            DirectorySecurity dsec = dirinfo.GetAccessControl(AccessControlSections.All);
            AuthorizationRuleCollection rules = dsec.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
            foreach (AccessRule rule in rules)
            {
                if (rule.IdentityReference.Value == Username)
                {
                    bool value;
                    dsec.PurgeAccessRules(rule.IdentityReference);
                    dsec.ModifyAccessRule(AccessControlModification.RemoveAll, rule, out value);
                    dirinfo.SetAccessControl(dsec);
                }
            }
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="Path">Directory path</param>
        /// <param name="Name">Directory Name</param>
        /// <param name="Username">Username</param>
        /// <return>True or False</return>
        public static Boolean CheckUserHasFullAccess(String Path, String Name, String Username)
        {
            String strDirectory = BuildDirectory(Path, Name);
            DirectoryInfo dirinfo = new DirectoryInfo(strDirectory);
            DirectorySecurity dsec = dirinfo.GetAccessControl(AccessControlSections.All);
            AuthorizationRuleCollection rules = dsec.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
            foreach (AccessRule rule in rules)
            {
                if (rule.IdentityReference.Value == Username)
                {
                    if ((((FileSystemAccessRule)rule).FileSystemRights & FileSystemRights.FullControl) > 0)
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="Path">Directory path</param>
        /// <param name="Name">Directory Name</param>
        /// <param name="Username">Username</param>
        /// <return>True or False</return>
        public static Boolean CheckUserHasModifyAccess(String Path, String Name, String Username)
        {
            String strDirectory = BuildDirectory(Path, Name);
            DirectoryInfo dirinfo = new DirectoryInfo(strDirectory);
            DirectorySecurity dsec = dirinfo.GetAccessControl(AccessControlSections.All);
            AuthorizationRuleCollection rules = dsec.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
            foreach (AccessRule rule in rules)
            {
                if (rule.IdentityReference.Value == Username)
                {
                    if ((((FileSystemAccessRule)rule).FileSystemRights & FileSystemRights.Modify) > 0)
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="Path">Directory path</param>
        /// <param name="Name">Directory Name</param>
        /// <param name="Username">Username</param>
        /// <return>True or False</return>
        public static Boolean CheckUserHasWriteAccess(String Path, String Name, String Username)
        {
            String strDirectory = BuildDirectory(Path, Name);
            DirectoryInfo dirinfo = new DirectoryInfo(strDirectory);
            DirectorySecurity dsec = dirinfo.GetAccessControl(AccessControlSections.All);
            AuthorizationRuleCollection rules = dsec.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
            foreach (AccessRule rule in rules)
            {
                if (rule.IdentityReference.Value == Username)
                {
                    if ((((FileSystemAccessRule)rule).FileSystemRights & FileSystemRights.Write) > 0)
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="Path">Directory path</param>
        /// <param name="Name">Directory Name</param>
        /// <param name="Username">Username</param>
        /// <return>True or False</return>
        public static Boolean CheckUserHasReadAccess(String Path, String Name, String Username)
        {
            String strDirectory = BuildDirectory(Path, Name);
            DirectoryInfo dirinfo = new DirectoryInfo(strDirectory);
            DirectorySecurity dsec = dirinfo.GetAccessControl(AccessControlSections.All);
            AuthorizationRuleCollection rules = dsec.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
            foreach (AccessRule rule in rules)
            {
                if (rule.IdentityReference.Value == Username)
                {
                    if ((((FileSystemAccessRule)rule).FileSystemRights & FileSystemRights.Read) > 0)
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="Path">Directory path</param>
        /// <param name="Name">Directory Name</param>
        /// <param name="Username">Username</param>
        /// <return>True or False</return>
        public static Boolean CheckUserHasReadAndExecuteAccess(String Path, String Name, String Username)
        {
            String strDirectory = BuildDirectory(Path, Name);
            DirectoryInfo dirinfo = new DirectoryInfo(strDirectory);
            DirectorySecurity dsec = dirinfo.GetAccessControl(AccessControlSections.All);
            AuthorizationRuleCollection rules = dsec.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
            foreach (AccessRule rule in rules)
            {
                if (rule.IdentityReference.Value == Username)
                {
                    if ((((FileSystemAccessRule)rule).FileSystemRights & FileSystemRights.ReadAndExecute) > 0)
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="Path">Directory path</param>
        /// <param name="Name">Directory Name</param>
        /// <param name="Username">Username</param>
        /// <return>True or False</return>
        public static Boolean CheckUserHasListDirectoryAccess(String Path, String Name, String Username)
        {
            String strDirectory = BuildDirectory(Path, Name);
            DirectoryInfo dirinfo = new DirectoryInfo(strDirectory);
            DirectorySecurity dsec = dirinfo.GetAccessControl(AccessControlSections.All);
            AuthorizationRuleCollection rules = dsec.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
            foreach (AccessRule rule in rules)
            {
                if (rule.IdentityReference.Value == Username)
                {
                    if ((((FileSystemAccessRule)rule).FileSystemRights & FileSystemRights.ListDirectory) > 0)
                        return true;
                }
            }
            return false;
        }
        #endregion
        #region InternalMethods
        private static String BuildDirectory(String Path, String Name)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Path);
            sb.Append(Name);
            return sb.ToString();
        }
        private static void LogError(Exception ex)
        {
            StringBuilder sbFileName = new StringBuilder();
            sbFileName.Append("error-");
            sbFileName.Append(DateTime.Now.ToString("yyyy-mm-dd-hh-mm-ss-tt"));
            sbFileName.Append(".log");
            String strFileName = sbFileName.ToString();

            string message = string.Format("Time: {0}", DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("Message: {0}", ex.Message);
            message += Environment.NewLine;
            message += string.Format("StackTrace: {0}", ex.StackTrace);
            message += Environment.NewLine;
            message += string.Format("Source: {0}", ex.Source);
            message += Environment.NewLine;
            message += string.Format("TargetSite: {0}", ex.TargetSite.ToString());
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;

            using (StreamWriter writer = new StreamWriter(strFileName, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }
        public static bool PathIsValid(string inputPath)
        {
            try
            {
                Path.GetFullPath(inputPath);
            }
            catch (PathTooLongException ex)
            {
#if DEBUG
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("-----------------------------------------------------------");
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine(string.Format("Message: {0}", ex.Message));
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine(string.Format("StackTrace: {0}", ex.StackTrace));
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine(string.Format("Source: {0}", ex.Source));
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine(string.Format("TargetSite: {0}", ex.TargetSite.ToString()));
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("-----------------------------------------------------------");
                Console.WriteLine(Environment.NewLine);
#endif
                LogError(ex);
                return false;
            }
            return true;
        }
        private static Boolean IsElevated
        {
            get
            {
                return new WindowsPrincipal(WindowsIdentity.GetCurrent())
                    .IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
        #endregion
    }


}