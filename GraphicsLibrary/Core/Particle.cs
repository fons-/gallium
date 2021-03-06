﻿using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GraphicsLibrary.Content;

namespace GraphicsLibrary.Core
{
	public struct Particle
	{
		public Vector3 position;
		public Vector3 velocity;
		public Material material;
		public float rotation;
		public float size;
		public float age;
		public float lifetime;
	}

	public struct ParticleVertex
	{
		public Vector3 pos;
		public Vector4 dfs;
		public Vector2 tex;

	}

	public class ParticleEmitter:Node
	{
		private readonly Random rnd = new Random();
		private bool allParticlesAreDead = false;

		public Material baseMaterial = new Material();
		public Particle[] particles;
		public bool skipNewParticles = false;

		public float force = .05f;
		public float maxLifetime = 8;
		public float minLifetime = 2;
		public float maxSize = 300;
		public float minSize = 100;
		public float maxVelocity = 5000;
		public float minVelocity = 1000;

		public ParticleEmitter(int amountOfParticles, string name, Material baseMaterial)
			: base(name)
		{
			particles = new Particle[amountOfParticles];
			for(int i = 0; i < amountOfParticles; i++)
			{
				particles[i] = new Particle();
			}
			this.baseMaterial = baseMaterial;
		}

		public void RandomizeParticles(float force, float maxLifetime, float minLifetime, float maxSize, float minSize,
			float maxVelocity, float minVelocity)
		{
			this.force = force;
			this.maxLifetime = maxLifetime;
			this.minLifetime = minLifetime;
			this.maxSize = maxSize;
			this.minSize = minSize;
			this.maxVelocity = maxVelocity;
			this.minVelocity = minVelocity;

			RandomizeParticles();
		}

		public void RandomizeParticles()
		{
			for(int i = 0; i < particles.Length; i++)
			{
				particles[i].age = 0f;
				particles[i].lifetime = (float)rnd.NextDouble() * (maxLifetime - minLifetime) + minLifetime;//2 PARAM
				particles[i].size = (float)rnd.NextDouble() * (maxSize - minSize) + minSize;//PARAM
				particles[i].material = baseMaterial;

				Vector3 unit = new Vector3((float)rnd.NextDouble() - .5f, (float)rnd.NextDouble() - .5f, (float)rnd.NextDouble() - .5f);//(float)i/particles.Length - .5f);

				unit.Normalize();

				particles[i].position = Vector3.Zero;
				particles[i].velocity = Vector3.Multiply(unit, (float)rnd.NextDouble() * (maxVelocity - minVelocity) + minVelocity);// 2 PARAM
			}

			allParticlesAreDead = false;
		}

		public override void UpdateNode(float timeSinceLastUpdate)
		{
			base.UpdateNode(timeSinceLastUpdate);
			if(!allParticlesAreDead)
			{
				allParticlesAreDead = true;
				for(int i = 0; i < particles.Length; i++)
				{

					if(particles[i].age > particles[i].lifetime) //TODO: skip some particles
					{

					}
					else
					{
						allParticlesAreDead = false;
						particles[i].age += timeSinceLastUpdate;
						//float lifeRatio = particles[i].age/particles[i].lifetime;
						particles[i].velocity *= (float)Math.Pow(force, timeSinceLastUpdate);
						particles[i].position += Vector3.Multiply(particles[i].velocity, timeSinceLastUpdate);
						//TODO: Rotation
						//TODO: Scaling
					}
				}
			}
		}

		public override void Render(int pass)
		{
			if(isVisible && renderPass == pass)
			{
				GL.PushMatrix();

				GL.Translate(derivedPosition);

				GL.DepthMask(false);
				GL.Disable(EnableCap.Lighting);
				GL.Disable(EnableCap.DepthTest);

				double ratio = 1;
				if(skipNewParticles)
				{
					ratio = Math.Min(5 * particles[0].age / particles[0].lifetime, 1);
				}

				Vector3 top = Node.RotateVector(new Vector3(0f, 1f, 0f), Quaternion.Invert(Camera.Instance.derivedOrientation));
				Vector3 right = Node.RotateVector(new Vector3(1f, 0f, 0f), Quaternion.Invert(Camera.Instance.derivedOrientation));
				Vector3 topright = top + right;

				for(int i = 0; i < particles.Length * ratio; i++)
				{
					if(particles[i].age < particles[i].lifetime) //TODO: VBO or geometry instancing
					{
						GL.BindTexture(TextureTarget.Texture2D,
							TextureManager.GetTexture(particles[i].material.GetCurrentTexture(particles[i].age / particles[i].lifetime)));
						GL.Color4(particles[i].material.GetCurrentColor(particles[i].age / particles[i].lifetime));

						GL.Begin(BeginMode.Quads);

						GL.TexCoord2(0, 0);
						GL.Vertex3(particles[i].size * top + particles[i].position);
						GL.TexCoord2(0, 1);
						GL.Vertex3(new Vector3(0, 0, 0) + particles[i].position);
						GL.TexCoord2(1, 1);
						GL.Vertex3(particles[i].size * right + particles[i].position);
						GL.TexCoord2(1, 0);
						GL.Vertex3(particles[i].size * topright + particles[i].position);

						GL.End();
					}/*
					else
					{
						i -= i%3;
						i += 3;
					}*/
				}
				GL.DepthMask(true);
				GL.Enable(EnableCap.Lighting);
				GL.Enable(EnableCap.DepthTest);

				GL.PopMatrix();
			}
		}
	}

