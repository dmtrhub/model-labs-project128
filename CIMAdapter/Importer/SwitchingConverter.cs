using System;
using System.Collections.Generic;
using FTN.Common;
using FTN.ESI.SIMES.CIM.CIMAdapter.Manager;
using FTN.ServiceContracts;

namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
    /// <summary>
    /// SwitchingConverter - konvertuje CIM Switching profile objekte u NMS Delta
    /// </summary>
    public static class SwitchingConverter
    {
        #region Populate ResourceDescription

        // ===== CURVE =====
        public static void PopulateCurveProperties(FTN.Curve cimCurve, ResourceDescription rd)
        {
            if ((cimCurve != null) && (rd != null))
            {
                if (cimCurve.NameHasValue)
                    rd.AddProperty(new Property(ModelCode.IDOBJ_NAME, cimCurve.Name));

                if (cimCurve.MRIDHasValue)
                    rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, cimCurve.MRID));

                if (cimCurve.CurveStyleHasValue)
                    rd.AddProperty(new Property(ModelCode.CURVE_CURVESTYLE, (short)cimCurve.CurveStyle));

                if (cimCurve.XMultiplierHasValue)
                    rd.AddProperty(new Property(ModelCode.CURVE_XMULTIPLIER, (short)cimCurve.XMultiplier));

                if (cimCurve.XUnitHasValue)
                    rd.AddProperty(new Property(ModelCode.CURVE_XUNIT, (short)cimCurve.XUnit));

                if (cimCurve.Y1MultiplierHasValue)
                    rd.AddProperty(new Property(ModelCode.CURVE_Y1MULTIPLIER, (short)cimCurve.Y1Multiplier));

                if (cimCurve.Y1UnitHasValue)
                    rd.AddProperty(new Property(ModelCode.CURVE_Y1UNIT, (short)cimCurve.Y1Unit));

                if (cimCurve.Y2MultiplierHasValue)
                    rd.AddProperty(new Property(ModelCode.CURVE_Y2MULTIPLIER, (short)cimCurve.Y2Multiplier));

                if (cimCurve.Y2UnitHasValue)
                    rd.AddProperty(new Property(ModelCode.CURVE_Y2UNIT, (short)cimCurve.Y2Unit));

                if (cimCurve.Y3MultiplierHasValue)
                    rd.AddProperty(new Property(ModelCode.CURVE_Y3MULTIPLIER, (short)cimCurve.Y3Multiplier));

                if (cimCurve.Y3UnitHasValue)
                    rd.AddProperty(new Property(ModelCode.CURVE_Y3UNIT, (short)cimCurve.Y3Unit));
            }
        }

        // ===== CURVEDATA =====
        public static void PopulateCurveDataProperties(FTN.CurveData cimCurveData, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimCurveData != null) && (rd != null))
            {
                if (cimCurveData.NameHasValue)
                    rd.AddProperty(new Property(ModelCode.IDOBJ_NAME, cimCurveData.Name));

                if (cimCurveData.MRIDHasValue)
                    rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, cimCurveData.MRID));

                if (cimCurveData.XvalueHasValue)
                    rd.AddProperty(new Property(ModelCode.CURVEDATA_XVALUE, cimCurveData.Xvalue));

                if (cimCurveData.Y1valueHasValue)
                    rd.AddProperty(new Property(ModelCode.CURVEDATA_Y1VALUE, cimCurveData.Y1value));

                if (cimCurveData.Y2valueHasValue)
                    rd.AddProperty(new Property(ModelCode.CURVEDATA_Y2VALUE, cimCurveData.Y2value));

                if (cimCurveData.Y3valueHasValue)
                    rd.AddProperty(new Property(ModelCode.CURVEDATA_Y3VALUE, cimCurveData.Y3value));

                if (cimCurveData.CurveHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimCurveData.Curve.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(cimCurveData.GetType().ToString()).Append(" rdfID = \"").Append(cimCurveData.ID);
                        report.Report.Append("\" - Failed to set reference to Curve: rdfID \"").Append(cimCurveData.Curve.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.CURVEDATA_CURVE, gid));
                }
            }
        }

        // ===== REGULARINTERVALSCHEDULE =====
        public static void PopulateRegularIntervalScheduleProperties(FTN.RegularIntervalSchedule cimSchedule, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimSchedule != null) && (rd != null))
            {
                if (cimSchedule.NameHasValue)
                    rd.AddProperty(new Property(ModelCode.IDOBJ_NAME, cimSchedule.Name));

                if (cimSchedule.MRIDHasValue)
                    rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, cimSchedule.MRID));

                if (cimSchedule.StartTimeHasValue)
                    rd.AddProperty(new Property(ModelCode.BASICINTSCHEDULE_STARTTIME, cimSchedule.StartTime.ToString()));

                if (cimSchedule.EndTimeHasValue)
                    rd.AddProperty(new Property(ModelCode.REGULARINTSCHEDULE_ENDTIME, cimSchedule.EndTime.ToString()));

                if (cimSchedule.TimeStepHasValue)
                    rd.AddProperty(new Property(ModelCode.REGULARINTSCHEDULE_TIMESTEP, cimSchedule.TimeStep));
            }
        }

        // ===== REGULARTIMEPOINT =====
        public static void PopulateRegularTimePointProperties(FTN.RegularTimePoint cimTimePoint, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimTimePoint != null) && (rd != null))
            {
                if (cimTimePoint.NameHasValue)
                    rd.AddProperty(new Property(ModelCode.IDOBJ_NAME, cimTimePoint.Name));

                if (cimTimePoint.MRIDHasValue)
                    rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, cimTimePoint.MRID));

                if (cimTimePoint.SequenceNumberHasValue)
                    rd.AddProperty(new Property(ModelCode.REGULARTIMEPOINT_SEQUENCENUMBER, cimTimePoint.SequenceNumber));

                if (cimTimePoint.Value1HasValue)
                    rd.AddProperty(new Property(ModelCode.REGULARTIMEPOINT_VALUE1, cimTimePoint.Value1));

                if (cimTimePoint.Value2HasValue)
                    rd.AddProperty(new Property(ModelCode.REGULARTIMEPOINT_VALUE2, cimTimePoint.Value2));

                if (cimTimePoint.IntervalScheduleHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimTimePoint.IntervalSchedule.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(cimTimePoint.GetType().ToString()).Append(" rdfID = \"").Append(cimTimePoint.ID);
                        report.Report.Append("\" - Failed to set reference to IntervalSchedule: rdfID \"").Append(cimTimePoint.IntervalSchedule.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.REGULARTIMEPOINT_INTERVALSCHEDULE, gid));
                }
            }
        }

        // ===== IRREGULARINTERVALSCHEDULE =====
        public static void PopulateIrregularIntervalScheduleProperties(FTN.IrregularIntervalSchedule cimSchedule, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimSchedule != null) && (rd != null))
            {
                if (cimSchedule.NameHasValue)
                    rd.AddProperty(new Property(ModelCode.IDOBJ_NAME, cimSchedule.Name));

                if (cimSchedule.MRIDHasValue)
                    rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, cimSchedule.MRID));

                if (cimSchedule.StartTimeHasValue)
                    rd.AddProperty(new Property(ModelCode.BASICINTSCHEDULE_STARTTIME, cimSchedule.StartTime.ToString()));
            }
        }

        // ===== IRREGULARTIMEPOINT =====
        public static void PopulateIrregularTimePointProperties(FTN.IrregularTimePoint cimTimePoint, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimTimePoint != null) && (rd != null))
            {
                if (cimTimePoint.NameHasValue)
                    rd.AddProperty(new Property(ModelCode.IDOBJ_NAME, cimTimePoint.Name));

                if (cimTimePoint.MRIDHasValue)
                    rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, cimTimePoint.MRID));

                if (cimTimePoint.TimeHasValue)
                    rd.AddProperty(new Property(ModelCode.IRREGULARTIMEPOINT_TIME, cimTimePoint.Time));

                if (cimTimePoint.Value1HasValue)
                    rd.AddProperty(new Property(ModelCode.IRREGULARTIMEPOINT_VALUE1, cimTimePoint.Value1));

                if (cimTimePoint.Value2HasValue)
                    rd.AddProperty(new Property(ModelCode.IRREGULARTIMEPOINT_VALUE2, cimTimePoint.Value2));

                if (cimTimePoint.IntervalScheduleHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimTimePoint.IntervalSchedule.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(cimTimePoint.GetType().ToString()).Append(" rdfID = \"").Append(cimTimePoint.ID);
                        report.Report.Append("\" - Failed to set reference to IntervalSchedule: rdfID \"").Append(cimTimePoint.IntervalSchedule.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.IRREGULARTIMEPOINT_INTERVALSCHEDULE, gid));
                }
            }
        }

        // ===== OUTAGESCHEDULE =====
        public static void PopulateOutageScheduleProperties(FTN.OutageSchedule cimSchedule, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimSchedule != null) && (rd != null))
            {
                if (cimSchedule.NameHasValue)
                    rd.AddProperty(new Property(ModelCode.IDOBJ_NAME, cimSchedule.Name));

                if (cimSchedule.MRIDHasValue)
                    rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, cimSchedule.MRID));

                if (cimSchedule.StartTimeHasValue)
                    rd.AddProperty(new Property(ModelCode.BASICINTSCHEDULE_STARTTIME, cimSchedule.StartTime.ToString()));

                if (cimSchedule.PowerSystemResourceHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimSchedule.PowerSystemResource.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(cimSchedule.GetType().ToString()).Append(" rdfID = \"").Append(cimSchedule.ID);
                        report.Report.Append("\" - Failed to set reference to PowerSystemResource: rdfID \"").Append(cimSchedule.PowerSystemResource.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.OUTAGESCHEDULE_POWERSYSTEMRESOURCE, gid));
                }
            }
        }

        // ===== SWITCH =====
        public static void PopulateSwitchProperties(FTN.Switch cimSwitch, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimSwitch != null) && (rd != null))
            {
                if (cimSwitch.NameHasValue)
                    rd.AddProperty(new Property(ModelCode.IDOBJ_NAME, cimSwitch.Name));

                if (cimSwitch.MRIDHasValue)
                    rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, cimSwitch.MRID));

                if (cimSwitch.SwitchingOperationsHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimSwitch.SwitchingOperations.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(cimSwitch.GetType().ToString()).Append(" rdfID = \"").Append(cimSwitch.ID);
                        report.Report.Append("\" - Failed to set reference to SwitchingOperations: rdfID \"").Append(cimSwitch.SwitchingOperations.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.SWITCH_SWITCHINGOPERATIONS, gid));
                }
            }
        }

        // ===== SWITCHINGOPERATION =====
        public static void PopulateSwitchingOperationProperties(FTN.SwitchingOperation cimSwitchingOperation, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimSwitchingOperation != null) && (rd != null))
            {
                if (cimSwitchingOperation.NameHasValue)
                    rd.AddProperty(new Property(ModelCode.IDOBJ_NAME, cimSwitchingOperation.Name));

                if (cimSwitchingOperation.MRIDHasValue)
                    rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, cimSwitchingOperation.MRID));

                if (cimSwitchingOperation.NewStateHasValue)
                    rd.AddProperty(new Property(ModelCode.SWITCHINGOPERATION_NEWSTATE, (short)cimSwitchingOperation.NewState));

                if (cimSwitchingOperation.OperationTimeHasValue)
                    rd.AddProperty(new Property(ModelCode.SWITCHINGOPERATION_OPERATIONTIME, cimSwitchingOperation.OperationTime.ToString()));

                if (cimSwitchingOperation.OutageScheduleHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimSwitchingOperation.OutageSchedule.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(cimSwitchingOperation.GetType().ToString()).Append(" rdfID = \"").Append(cimSwitchingOperation.ID);
                        report.Report.Append("\" - Failed to set reference to OutageSchedule: rdfID \"").Append(cimSwitchingOperation.OutageSchedule.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.SWITCHINGOPERATION_OUTAGESCHEDULE, gid));
                }
            }
        }

        #endregion Populate ResourceDescription
    }
}