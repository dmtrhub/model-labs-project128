using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel
{
    public class Curve : IdentifiedObject
    {
        private short curveStyle = 0;
        private short xMultiplier = 0;
        private short xUnit = 0;
        private short y1Multiplier = 0;
        private short y1Unit = 0;
        private short y2Multiplier = 0;
        private short y2Unit = 0;
        private short y3Multiplier = 0;
        private short y3Unit = 0;
        private List<long> curveData = new List<long>();

        public Curve(long globalId) : base(globalId) { }

        public short CurveStyle
        {
            get { return curveStyle; }
            set { curveStyle = value; }
        }

        public short XMultiplier
        {
            get { return xMultiplier; }
            set { xMultiplier = value; }
        }

        public short XUnit
        {
            get { return xUnit; }
            set { xUnit = value; }
        }

        public short Y1Multiplier
        {
            get { return y1Multiplier; }
            set { y1Multiplier = value; }
        }

        public short Y1Unit
        {
            get { return y1Unit; }
            set { y1Unit = value; }
        }

        public short Y2Multiplier
        {
            get { return y2Multiplier; }
            set { y2Multiplier = value; }
        }

        public short Y2Unit
        {
            get { return y2Unit; }
            set { y2Unit = value; }
        }

        public short Y3Multiplier
        {
            get { return y3Multiplier; }
            set { y3Multiplier = value; }
        }

        public short Y3Unit
        {
            get { return y3Unit; }
            set { y3Unit = value; }
        }

        public List<long> CurveData
        {
            get { return curveData; }
        }

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.CURVE_CURVESTYLE:
                case ModelCode.CURVE_XMULTIPLIER:
                case ModelCode.CURVE_XUNIT:
                case ModelCode.CURVE_Y1MULTIPLIER:
                case ModelCode.CURVE_Y1UNIT:
                case ModelCode.CURVE_Y2MULTIPLIER:
                case ModelCode.CURVE_Y2UNIT:
                case ModelCode.CURVE_Y3MULTIPLIER:
                case ModelCode.CURVE_Y3UNIT:
                case ModelCode.CURVE_CURVEDATAS:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.CURVE_CURVESTYLE:
                    property.SetValue(curveStyle);
                    break;
                case ModelCode.CURVE_XMULTIPLIER:
                    property.SetValue(xMultiplier);
                    break;
                case ModelCode.CURVE_XUNIT:
                    property.SetValue(xUnit);
                    break;
                case ModelCode.CURVE_Y1MULTIPLIER:
                    property.SetValue(y1Multiplier);
                    break;
                case ModelCode.CURVE_Y1UNIT:
                    property.SetValue(y1Unit);
                    break;
                case ModelCode.CURVE_Y2MULTIPLIER:
                    property.SetValue(y2Multiplier);
                    break;
                case ModelCode.CURVE_Y2UNIT:
                    property.SetValue(y2Unit);
                    break;
                case ModelCode.CURVE_Y3MULTIPLIER:
                    property.SetValue(y3Multiplier);
                    break;
                case ModelCode.CURVE_Y3UNIT:
                    property.SetValue(y3Unit);
                    break;
                case ModelCode.CURVE_CURVEDATAS:
                    property.SetValue(curveData);
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
                case ModelCode.CURVE_CURVESTYLE:
                    curveStyle = property.AsEnum();
                    break;
                case ModelCode.CURVE_XMULTIPLIER:
                    xMultiplier = property.AsEnum();
                    break;
                case ModelCode.CURVE_XUNIT:
                    xUnit = property.AsEnum();
                    break;
                case ModelCode.CURVE_Y1MULTIPLIER:
                    y1Multiplier = property.AsEnum();
                    break;
                case ModelCode.CURVE_Y1UNIT:
                    y1Unit = property.AsEnum();
                    break;
                case ModelCode.CURVE_Y2MULTIPLIER:
                    y2Multiplier = property.AsEnum();
                    break;
                case ModelCode.CURVE_Y2UNIT:
                    y2Unit = property.AsEnum();
                    break;
                case ModelCode.CURVE_Y3MULTIPLIER:
                    y3Multiplier = property.AsEnum();
                    break;
                case ModelCode.CURVE_Y3UNIT:
                    y3Unit = property.AsEnum();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        public override bool IsReferenced
        {
            get { return curveData.Count > 0 || base.IsReferenced; }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (curveData != null && curveData.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.CURVE_CURVEDATAS] = curveData.GetRange(0, curveData.Count);
            }
            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.CURVE_CURVEDATAS:
                case ModelCode.CURVEDATA_CURVE:  // Child side ModelCode
                    curveData.Add(globalId);
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
                case ModelCode.CURVE_CURVEDATAS:
                case ModelCode.CURVEDATA_CURVE:  // Child side ModelCode
                    curveData.Remove(globalId);
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

            Curve other = (Curve)obj;

            if (curveStyle != other.curveStyle)
                return false;
            if (xMultiplier != other.xMultiplier)
                return false;
            if (xUnit != other.xUnit)
                return false;
            if (y1Multiplier != other.y1Multiplier)
                return false;
            if (y1Unit != other.y1Unit)
                return false;
            if (y2Multiplier != other.y2Multiplier)
                return false;
            if (y2Unit != other.y2Unit)
                return false;
            if (y3Multiplier != other.y3Multiplier)
                return false;
            if (y3Unit != other.y3Unit)
                return false;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}