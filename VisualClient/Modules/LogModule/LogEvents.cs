﻿using System;
using CommonStructures;
using VisualServer;

namespace VisualClient.Modules.LogModule
{
    internal static class LogEvents
    {
        internal static void Init()
        {
            SingleServer.Instance.OnAcceptedConnection += _onAcceptedConnection;
            SingleServer.Instance.OnLoginAttempt += _onLoginAttempt;

            Connection.OnConnectionEnd += _onConnectionEnd;
            Connection.OnDataReceived += _onDataReceived;
            Connection.OnWrongCommand += _onWrongCommand;

            SerializationManager.OnSuccessfulSaving += () => _onSuccessfulAction("Saved");
            SerializationManager.OnSuccessfulOpening += () => _onSuccessfulAction("Opened");

            SerializationManager.OnSavingException += 
                ex => _onSerializationException("Saving", ex);
            SerializationManager.OnOpeningException +=
                ex => _onSerializationException("Opening", ex);
        }

        #region SingleServer

        private static void _onAcceptedConnection(object sender, EventArgs args)
        {
            Log.Instance.Write("Connection accepted");
        }

        private static void _onLoginAttempt(object sender, Server.LoginArgs args)
        {
            var message = string.Empty;

            switch (args.Result)
            {
                case LoginResult.Successful:
                    message = "Successful attempt to enter";
                    break;

                case LoginResult.Unsuccessful:
                    message = "Unsuccessful attempt to enter";
                    break;

                case LoginResult.Banned:
                    message = "Attempt to enter by banned account";
                    break;

                default:
                    message = "Unknown LoginResult";
                    break;
            }

            Log.Instance.Write(message + $"\n\tEmail: {args.Email}");
        }

        #endregion

        #region Connection

        private static void _onConnectionEnd(object sender, Connection.ConnectionArgs args)
        {
            Log.Instance.Write("Connection end. Login: " + args.Connection.Account.Login);
        }

        private static void _onDataReceived(object sender, Connection.DataArgs args)
        {
            Log.Instance.Write("Data received: " + args.Data);
        }

        private static void _onWrongCommand(object sender, Connection.DataArgs args)
        {
            Log.Instance.Write("Wrong command received: " + args.Data);
        }
    
        #endregion

        #region SerializationManager

        private static void _onSuccessfulAction(string action)
        {
            Log.Instance.Write($"{action} successful");
        }

        private static void _onSerializationException(string process, Exception exception)
        {
            Log.Instance.Exception(exception, "{process} exception was catched");
        }

        #endregion
    }
}

