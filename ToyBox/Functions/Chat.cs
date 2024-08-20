using ToyBox.IPC;

namespace ToyBox.Functions;

public static class Chat
{

    public static void HandleBroadcastCommands(List<string> message)
    {
        switch (message[1])
        {
            case "/logout":
                IPCProvider.CharacterLogoutAction();
                break;
            case "/shutdown":
                IPCProvider.GameShutdownAction();
                break;
            default:
                IPCProvider.SendChatAction(message[1]);
                break;
        }
    }

}