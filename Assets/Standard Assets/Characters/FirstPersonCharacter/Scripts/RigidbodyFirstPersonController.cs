using System;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityStandardAssets.CrossPlatformInput;
using TerrainClass;
using MinecraftKnockOff;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections;


namespace UnityStandardAssets.Characters.FirstPerson
{
	using Data = Dictionary<string, string>;
	using EventHandler = Action<Dictionary<string, string>>;

	[RequireComponent(typeof (Rigidbody))]
	[RequireComponent(typeof (CapsuleCollider))]
	public class RigidbodyFirstPersonController : MonoBehaviour
	{
		[Serializable]
		public class MovementSettings
		{
			public float ForwardSpeed = 8.0f;   // Speed when walking forward
			public float BackwardSpeed = 4.0f;  // Speed when walking backwards
			public float StrafeSpeed = 4.0f;    // Speed when walking sideways
			public float RunMultiplier = 2.0f;   // Speed when sprinting
			public KeyCode RunKey = KeyCode.LeftShift;
			public float JumpForce = 30f;
			public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
			[HideInInspector] public float CurrentTargetSpeed = 8f;

			#if !MOBILE_INPUT
			private bool m_Running;
			#endif

			public void UpdateDesiredTargetSpeed(Vector2 input)
			{
				if (input == Vector2.zero) return;
				if (input.x > 0 || input.x < 0)
				{
					//strafe
					CurrentTargetSpeed = StrafeSpeed;
				}
				if (input.y < 0)
				{
					//backwards
					CurrentTargetSpeed = BackwardSpeed;
				}
				if (input.y > 0)
				{
					//forwards
					//handled last as if strafing and moving forward at the same time forwards speed should take precedence
					CurrentTargetSpeed = ForwardSpeed;
				}
				#if !MOBILE_INPUT
				if (Input.GetKey(RunKey))
				{
					CurrentTargetSpeed *= RunMultiplier;
					m_Running = true;
				}
				else
				{
					m_Running = false;
				}
				#endif
			}

			#if !MOBILE_INPUT
			public bool Running
			{
				get { return m_Running; }
			}
			#endif
		}


		[Serializable]
		public class AdvancedSettings
		{
			public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
			public float stickToGroundHelperDistance = 0.5f; // stops the character
			public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
			public bool airControl; // can the user control the direction that is being moved in the air
			[Tooltip("set it to 0.1 or more if you get stuck in wall")]
			public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
		}


		public Camera cam;
		public MovementSettings movementSettings = new MovementSettings();
		public MouseLook mouseLook = new MouseLook();
		public AdvancedSettings advancedSettings = new AdvancedSettings();


		private Rigidbody m_RigidBody;
		private CapsuleCollider m_Capsule;
		private float m_YRotation;
		private Vector3 m_GroundContactNormal;
		private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded;
		private VisibleTerrain terrainGrid;

		private WebSocketClient wsClient;
		private static GameObject networkedPlayer;

		private bool gameStateIsActive = false;
        private Vector3 prevPos = Vector3.zero;
		private Vector3 prevAngle = Vector3.zero;
		public static bool singlePlayer = true;
		public static string ipAddress = "";
		public static bool restarted = false;

		public Vector3 Velocity
		{
			get { return m_RigidBody.velocity; }
		}

		public bool Grounded
		{
			get { return m_IsGrounded; }
		}

		public bool Jumping
		{
			get { return m_Jumping; }
		}

		public bool Running
		{
			get
			{
				#if !MOBILE_INPUT
				return movementSettings.Running;
				#else
				return false;
				#endif
			}
		}


