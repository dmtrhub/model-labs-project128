using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel
{
    public class RegularTimePoint : IdentifiedObject
    {
        private int sequenceNumber = 0;
        private float value1 = 0;
        private float value2 = 0;
        private long intervalScheduleGID = 0;

        public RegularTimePoint(long globalId) : base(globalId) { }

        public int SequenceNumber
        {
            get { return sequenceNumber; }
            set { sequenceNumber = value; }
        }

        public float Value1
        {
            get { return value1; }
            set { value1 = value; }
        }

        public float Value2
        {
            get { return value2; }
            set { value2 = value; }
        }

        public long IntervalScheduleGID
        {
            get { return intervalScheduleGID; }
            set { intervalScheduleGID = value; }
        }

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.REGULARTIMEPOINT_SEQUENCENUMBER:
                case ModelCode.REGULARTIMEPOINT_VALUE1:
                case ModelCode.REGULARTIMEPOINT_VALUE2:
                case ModelCode.REGULARTIMEPOINT_INTERVALSCHEDULE:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.REGULARTIMEPOINT_SEQUENCENUMBER:
                    property.SetValue(sequenceNumber);
                    break;
                case ModelCode.REGULARTIMEPOINT_VALUE1:
                    property.SetValue(value1);
                    break;
                case ModelCode.REGULARTIMEPOINT_VALUE2:
                    property.SetValue(value2);
                    break;
                case ModelCode.REGULARTIMEPOINT_INTERVALSCHEDULE:
                    property.SetValue(intervalScheduleGID);
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
                case ModelCode.REGULARTIMEPOINT_SEQUENCENUMBER:
                    sequenceNumber = property.AsInt();
                    break;
                case ModelCode.REGULARTIMEPOINT_VALUE1:
                    value1 = property.AsFloat();
                    break;
                case ModelCode.REGULARTIMEPOINT_VALUE2:
                    value2 = property.AsFloat();
                    break;
                case ModelCode.REGULARTIMEPOINT_INTERVALSCHEDULE:
                    intervalScheduleGID = property.AsReference();
                    break;
                case ModelCode.BASICINTSCHEDULE_STARTTIME:
                case ModelCode.BASICINTSCHEDULE_VALUE1MULTIPLIER:
                case ModelCode.BASICINTSCHEDULE_VALUE1UNIT:
                case ModelCode.BASICINTSCHEDULE_VALUE2MULTIPLIER:
                case ModelCode.BASICINTSCHEDULE_VALUE2UNIT:
                case ModelCode.REGULARINTSCHEDULE_ENDTIME:
                case ModelCode.REGULARINTSCHEDULE_TIMESTEP:
                    // Ancestor properties - RegularTimePoint ih ne ?uva
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        public override bool IsReferenced
        {
            get { return intervalScheduleGID != 0 || base.IsReferenced; }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            // TimePoint je child entitet - ne vra?a target references
            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.REGULARTIMEPOINT_INTERVALSCHEDULE:
                    intervalScheduleGID = globalId;
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
                case ModelCode.REGULARTIMEPOINT_INTERVALSCHEDULE:
                    intervalScheduleGID = 0;
                    break;
                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            RegularTimePoint other = (RegularTimePoint)obj;

            if (sequenceNumber != other.sequenceNumber)
                return false;
            if (value1 != other.value1)
                return false;
            if (value2 != other.value2)
                return false;
            if (intervalScheduleGID != other.intervalScheduleGID)
                return false;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}