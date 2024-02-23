using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;
using Unity.Netcode;

namespace ZE.Purastic {

	public sealed class PlayableCharacter : MonoBehaviour, IPlayerControllable, IContainable
	{
		//[SerializeField] private Animator _animator;
		[SerializeField] private CharacterSettings _characterSettings;

		[SerializeField] private CharacterModel _model;
		[SerializeField] private Rigidbody _rigidbody;
		private bool _isAuthoritative = true;
		private Vector3 _realPosition;
		private JumpHandler _jumpHandler;
		private GravityHandler _gravityHandler;
		private MovementHandler _moveHandler;
		private IInputController _controller;
		//private readonly int JUMP_PARAMETER = Animator.StringToHash("Jump"), RUN_PARAMETER = Animator.StringToHash("Run");
		public int ContainerID { get; set; }
		public Vector3 MoveVector { get; private set; }
		public IColliderOwner GetColliderOwner() => _model;
		public ViewPointInfo GetViewPointInfo() => new ViewPointInfo(_model.ViewPoint, _characterSettings.ViewSettings);
		public CharacterHandsModule HandsModule { get; private set; }

        private void Start()
        {
			_jumpHandler = new JumpHandler(_characterSettings.JumpConfig);
			_gravityHandler = new GravityHandler(_characterSettings.GravityConfig, _model.ZeroPoint);
			_moveHandler= new MovementHandler(_characterSettings.MoveConfig);
			if (_controller != null) _moveHandler.SetInputController(_controller);
			HandsModule = new CharacterHandsModule(_model.HandPoint);
        }

		public void AssignController(IInputController input, bool isAuthoritative)
		{
			_isAuthoritative = isAuthoritative;
			_controller = input;
            _controller.SetControlObject(this);
			_moveHandler?.SetInputController(_controller);
        }
		public void Dispose()
		{
			_controller = null;
			_moveHandler.SetInputController(null);
		}
		public void Jump()
		{
			if (!_gravityHandler.IsFalling) _jumpHandler.StartJump();
		}
        private void FixedUpdate()
        {
            float deltaTime = Time.fixedDeltaTime, heightChange = 0f;
			if (_jumpHandler.IsJumping) heightChange = _jumpHandler.Jump(deltaTime);
			else
			{
				heightChange = _gravityHandler.FixedUpdate(deltaTime);
			}
			Vector3 prevPos = _rigidbody.position;
            Vector3 pos = prevPos +  _moveHandler.Update(deltaTime) + heightChange * Vector3.up;

			if (pos != prevPos)
			{
				if (_isAuthoritative) _rigidbody.MovePosition(pos);
				else _rigidbody.MovePosition(Vector3.Lerp(pos, _realPosition, 0.9f));
				MoveVector = (pos - prevPos).normalized;			
			}
			else
			{
				MoveVector = Vector3.zero;
			}
        }
        private void Update()
        {
			Vector3 vector = _moveHandler.LastMoveVector;
			if (vector.sqrMagnitude !=0f)
			{
                _model.transform.rotation = Quaternion.RotateTowards(_model.transform.rotation, Quaternion.LookRotation(vector.normalized, Vector3.up), Time.deltaTime * 360f);
                //_model.transform.rotation = Quaternion.RotateTowards(_model.transform.rotation, Quaternion.LookRotation(vector.normalized, Vector3.up), Time.deltaTime * 360f);
                //transform.rotation = Quaternion.LookRotation(_moveHandler.InputMoveVector.normalized, Vector3.up);
            }
        }

		public Synchronizer GetSync() => new Synchronizer() { Forward = _model.transform.forward, Position= _rigidbody.position };
		public void Sync(Synchronizer sync)
		{
			Vector3 velocity = Vector3.zero;
			_realPosition = sync.Position;
			Vector3 fwd = Vector3.SmoothDamp(_model.transform.forward, sync.Forward, ref velocity, 0.1f);
			if (fwd.sqrMagnitude != 0f) _model.transform.rotation = Quaternion.LookRotation(fwd, Vector3.up);
		}

        public class Factory : ContainerObjectFactory<PlayableCharacter>
		{
			private readonly GameResourcesPack _resourcesPack;

