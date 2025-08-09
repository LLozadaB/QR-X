/*
MESSAGE FROM CREATOR: This script was coded by Mena. You can use it in your games either these are commercial or
personal projects. You can even add or remove functions as you wish. However, you cannot sell copies of this
script by itself, since it is originally distributed as a free product.
I wish you the best for your project. Good luck!
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrometeoCarController : MonoBehaviour
{

  //CAR SETUP

  [Space(20)]
  //[Header("CAR SETUP")]
  [Space(10)]
  [Range(50, 500)]
  public int maxSpeed = 240; //The maximum speed that the car can reach in km/h.
  [Range(60, 120)]
  public int maxReverseSpeed = 70; //The maximum speed that the car can reach while going on reverse in km/h.
  [Range(3, 12)]
  public int accelerationMultiplier = 7; // How fast the car can accelerate. 1 is a slow acceleration and 10 is the fastest.
  [Space(10)]
  [Range(10, 45)]
  public int maxSteeringAngle = 27; // The maximum angle that the tires can reach while rotating the steering wheel.
  [Range(0.1f, 1f)]
  public float steeringSpeed = 0.5f; // How fast the steering wheel turns.
  [Space(10)]
  [Range(100, 600)]
  public int brakeForce = 350; // The strength of the wheel brakes.
  [Range(1, 10)]
  public int decelerationMultiplier = 2; // How fast the car decelerates when the user is not using the throttle.
  [Range(1, 10)]
  public int handbrakeDriftMultiplier = 5; // How much grip the car loses when the user hit the handbrake.
  [Space(10)]
  public Vector3 bodyMassCenter; // This is a vector that contains the center of mass of the car. I recommend to set this value
                                 // in the points x = 0 and z = 0 of your car. You can select the value that you want in the y axis,
                                 // however, you must notice that the higher this value is, the more unstable the car becomes.
                                 // Usually the y value goes from 0 to 1.5.

  //WHEELS

  //[Header("WHEELS")]

  /*
  The following variables are used to store the wheels' data of the car. We need both the mesh-only game objects and wheel
  collider components of the wheels. The wheel collider components and 3D meshes of the wheels cannot come from the same
  game object; they must be separate game objects.
  */
  public GameObject frontLeftMesh;
  public WheelCollider frontLeftCollider;
  [Space(10)]
  public GameObject frontRightMesh;
  public WheelCollider frontRightCollider;
  [Space(10)]
  public GameObject rearLeftMesh;
  public WheelCollider rearLeftCollider;
  [Space(10)]
  public GameObject rearRightMesh;
  public WheelCollider rearRightCollider;

  //PARTICLE SYSTEMS

  [Space(20)]
  //[Header("EFFECTS")]
  [Space(10)]
  //The following variable lets you to set up particle systems in your car
  public bool useEffects = false;

  // The following particle systems are used as tire smoke when the car drifts.
  public ParticleSystem RLWParticleSystem;
  public ParticleSystem RRWParticleSystem;

  //Nitro particle system
  public ParticleSystem NitroParticleSystemOne;
  public bool twoNitro = false;
  public ParticleSystem NitroParticleSystemTwo;
  public float nitroValue = 10f;
  public bool nitroFlag = false;
  public int nitroAccelLimit;
  public Camera camera;

  [Space(10)]
  // The following trail renderers are used as tire skids when the car loses traction.
  public TrailRenderer RLWTireSkid;
  public TrailRenderer RRWTireSkid;
  public ParticleSystem LightDamageParticleSystem;
  public ParticleSystem MediumDamageParticleSystem;
  public ParticleSystem HeavyDamageParticleSystem;
  public ParticleSystem WreckedParticleSystem;

  //SPEED TEXT (UI)

  [Space(20)]
  //[Header("UI")]
  [Space(10)]
  //The following variable lets you to set up a UI text to display the speed of your car.
  public bool useUI = false;
  public Text carSpeedText; // Used to store the UI object that is going to show the speed of the car.
  public Slider NitroSlider; // Used to store the UI slider that shows the nitro value of the car.

  //SOUNDS

  [Space(20)]
  //[Header("Sounds")]
  [Space(10)]
  //The following variable lets you to set up sounds for your car such as the car engine or tire screech sounds.
  public bool useSounds = false;
  public AudioSource carEngineSound; // This variable stores the sound of the car engine.
  public AudioSource tireScreechSound; // This variable stores the sound of the tire screech (when the car is drifting).
  public float initialCarEngineSoundPitch; // Used to store the initial pitch of the car engine sound.
  public AudioSource ramSound;
  public AudioSource ramHitSound;

  //CONTROLS

  [Space(20)]
  //[Header("CONTROLS")]
  [Space(10)]
  //The following variables lets you to set up touch controls for mobile devices.
  public bool useTouchControls = false;
  public GameObject throttleButton;
  PrometeoTouchInput throttlePTI;
  public GameObject reverseButton;
  PrometeoTouchInput reversePTI;
  public GameObject turnRightButton;
  PrometeoTouchInput turnRightPTI;
  public GameObject turnLeftButton;
  PrometeoTouchInput turnLeftPTI;
  public GameObject handbrakeButton;
  PrometeoTouchInput handbrakePTI;
  public TrackWaypoints waypoints;
  [Range(0, 10)] public int distanceOffset;
  [Range(0, 5)] public float AISteerSpeedMultiplier;


  //CAR DATA

  [HideInInspector]
  public float carSpeed; // Used to store the speed of the car.
  [HideInInspector]
  public bool isDrifting; // Used to know whether the car is drifting or not.
  [HideInInspector]
  public bool isTractionLocked; // Used to know whether the traction of the car is locked or not.

  //PRIVATE VARIABLES

  /*
  IMPORTANT: The following variables should not be modified manually since their values are automatically given via script.
  */
  Rigidbody carRigidbody; // Stores the car's rigidbody.
  float steeringAxis; // Used to know whether the steering wheel has reached the maximum value. It goes from -1 to 1.
  float throttleAxis; // Used to know whether the throttle has reached the maximum value. It goes from -1 to 1.
  float driftingAxis;
  float localVelocityZ;
  float localVelocityX;
  bool deceleratingCar;
  bool touchControlsSetup = false;
  /*
  The following variables are used to store information about sideways friction of the wheels (such as
  extremumSlip,extremumValue, asymptoteSlip, asymptoteValue and stiffness). We change this values to
  make the car to start drifting.
  */
  WheelFrictionCurve FLwheelFriction;
  float FLWextremumSlip;
  WheelFrictionCurve FRwheelFriction;
  float FRWextremumSlip;
  WheelFrictionCurve RLwheelFriction;
  float RLWextremumSlip;
  WheelFrictionCurve RRwheelFriction;
  float RRWextremumSlip;

  public enum DriverType
  {
    AI,
    Player,
    Wrecked
  }

  [SerializeField] public DriverType DriveController;

  public List<Transform> nodes = new List<Transform>();
  public Transform currentWaypoint;
  private int previousWaypointIndex = 0;
  private float stuckTimer = 0f;
  private float stuckThreshold = 3f; // seconds before AI resets
  private float speedThreshold = 20f; // below this speed = "stuck"
  [Range(250, 1000)] public int health = 500;
  public int currentHealth = 500;
  public Slider HealthSlider;
  [HideInInspector]
  public bool canMove = false;
  public float ramCooldown = 0f;
  private bool isRamming = false;
  private Coroutine ramCoroutine;
  private Dictionary<PrometeoCarController, float> ramHitCooldowns = new Dictionary<PrometeoCarController, float>();
  private float ramHitDelay = 1.0f; // 1 second delay before damage can be applied again

  private void Awake()
  {
    waypoints = GameObject.FindGameObjectWithTag("Path").GetComponent<TrackWaypoints>();
    nodes = waypoints.nodes;
  }

  public int currentNode = 0;
  public int waypointI = 0;
  private void CalculateDistanceOfWaypoints()
  {
    Vector3 position = gameObject.transform.position;
    float distance = Mathf.Infinity;
    for (int i = 0; i < nodes.Count; i++)
    {
      Vector3 difference = nodes[i].transform.position - position;
      float currentDistance = difference.magnitude;
      if (currentDistance < distance)
      {
        int candidateIndex = i + distanceOffset;
        if (i + distanceOffset >= nodes.Count)
        {
          candidateIndex %= nodes.Count;
        }
        else if (i + distanceOffset < 0)
        {
          candidateIndex = nodes.Count + candidateIndex;
        }
        if (currentWaypoint != null)
        {
          previousWaypointIndex = nodes.IndexOf(currentWaypoint);
        }
        currentWaypoint = nodes[candidateIndex];
        distance = currentDistance;
        waypointI = i;
      }
    }
    //Players use no offset, but AI do. This subtraction balances positioning in the race
    currentNode = waypointI;
  }

  private void ResetToPreviousWaypoint()
  {
    if (nodes == null || nodes.Count == 0) return;
    int resetIndex = previousWaypointIndex;
    if (resetIndex < 0 || resetIndex >= nodes.Count)
      resetIndex = 0;
    Transform resetPoint = nodes[resetIndex];
    // Reset position and rotation
    transform.position = resetPoint.position + Vector3.up * 1.5f;
    transform.rotation = resetPoint.rotation;
    carRigidbody.linearVelocity = Vector3.zero;
    carRigidbody.angularVelocity = Vector3.zero;
    // If current health - 15% is less than 25% of health, set current health to 25% of health.
    // Otherwise, reduce current health by 15% of health. If health is at or below 25%, it will not go below that.
    if (currentHealth <= ((int)(health * 0.25)))
    {
      //Do nothing, health is already at/below 25%
      currentHealth = currentHealth;
    }
    else if ((currentHealth - ((int)(health * 0.15))) < ((int)(health * 0.25)))
    {
      currentHealth = (int)(health * 0.25);
    }
    else
    {
      currentHealth -= (int)(health * 0.15);
    }
  }

  private void OnDrawGizmos()
  {
    if (currentWaypoint != null)
    {
      Gizmos.DrawWireSphere(currentWaypoint.position, 2f);
    }
  }

  // Start is called before the first frame update
  void Start()
  {
    //In this part, we set the 'carRigidbody' value with the Rigidbody attached to this
    //gameObject. Also, we define the center of mass of the car with the Vector3 given
    //in the inspector.
    carRigidbody = gameObject.GetComponent<Rigidbody>();
    carRigidbody.centerOfMass = bodyMassCenter;
    nitroAccelLimit = accelerationMultiplier;
    nitroValue = 10;
    currentHealth = health;

    //Initial setup to calculate the drift value of the car. This part could look a bit
    //complicated, but do not be afraid, the only thing we're doing here is to save the default
    //friction values of the car wheels so we can set an appropiate drifting value later.
    FLwheelFriction = new WheelFrictionCurve();
    FLwheelFriction.extremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
    FLWextremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
    FLwheelFriction.extremumValue = frontLeftCollider.sidewaysFriction.extremumValue;
    FLwheelFriction.asymptoteSlip = frontLeftCollider.sidewaysFriction.asymptoteSlip;
    FLwheelFriction.asymptoteValue = frontLeftCollider.sidewaysFriction.asymptoteValue;
    FLwheelFriction.stiffness = frontLeftCollider.sidewaysFriction.stiffness;
    FRwheelFriction = new WheelFrictionCurve();
    FRwheelFriction.extremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
    FRWextremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
    FRwheelFriction.extremumValue = frontRightCollider.sidewaysFriction.extremumValue;
    FRwheelFriction.asymptoteSlip = frontRightCollider.sidewaysFriction.asymptoteSlip;
    FRwheelFriction.asymptoteValue = frontRightCollider.sidewaysFriction.asymptoteValue;
    FRwheelFriction.stiffness = frontRightCollider.sidewaysFriction.stiffness;
    RLwheelFriction = new WheelFrictionCurve();
    RLwheelFriction.extremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
    RLWextremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
    RLwheelFriction.extremumValue = rearLeftCollider.sidewaysFriction.extremumValue;
    RLwheelFriction.asymptoteSlip = rearLeftCollider.sidewaysFriction.asymptoteSlip;
    RLwheelFriction.asymptoteValue = rearLeftCollider.sidewaysFriction.asymptoteValue;
    RLwheelFriction.stiffness = rearLeftCollider.sidewaysFriction.stiffness;
    RRwheelFriction = new WheelFrictionCurve();
    RRwheelFriction.extremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
    RRWextremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
    RRwheelFriction.extremumValue = rearRightCollider.sidewaysFriction.extremumValue;
    RRwheelFriction.asymptoteSlip = rearRightCollider.sidewaysFriction.asymptoteSlip;
    RRwheelFriction.asymptoteValue = rearRightCollider.sidewaysFriction.asymptoteValue;
    RRwheelFriction.stiffness = rearRightCollider.sidewaysFriction.stiffness;

    // We invoke 2 methods inside this script. CarSpeedUI() changes the text of the UI object that stores
    // the speed of the car and CarSounds() controls the engine and drifting sounds. Both methods are invoked
    // in 0 seconds, and repeatedly called every 0.1 seconds.
    if (useUI)
    {
      InvokeRepeating("CarSpeedUI", 0f, 0.1f);
      InvokeRepeating("NitroUI", 0f, 0.1f);
      InvokeRepeating("HealthUI", 0f, 0.1f);
    }
    else if (!useUI)
    {
      if (carSpeedText != null)
      {
        carSpeedText.text = "0";
      }
      if (NitroSlider != null)
      {
        NitroSlider.value = 0f;
      }
      if (HealthSlider != null)
      {
        HealthSlider.value = 0f;
      }
    }

    if (useSounds)
    {
      InvokeRepeating("CarSounds", 0f, 0.1f);
    }
    else if (!useSounds)
    {
      if (carEngineSound != null)
      {
        carEngineSound.Stop();
      }
      if (tireScreechSound != null)
      {
        tireScreechSound.Stop();
      }
    }

    if (!useEffects)
    {
      if (RLWParticleSystem != null)
      {
        RLWParticleSystem.Stop();
      }
      if (RRWParticleSystem != null)
      {
        RRWParticleSystem.Stop();
      }
      if (RLWTireSkid != null)
      {
        RLWTireSkid.emitting = false;
      }
      if (RRWTireSkid != null)
      {
        RRWTireSkid.emitting = false;
      }
      if (NitroParticleSystemOne != null)
      {
        NitroParticleSystemOne.Stop();
      }
      if (!twoNitro)
      {
        if (NitroParticleSystemTwo != null)
        {
          NitroParticleSystemTwo.Stop();
        }
      }
    }


    if (useTouchControls)
    {
      if (throttleButton != null && reverseButton != null &&
      turnRightButton != null && turnLeftButton != null
      && handbrakeButton != null)
      {

        throttlePTI = throttleButton.GetComponent<PrometeoTouchInput>();
        reversePTI = reverseButton.GetComponent<PrometeoTouchInput>();
        turnLeftPTI = turnLeftButton.GetComponent<PrometeoTouchInput>();
        turnRightPTI = turnRightButton.GetComponent<PrometeoTouchInput>();
        handbrakePTI = handbrakeButton.GetComponent<PrometeoTouchInput>();
        touchControlsSetup = true;

      }
      else
      {
        String ex = "Touch controls are not completely set up. You must drag and drop your scene buttons in the" +
        " PrometeoCarController component.";
        Debug.LogWarning(ex);
      }
    }

  }

  // Update is called once per frame
  void Update()
  {

    //CAR DATA
    AddDownforce();
    CalculateDistanceOfWaypoints();
    // We determine the speed of the car.
    carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;
    // Save the local velocity of the car in the x axis. Used to know if the car is drifting.
    localVelocityX = transform.InverseTransformDirection(carRigidbody.linearVelocity).x;
    // Save the local velocity of the car in the z axis. Used to know if the car is going forward or backwards.
    localVelocityZ = transform.InverseTransformDirection(carRigidbody.linearVelocity).z;

    if (currentHealth <= health * 0.75 && currentHealth > health * 0.5)
    {
      ApplyLightDamageEffect();
    }
    else if (currentHealth <= health * 0.5 && currentHealth > health * 0.25)
    {
      ApplyMediumDamageEffect();
    }
    else if (currentHealth <= health * 0.25 && currentHealth > 0)
    {
      ApplyHeavyDamageEffect();
    }
    else if (currentHealth <= 0)
    {
      ApplyWreckedDamageEffect();
    }

    if (!canMove)
    {
      return;
    }

    if (ramCooldown > 0f)
    {
      ramCooldown -= Time.deltaTime;
      if (ramCooldown < 0f) ramCooldown = 0f;
    }

    if (DriveController == DriverType.AI)
    {
      AIDrive();
    }
    else if (DriveController == DriverType.Wrecked || currentHealth <= 0)
    {
      Wrecked();
    }
    else
    {

      //CAR PHYSICS

      /*
      The next part is regarding to the car controller. First, it checks if the user wants to use touch controls (for
      mobile devices) or analog controls (WASD + Space).

      The following methods are called whenever a certain key is pressed.

      In this part of the code we specify what the car needs to do if the user presses keyboard inputs.
      */
      if (useTouchControls && touchControlsSetup)
      {

        if (throttlePTI.buttonPressed)
        {
          CancelInvoke("DecelerateCar");
          deceleratingCar = false;
          GoForward();
        }
        if (reversePTI.buttonPressed)
        {
          CancelInvoke("DecelerateCar");
          deceleratingCar = false;
          GoReverse();
        }

        if (turnLeftPTI.buttonPressed)
        {
          TurnLeft();
        }
        if (turnRightPTI.buttonPressed)
        {
          TurnRight();
        }
        if (handbrakePTI.buttonPressed)
        {
          CancelInvoke("DecelerateCar");
          deceleratingCar = false;
          Handbrake();
        }
        if (!handbrakePTI.buttonPressed)
        {
          RecoverTraction();
        }
        if ((!throttlePTI.buttonPressed && !reversePTI.buttonPressed))
        {
          ThrottleOff();
        }
        if ((!reversePTI.buttonPressed && !throttlePTI.buttonPressed) && !handbrakePTI.buttonPressed && !deceleratingCar)
        {
          InvokeRepeating("DecelerateCar", 0f, 0.1f);
          deceleratingCar = true;
        }
        if (!turnLeftPTI.buttonPressed && !turnRightPTI.buttonPressed && steeringAxis != 0f)
        {
          ResetSteeringAngle();
        }

      }
      else
      {

        if (Input.GetKey(KeyCode.W))
        {
          CancelInvoke("DecelerateCar");
          deceleratingCar = false;
          GoForward();
        }
        if (Input.GetKey(KeyCode.S))
        {
          CancelInvoke("DecelerateCar");
          deceleratingCar = false;
          GoReverse();
        }
        //Nitro key input
        if (Input.GetKey(KeyCode.LeftShift))
        {
          nitroFlag = true;
          Nitro();
          BoostFOV();
        }
        else
        {
          nitroFlag = false;
          Nitro();
          BoostFOV();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
          ResetToPreviousWaypoint();
        }
        // If the player falls off/under the map, reset to previous waypoint.
        if (transform.position.y < -25f)
        {
          ResetToPreviousWaypoint();
        }
        if (Input.GetKey(KeyCode.A))
        {
          TurnLeft();
        }
        if (Input.GetKey(KeyCode.D))
        {
          TurnRight();
        }
        //Ram key inputs, right shift is the combat modifier key. Has to be over 100 km/h
        if (Input.GetKey(KeyCode.A) && Input.GetKeyDown(KeyCode.RightShift) && ramCooldown <= 0f && (carRigidbody.linearVelocity.magnitude * 3.6f) >= 100f)
        {
          if (ramCoroutine == null)
          {
            ramCoroutine = StartCoroutine(Ram(-1)); // Left ram
          }
        }
        if (Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.RightShift) && ramCooldown <= 0f && (carRigidbody.linearVelocity.magnitude * 3.6f) >= 100f)
        {
          if (ramCoroutine == null)
          {
            ramCoroutine = StartCoroutine(Ram(1)); // Right ram
          }
        }
        if (Input.GetKey(KeyCode.Space))
        {
          CancelInvoke("DecelerateCar");
          deceleratingCar = false;
          Handbrake();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
          RecoverTraction();
        }
        if ((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)))
        {
          ThrottleOff();
        }
        if ((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) && !Input.GetKey(KeyCode.Space) && !deceleratingCar)
        {
          InvokeRepeating("DecelerateCar", 0f, 0.1f);
          deceleratingCar = true;
        }
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && steeringAxis != 0f)
        {
          ResetSteeringAngle();
        }
      }
    }


    // We call the method AnimateWheelMeshes() in order to match the wheel collider movements with the 3D meshes of the wheels.
    AnimateWheelMeshes();

    // Update ram hit cooldowns
    if (ramHitCooldowns.Count > 0)
    {
        var keys = new List<PrometeoCarController>(ramHitCooldowns.Keys);
        foreach (var key in keys)
        {
            ramHitCooldowns[key] -= Time.deltaTime;
            if (ramHitCooldowns[key] <= 0f)
                ramHitCooldowns.Remove(key);
        }
    }
  }

  // This method converts the car speed data from float to string, and then set the text of the UI carSpeedText with this value.
  public void CarSpeedUI()
  {

    if (useUI)
    {
      try
      {
        float absoluteCarSpeed = carRigidbody.linearVelocity.magnitude * 3.6f;
        carSpeedText.text = Mathf.RoundToInt(absoluteCarSpeed) + "";
      }
      catch (Exception ex)
      {
        Debug.LogWarning(ex);
      }
    }
  }

  private void AddDownforce()
  {
    float downforce = 100f;
    carRigidbody.AddForce(-transform.up * downforce * carRigidbody.linearVelocity.magnitude);
  }

  public void NitroUI()
  {

    if (useUI)
    {
      try
      {
        NitroSlider.value = nitroValue;
      }
      catch (Exception ex)
      {
        Debug.LogWarning(ex);
      }
    }
  }

  public void HealthUI()
  {

    if (useUI)
    {
      try
      {
        HealthSlider.maxValue = health;
        HealthSlider.value = currentHealth;
      }
      catch (Exception ex)
      {
        Debug.LogWarning(ex);
      }
    }
  }

  // This method controls the car sounds. For example, the car engine will sound slow when the car speed is low because the
  // pitch of the sound will be at its lowest point. On the other hand, it will sound fast when the car speed is high because
  // the pitch of the sound will be the sum of the initial pitch + the car speed divided by 100f.
  // Apart from that, the tireScreechSound will play whenever the car starts drifting or losing traction.
  public void CarSounds()
  {

    if (useSounds)
    {
      try
      {
        if (carEngineSound != null)
        {
          float engineSoundPitch = initialCarEngineSoundPitch + (Mathf.Abs(carRigidbody.linearVelocity.magnitude) / 26f);
          carEngineSound.pitch = engineSoundPitch;
        }
        if ((isDrifting || (isTractionLocked && Mathf.Abs(carSpeed) > 12f)) &&
       rearLeftCollider.isGrounded && rearRightCollider.isGrounded)
        {
          if (!tireScreechSound.isPlaying)
          {
            tireScreechSound.Play();
          }
        }
        else if (((!isDrifting) && (!isTractionLocked || Mathf.Abs(carSpeed) < 12f)) || DriveController == DriverType.Wrecked)
        {
          tireScreechSound.Stop();
        }
      }
      catch (Exception ex)
      {
        Debug.LogWarning(ex);
      }
    }
    else if (!useSounds)
    {
      if (carEngineSound != null && carEngineSound.isPlaying)
      {
        carEngineSound.Stop();
      }
      if (tireScreechSound != null && tireScreechSound.isPlaying)
      {
        tireScreechSound.Stop();
      }
    }

  }

  //
  //STEERING METHODS
  //

  //The following method turns the front car wheels to the left. The speed of this movement will depend on the steeringSpeed variable.
  public void TurnLeft()
  {
    steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
    if (steeringAxis < -1f)
    {
      steeringAxis = -1f;
    }
    var steeringAngle = steeringAxis * maxSteeringAngle;
    frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
    frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
  }

  //The following method turns the front car wheels to the right. The speed of this movement will depend on the steeringSpeed variable.
  public void TurnRight()
  {
    steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
    if (steeringAxis > 1f)
    {
      steeringAxis = 1f;
    }
    var steeringAngle = steeringAxis * maxSteeringAngle;
    frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
    frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
  }

  //The following method takes the front car wheels to their default position (rotation = 0). The speed of this movement will depend
  // on the steeringSpeed variable.
  public void ResetSteeringAngle()
  {
    if (steeringAxis < 0f)
    {
      steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
    }
    else if (steeringAxis > 0f)
    {
      steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
    }
    if (Mathf.Abs(frontLeftCollider.steerAngle) < 1f)
    {
      steeringAxis = 0f;
    }
    var steeringAngle = steeringAxis * maxSteeringAngle;
    frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
    frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
  }

  // This method matches both the position and rotation of the WheelColliders with the WheelMeshes.
  void AnimateWheelMeshes()
  {
    try
    {
      Quaternion FLWRotation;
      Vector3 FLWPosition;
      frontLeftCollider.GetWorldPose(out FLWPosition, out FLWRotation);
      frontLeftMesh.transform.position = FLWPosition;
      frontLeftMesh.transform.rotation = FLWRotation;

      Quaternion FRWRotation;
      Vector3 FRWPosition;
      frontRightCollider.GetWorldPose(out FRWPosition, out FRWRotation);
      frontRightMesh.transform.position = FRWPosition;
      frontRightMesh.transform.rotation = FRWRotation;

      Quaternion RLWRotation;
      Vector3 RLWPosition;
      rearLeftCollider.GetWorldPose(out RLWPosition, out RLWRotation);
      rearLeftMesh.transform.position = RLWPosition;
      rearLeftMesh.transform.rotation = RLWRotation;

      Quaternion RRWRotation;
      Vector3 RRWPosition;
      rearRightCollider.GetWorldPose(out RRWPosition, out RRWRotation);
      rearRightMesh.transform.position = RRWPosition;
      rearRightMesh.transform.rotation = RRWRotation;
    }
    catch (Exception ex)
    {
      Debug.LogWarning(ex);
    }
  }

  //
  //ENGINE AND BRAKING METHODS
  //

  // This method apply positive torque to the wheels in order to go forward.
  public void GoForward()
  {
    //If the forces aplied to the rigidbody in the 'x' asis are greater than
    //3f, it means that the car is losing traction, then the car will start emitting particle systems.
    if (Mathf.Abs(localVelocityX) > 2.5f)
    {
      isDrifting = true;
      DriftCarPS();
    }
    else
    {
      isDrifting = false;
      DriftCarPS();
    }
    // The following part sets the throttle power to 1 smoothly.
    throttleAxis = throttleAxis + (Time.deltaTime * 3f);
    if (throttleAxis > 1f)
    {
      throttleAxis = 1f;
    }
    //If the car is going backwards, then apply brakes in order to avoid strange
    //behaviours. If the local velocity in the 'z' axis is less than -1f, then it
    //is safe to apply positive torque to go forward.
    if (localVelocityZ < -1f)
    {
      Brakes();
    }
    else
    {
      if (Mathf.RoundToInt(carSpeed) < maxSpeed)
      {
        //Apply positive torque in all wheels to go forward if maxSpeed has not been reached.
        frontLeftCollider.brakeTorque = 0;
        frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
        frontRightCollider.brakeTorque = 0;
        frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
        rearLeftCollider.brakeTorque = 0;
        rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
        rearRightCollider.brakeTorque = 0;
        rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
      }
      else
      {
        // If the maxSpeed has been reached, then stop applying torque to the wheels.
        // IMPORTANT: The maxSpeed variable should be considered as an approximation; the speed of the car
        // could be a bit higher than expected.
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
      }
    }
  }

  public void Nitro()
  {
    if (!nitroFlag && nitroValue <= 10)
    {
      nitroValue += Time.deltaTime / 2.5f;
    }
    else
    {
      nitroValue -= (nitroValue <= 0f) ? 0f : Time.deltaTime * 1.6f;
    }
    if (nitroValue < 0f)
    {
      nitroValue = 0f;
      nitroFlag = false;
    }
    if (nitroValue > 10)
    {
      nitroValue = 10;
    }
    if (nitroFlag)
    {
      if (nitroValue > 0f)
      {
        if (useEffects && NitroParticleSystemOne != null && nitroValue > 0f)
        {
          if (!NitroParticleSystemOne.isPlaying)
          {
            NitroParticleSystemOne.Play();
          }
          if (twoNitro && NitroParticleSystemTwo != null && !NitroParticleSystemTwo.isPlaying)
          {
            NitroParticleSystemTwo.Play();
          }
          accelerationMultiplier = nitroAccelLimit + 5;
        }
        else
        {
          if (NitroParticleSystemOne != null && NitroParticleSystemOne.isPlaying)
          {
            NitroParticleSystemOne.Stop();
          }
          if (twoNitro && NitroParticleSystemTwo != null && NitroParticleSystemTwo.isPlaying)
          {
            NitroParticleSystemTwo.Stop();
          }

        }
      }
    }
    else
    {
      if (useEffects && NitroParticleSystemOne != null && NitroParticleSystemOne.isPlaying)
      {
        NitroParticleSystemOne.Stop();
      }
      if (twoNitro && NitroParticleSystemTwo != null)
      {
        NitroParticleSystemTwo.Stop();
      }
      accelerationMultiplier = nitroAccelLimit;
      nitroFlag = false;
    }

  }

  private void BoostFOV()
  {
    if (nitroFlag && camera != null && nitroValue > 0f)
    {
      camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 84f, Time.deltaTime * 2f);
    }
    else if (!nitroFlag && camera != null)
    {
      camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 60f, Time.deltaTime * 2f);
    }
  }

  // This method apply negative torque to the wheels in order to go backwards.
  public void GoReverse()
  {
    //If the forces aplied to the rigidbody in the 'x' axis are greater than
    //3f, it means that the car is losing traction, then the car will start emitting particle systems.
    //If the wheels are in the air, do not emit the particle systems.
    if (Mathf.Abs(localVelocityX) > 3f)
    {
      isDrifting = true;
      DriftCarPS();
    }
    else
    {
      isDrifting = false;
      DriftCarPS();
    }
    // The following part sets the throttle power to -1 smoothly.
    throttleAxis = throttleAxis - (Time.deltaTime * 3f);
    if (throttleAxis < -1f)
    {
      throttleAxis = -1f;
    }
    //If the car is still going forward, then apply brakes in order to avoid strange
    //behaviours. If the local velocity in the 'z' axis is greater than 1f, then it
    //is safe to apply negative torque to go reverse.
    if (localVelocityZ > 1f)
    {
      Brakes();
    }
    else
    {
      if (Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed)
      {
        //Apply negative torque in all wheels to go in reverse if maxReverseSpeed has not been reached.
        frontLeftCollider.brakeTorque = 0;
        frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
        frontRightCollider.brakeTorque = 0;
        frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
        rearLeftCollider.brakeTorque = 0;
        rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
        rearRightCollider.brakeTorque = 0;
        rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
      }
      else
      {
        //If the maxReverseSpeed has been reached, then stop applying torque to the wheels.
        // IMPORTANT: The maxReverseSpeed variable should be considered as an approximation; the speed of the car
        // could be a bit higher than expected.
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
      }
    }
  }

  //The following function set the motor torque to 0 (in case the user is not pressing either W or S).
  public void ThrottleOff()
  {
    frontLeftCollider.motorTorque = 0;
    frontRightCollider.motorTorque = 0;
    rearLeftCollider.motorTorque = 0;
    rearRightCollider.motorTorque = 0;
  }

  // The following method decelerates the speed of the car according to the decelerationMultiplier variable, where
  // 1 is the slowest and 10 is the fastest deceleration. This method is called by the function InvokeRepeating,
  // usually every 0.1f when the user is not pressing W (throttle), S (reverse) or Space bar (handbrake).
  public void DecelerateCar()
  {
    if (Mathf.Abs(localVelocityX) > 2.5f)
    {
      isDrifting = true;
      DriftCarPS();
    }
    else
    {
      isDrifting = false;
      DriftCarPS();
    }
    // The following part resets the throttle power to 0 smoothly.
    if (throttleAxis != 0f)
    {
      if (throttleAxis > 0f)
      {
        throttleAxis = throttleAxis - (Time.deltaTime * 10f);
      }
      else if (throttleAxis < 0f)
      {
        throttleAxis = throttleAxis + (Time.deltaTime * 10f);
      }
      if (Mathf.Abs(throttleAxis) < 0.15f)
      {
        throttleAxis = 0f;
      }
    }
    carRigidbody.linearVelocity = carRigidbody.linearVelocity * (1f / (1f + (0.025f * decelerationMultiplier)));
    // Since we want to decelerate the car, we are going to remove the torque from the wheels of the car.
    frontLeftCollider.motorTorque = 0;
    frontRightCollider.motorTorque = 0;
    rearLeftCollider.motorTorque = 0;
    rearRightCollider.motorTorque = 0;
    // If the magnitude of the car's velocity is less than 0.25f (very slow velocity), then stop the car completely and
    // also cancel the invoke of this method.
    if (carRigidbody.linearVelocity.magnitude < 1f)
    {
      carRigidbody.linearVelocity = Vector3.zero;
      CancelInvoke("DecelerateCar");
    }
  }

  // This function applies brake torque to the wheels according to the brake force given by the user.
  public void Brakes()
  {
    frontLeftCollider.brakeTorque = brakeForce;
    frontRightCollider.brakeTorque = brakeForce;
    rearLeftCollider.brakeTorque = brakeForce;
    rearRightCollider.brakeTorque = brakeForce;
  }

  // This function is used to make the car lose traction. By using this, the car will start drifting. The amount of traction lost
  // will depend on the handbrakeDriftMultiplier variable. If this value is small, then the car will not drift too much, but if
  // it is high, then you could make the car to feel like going on ice.
  public void Handbrake()
  {
    CancelInvoke("RecoverTraction");
    // We are going to start losing traction smoothly, there is were our 'driftingAxis' variable takes
    // place. This variable will start from 0 and will reach a top value of 1, which means that the maximum
    // drifting value has been reached. It will increase smoothly by using the variable Time.deltaTime.
    driftingAxis = driftingAxis + (Time.deltaTime);
    float secureStartingPoint = driftingAxis * FLWextremumSlip * handbrakeDriftMultiplier;

    if (secureStartingPoint < FLWextremumSlip)
    {
      driftingAxis = FLWextremumSlip / (FLWextremumSlip * handbrakeDriftMultiplier);
    }
    if (driftingAxis > 1f)
    {
      driftingAxis = 1f;
    }
    //If the forces aplied to the rigidbody in the 'x' asis are greater than
    //3f, it means that the car lost its traction, then the car will start emitting particle systems.
    if (Mathf.Abs(localVelocityX) > 3f)
    {
      isDrifting = true;
    }
    else
    {
      isDrifting = false;
    }
    //If the 'driftingAxis' value is not 1f, it means that the wheels have not reach their maximum drifting
    //value, so, we are going to continue increasing the sideways friction of the wheels until driftingAxis
    // = 1f.
    if (driftingAxis < 1f)
    {
      FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
      frontLeftCollider.sidewaysFriction = FLwheelFriction;

      FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
      frontRightCollider.sidewaysFriction = FRwheelFriction;

      RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
      rearLeftCollider.sidewaysFriction = RLwheelFriction;

      RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
      rearRightCollider.sidewaysFriction = RRwheelFriction;
    }

    // Whenever the player uses the handbrake, it means that the wheels are locked, so we set 'isTractionLocked = true'
    // and, as a consequense, the car starts to emit trails to simulate the wheel skids.
    isTractionLocked = true;
    DriftCarPS();

  }

  // This function is used to emit both the particle systems of the tires' smoke and the trail renderers of the tire skids
  // depending on the value of the bool variables 'isDrifting' and 'isTractionLocked'.
  public void DriftCarPS()
  {

    if (useEffects)
    {
      try
      {
        if (isDrifting && rearLeftCollider.isGrounded && rearRightCollider.isGrounded)
        {
          RLWParticleSystem.Play();
          RRWParticleSystem.Play();
        }
        else if (!isDrifting)
        {
          RLWParticleSystem.Stop();
          RRWParticleSystem.Stop();
        }
      }
      catch (Exception ex)
      {
        Debug.LogWarning(ex);
      }

      try
      {
        if ((isTractionLocked || Mathf.Abs(localVelocityX) > 5f) && Mathf.Abs(carSpeed) > 12f &&
       rearLeftCollider.isGrounded && rearRightCollider.isGrounded)
        {
          RLWTireSkid.emitting = true;
          RRWTireSkid.emitting = true;
        }
        else
        {
          RLWTireSkid.emitting = false;
          RRWTireSkid.emitting = false;
        }
      }
      catch (Exception ex)
      {
        Debug.LogWarning(ex);
      }
    }
    else if (!useEffects)
    {
      if (RLWParticleSystem != null)
      {
        RLWParticleSystem.Stop();
      }
      if (RRWParticleSystem != null)
      {
        RRWParticleSystem.Stop();
      }
      if (RLWTireSkid != null)
      {
        RLWTireSkid.emitting = false;
      }
      if (RRWTireSkid != null)
      {
        RRWTireSkid.emitting = false;
      }
    }

  }

  // This function is used to recover the traction of the car when the user has stopped using the car's handbrake.
  public void RecoverTraction()
  {
    isTractionLocked = false;
    driftingAxis = driftingAxis - (Time.deltaTime / 1.5f);
    if (driftingAxis < 0f)
    {
      driftingAxis = 0f;
    }

    //If the 'driftingAxis' value is not 0f, it means that the wheels have not recovered their traction.
    //We are going to continue decreasing the sideways friction of the wheels until we reach the initial
    // car's grip.
    if (FLwheelFriction.extremumSlip > FLWextremumSlip)
    {
      FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
      frontLeftCollider.sidewaysFriction = FLwheelFriction;

      FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
      frontRightCollider.sidewaysFriction = FRwheelFriction;

      RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
      rearLeftCollider.sidewaysFriction = RLwheelFriction;

      RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
      rearRightCollider.sidewaysFriction = RRwheelFriction;

      Invoke("RecoverTraction", Time.deltaTime);

    }
    else if (FLwheelFriction.extremumSlip < FLWextremumSlip)
    {
      FLwheelFriction.extremumSlip = FLWextremumSlip;
      frontLeftCollider.sidewaysFriction = FLwheelFriction;

      FRwheelFriction.extremumSlip = FRWextremumSlip;
      frontRightCollider.sidewaysFriction = FRwheelFriction;

      RLwheelFriction.extremumSlip = RLWextremumSlip;
      rearLeftCollider.sidewaysFriction = RLwheelFriction;

      RRwheelFriction.extremumSlip = RRWextremumSlip;
      rearRightCollider.sidewaysFriction = RRwheelFriction;

      driftingAxis = 0f;
    }
  }

  private void AIDrive()
  {
    if (currentWaypoint == null) return;

    // Direction to waypoint
    Vector3 localTarget = transform.InverseTransformPoint(currentWaypoint.transform.position);
    float distanceToWaypoint = localTarget.magnitude;

    // Steering
    float steer = localTarget.x / localTarget.magnitude;

    // Set steering axis
    steeringAxis = Mathf.MoveTowards(steeringAxis, steer, Time.deltaTime * 2f);
    var steeringAngle = steeringAxis * maxSteeringAngle;
    frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, (steeringSpeed * AISteerSpeedMultiplier));
    frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, (steeringSpeed * AISteerSpeedMultiplier));
    throttleAxis = 0.6f;

    // Throttle
    if (distanceToWaypoint > 20f)
    {
      GoForward();
    }
    else
    {
      ThrottleOff();
    }

    // AI stuck detection
    float absSpeed = Mathf.Abs(carSpeed);
    if (absSpeed < speedThreshold)
    {
      stuckTimer += Time.deltaTime;
      if (stuckTimer > stuckThreshold)
      {
        ResetToPreviousWaypoint();
        stuckTimer = 0f;
      }
    }
    else
    {
      stuckTimer = 0f;
    }

    // Optional
    if (distanceToWaypoint < 5f) Brakes();

    // AI ramming
    if (ramCooldown <= 0f && (carRigidbody.linearVelocity.magnitude * 3.6f) >= 100f && ramCoroutine == null)
    {
        foreach (var otherCar in FindObjectsOfType<PrometeoCarController>())
        {
            if (otherCar == this || otherCar.currentHealth <= 0) continue;

            Vector3 toOther = otherCar.transform.position - transform.position;
            float sideDist = Vector3.Dot(transform.right, toOther);
            float forwardDist = Vector3.Dot(transform.forward, toOther);

            // Only consider cars close enough
            if (Mathf.Abs(forwardDist) < 6f && Mathf.Abs(sideDist) > 2f && Mathf.Abs(sideDist) < 5f)
            {
                // 25% chance to ram
                if (UnityEngine.Random.value < 0.25f)
                {
                    if (sideDist > 0)
                        ramCoroutine = StartCoroutine(Ram(1)); // Ram right
                    else
                        ramCoroutine = StartCoroutine(Ram(-1)); // Ram left
                    break;
                }
            }
        }
    }
  }

  private void Wrecked()
  {
    // If wrecked, stop all movement and set health to 0.
    frontLeftCollider.motorTorque = 0;
    frontRightCollider.motorTorque = 0;
    rearLeftCollider.motorTorque = 0;
    rearRightCollider.motorTorque = 0;
    frontLeftCollider.brakeTorque = brakeForce;
    frontRightCollider.brakeTorque = brakeForce;
    rearLeftCollider.brakeTorque = brakeForce;
    rearRightCollider.brakeTorque = brakeForce;
    currentHealth = 0;
    carEngineSound.Stop();
    tireScreechSound.Stop();
  }

  private IEnumerator Ram(int direction)
  {
    ramCooldown = 2f;
    float ramForce = 27500f;
    float ramDuration = 0.2f;
    isRamming = true;
    carRigidbody.angularVelocity = Vector3.zero;
    carRigidbody.AddForce(transform.right * direction * ramForce, ForceMode.Impulse);

    if (useSounds && ramSound != null)
    {
      ramSound.Play();
    }

    yield return new WaitForSeconds(ramDuration);
    isRamming = false;
    Vector3 localVel = transform.InverseTransformDirection(carRigidbody.linearVelocity);
    localVel.x = 0f;
    carRigidbody.linearVelocity = transform.TransformDirection(localVel);

    ramCoroutine = null;
  }

  private void ApplyLightDamageEffect()
  {
    LightDamageParticleSystem.Play();
  }
  private void ApplyMediumDamageEffect()
  {
    LightDamageParticleSystem.Stop();
    MediumDamageParticleSystem.Play();
  }
  private void ApplyHeavyDamageEffect()
  {
    LightDamageParticleSystem.Stop();
    MediumDamageParticleSystem.Stop();
    HeavyDamageParticleSystem.Play();
  }
  private void ApplyWreckedDamageEffect()
  {
    LightDamageParticleSystem.Stop();
    MediumDamageParticleSystem.Stop();
    HeavyDamageParticleSystem.Play();
    WreckedParticleSystem.Play();
  }

  // Logic for vehicle ram collisions
  void OnCollisionEnter(Collision collision)
  {
      // Only run if this car is ramming
      if (!isRamming)
      {
        return;
      }

      // Try to get the other car
      PrometeoCarController victimCar = collision.gameObject.GetComponent<PrometeoCarController>();
      if (victimCar == null || victimCar == this){
        return;
      }

      // Check if already rammed recently
      if (ramHitCooldowns.ContainsKey(victimCar))
      {
        return;
      }

      nitroValue = Mathf.Min(nitroValue + 5, 10); // Don't pass the max of 10
      // Victim loses health equivalent to attacker's max health * 0.2
      // Cars with more maximum health deal more damage
      victimCar.currentHealth -= (int)(health * 0.2f);

      // Add sound here
      if (useSounds && victimCar.ramHitSound != null)
      {
        victimCar.ramHitSound.Play();
      }

      // Add cooldown to avoid damage repeat
      ramHitCooldowns[victimCar] = ramHitDelay;
  }
}