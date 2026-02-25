using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel
{
    public class RegularIntervalSchedule : BasicIntervalSchedule
    {
        private string endTime = string.Empty;
        private float timeStep = 0;
        private List<long> timePoints = new List<long>();

        public RegularIntervalSchedule(long globalId) : base(globalId) { }

        public string EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        public float TimeStep
        {
            get { return timeStep; }
            set { timeStep = value; }
        }

        public List<long> TimePoints
        {
            get { return timePoints; }
        }

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.REGULARINTSCHEDULE_ENDTIME:
                case ModelCode.REGULARINTSCHEDULE_TIMESTEP:
                case ModelCode.REGULARINTSCHEDULE_TIMEPOINTS:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.REGULARINTSCHEDULE_ENDTIME:
                    property.SetValue(endTime);
                    break;
                case ModelCode.REGULARINTSCHEDULE_TIMESTEP:
                    property.SetValue(timeStep);
                    break;
                case ModelCode.REGULARINTSCHEDULE_TIMEPOINTS:
                    property.SetValue(timePoints);
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
                case ModelCode.REGULARINTSCHEDULE_ENDTIME:
                    endTime = property.AsString();
                    break;
                case ModelCode.REGULARINTSCHEDULE_TIMESTEP:
                    timeStep = property.AsFloat();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        public override bool IsReferenced
        {
            get { return timePoints.Count > 0 || base.IsReferenced; }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (timePoints != null && timePoints.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.REGULARINTSCHEDULE_TIMEPOINTS] = timePoints.GetRange(0, timePoints.Count);
            }
            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.REGULARINTSCHEDULE_TIMEPOINTS:
                case ModelCode.REGULARTIMEPOINT_INTERVALSCHEDULE:  // Child side ModelCode
                    timePoints.Add(globalId);
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
                case ModelCode.REGULARINTSCHEDULE_TIMEPOINTS:
                case ModelCode.REGULARTIMEPOINT_INTERVALSCHEDULE:  // Child side ModelCode
                    timePoints.Remove(globalId);
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

            RegularIntervalSchedule other = (RegularIntervalSchedule)obj;

            if (endTime != other.endTime)
                return false;
            if (timeStep != other.timeStep)
                return false;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}