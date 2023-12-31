﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKanave.DB;
using VKanave.Networking.NetObjects;
using VKanaveServer;
using VKanaveServer.Core;

namespace VKanave.Networking.NetMessages
{
    public class NMChatMessage : NMAction
    {
        public NMChatMessage()
        {
            TokenRequired = true;
        }

        public override void Action(Connection from)
        {
            base.Action(from);
            int sqlFlags = SystemMessage ? (int)ChatMessageFlags.SYSTEM : (int)ChatMessageFlags.UNREAD;
            Database.RunCommand($"INSERT INTO `messages` (`id_from`, `id_to`, `date`, `content`, `flags`) VALUES ('{id_from}', '{id_to}', '{Database.GetUnixTime()}', '{ChatMessage.Content}', '{sqlFlags}');", out var tb1);

            this.ChatMessage.ID = tb1.LastInsertedId;

            Program.Log(LogType.Console, $"Chat message from {id_from} to {id_to}. ID: {this.ChatMessage.ID}");

            this.ChatMessage.Flags = (int)(ChatMessageFlags.OUTBOX | ChatMessageFlags.UNREAD);
            if (SystemMessage)
                this.ChatMessage.Flags = (int)(ChatMessageFlags.SYSTEM);
            from.Send(new NMChatMessage()
            {
                ChatMessage = this.ChatMessage,
                id_to = this.id_to,
                id_from = this.id_from
            });
            Connection? connectionTo = Server.Current?.GetConnectionFromUserId(id_to);
            if (connectionTo != null)
            {
                this.ChatMessage.Flags = (int)(ChatMessageFlags.UNREAD);
                if (SystemMessage)
                    this.ChatMessage.Flags = (int)(ChatMessageFlags.SYSTEM);
                connectionTo.Send(new NMChatMessage()
                {
                    ChatMessage = this.ChatMessage,
                    id_to = this.id_to,
                    id_from = this.id_from
                });
            }
        }

        protected override void OnSerialize()
        {
            Write(ChatMessage);
        }

        protected override void OnDeserialize()
        {
            ChatMessage = ReadChatMessage();
        }

        public ChatMessage ChatMessage
        {
            get; set;
        }

        public bool SystemMessage
        {
            get; set;
        }

        public long id_to, id_from;

    }
}
