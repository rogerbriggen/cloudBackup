using System;

namespace CloudBackupCore;

public class BackupRoot
{
    public string? RemoteRootPath { get; set; }
    public BackupLocalRoot[]? LocalRoots { get; set; }

}


public class BackupLocalRoot
{
    public string? LocalRootPath { get; set; }
    public string[]? IncludeInBackup { get; set; }
    public string[]? ExcludeFromBackup { get; set; }

}


public class BackupJob
{
    public string? Description { get; set; }
    public BackupRoot? BackupRoot { get; set; }
    public string[]? IncludeInBackup { get; set; }
    public string[]? ExcludeFromBackup { get; set; }
    public DateTime? LastSuccessfulRun { get; set; }
}

