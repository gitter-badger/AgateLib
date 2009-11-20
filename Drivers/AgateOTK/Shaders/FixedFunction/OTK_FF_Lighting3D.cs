﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AgateLib.Geometry;
using AgateLib.DisplayLib.Shaders;
using AgateLib.DisplayLib.Shaders.Implementation;
using OpenTK.Graphics.OpenGL;

namespace AgateOTK.Shaders.FixedFunction
{
	class OTK_FF_Lighting3D : Lighting3DImpl  
	{
		Color mAmbientLight;
		Matrix4x4 mProjection;
		Matrix4x4 mView;
		Matrix4x4 mWorld;
		AgateLib.DisplayLib.Shaders.Light[] mLights;

		public OTK_FF_Lighting3D()
		{
			int maxLights;
			GL.GetInteger(GetPName.MaxLights, out maxLights);

			mLights = new Light[maxLights];
		}
		public override Color AmbientLight
		{
			get { return mAmbientLight; }
			set { mAmbientLight = value; }
		}

		public override Light[] Lights
		{
			get { return mLights; }
		}

		public override Matrix4x4 Projection
		{
			get { return mProjection; }
			set { mProjection = value; }
		}

		public override Matrix4x4 View
		{
			get { return mView; }
			set { mView = value; }
		}

		public override Matrix4x4 World
		{
			get { return mWorld; }
			set { mWorld = value; }
		}

		public override void Begin()
		{
			
			OpenTK.Matrix4 otkProjection = GeoHelper.ConvertAgateMatrix(mProjection, false);

			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.LoadMatrix(ref otkProjection);

			OpenTK.Matrix4 view = GeoHelper.ConvertAgateMatrix(mView, false);

			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
			GL.LoadMatrix(ref view);

			if (EnableLighting == false)
			{
				GL.Disable(EnableCap.Lighting);
			}
			else
			{
				GL.Enable(EnableCap.Lighting);

				float[] array = new float[4];

				GL.Enable(EnableCap.Lighting);

				SetArray(array, AmbientLight);
				GL.LightModel(LightModelParameter.LightModelAmbient, array);

				GL.Enable(EnableCap.ColorMaterial);
				GL.ColorMaterial(MaterialFace.FrontAndBack,
								 ColorMaterialParameter.AmbientAndDiffuse);

				for (int i = 0; i < mLights.Length; i++)
				{
					EnableCap lightID = (EnableCap)((int)EnableCap.Light0 + i);
					LightName lightName = (LightName)((int)LightName.Light0 + i);

					if (mLights[i] == null || mLights[i].Enabled == false)
					{
						GL.Disable(lightID);
						continue;
					}

					GL.Enable(lightID);

					SetArray(array, mLights[i].DiffuseColor);
					GL.Light(lightName, LightParameter.Diffuse, array);

					SetArray(array, mLights[i].AmbientColor);
					GL.Light(lightName, LightParameter.Ambient, array);

					SetArray(array, mLights[i].Position);
					GL.Light(lightName, LightParameter.Position, array);

					GL.Light(lightName, LightParameter.ConstantAttenuation, mLights[i].AttenuationConstant);
					GL.Light(lightName, LightParameter.LinearAttenuation, mLights[i].AttenuationLinear);
					GL.Light(lightName, LightParameter.QuadraticAttenuation, mLights[i].AttenuationQuadratic);
				}
			}

			OpenTK.Matrix4 viewworld = GeoHelper.ConvertAgateMatrix(mView * mWorld, false);

			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
			GL.LoadMatrix(ref viewworld);

		}

		private void SetArray(float[] array, Vector3 vec)
		{
			array[0] = vec.X;
			array[1] = vec.Y;
			array[2] = vec.Z;
		}
		private void SetArray(float[] array, Color color)
		{
			array[0] = color.R / 255.0f;
			array[1] = color.G / 255.0f;
			array[2] = color.B / 255.0f;
			array[3] = color.A / 255.0f;
		}

		public override void BeginPass(int passIndex)
		{
		}

		public override void End()
		{
		}

		public override void EndPass()
		{
		}

		public override int Passes
		{
			get { return 1; }
		}

		public override void SetTexture(EffectTexture tex, string variableName)
		{
			throw new NotImplementedException();
		}

		public override void SetVariable(string name, Color color)
		{
			throw new NotImplementedException();
		}

		public override void SetVariable(string name, Matrix4x4 matrix)
		{
			throw new NotImplementedException();
		}

		public override void SetVariable(string name, params int[] v)
		{
			throw new NotImplementedException();
		}

		public override void SetVariable(string name, params float[] v)
		{
			throw new NotImplementedException();
		}
	}
}
