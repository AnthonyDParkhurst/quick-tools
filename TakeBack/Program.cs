using System.ComponentModel;
using System.Diagnostics;
using System.IO.Enumeration;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;

namespace TakeBack
{
    internal class Program
    {
        public static string RunningUser { get; } = $"{Environment.UserDomainName}\\{Environment.UserName}";
        public static NTAccount RunningAccount { get; } = new NTAccount(Environment.UserDomainName, Environment.UserName);

        static void Main(string[] args)
        {
            try
            {
                var eo = new EnumerationOptions();
                eo.IgnoreInaccessible = true;
                eo.RecurseSubdirectories = true;

                ////Console.Write("Enter folder: ");
                ////var root = Console.ReadLine();

                var root = @"C:\Temp\Windows.old";


                ////foreach (var entry in Directory.EnumerateDirectories(root, "*", eo))
                ////{
                ////    TakeIt(new DirectoryInfo(entry));
                ////}

                foreach (var entry in Directory.EnumerateFiles(root, "*", eo))
                {
                    TakeIt(new FileInfo(entry));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }

            Console.Write("Enter key to finish...");
            Console.ReadKey();
        }

        private static void TakeIt(FileInfo file)
        {
            Console.WriteLine(file.FullName);

            var si = new ProcessStartInfo("takeown");
            si.UseShellExecute = true;
            si.ArgumentList.Add("/F");
            si.ArgumentList.Add($"{file.Name}");

            using (var p = Process.Start(si))
            {
                p?.WaitForExit();
            }

            ////var acl = file.GetAccessControl(System.Security.AccessControl.AccessControlSections.All);

            ////acl.SetOwner(RunningAccount);

            ////acl.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule(RunningUser,
            ////    System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow));

            ////file.SetAccessControl(acl);
        }

        private static void TakeIt(DirectoryInfo dir)
        {
            Console.WriteLine(dir.FullName);

            ////var acl = dir.GetAccessControl(System.Security.AccessControl.AccessControlSections.All);

            ////acl.SetOwner(RunningAccount);

            ////acl.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule(RunningUser,
            ////    System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow));

            ////dir.SetAccessControl(acl);
        }
    }
}