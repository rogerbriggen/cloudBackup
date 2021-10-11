using System;

namespace cloudBackupCore
{
    public class BackupRoot
    {
        public string RemoteRootPath { get; set; }
        public BackupLocalRoot[] localRoots { get; set; }
       
    }


    public class BackupLocalRoot
    {
        public string LocalRootPath { get; set; }
        public string[] include;
        public string[] exclude;
    }


    public class BackupJob
    {
        public BackupRoot BackupRoot { get; set; }
        public string[] InBackupIncluded;
        public string[] InBackupExclude;
        public DateTime LastSuccessfulRun;
    }
}
