using System;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using SLP.Core;

namespace SLP.Features.Commands;

public class CommandsModule : Module
{
    public override string Name => "Commands";
    public override Version Version => new(1, 0, 0);
}

[CommandHandler(typeof(ClientCommandHandler))]
public class ScanCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var player = Player.Get(sender);
        
        if (player.CurrentRoom.Type != RoomType.EzIntercom)
        {
            response = "Вы не в интеркоме";
            return false;
        }
        
        response = "Готово";
        return true;
    }

    public string Command { get; } = "scan";
    public string[] Aliases { get; } = []; 
    public string Description { get; } = "Начинает сканирование комплекса";
}