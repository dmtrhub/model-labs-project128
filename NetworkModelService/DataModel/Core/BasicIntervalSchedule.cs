using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel
{
    public abstract class BasicIntervalSchedule : IdentifiedObject
    {
        private string startTime = string.Empty;
        private short value1Multiplier = 0;
        private short value1Unit = 0;
        private short value2Multiplier = 0;
        private short value2Unit = 0;

        public BasicIntervalSchedule(long globalId) : base(globalId) { }

        public string StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        public short Value1Multiplier
        {
            get { return value1Multiplier; }
            set { value1Multiplier = value; }
        }

        public short Value1Unit
        {
            get { return value1Unit; }
            set { value1Unit = value; }
        }

        public short Value2Multiplier
        {
            get { return value2Multiplier; }
            set { value2Multiplier = value; }
        }

        public short Value2Unit
        {
            get { return value2Unit; }
            set { value2Unit = value; }
        }

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.BASICINTSCHEDULE_STARTTIME:
                case ModelCode.BASICINTSCHEDULE_VALUE1MULTIPLIER:
                case ModelCode.BASICINTSCHEDULE_VALUE1UNIT:
                case ModelCode.BASICINTSCHEDULE_VALUE2MULTIPLIER:
                case ModelCode.BASICINTSCHEDULE_VALUE2UNIT:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.BASICINTSCHEDULE_STARTTIME:
                    property.SetValue(startTime);
                    break;
                case ModelCode.BASICINTSCHEDULE_VALUE1MULTIPLIER:
                    property.SetValue(value1Multiplier);
                    break;
                case ModelCode.BASICINTSCHEDULE_VALUE1UNIT:
                    property.SetValue(value1Unit);
                    break;
                case ModelCode.BASICINTSCHEDULE_VALUE2MULTIPLIER:
                    property.SetValue(value2Multiplier);
                    break;
                case ModelCode.BASICINTSCHEDULE_VALUE2UNIT:
                    property.SetValue(value2Unit);
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
                case ModelCode.BASICINTSCHEDULE_STARTTIME:
                    startTime = property.AsString();
                    break;
                case ModelCode.BASICINTSCHEDULE_VALUE1MULTIPLIER:
                    value1Multiplier = property.AsEnum();
                    break;
                case ModelCode.BASICINTSCHEDULE_VALUE1UNIT:
                    value1Unit = property.AsEnum();
                    break;
                case ModelCode.BASICINTSCHEDULE_VALUE2MULTIPLIER:
                    value2Multiplier = property.AsEnum();
                    break;
                case ModelCode.BASICINTSCHEDULE_VALUE2UNIT:
                    value2Unit = property.AsEnum();
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

            BasicIntervalSchedule other = (BasicIntervalSchedule)obj;

            if (startTime != other.startTime) return false;
            if (value1Multiplier != other.value1Multiplier) return false;
            if (value1Unit != other.value1Unit) return false;
            if (value2Multiplier != other.value2Multiplier) return false;
            if (value2Unit != other.value2Unit) return false;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