	public class ParticleField:Node
	{
		private readonly Random rnd = new Random();
		private bool allParticlesAreDead = false;

		public Material baseMaterial = new Material();
		public Particle[] particles;
		public bool skipNewParticles = false;

		public float maxSize = 300, minSize = 100;
		public float width = 5000f, length = 5000f, height = 5000f;
		public Vector3 center = Vector3.Zero;

		public ParticleField(int amountOfParticles, string name, Material baseMaterial)
			: base(name)
		{
			particles = new Particle[amountOfParticles];
			for(int i = 0; i < amountOfParticles; i++)
			{
				particles[i] = new Particle();
			}
			this.baseMaterial = baseMaterial;
		}

		public void RandomizeParticles(float maxSize, float minSize)
		{
			this.maxSize = maxSize;
			this.minSize = minSize;

			RandomizeParticles();
		}

		public void RandomizeParticles()
		{
			for(int i = 0; i < particles.Length; i++)
			{
				particles[i].age = 0f;
				particles[i].lifetime = float.MaxValue;
				particles[i].size = (float)rnd.NextDouble() * (maxSize - minSize) + minSize;
				particles[i].material = baseMaterial;

				particles[i].position = center + new Vector3(((float)rnd.NextDouble() - .5f) * width, ((float)rnd.NextDouble() - .5f) * height, ((float)rnd.NextDouble() - .5f) * length);
				particles[i].velocity = Vector3.Zero;
			}

			allParticlesAreDead = false;
		}

		public override void UpdateNode(float timeSinceLastUpdate)
		{
			base.UpdateNode(timeSinceLastUpdate);

			Vector3 boundA = center + 0.5f * new Vector3(width, height, length);
			Vector3 boundB = center - 0.5f * new Vector3(width, height, length);

			for(int i = 0; i < particles.Length; i++)
			{
				if(particles[i].position.X > boundA.X)
					particles[i].position.X -= width;
				if(particles[i].position.X < boundB.X)
					particles[i].position.X += width;
				if(particles[i].position.Y > boundA.Y)
					particles[i].position.Y -= height;
				if(particles[i].position.Y < boundB.Y)
					particles[i].position.Y += height;
				if(particles[i].position.Z > boundA.Z)
					particles[i].position.Z -= length;
				if(particles[i].position.Z < boundB.Z)
					particles[i].position.Z += length;

				//TODO: Particle movement
			}
		}

		public override void Render(int pass)
		{
			if(isVisible && renderPass == pass)
			{
				Shader.particleShaderCompiled.Enable();
				GL.PushMatrix();

				GL.Translate(derivedPosition);

				GL.DepthMask(false);
				GL.Disable(EnableCap.Lighting);
				//GL.Disable(EnableCap.DepthTest);

				double ratio = 1;
				if(skipNewParticles)
				{
					ratio = Math.Min(5 * particles[0].age / particles[0].lifetime, 1);
				}

				Vector3 top = RotateVector(new Vector3(0f, 1f, 0f), Quaternion.Invert(Camera.Instance.derivedOrientation));
				Vector3 right = RotateVector(new Vector3(1f, 0f, 0f), Quaternion.Invert(Camera.Instance.derivedOrientation));
				Vector3 topright = top + right;

				for(int i = 0; i < particles.Length * ratio; i++)
				{
					if(particles[i].age < particles[i].lifetime) //TODO: VBO or geometry instancing
					{
						GL.BindTexture(TextureTarget.Texture2D,
							TextureManager.GetTexture(particles[i].material.GetCurrentTexture(particles[i].age / particles[i].lifetime)));
						GL.Color4(particles[i].material.GetCurrentColor(particles[i].age / particles[i].lifetime));

						GL.Begin(BeginMode.Quads);

						GL.TexCoord2(0, 0);
						GL.Vertex3(particles[i].size * top + particles[i].position);
						GL.TexCoord2(0, 1);
						GL.Vertex3(new Vector3(0, 0, 0) + particles[i].position);
						GL.TexCoord2(1, 1);
						GL.Vertex3(particles[i].size * right + particles[i].position);
						GL.TexCoord2(1, 0);
						GL.Vertex3(particles[i].size * topright + particles[i].position);

						GL.End();
					}/*
					else
					{
						i -= i%3;
						i += 3;
					}*/
				}
				GL.DepthMask(true);
				GL.Enable(EnableCap.Lighting);
				//GL.Enable(EnableCap.DepthTest);

				GL.PopMatrix();
			}
		}
	}

	//Sample usage:
	//particleEmitter = new ParticleEmitter(1000, "partics1", new Material("heavyTank", Color4.Red, true));
	//particleEmitter.baseMaterial.AddTransitionColor(new Color4(0f, 1f, 0f, 0.5f), 0.5f);
	//particleEmitter.baseMaterial.AddTransitionColor(new Color4(0f, 0f, 1f, 0f), 1.0f);
	//
	//particleEmitter.baseMaterial.AddTransitionTexture("monsterTexture", 0.3f);
	//particleEmitter.baseMaterial.AddTransitionTexture("font2", 0.6f);
	//particleEmitter.renderPass = 1;
	//particleEmitter.RandomizeParticles(0.001f, 10, 9, 100, 99, 6000, 5999);
}