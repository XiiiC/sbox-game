using System;
using Sandbox;

namespace Template
{
	[Library( "test_weapon" )]
	partial class TestWeapon : BaseWeapon
	{
		public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";
		public override float PrimaryRate => 15f;

		private Particles beamParticles;

		public override void Spawn()
		{
			base.Spawn();
			SetModel( "weapons/rust_smg/rust_smg.vmdl" );
		}

		public override void SimulateAnimator( PawnAnimator anim )
		{
			base.SimulateAnimator( anim );
			anim.SetParam( "holdtype", 2 );
		}

		public override bool CanPrimaryAttack()
		{
			if ( !Input.Down( InputButton.Attack1 ) )
				return false;

			if ( Owner.Health <= 0 )
				return false;

			return base.CanPrimaryAttack();
		}

		public override void AttackPrimary()
		{
			TimeSincePrimaryAttack = 0;
			TimeSinceSecondaryAttack = 0;

			Shoot( Owner.EyePos, Owner.EyeRot.Forward );
		}

		private void Shoot( Vector3 pos, Vector3 dir )
		{
			var forward = dir * 10000;

			foreach ( var tr in TraceBullet( pos, pos + dir * 100000, 12.5f ) )
			{
				tr.Surface.DoBulletImpact( tr );

				// Do beam particles on client and server
				beamParticles?.Destroy( true );
				beamParticles = Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity,
					"muzzle" );

				if ( !IsServer ) continue;
				if ( !tr.Entity.IsValid() ) continue;

				using ( Prediction.Off() )
				{
					var damage = DamageInfo.FromBullet( tr.EndPos, forward.Normal * 20, 10 )
						.UsingTraceResult( tr )
						.WithAttacker( Owner )
						.WithWeapon( this );

					tr.Entity.TakeDamage( damage );
				}
			}

			ShootEffects();
		}

		public override void Simulate( Client owner )
		{
			base.Simulate( owner );

			if ( beamParticles != null )
			{
				var tr = Trace.Ray( Owner.EyePos, Owner.EyeRot.Forward * 1000000f ).Ignore( Owner ).WorldOnly().Run();
				beamParticles.SetPosition( 1, tr.EndPos );
			}
		}

		[ClientRpc]
		public virtual void ShootEffects()
		{
			Host.AssertClient();

			Sound.FromEntity( "rust_smg.shoot", this );

			ViewModelEntity?.SetAnimBool( "fire", true );
			CrosshairPanel?.OnEvent( "onattack" );

			if ( IsLocalPawn )
			{
				_ = new Sandbox.ScreenShake.Perlin( 0.5f, 1.0f, 2.0f );
			}
		}
	}
}
