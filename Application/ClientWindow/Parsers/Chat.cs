using Domen.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ClientWindow.Parsers
{
    public class Chat
    {
        ClientParams _clientParams;

        public Chat(ClientParams clientParams)
        {
            _clientParams = clientParams;
        }
        public List<ChatPlayer> GetInfo()
        {
            var Persons = UITreeReader.GetUITrees(_clientParams, "ChatWindowStack");
            if (Persons == null)
                return new List<ChatPlayer>();

            Persons = Persons.FindEntityOfString("XmppChatSimpleUserEntry");
            if (Persons == null)
                return new List<ChatPlayer>();
            var PersonsEntry = Persons.handleEntity("XmppChatSimpleUserEntry");

            List<ChatPlayer> ChatInfo = new List<ChatPlayer>();
            for (int i = 0; i < PersonsEntry.children.Length; i++)
            {
                if (PersonsEntry.children[i] == null)
                    continue;
                if (PersonsEntry.children[i].children == null)
                    continue;
                if (PersonsEntry.children[i].children.Length < 3)
                    continue;
                if (PersonsEntry.children[i].children[2] == null)
                    continue;
                if (PersonsEntry.children[i].children[2].children == null)
                    continue;
                if (PersonsEntry.children[i].children[2].children.Length == 0)
                    continue;

                if (PersonsEntry.children[i].children[2].children[0].pythonObjectTypeName != "FlagIconWithState")
                    continue;

                ChatPlayer ChatPlayerInfo = new ChatPlayer();

                ChatPlayerInfo.PlayerType = PersonsEntry.children[i].children[2].children[0]
                .dictEntriesOfInterest["_hint"].ToString();

                ChatInfo.Add(ChatPlayerInfo);
            }
            return ChatInfo;
            //Pilot is a criminal
            //Pilot is a suspect
            //FlagIconWithState
        }
    }
}
