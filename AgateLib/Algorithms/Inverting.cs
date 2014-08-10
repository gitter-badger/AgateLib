﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgateLib.Algorithms
{
	public class Inverting
	{
		public static double IterateInvert(Func<double, double> func, double targetVal)
		{
			return IterateInvert(func, targetVal, 0);
		}
		public static double IterateInvert(Func<double, double> func, double targetVal, double initialPt)
		{
			bool hasLower = false;
			bool hasUpper = false;
			int iter = 0;
			const int itermax = 500;

			var p1 = new Pair<double, double>(initialPt, func(initialPt));
			Pair<double, double> lowerVal, upperVal;

			int sign;

			if (p1.Second < targetVal)
			{
				hasLower = true;
				upperVal = lowerVal = p1;

				sign = 1;
			}
			else if (p1.Second > targetVal)
			{
				hasUpper = true;
				lowerVal = upperVal = p1;

				sign = -1;
			}
			else
				return initialPt;

			int step = 1 * sign;

			while ((hasUpper && hasLower) == false)
			{
				p1.First += step;
				p1.Second = func(p1.First);

				if (p1.Second < targetVal)
				{
					lowerVal = p1;
					hasLower = true;
				}
				else if (p1.Second > targetVal)
				{
					upperVal = p1;
					hasUpper = true;
				}
				else
					return p1.First;

				iter++;
				step *= 2;

				if (iter > itermax)
					throw new Exception("No solution found.");

			}

			while (Math.Abs(p1.Second - targetVal) > 1e-7)
			{
				if (iter % 3 == 0)
				{
					p1.First = (upperVal.First + lowerVal.First) / 2;
					p1.Second = func(p1.First);
				}
				else
				{
					double invslope = (upperVal.First - lowerVal.First) /
									  (upperVal.Second - lowerVal.Second);

					p1.First = lowerVal.First + invslope * (targetVal - lowerVal.Second);
					p1.Second = func(p1.First);
				}

				if (p1.Second < targetVal)
					lowerVal = p1;
				else if (p1.Second > targetVal)
					upperVal = p1;
				else
					return p1.First;

				iter++;
				if (iter > itermax)
					throw new Exception("No solution found.");
			}

			return p1.First;
		}
	}
}
