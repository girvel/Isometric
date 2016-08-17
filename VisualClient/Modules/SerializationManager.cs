﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GameCore.Modules.WorldModule;
using VisualServer;
using GameCore.Modules;
using CustomProperty.Dynamic;

namespace VisualClient.Modules
{
    public class SerializationManager
    {
        #region Singleton-part

        [Obsolete("using backing field")]
        private static SerializationManager _instance;

        #pragma warning disable 618

        public static SerializationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SerializationManager();
                }

                return _instance;
            }

            set
            {
                #if DEBUG

                if (_instance != null)
                {
                    throw new ArgumentException("Instance is already set");
                }

                #endif

                _instance = value;
            }
        }

        #pragma warning restore 618

        #endregion



        public string SavingDirectory { get; set; } = "saves";
        public string SavingFile { get; set; } = "game-full-save";

        public uint SavingPeriodMilliseconds = 60000;



        public static event Action OnSuccessfulSaving;
        public static event Action<Exception> OnSavingException;

        public static event Action OnSuccessfulOpening;
        public static event Action<Exception> OnOpeningException;



        public delegate object Accessor();
        public delegate void Mutator(object value);

        public Property[] SerializationList { get; set; } = 
            new Property[] 
            {
                new Property(
                    @get: () => Core.Version,
                    @set: value => Core.SavingVersion = value),

                new Property(
                    @get: () => SingleServer.Instance,
                    @set: value => SingleServer.Instance = value),

                new Property(
                    @get: () => World.Instance,
                    @set: value => World.Instance = value),

                new Property(
                    @get: () => Instance.SavingPeriodMilliseconds,
                    @set: value => Instance.SavingPeriodMilliseconds = value),
            };





        public bool TrySave()
        {
            try
            {
                if (!Directory.Exists(SavingDirectory))
                {
                    Directory.CreateDirectory(SavingDirectory);
                }

                using (FileStream mainStream = File.OpenWrite($"{SavingDirectory}/{SavingFile}"))
                {
                    var serializer = new BinaryFormatter();

                    foreach (var property in SerializationList)
                    { 
                        serializer.Serialize(mainStream, property.Get());
                    }

                    OnSuccessfulSaving?.Invoke();

                    return true;
                }
            }
            catch (Exception ex)
            {
                OnSavingException?.Invoke(ex);

                #if DEBUG
                throw;
                #else
                return false;
                #endif
            }
        } 

        public bool TryOpen()
        {
            try
            {
                if (!File.Exists($"{SavingDirectory}/{SavingFile}"))
                {
                    return false;
                }

                using (FileStream mainStream = File.OpenRead(
                    $"{SavingDirectory}/{SavingFile}"))
                {
                    var serializer = new BinaryFormatter();

                    foreach (var property in SerializationList)
                    {
                        property.Set(serializer.Deserialize(mainStream));
                    }
                }
                OnSuccessfulOpening?.Invoke();

                return true;
            }
            catch (Exception ex)
            {
                OnOpeningException?.Invoke(ex);

                #if DEBUG
                throw;
                #else
                return false;
                #endif
            }
        }
    }
}
