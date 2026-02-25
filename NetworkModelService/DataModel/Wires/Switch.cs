using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel
{
    public class Switch : ConductingEquipment
    {
        private long switchingOperationGID = 0;

        public Switch(long globalId) : base(globalId) { }

        public long SwitchingOperationGID
        {
            get { return switchingOperationGID; }
            set { switchingOperationGID = value; }
        }

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.SWITCH_SWITCHINGOPERATIONS:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.SWITCH_SWITCHINGOPERATIONS:
                    property.SetValue(switchingOperationGID);
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
                case ModelCode.SWITCH_SWITCHINGOPERATIONS:
                    switchingOperationGID = property.AsReference();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        public override bool IsReferenced
        {
            get { return base.IsReferenced; }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (switchingOperationGID != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.SWITCH_SWITCHINGOPERATIONS] = new List<long>() { switchingOperationGID };
            }
            base.GetReferences(references, refType);
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null) return false;
            if (GetType() != obj.GetType()) return false;
            Switch other = (Switch)obj;
            if (switchingOperationGID != other.switchingOperationGID) return false;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}