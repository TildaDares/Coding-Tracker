using System.ComponentModel;

namespace CodingTracker.Enums;

public enum MainMenuOptions
{
    [Description("Insert coding session")]
    InsertCodingSession = 0,
    [Description("Get coding session")]
    GetCodingSession,
    [Description("Get coding sessions")]
    GetCodingSessions,
    [Description("Update coding session")]
    UpdateCodingSession,
    [Description("Delete coding session")]
    DeleteCodingSession,
    [Description("Start coding session")]
    StartCodingSession,
    [Description("Goals")]
    Goals,
    [Description("Exit")]
    Exit
}
