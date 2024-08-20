namespace ToyBox.IPC;

public enum MessageType
{
    None                = 0,
    BCAdd               = 1,
    BCRemove            = 2,
    BCEnabled           = 3,

    FormationData       = 4,
    FormationStop       = 5,

    PartyInviteAccept   = 41,
    PartyPromote        = 42,
    PartyEnterHouse     = 43,
    PartyTeleport       = 44,
    PartyFollow         = 45,
    PartyLeave          = 46,

    SetGfx              = 50,    //Set <bool> true=low false=normal

    CamHack             = 70,
    Chat                = 85,
    ClientLogout        = 98,
    GameShutdown        = 99
}