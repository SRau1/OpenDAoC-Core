using System;
using DOL.Language;
using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
	[CmdAttribute("&autosplit",
		 ePrivLevel.Player,
		 "Choose how the loot and money are split between members of group",
		 "/autosplit on/off (Leader only: Toggles both coins and loot for entire group)",
		 "/autosplit coins (Leader only: When turned off, will send coins to the person who picked it up, instead of splitting it evenly across other members)",
		 "/autosplit loot (Leader only: When turned off, will send loot to the person who picked it up, instead of splitting it evenly across other members)",
		 "/autosplit master [playername] (Leader only: Sets master looter - all items go to this player, coins still autosplit. Use without name to clear)",
		 "/autosplit self (Any group member: Choose not to receive autosplit loot items)")]
	public class AutosplitCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public void OnCommand(GameClient client, string[] args)
		{
			// If they are not in a group, then this command should not work at all
			if (client.Player.Group == null)
			{
				DisplayMessage(client, LanguageMgr.GetTranslation(client.Account.Language, "Scripts.Players.Autosplit.InGroup"));
				return;
			}

			if (args.Length < 2)
			{
				DisplaySyntax(client);
				return;
			}

			string command = args[1].ToLower();

			// /autosplit master command - handle separately as it takes an optional parameter
			if (command == "master")
			{
				if (client.Player != client.Player.Group.Leader)
				{
					DisplayMessage(client, LanguageMgr.GetTranslation(client.Account.Language, "Scripts.Players.Autosplit.Leader"));
					return;
				}

				// If no name provided, clear master looter
				if (args.Length < 3 || string.IsNullOrWhiteSpace(args[2]))
				{
					client.Player.Group.MasterLooter = null;
					client.Player.Group.SendMessageToGroupMembers(LanguageMgr.GetTranslation(client.Account.Language, "Scripts.Players.Autosplit.MasterLootCleared"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
					return;
				}

				// Find the player in the group by name
				string targetName = args[2];
				GamePlayer targetPlayer = null;
				
				foreach (GamePlayer member in client.Player.Group.GetPlayersInTheGroup())
				{
					if (member.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase))
					{
						targetPlayer = member;
						break;
					}
				}

				if (targetPlayer == null)
				{
					DisplayMessage(client, LanguageMgr.GetTranslation(client.Account.Language, "Scripts.Players.Autosplit.PlayerNotFound", targetName));
					return;
				}

				client.Player.Group.MasterLooter = targetPlayer;
				client.Player.Group.SendMessageToGroupMembers(LanguageMgr.GetTranslation(client.Account.Language, "Scripts.Players.Autosplit.MasterLootSet", targetPlayer.Name), eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}

			// /autosplit for leaders -- Make sure it is the group leader using this command, if it is, execute it.
			if (command == "on" || command == "off" || command == "coins" || command == "loot")
			{
				if (client.Player != client.Player.Group.Leader)
				{
					DisplayMessage(client, LanguageMgr.GetTranslation(client.Account.Language, "Scripts.Players.Autosplit.Leader"));
					return;
				}

				switch (command)
				{
					case "on":
						{
							client.Player.Group.AutosplitLoot = true;
							client.Player.Group.AutosplitCoins = true;
							client.Player.Group.MasterLooter = null;
							client.Player.Group.SendMessageToGroupMembers(LanguageMgr.GetTranslation(client.Account.Language, "Scripts.Players.Autosplit.On"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
							break;
						}

					case "off":
						{
							client.Player.Group.AutosplitLoot = false;
							client.Player.Group.AutosplitCoins = false;
							client.Player.Group.MasterLooter = null;
							client.Player.Group.SendMessageToGroupMembers(LanguageMgr.GetTranslation(client.Account.Language, "Scripts.Players.Autosplit.Off"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
							break;
						}
					case "coins":
						{
							client.Player.Group.AutosplitCoins = !client.Player.Group.AutosplitCoins;
							client.Player.Group.SendMessageToGroupMembers(LanguageMgr.GetTranslation(client.Account.Language, "Scripts.Players.Autosplit.Coins") + (client.Player.Group.AutosplitCoins ? " on" : " off") + " the autosplit coin", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							break;
						}
					case "loot":
						{
							client.Player.Group.AutosplitLoot = !client.Player.Group.AutosplitLoot;
							client.Player.Group.SendMessageToGroupMembers(LanguageMgr.GetTranslation(client.Account.Language, "Scripts.Players.Autosplit.Loot") + (client.Player.Group.AutosplitLoot ? " on" : " off") + " the autosplit loot", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							break;
						}
				}
				return;
			}

			// /autosplit for Members including leader -- 
			if (command == "self")
			{
				client.Player.AutoSplitLoot = !client.Player.AutoSplitLoot;
				client.Player.Group.SendMessageToGroupMembers(LanguageMgr.GetTranslation(client.Account.Language, "Scripts.Players.Autosplit.Self", client.Player.Name) + (client.Player.AutoSplitLoot ? " on" : " off") + " their autosplit loot", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}

			//if nothing matched, then they tried to invent thier own commands -- show syntax
			DisplaySyntax(client);
		}
	}
}