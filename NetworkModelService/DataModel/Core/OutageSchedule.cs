using System;
using System.Collections.Generic;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService.DataModel
{
    public class OutageSchedule : IrregularIntervalSchedule
    {
        private List<long> switchingOperations = new List<long>();
        private long powerSystemResourceGID = 0;

        public OutageSchedule(long globalId) : base(globalId) { }

        public List<long> SwitchingOperations
        {
            get { return switchingOperations; }
        }

        public long PowerSystemResourceGID
        {
            get { return powerSystemResourceGID; }
            set { powerSystemResourceGID = value; }
        }

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.OUTAGESCHEDULE_SWITCHINGOPERATIONS:
                case ModelCode.OUTAGESCHEDULE_POWERSYSTEMRESOURCE:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.OUTAGESCHEDULE_SWITCHINGOPERATIONS:
                    property.SetValue(switchingOperations);
                    break;
                case ModelCode.OUTAGESCHEDULE_POWERSYSTEMRESOURCE:
                    property.SetValue(powerSystemResourceGID);
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
                case ModelCode.OUTAGESCHEDULE_POWERSYSTEMRESOURCE:
                    powerSystemResourceGID = property.AsReference();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null) return false;
            if (GetType() != obj.GetType()) return false;
            OutageSchedule other = (OutageSchedule)obj;
            if (powerSystemResourceGID != other.powerSystemResourceGID) return false;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool IsReferenced
        {
            get { return switchingOperations.Count > 0 || base.IsReferenced; }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (switchingOperations != null && switchingOperations.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.OUTAGESCHEDULE_SWITCHINGOPERATIONS] = switchingOperations.GetRange(0, switchingOperations.Count);
            }
            if (powerSystemResourceGID != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.OUTAGESCHEDULE_POWERSYSTEMRESOURCE] = new List<long>() { powerSystemResourceGID };
            }
            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.SWITCHINGOPERATION_OUTAGESCHEDULE:
                    switchingOperations.Add(globalId);
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
                case ModelCode.SWITCHINGOPERATION_OUTAGESCHEDULE:
                    switchingOperations.Remove(globalId);
                    break;
                case ModelCode.OUTAGESCHEDULE_POWERSYSTEMRESOURCE:
                    powerSystemResourceGID = 0;
                    break;
                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }
    }
}