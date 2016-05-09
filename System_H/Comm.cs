﻿using HSFSystem;
using HSFUniverse;
using MissionElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Utilities;

namespace HSFSubsystem
{
    public class Comm : Subsystem
    {
        #region Attributes
        public static StateVarKey<double> DATARATE_KEY;
        #endregion

        #region Constructors
        public Comm(XmlNode CommXmlNode, Dependencies dependencies, Asset asset)
        {
            DefaultSubName = "Comm";
            Asset = asset;
            getSubNameFromXmlNode(CommXmlNode);
            SubsystemDependencyFunctions = new Dictionary<string, Delegate>();
            DependentSubsystems = new List<ISubsystem>();
            DATARATE_KEY = new StateVarKey<double>(Asset.Name + "." + "DataRate(MB/s)");
            addKey(DATARATE_KEY);
            dependencies.Add("PowerfromComm", new Func<SystemState, HSFProfile<double>>(POWERSUB_PowerProfile_COMMSUB));
        }
        #endregion

        #region Methods
        public override bool canPerform(SystemState oldState, SystemState newState,
                            Dictionary<Asset, Task> tasks, Universe environment)
        {
            if(!base.canPerform(oldState, newState, tasks, environment))
                return false;
            if (_task.Type == TaskType.COMM)
            {
                HSFProfile<double> newProf = DependencyCollector(newState);
                if (!newProf.Empty())
                    newState.setProfile(DATARATE_KEY, newProf);
            }
            return true;
        }

        HSFProfile<double> POWERSUB_PowerProfile_COMMSUB(SystemState state)
        {
            return state.GetProfile(DATARATE_KEY) * 20;
        }
        #endregion
    }
}