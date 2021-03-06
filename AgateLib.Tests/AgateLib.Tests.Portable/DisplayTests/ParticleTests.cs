using System;
using System.Linq;
using AgateLib.ApplicationModels;
using AgateLib.Configuration;
using AgateLib.DisplayLib;
using AgateLib.DisplayLib.Sprites;
using AgateLib.Geometry;
using AgateLib.Particles;

namespace AgateLib.Tests.DisplayTests.ParticleTest
{
	public class PixelParticleTest : IAgateTest
	{
		Random ran = new Random();

		// PixelParticle
		PixelEmitter pe;
		GravityManipulator gm;
		GravityManipulator gm2;
		
		//SurfaceParticle
		SurfaceEmitter sm;
		
		//SpriteParticle
		SpriteEmitter se;
		
		FadeOutManipulator fom;
		FadeOutManipulator fom2;
		
		GravityPointManipulator gpm;

		public string Name { get { return "Particles"; } }

		public string Category { get { return "Display"; } }

		public AgateConfig Configuration { get; set; }

		public void ModifySetup(IAgateSetup setup)
		{
			setup.DesiredDisplayWindowResolution = new Size(800, 600);
		}

		public void Run()
		{
			Initialize();

			while(Configuration.DisplayWindows.First().IsClosed == false)
			{
				Update(Display.DeltaTime);
				Draw();
			}
		}

		private void Initialize()
		{
			//PixelParticle
			pe = new PixelEmitter(new Vector2(400f, 550f) ,Color.Blue, 2000);
			pe.EmitLife = 15f;
			pe.EmitFrequency = 0.01f;
			pe.PixelSize = new Size(3, 3);
			
			//SurfaceParticle
			sm = new SurfaceEmitter(new Vector2(150f, 550f), 4.2f, 50, 0);
			Surface surf = new Surface(@"smoke2.png");
			sm.AddSurface(surf);
			sm.EmitFrequency = 0.1f;
			sm.EmitAlpha = 1d;
			sm.EmitAcceleration = new Vector2(0, -20);
			sm.EmitVelocity = new Vector2(0, -10);
			
			//SpriteParticle
			Surface surf2 = new Surface(@"smoke.png");
			Sprite sprite = new Sprite(100, 100);
			sprite.AddFrame(surf);
			sprite.AddFrame(surf2);
			sprite.TimePerFrame = 3d;
			sprite.AnimationType = SpriteAnimType.Looping;
			se = new SpriteEmitter(new Vector2(600f, 550f), 4.2f, 100, 0);
			se.AddSprite(sprite);
			se.EmitFrequency = 0.05f;
			se.EmitAlpha = 1d;
			se.EmitAcceleration = new Vector2(0, -20);
			se.EmitVelocity = new Vector2(0, -10);
			
			//Manipulators
			gm = new GravityManipulator(new Vector2(0f, -20f));
			gm.SubscribeToEmitter(sm);
			gm.SubscribeToEmitter(se);
			
			gm2 = new GravityManipulator(Vector2.Empty);
			//gm2.SubscribeToEmitter(pe);
			gm2.SubscribeToEmitter(sm);
			gm2.SubscribeToEmitter(se);
			
			fom = new FadeOutManipulator(2.5f, 0.6f);
			fom.SubscribeToEmitter(pe);
			
			fom2 = new FadeOutManipulator(4f, 0.3f);
			fom2.SubscribeToEmitter(sm);
			fom2.SubscribeToEmitter(se);
			
			gpm = new GravityPointManipulator(new Vector2(400f, 350f), -1f);
			gpm.SubscribeToEmitter(pe);
		}		

		public void Update(double deltaT)
		{
			gm2.Gravity = new Vector2((float)ran.Next(-300, 300), 0f);
			
			fom.AlphaAmount = (float)ran.NextDouble() * 1.3f;
			fom.LifeBarrier = (float)ran.NextDouble() * 5f;
			
			pe.Update(deltaT);
			pe.EmitVelocity = new Vector2((float)ran.Next(-10, 10), 0f);
			
			sm.Update(deltaT);
			
			se.Update(deltaT);
			se.GetSpriteByKey(0).TimePerFrame = ran.NextDouble() * 3 + 1.5d;
		}
		
		public void Draw()
		{
			Display.BeginFrame();
			Display.Clear(Color.Black);
			IFont font = AgateLib.DefaultAssets.Fonts.AgateSans;
			font.Size = 14;

			font.DrawText("FPS: " + Display.FramesPerSecond);
			
			pe.Draw();
			font.DrawText((int)pe.Position.X, (int)pe.Position.Y, "Particles: " + pe.Particles.Count + "/" + pe.Particles.Capacity);
			
			sm.Draw();
			font.DrawText((int)sm.Position.X, (int)sm.Position.Y, "Particles: " + sm.Particles.Count + "/" + sm.Particles.Capacity);
			
			se.Draw();
			font.DrawText((int)se.Position.X, (int)se.Position.Y, "Particles: " + se.Particles.Count + "/" + se.Particles.Capacity);

			Display.EndFrame();
			Core.KeepAlive();
		}
	}
}