			public Factory(Container container) : base(container)
			{
				_resourcesPack = ServiceLocatorObject.Get<GameResourcesPack>();
			}
            protected override PlayableCharacter Instantiate()
            {
                return Object.Instantiate(_resourcesPack.DefaultCharacter);
            }
        }

		private class JumpHandler
		{
			private float _jumpCharge = 0f, _height =0f;
			private readonly JumpConfig _jumpConfig;
			public bool IsJumping { get; private set; } = false;
			public JumpHandler(JumpConfig jumpConfig)
			{
				_jumpConfig = jumpConfig;
			}

			public void StartJump()
			{
				if (!IsJumping)
				{
					IsJumping = true;
					_jumpCharge = 1f;
					_height = 0f;
				}
			}
			public float Jump(float deltaTime)
			{
                _jumpCharge = Mathf.MoveTowards(_jumpCharge, 0f, deltaTime / _jumpConfig.Duration);
				float nextHeight = _jumpConfig.HeightCurve.Evaluate(1f - _jumpCharge) * _jumpConfig.Height,
					currentHeight = _height;
				_height = nextHeight;
                 IsJumping = _jumpCharge != 0f;
				return  nextHeight - currentHeight;
			}
		}
		private class GravityHandler
		{
			private float _gravityAcceleration = 0f;
			private const float UP_CAST_GAP = 0.1f;
			private readonly int _castMask;
			private readonly GravityConfig _gravityConfig;
			private readonly Transform _zeroPoint;
			public bool IsFalling => _gravityAcceleration != 0f;

			public GravityHandler(GravityConfig gravityConfig, Transform zeroPoint)
			{
				_gravityConfig= gravityConfig;
				_zeroPoint= zeroPoint;
				_castMask = gravityConfig._groundCastLayerMask.ToInt();
			}
			public float Update(float deltaTime)
			{
				_gravityAcceleration += _gravityConfig.Gravity * deltaTime;
				if (Physics.Raycast(_zeroPoint.position + UP_CAST_GAP * Vector3.up, Vector3.down,maxDistance: _gravityAcceleration + UP_CAST_GAP,hitInfo: out var rh, layerMask: _castMask))
				{
					if (rh.distance < UP_CAST_GAP)
					{
						OnFall();
						return rh.distance;
					}
					else return -rh.distance;
				}
				else
				{
					return -_gravityAcceleration;
				}
			}
			public float FixedUpdate(float time)
			{
                _gravityAcceleration += _gravityConfig.Gravity * time;
                if (Physics.Raycast(_zeroPoint.position + UP_CAST_GAP * Vector3.up, Vector3.down, maxDistance: _gravityAcceleration + UP_CAST_GAP, hitInfo: out var rh, layerMask: _castMask))
                {
                    if (rh.distance < UP_CAST_GAP)
                    {
                        OnFall();
                        return 0f;
                    }
                    else return -rh.distance + UP_CAST_GAP;
                }
                else
                {
                    return -_gravityAcceleration;
                }
            }
			private void OnFall()
			{
				// hit the ground with gravity acceleration
				_gravityAcceleration = 0f;
			}
		}
		private class MovementHandler
		{
			private IInputController _inputController;
			public Vector3 LastMoveVector { get; private set; } = Vector3.zero;
			private readonly MoveConfig _moveConfig;
			public Vector3 VelocityVector;

			public MovementHandler(MoveConfig config)
			{
				_moveConfig= config;
			}
			public void SetInputController(IInputController input) => _inputController= input;

			public Vector3 Update(float deltaTime) {
				LastMoveVector = _inputController?.MoveVector ?? Vector3.zero;
				return LastMoveVector * _moveConfig.Speed* deltaTime;
			}
		}

		public struct Synchronizer : INetworkSerializable
		{
			public Vector3 Position;
			public Vector2 Forward;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
			{
                serializer.SerializeValue(ref Position);
                serializer.SerializeValue(ref Forward);
            }            
		}
	}

	[System.Serializable]
	public class MoveConfig
	{
		public float Speed = 5f;
	}
	[System.Serializable]
	public class JumpConfig
	{
		public float Duration = 1f;
		public float Height = 2f;
		public AnimationCurve HeightCurve;
	}
	[System.Serializable]
	public class GravityConfig
	{
		public float Gravity = 9.8f;
		public CustomLayermask _groundCastLayerMask = CustomLayermask.GroundCast;
	}
}
