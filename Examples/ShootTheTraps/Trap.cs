using System;
using System.Collections.Generic;
using System.Text;
using AgateLib;
using AgateLib.DisplayLib;
using AgateLib.Geometry;

namespace ShootTheTraps
{
	public class Trap : GameObject
	{
		const int width = 24;
		const int height = 16;
		Color mColor = Color.Red;

		bool mDeleteMe = false;

		static Random sRandom;
		static Color[] sColors = 
		{
			Color.White, Color.Blue, Color.Red, 
			Color.Purple, Color.Yellow, Color.Green,
			Color.Cyan,
		};

		public static Surface Image { get; set; }

		/// <summary>
		/// Creates a new instance of Trap 
		/// </summary>
		public Trap()
		{
			if (sRandom == null)
				sRandom = new Random();

			// only gravity affects this object.
			Acceleration = new Vector3d(0, Gravity, 0);

			mColor = sColors[sRandom.Next(sColors.Length)];
		}

		public override Rectangle BoundingRect
		{
			get
			{
				int width = Image.DisplayWidth;
				int height = Image.DisplayHeight;

				return new Rectangle(
					(int)Position.X - width / 2,
					(int)Position.Y - height / 2,
					width,
					height);
			}
		}

		public void SetDeleteMeFlag()
		{
			mDeleteMe = true;
		}

		public bool ContainsPoint(Vector3d pt)
		{
			Vector3d dist = Position - pt;

			if (Math.Abs(dist.X) > width / 2) return false;
			if (Math.Abs(dist.Y) > height / 2) return false;

			return true;
		}

		public override void Draw()
		{
			Image.DisplayAlignment = OriginAlignment.Center;
			Image.RotationCenter = OriginAlignment.Center;

			Image.Color = mColor;
			Image.RotationAngle = RotationAngle;

			Image.Draw((float)Position.X, (float)Position.Y);

			if (OutsideField && Velocity.Y > 0)
				mDeleteMe = true;
			else
				mDeleteMe = false;
		}

		public override bool DeleteMe
		{
			get { return mDeleteMe; }
		}

		public Color Color
		{
			get { return mColor; }
			set { mColor = value; }
		}

		const int NumberOfParticles = 20;
		const double ParticleSpeed = 100;

		protected override List<GameObject> ProtectedCreateDebris()
		{
			List<GameObject> retval = new List<GameObject>();
			Vector3d totalVelocity = new Vector3d(0, 0, 0);
			Random rnd = new Random();

			for (int i = 0; i < NumberOfParticles; i++)
			{
				Particle p = new Particle(Color, rnd);

				p.Position = Position;

				p.Velocity.X = sRandom.NextDouble() * 2 - 1;
				p.Velocity.Y = sRandom.NextDouble() * 2 - 1;

				p.Velocity = p.Velocity.Normalize() * (sRandom.NextDouble() * ParticleSpeed);
				p.RotationalVelocity = (sRandom.NextDouble() - 0.5) * 40;

				totalVelocity = totalVelocity + p.Velocity;
				retval.Add(p);
			}

			// now apply conservation of momentum, by giving a small portion
			// of the excess momentum to each particle
			Vector3d give = totalVelocity * (-1.0 / NumberOfParticles);

			for (int i = 0; i < NumberOfParticles; i++)
			{
				Particle p = (Particle)retval[i];

				p.Velocity = p.Velocity + Velocity + give;
			}


			return retval;
		}

	}
}