﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKanave.Views;

namespace VKanave.Networking.NetMessages
{
    [Serializable]
    public class NMAuth : NMAction
    {
        internal NMAuth(string username, string password)
        {
            this.username = username;
            this.password = password;
            token = string.Empty;
            displayName = string.Empty;
            bio = string.Empty;
        }

        public NMAuth()
        {
            username = string.Empty;
            password = string.Empty;
            token = string.Empty;
            displayName = string.Empty;
            bio = string.Empty;
        }

        public override void Action(Connection connection)
        {
            if (LoginPage.Current != null)
            {
                LoginPage.Current.SignIn(username, token, id, reg, displayName, bio);
            }
        }

        public string username, password, token, displayName, bio;
        public long id;
        public int reg;

    }
}
