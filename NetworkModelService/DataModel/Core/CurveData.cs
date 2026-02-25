using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel
{
    public class CurveData : IdentifiedObject
    {
        private float xvalue = 0;
        private float y1value = 0;
        private float y2value = 0;
        private float y3value = 0;
        private long curveGID = 0;

        public CurveData(long globalId) : base(globalId) { }

        public float Xvalue
        {
            get { return xvalue; }
            set { xvalue = value; }
        }

        public float Y1value
        {
            get { return y1value; }
            set { y1value = value; }
        }

        public float Y2value
        {
            get { return y2value; }
            set { y2value = value; }
        }

        public float Y3value
        {
            get { return y3value; }
            set { y3value = value; }
        }

        public long CurveGID
        {
            get { return curveGID; }
            set { curveGID = value; }
        }

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.CURVEDATA_XVALUE:
                case ModelCode.CURVEDATA_Y1VALUE:
                case ModelCode.CURVEDATA_Y2VALUE:
                case ModelCode.CURVEDATA_Y3VALUE:
                case ModelCode.CURVEDATA_CURVE:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.CURVEDATA_XVALUE:
                    property.SetValue(xvalue);
                    break;
                case ModelCode.CURVEDATA_Y1VALUE:
                    property.SetValue(y1value);
                    break;
                case ModelCode.CURVEDATA_Y2VALUE:
                    property.SetValue(y2value);
                    break;
                case ModelCode.CURVEDATA_Y3VALUE:
                    property.SetValue(y3value);
                    break;
                case ModelCode.CURVEDATA_CURVE:
                    property.SetValue(curveGID);
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
                case ModelCode.CURVEDATA_XVALUE:
                    xvalue = property.AsFloat();
                    break;
                case ModelCode.CURVEDATA_Y1VALUE:
                    y1value = property.AsFloat();
                    break;
                case ModelCode.CURVEDATA_Y2VALUE:
                    y2value = property.AsFloat();
                    break;
                case ModelCode.CURVEDATA_Y3VALUE:
                    y3value = property.AsFloat();
                    break;
                case ModelCode.CURVEDATA_CURVE:
                    curveGID = property.AsReference();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        //public override bool IsReferenced
        //{
        //    get { return curveGID != 0 || base.IsReferenced; }
        //}

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if ((refType & TypeOfReference.Reference) != 0)
            {
                if (curveGID != 0)
                    references[ModelCode.CURVEDATA_CURVE] = new List<long>() { curveGID };
            }
            base.GetReferences(references, refType);
        }

        //public override void AddReference(ModelCode referenceId, long globalId)
        //{
        //    switch (referenceId)
        //    {
        //        case ModelCode.CURVEDATA_CURVE:
        //            curveGID = globalId;
        //            break;
        //        default:
        //            base.AddReference(referenceId, globalId);
        //            break;
        //    }
        //}

        //public override void RemoveReference(ModelCode referenceId, long globalId)
        //{
        //    switch (referenceId)
        //    {
        //        case ModelCode.CURVEDATA_CURVE:
        //            curveGID = 0;
        //            break;
        //        default:
        //            base.RemoveReference(referenceId, globalId);
        //            break;
        //    }
        //}

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            CurveData other = (CurveData)obj;

            if (xvalue != other.xvalue)
                return false;
            if (y1value != other.y1value)
                return false;
            if (y2value != other.y2value)
                return false;
            if (y3value != other.y3value)
                return false;
            if (curveGID != other.curveGID)
                return false;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}