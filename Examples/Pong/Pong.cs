using System;
using System.Collections.Generic;
using AgateLib;
using AgateLib.DisplayLib;
using AgateLib.Geometry;
using AgateLib.InputLib;
using AgateLib.ApplicationModels;
using AgateLib.InputLib.Legacy;
using AgateLib.Platform.WinForms.ApplicationModels;
using AgateLib.Platform.WinForms;

namespace Pong
{
	class Program
	{

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			using (var setup = new AgateSetupWinForms(args))
			{
				setup.DesiredDisplayWindowResolution = new Size(800, 600);
				setup.InitializeAgateLib();

				new Program().Run(args);
			}
		}

		IFont font;
		int[] score = new int[2];
		string[] names = new string[] { "Player", "CPU" };

		Vector2 ball, ballvelocity;
		Vector2[] paddle = new Vector2[2];

		const int paddleHeight = 80;
		const int paddleWidth = 16;
		const int borderSize = paddleWidth;
		const int ballSize = paddleWidth;
		const int displayWidth = 800;
		const int playAreaWidth = 700;
		const int displayHeight = 600;
		const float paddleSpeed = 150.0f;

		Color paddleColor = Color.LightGray;
		Color ballColor = Color.LightGray;

		void Run(string[] args)
		{
			font = Font.AgateSans;
			font.Size = 14;

			paddle[0] = new Vector2(50, displayHeight / 2);
			paddle[1] = new Vector2(playAreaWidth - 50 - paddleWidth, displayHeight / 2);
			ball = new Vector2(playAreaWidth / 2, displayHeight / 2);
			ballvelocity = new Vector2(-70, 70);

			while (Display.CurrentWindow.IsClosed == false)
			{
				Display.BeginFrame();
				Display.Clear(Color.DarkGray);

				DrawBorder();
				DrawPaddles();
				DrawBall();
				DrawScore();

				Display.EndFrame();
				Core.KeepAlive();

				if (Input.Unhandled.Keys[KeyCode.Escape])
					return;

				float time_s = (float)Display.DeltaTime / 1000.0f;

				UpdatePaddles(time_s);
				UpdateBall(time_s);
			}
		}

		private void DrawScore()
		{
			int x = playAreaWidth + borderSize;
			int y = borderSize * 2;

			font.DrawText(x, y, "Score");

			for (int i = 0; i < 2; i++)
			{
				y += font.FontHeight * 2;
				font.DrawText(x, y, names[i]);

				y += font.FontHeight;
				font.DrawText(x, y, score[i].ToString());
			}
		}

		private void UpdateBall(float time_s)
		{
			bool newBall = false;

			ball += ballvelocity * time_s;

			// collision with bottom wall
			if (ball.Y + ballSize > displayHeight - borderSize && ballvelocity.Y > 0)
				ballvelocity.Y *= -1;

			// collision with top wall
			if (ball.Y < borderSize && ballvelocity.Y < 0)
				ballvelocity.Y *= -1;

			if (ball.X < borderSize)
			{
				newBall = true;
				score[1]++;
			}
			else if (ball.X + ballSize > playAreaWidth)
			{
				newBall = true;
				score[0]++;
			}

			if (newBall)
			{
				ball = new Vector2(playAreaWidth / 2, displayHeight / 2);
				ballvelocity = new Vector2(-90, 90);
			}

			// check for paddles
			bool increaseSpeed = false;

			if (ball.X < paddle[0].X + paddleWidth && ballvelocity.X < 0)
			{
				if (ball.Y + ballSize - 1 >= paddle[0].Y &&
					ball.Y <= paddle[0].Y + paddleHeight)
				{
					ballvelocity.X *= -1;
					increaseSpeed = true;
				}
			}
			if (ball.X + ballSize >= paddle[1].X && ballvelocity.X > 0)
			{
				if (ball.Y + ballSize - 1 >= paddle[1].Y &&
					ball.Y <= paddle[1].Y + paddleHeight)
				{
					ballvelocity.X *= -1;
					increaseSpeed = true;
				}
			}

			if (increaseSpeed)
			{
				ballvelocity.X += Math.Sign(ballvelocity.X) * 10.0f;
				ballvelocity.Y += Math.Sign(ballvelocity.Y) * 10.0f;
			}
		}

		private void UpdatePaddles(float time_s)
		{
			float paddleMove = paddleSpeed * time_s;

			if (Input.Unhandled.Keys[KeyCode.Down]) paddle[0].Y += paddleMove;
			if (Input.Unhandled.Keys[KeyCode.Up]) paddle[0].Y -= paddleMove;

			// do AI
			if (ballvelocity.X > 0)
			{
				if (ball.Y + ballSize * 2 > paddle[1].Y + paddleHeight) paddle[1].Y += paddleMove;
				if (ball.Y - ballSize < paddle[1].Y) paddle[1].Y -= paddleMove;
			}

			for (int i = 0; i < 2; i++)
			{
				paddle[i].Y = Math.Max(paddle[i].Y, borderSize);
				paddle[i].Y = Math.Min(paddle[i].Y, displayHeight - borderSize - paddleHeight);
			}
		}

		private void DrawBall()
		{
			Display.FillRect(new Rectangle((int)ball.X, (int)ball.Y, ballSize, ballSize), ballColor);
		}

		private void DrawPaddles()
		{
			for (int i = 0; i < 2; i++)
			{
				Display.FillRect(
					new Rectangle((int)paddle[i].X, (int)paddle[i].Y, paddleWidth, paddleHeight), paddleColor);
			}
		}

		private void DrawBorder()
		{
			Color borderColor = paddleColor;

			Display.FillRect(new Rectangle(0, 0, displayWidth, borderSize), borderColor);
			Display.FillRect(new Rectangle(0, 0, borderSize, displayHeight), borderColor);
			Display.FillRect(new Rectangle(0, displayHeight - borderSize, displayWidth, borderSize), borderColor);
			Display.FillRect(new Rectangle(displayWidth - borderSize, 0, borderSize, displayHeight), borderColor);
			Display.FillRect(new Rectangle(playAreaWidth - borderSize, 0, borderSize, displayHeight), borderColor);

		}

		void Mouse_MouseMove(object sender, AgateInputEventArgs e)
		{
			paddle[0].Y = e.MousePosition.Y;
		}
	}
}