		private void Start()
		{
			if (restarted) {
				Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
				restarted = false;
			}
			m_RigidBody = GetComponent<Rigidbody> ();
			m_RigidBody.useGravity = false;
			m_Capsule = GetComponent<CapsuleCollider> ();
            m_RigidBody.transform.position = new Vector3(0,15,0);
			mouseLook.Init (transform, cam.transform);
			advancedSettings.airControl = true; 
			// terrainGrid = new VisibleTerrain (Mathf.RoundToInt (transform.position.x - 0.5f), Mathf.RoundToInt (transform.position.z - 0.5f));
			Debug.Log ("I am alive!");
			if (!singlePlayer) {
				Debug.Log (ipAddress);
				networkedPlayer = Instantiate (Resources.Load ("Mesh_Kangaroo")) as GameObject;
				networkedPlayer.transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);
				Transform networkedPlayerTransform = GetComponent<Transform> ();
				networkedPlayer.transform.position = networkedPlayerTransform.position;
				// networkedPlayer.transform.Translate(Vector3.forward * 1.5f);
				Rigidbody networkedPlayerRigidBody = networkedPlayer.AddComponent<Rigidbody> ();
				networkedPlayerRigidBody.freezeRotation = true;
				networkedPlayerRigidBody.useGravity = false;

				if (ipAddress != "") {
					Debug.Log ("connect");
					wsClient = new WebSocketClient (ipAddress, 8080);
				} else { 
					wsClient = new WebSocketClient ();
				}
				wsClient.RegisterEventHandler ("init", new EventHandler ((data) => {
					int heightSeed = int.Parse (data ["heightSeed"]);
					int biomeSeed = int.Parse (data ["biomeSeed"]);

					terrainGrid = new VisibleTerrain (Mathf.RoundToInt (transform.position.x - 0.5f), Mathf.RoundToInt (transform.position.z - 0.5f), heightSeed, biomeSeed);
					gameStateIsActive = true;
					m_RigidBody.useGravity = true;
				}));
				wsClient.RegisterEventHandler ("pos", new EventHandler ((data) => {
					float x = float.Parse (data ["x"]);
					float y = float.Parse (data ["y"]);
					float z = float.Parse (data ["z"]);

					networkedPlayer.transform.position = new Vector3 (x, y, z);
				}));
				wsClient.RegisterEventHandler ("angle", new EventHandler((data) => {
					float x = float.Parse (data ["x"]);
					float y = float.Parse (data ["y"]);
					float z = float.Parse (data ["z"]);

					networkedPlayer.transform.eulerAngles = new Vector3 (x, y, z);
				}));
				wsClient.RegisterEventHandler ("delete_cube", new EventHandler ((data) => {
					float x = float.Parse (data ["x"]);
					float y = float.Parse (data ["y"]);
					float z = float.Parse (data ["z"]);

					VisibleTerrain.getInstance ().deleteCube (new Vector3 (x, y, z));
				}));
				wsClient.RegisterEventHandler ("add_cube", new EventHandler ((data) => {
					float x = float.Parse (data ["x"]);
					float y = float.Parse (data ["y"]);
					float z = float.Parse (data ["z"]);
					Material m = PlayerCube.getMaterial (data ["material"]);

					VisibleTerrain.getInstance ().addCube (new Vector3 (x, y, z), m);
				}));

                wsClient.Connect();
                Debug.Log("nonono");
            } else
            {
                terrainGrid = new VisibleTerrain(Mathf.RoundToInt(transform.position.x - 0.5f), Mathf.RoundToInt(transform.position.z - 0.5f));
                gameStateIsActive = true;
                m_RigidBody.useGravity = true;
            }
		}

