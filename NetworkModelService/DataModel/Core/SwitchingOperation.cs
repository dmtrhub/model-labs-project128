using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FTN.Services.NetworkModelService.DataModel
{
    public class SwitchingOperation : IdentifiedObject
    {
        private short newState = 0;
        private string operationTime = string.Empty;
        private long outageScheduleGID = 0;
        private List<long> switches = new List<long>();

        public SwitchingOperation(long globalId) : base(globalId) { }

        public short NewState
        {
            get { return newState; }
            set { newState = value; }
        }

        public string OperationTime
        {
            get { return operationTime; }
            set { operationTime = value; }
        }

        public long OutageScheduleGID
        {
            get { return outageScheduleGID; }
            set { outageScheduleGID = value; }
        }

        public List<long> Switches
        {
            get { return switches; }
        }

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.SWITCHINGOPERATION_NEWSTATE:
                case ModelCode.SWITCHINGOPERATION_OPERATIONTIME:
                case ModelCode.SWITCHINGOPERATION_OUTAGESCHEDULE:
                case ModelCode.SWITCHINGOPERATION_SWITCHES:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.SWITCHINGOPERATION_NEWSTATE:
                    property.SetValue(newState);
                    break;
                case ModelCode.SWITCHINGOPERATION_OPERATIONTIME:
                    property.SetValue(operationTime);
                    break;
                case ModelCode.SWITCHINGOPERATION_OUTAGESCHEDULE:
                    property.SetValue(outageScheduleGID);
                    break;
                case ModelCode.SWITCHINGOPERATION_SWITCHES:
                    property.SetValue(switches);
                    break;
                default:
                    base.GetProperty(property);
                    break;
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.SWITCHINGOPERATION_NEWSTATE:
                    newState = property.AsEnum();
                    break;
                case ModelCode.SWITCHINGOPERATION_OPERATIONTIME:
                    operationTime = property.AsString();
                    break;
                case ModelCode.SWITCHINGOPERATION_OUTAGESCHEDULE:
                    outageScheduleGID = property.AsReference();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        public override bool IsReferenced
        {
            get { return switches.Count > 0 || base.IsReferenced; }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (outageScheduleGID != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.SWITCHINGOPERATION_OUTAGESCHEDULE] = new List<long>() { outageScheduleGID };
            }
            if (switches != null && switches.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.SWITCHINGOPERATION_SWITCHES] = switches.GetRange(0, switches.Count);
            }
            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.OUTAGESCHEDULE_SWITCHINGOPERATIONS:
                    outageScheduleGID = globalId;
                    break;
                case ModelCode.SWITCH_SWITCHINGOPERATIONS:
                    switches.Add(globalId);
                    break;
                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.OUTAGESCHEDULE_SWITCHINGOPERATIONS:
                    outageScheduleGID = 0;
                    break;
                case ModelCode.SWITCH_SWITCHINGOPERATIONS:
                    switches.Remove(globalId);
                    break;
                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null) return false;
            if (GetType() != obj.GetType()) return false;
            SwitchingOperation other = (SwitchingOperation)obj;
            if (newState != other.newState) return false;
            if (operationTime != other.operationTime) return false;
            if (outageScheduleGID != other.outageScheduleGID) return false;
            if (!switches.SequenceEqual(other.switches)) return false;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
