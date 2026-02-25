using System;

namespace FTN.Common
{	
	public enum PhaseCode : short
	{
		Unknown = 0x0,
		N = 0x1,
		C = 0x2,
		CN = 0x3,
		B = 0x4,
		BN = 0x5,
		BC = 0x6,
		BCN = 0x7,
		A = 0x8,
		AN = 0x9,
		AC = 0xA,
		ACN = 0xB,
		AB = 0xC,
		ABN = 0xD,
		ABC = 0xE,
		ABCN = 0xF
	}
	
	public enum TransformerFunction : short
	{
		Supply = 1,				// Supply transformer
		Consumer = 2,			// Transformer supplying a consumer
		Grounding = 3,			// Transformer used only for grounding of network neutral
		Voltreg = 4,			// Feeder voltage regulator
		Step = 5,				// Step
		Generator = 6,			// Step-up transformer next to a generator.
		Transmission = 7,		// HV/HV transformer within transmission network.
		Interconnection = 8		// HV/HV transformer linking transmission network with other transmission networks.
	}
	
	public enum WindingConnection : short
	{
		Y = 1,		// Wye
		D = 2,		// Delta
		Z = 3,		// ZigZag
		I = 4,		// Single-phase connection. Phase-to-phase or phase-to-ground is determined by elements' phase attribute.
		Scott = 5,   // Scott T-connection. The primary winding is 2-phase, split in 8.66:1 ratio
		OY = 6,		// 2-phase open wye. Not used in Network Model, only as result of Topology Analysis.
		OD = 7		// 2-phase open delta. Not used in Network Model, only as result of Topology Analysis.
	}

	public enum WindingType : short
	{
		None = 0,
		Primary = 1,
		Secondary = 2,
		Tertiary = 3
	}	
	
	public enum SwitchState : short
	{
		open = 0,
		close = 1
	}

	/// <summary>
	/// CIM CurveStyle Enumeration
	/// </summary>
	public enum CurveStyle : short
	{
		straightLineYValues = 1,
		constantYValue = 2,
		formula = 3,
		rampYValue = 4
	}

	/// <summary>
	/// CIM UnitMultiplier Enumeration
	/// </summary>
	public enum UnitMultiplier : short
	{
		none = 0,
		m = 1,
		c = 2,
		d = 3,
		k = 4,
		M = 5,
		G = 6,
		T = 7,
		p = 8,
		n = 9,
		micro = 10
	}

	/// <summary>
	/// CIM UnitSymbol Enumeration
	/// </summary>
	public enum UnitSymbol : short
	{
		none = 0,
		V = 1,
		A = 2,
		W = 3,
		VAr = 4,
		VA = 5,
		Hz = 6,
		deg = 7,
		s = 8,
		ohm = 9,
		H = 10,
		F = 11,
		J = 12,
		N = 13,
		Pa = 14,
		S = 15,
		VAh = 16,
		VArh = 17,
		Wh = 18,
		degC = 19,
		g = 20,
		h = 21,
		m = 22,
		m2 = 23,
		m3 = 24,
		min = 25,
		rad = 26
	}
}