		private void Update()
		{
			if (!gameStateIsActive) {
				return;
			}

			RotateView();

			if (CrossPlatformInputManager.GetButtonDown("Jump") && !m_Jump)
			{
				m_Jump = true;
			}
				
            if (prevPos != transform.position && !singlePlayer)
            {
                string x = Convert.ToString(transform.position.x);
                string y = Convert.ToString(transform.position.y);
                string z = Convert.ToString(transform.position.z);

                Data newPos = new Data();
                newPos["type"] = "pos";
                newPos["x"] = x;
                newPos["y"] = y;
                newPos["z"] = z;

                wsClient.Send(JsonConvert.SerializeObject(newPos));

                prevPos = transform.position;
            }
			if (prevAngle != transform.eulerAngles && !singlePlayer) {
				string x = Convert.ToString(transform.eulerAngles.x);
				string y = Convert.ToString(transform.eulerAngles.y);
				string z = Convert.ToString(transform.eulerAngles.z);

				Data newAngle = new Data();
				newAngle["type"] = "angle";
				newAngle["x"] = x;
				newAngle["y"] = y;
				newAngle["z"] = z;

				wsClient.Send(JsonConvert.SerializeObject(newAngle));

				prevAngle = transform.eulerAngles;
			}
		}


		private void FixedUpdate()
		{
			if(!gameStateIsActive) {
				return;
			}

			GroundCheck();
			Vector2 input = GetInput();

			if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (advancedSettings.airControl || m_IsGrounded))
			{
				// always move along the camera forward as it is the direction that it being aimed at
				Vector3 desiredMove = cam.transform.forward*input.y + cam.transform.right*input.x;
				desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;

				desiredMove.x = desiredMove.x*movementSettings.CurrentTargetSpeed;
				desiredMove.z = desiredMove.z*movementSettings.CurrentTargetSpeed;
				desiredMove.y = desiredMove.y*movementSettings.CurrentTargetSpeed;
				if (m_RigidBody.velocity.sqrMagnitude <
					(movementSettings.CurrentTargetSpeed*movementSettings.CurrentTargetSpeed))
				{
					m_RigidBody.AddForce(desiredMove*SlopeMultiplier(), ForceMode.Impulse);
				}
			}

			if (m_IsGrounded)
			{
				m_RigidBody.drag = 5f;

				if (m_Jump)
				{
					m_RigidBody.drag = 0f;
					m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
					m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
					m_Jumping = true;
				}

				if (!m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f)
				{
					m_RigidBody.Sleep();
				}
			}
			else
			{
				m_RigidBody.drag = 0f;
				if (m_PreviouslyGrounded && !m_Jumping)
				{
					StickToGroundHelper();
				}
			}
			m_Jump = false;
			terrainGrid.updateGrid (transform.position.x, transform.position.z);
		}


		private float SlopeMultiplier()
		{
			float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
			return movementSettings.SlopeCurveModifier.Evaluate(angle);
		}


		private void StickToGroundHelper()
		{
			RaycastHit hitInfo;
			if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
				((m_Capsule.height/2f) - m_Capsule.radius) +
				advancedSettings.stickToGroundHelperDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
			{
				if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
				{
					m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
				}
			}
		}


		private Vector2 GetInput()
		{

			Vector2 input = new Vector2
			{
				x = CrossPlatformInputManager.GetAxis("Horizontal"),
				y = CrossPlatformInputManager.GetAxis("Vertical")
			};
			movementSettings.UpdateDesiredTargetSpeed(input);
			return input;
		}


		private void RotateView()
		{
			//avoids the mouse looking if the game is effectively paused
			if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

			// get the rotation before it's changed
			float oldYRotation = transform.eulerAngles.y;

			mouseLook.LookRotation (transform, cam.transform);

			if (m_IsGrounded || advancedSettings.airControl)
			{
				// Rotate the rigidbody velocity to match the new direction that the character is looking
				Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
				m_RigidBody.velocity = velRotation*m_RigidBody.velocity;
			}
		}

		/// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
		private void GroundCheck()
		{
			m_PreviouslyGrounded = m_IsGrounded;
			RaycastHit hitInfo;
			if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
				((m_Capsule.height/2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
			{
				m_IsGrounded = true;
				m_GroundContactNormal = hitInfo.normal;
			}
			else
			{
				m_IsGrounded = false;
				m_GroundContactNormal = Vector3.up;
			}
			if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
			{
				m_Jumping = false;
			}
		}
	}
}
