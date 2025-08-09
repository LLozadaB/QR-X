using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(PrometeoCarController))]
[System.Serializable]
public class PrometeoEditor : Editor{

  enum displayFieldType {DisplayAsAutomaticFields, DisplayAsCustomizableGUIFields}
  displayFieldType DisplayFieldType;

  private PrometeoCarController prometeo;
  private SerializedObject SO;
  //
  //
  //CAR SETUP
  //
  //
  private SerializedProperty maxSpeed;
  private SerializedProperty maxReverseSpeed;
  private SerializedProperty accelerationMultiplier;
  private SerializedProperty maxSteeringAngle;
  private SerializedProperty steeringSpeed;
  private SerializedProperty brakeForce;
  private SerializedProperty decelerationMultiplier;
  private SerializedProperty handbrakeDriftMultiplier;
  private SerializedProperty bodyMassCenter;
  private SerializedProperty health;
  private SerializedProperty HealthSlider;
  //
  //
  //WHEELS VARIABLES
  //
  //
  private SerializedProperty frontLeftMesh;
  private SerializedProperty frontLeftCollider;
  private SerializedProperty frontRightMesh;
  private SerializedProperty frontRightCollider;
  private SerializedProperty rearLeftMesh;
  private SerializedProperty rearLeftCollider;
  private SerializedProperty rearRightMesh;
  private SerializedProperty rearRightCollider;
  //
  //
  //PARTICLE SYSTEMS' VARIABLES
  //
  //
  private SerializedProperty useEffects;
  private SerializedProperty RLWParticleSystem;
  private SerializedProperty RRWParticleSystem;
  private SerializedProperty camera;
  private SerializedProperty NitroParticleSystemOne;
  private SerializedProperty twoNitro;
  private SerializedProperty NitroParticleSystemTwo;
  private SerializedProperty RLWTireSkid;
  private SerializedProperty RRWTireSkid;
  private SerializedProperty LightDamageParticleSystem;
  private SerializedProperty MediumDamageParticleSystem;
  private SerializedProperty HeavyDamageParticleSystem;
  private SerializedProperty WreckedParticleSystem;
  //
  //
  //SPEED TEXT (UI) VARIABLES
  //
  //
  private SerializedProperty useUI;
  private SerializedProperty carSpeedText;
  private SerializedProperty NitroSlider; // Used to store the UI slider that shows the nitro value of the car.
  //
  //
  //SPEED TEXT (UI) VARIABLES
  //
  //
  private SerializedProperty useSounds;
  private SerializedProperty carEngineSound;
  private SerializedProperty tireScreechSound;
  private SerializedProperty initialCarEngineSoundPitch;
  private SerializedProperty ramSound;
  private SerializedProperty ramHitSound;
  //
  //
  //TOUCH CONTROLS VARIABLES
  //
  //
  private SerializedProperty useTouchControls;
  private SerializedProperty throttleButton;
  private SerializedProperty reverseButton;
  private SerializedProperty turnRightButton;
  private SerializedProperty turnLeftButton;
  private SerializedProperty handbrakeButton;
  private SerializedProperty DriveController;
  private SerializedProperty waypoints;
  private SerializedProperty nodes;
  private SerializedProperty distanceOffset;
  private SerializedProperty AISteerSpeedMultiplier;
  private void OnEnable()
  {
    prometeo = (PrometeoCarController)target;
    SO = new SerializedObject(target);

    maxSpeed = SO.FindProperty("maxSpeed");
    maxReverseSpeed = SO.FindProperty("maxReverseSpeed");
    accelerationMultiplier = SO.FindProperty("accelerationMultiplier");
    maxSteeringAngle = SO.FindProperty("maxSteeringAngle");
    steeringSpeed = SO.FindProperty("steeringSpeed");
    brakeForce = SO.FindProperty("brakeForce");
    decelerationMultiplier = SO.FindProperty("decelerationMultiplier");
    handbrakeDriftMultiplier = SO.FindProperty("handbrakeDriftMultiplier");
    bodyMassCenter = SO.FindProperty("bodyMassCenter");
    health = SO.FindProperty("health");
    HealthSlider = SO.FindProperty("HealthSlider");

    frontLeftMesh = SO.FindProperty("frontLeftMesh");
    frontLeftCollider = SO.FindProperty("frontLeftCollider");
    frontRightMesh = SO.FindProperty("frontRightMesh");
    frontRightCollider = SO.FindProperty("frontRightCollider");
    rearLeftMesh = SO.FindProperty("rearLeftMesh");
    rearLeftCollider = SO.FindProperty("rearLeftCollider");
    rearRightMesh = SO.FindProperty("rearRightMesh");
    rearRightCollider = SO.FindProperty("rearRightCollider");

    useEffects = SO.FindProperty("useEffects");
    RLWParticleSystem = SO.FindProperty("RLWParticleSystem");
    RRWParticleSystem = SO.FindProperty("RRWParticleSystem");
    camera = SO.FindProperty("camera");
    NitroParticleSystemOne = SO.FindProperty("NitroParticleSystemOne");
    twoNitro = SO.FindProperty("twoNitro");
    NitroParticleSystemTwo = SO.FindProperty("NitroParticleSystemTwo");
    RLWTireSkid = SO.FindProperty("RLWTireSkid");
    RRWTireSkid = SO.FindProperty("RRWTireSkid");
    LightDamageParticleSystem = SO.FindProperty("LightDamageParticleSystem");
    MediumDamageParticleSystem = SO.FindProperty("MediumDamageParticleSystem");
    HeavyDamageParticleSystem = SO.FindProperty("HeavyDamageParticleSystem");
    WreckedParticleSystem = SO.FindProperty("WreckedParticleSystem");

    useUI = SO.FindProperty("useUI");
    carSpeedText = SO.FindProperty("carSpeedText");
    NitroSlider = SO.FindProperty("NitroSlider");

    useSounds = SO.FindProperty("useSounds");
    carEngineSound = SO.FindProperty("carEngineSound");
    tireScreechSound = SO.FindProperty("tireScreechSound");
    initialCarEngineSoundPitch = SO.FindProperty("initialCarEngineSoundPitch");
    ramSound = SO.FindProperty("ramSound");
    ramHitSound = SO.FindProperty("ramHitSound");

    useTouchControls = SO.FindProperty("useTouchControls");
    throttleButton = SO.FindProperty("throttleButton");
    reverseButton = SO.FindProperty("reverseButton");
    turnRightButton = SO.FindProperty("turnRightButton");
    turnLeftButton = SO.FindProperty("turnLeftButton");
    handbrakeButton = SO.FindProperty("handbrakeButton");

    DriveController = SO.FindProperty("DriveController");
    waypoints = SO.FindProperty("waypoints");
    nodes = SO.FindProperty("nodes");
    distanceOffset = SO.FindProperty("distanceOffset");
    AISteerSpeedMultiplier = SO.FindProperty("AISteerSpeedMultiplier");

  }

  public override void OnInspectorGUI(){

    SO.Update();

    GUILayout.Space(25);
    GUILayout.Label("CAR SETUP", EditorStyles.boldLabel);
    GUILayout.Space(10);
    //
    //
    //CAR SETUP
    //
    //
    //
    maxSpeed.intValue = EditorGUILayout.IntSlider("Max Speed:", maxSpeed.intValue, 50, 500);
    maxReverseSpeed.intValue = EditorGUILayout.IntSlider("Max Reverse Speed:", maxReverseSpeed.intValue, 60, 120);
    accelerationMultiplier.intValue = EditorGUILayout.IntSlider("Acceleration Multiplier:", accelerationMultiplier.intValue, 3, 12);
    maxSteeringAngle.intValue = EditorGUILayout.IntSlider("Max Steering Angle:", maxSteeringAngle.intValue, 10, 45);
    steeringSpeed.floatValue = EditorGUILayout.Slider("Steering Speed:", steeringSpeed.floatValue, 0.1f, 1f);
    brakeForce.intValue = EditorGUILayout.IntSlider("Brake Force:", brakeForce.intValue, 100, 600);
    decelerationMultiplier.intValue = EditorGUILayout.IntSlider("Deceleration Multiplier:", decelerationMultiplier.intValue, 1, 10);
    handbrakeDriftMultiplier.intValue = EditorGUILayout.IntSlider("Drift Multiplier:", handbrakeDriftMultiplier.intValue, 1, 10);
    EditorGUILayout.PropertyField(bodyMassCenter, new GUIContent("Mass Center of Car: "));
    health.intValue = EditorGUILayout.IntSlider("Health: ", health.intValue, 250, 750);

    //
    //
    //WHEELS
    //
    //

    GUILayout.Space(25);
    GUILayout.Label("WHEELS", EditorStyles.boldLabel);
    GUILayout.Space(10);

    EditorGUILayout.PropertyField(frontLeftMesh, new GUIContent("Front Left Mesh: "));
    EditorGUILayout.PropertyField(frontLeftCollider, new GUIContent("Front Left Collider: "));

    EditorGUILayout.PropertyField(frontRightMesh, new GUIContent("Front Right Mesh: "));
    EditorGUILayout.PropertyField(frontRightCollider, new GUIContent("Front Right Collider: "));

    EditorGUILayout.PropertyField(rearLeftMesh, new GUIContent("Rear Left Mesh: "));
    EditorGUILayout.PropertyField(rearLeftCollider, new GUIContent("Rear Left Collider: "));

    EditorGUILayout.PropertyField(rearRightMesh, new GUIContent("Rear Right Mesh: "));
    EditorGUILayout.PropertyField(rearRightCollider, new GUIContent("Rear Right Collider: "));

    //
    //
    //EFFECTS
    //
    //

    GUILayout.Space(25);
    GUILayout.Label("EFFECTS", EditorStyles.boldLabel);
    GUILayout.Space(10);

    useEffects.boolValue = EditorGUILayout.BeginToggleGroup("Use effects (particle systems)?", useEffects.boolValue);
    GUILayout.Space(10);

        EditorGUILayout.PropertyField(RLWParticleSystem, new GUIContent("Rear Left Particle System: "));
        EditorGUILayout.PropertyField(RRWParticleSystem, new GUIContent("Rear Right Particle System: "));
        EditorGUILayout.PropertyField(camera, new GUIContent("Camera: "));
        EditorGUILayout.PropertyField(NitroParticleSystemOne, new GUIContent("Nitro One: "));
        twoNitro.boolValue = EditorGUILayout.BeginToggleGroup("Two nitro ports?", twoNitro.boolValue);
            
            EditorGUILayout.PropertyField(NitroParticleSystemTwo, new GUIContent("Nitro Two: "));

        EditorGUILayout.EndToggleGroup();

        EditorGUILayout.PropertyField(RLWTireSkid, new GUIContent("Rear Left Trail Renderer: "));
        EditorGUILayout.PropertyField(RRWTireSkid, new GUIContent("Rear Right Trail Renderer: "));
        EditorGUILayout.PropertyField(LightDamageParticleSystem, new GUIContent("Light Damage Particle System: "));
        EditorGUILayout.PropertyField(MediumDamageParticleSystem, new GUIContent("Medium Damage Particle System: "));
        EditorGUILayout.PropertyField(HeavyDamageParticleSystem, new GUIContent("Heavy Damage Particle System: "));
        EditorGUILayout.PropertyField(WreckedParticleSystem, new GUIContent("Wrecked Particle System: "));

    EditorGUILayout.EndToggleGroup();

    //
    //
    //UI
    //
    //

    GUILayout.Space(25);
    GUILayout.Label("UI", EditorStyles.boldLabel);
    GUILayout.Space(10);

    useUI.boolValue = EditorGUILayout.BeginToggleGroup("Use UI (Speed text)?", useUI.boolValue);
    GUILayout.Space(10);

        EditorGUILayout.PropertyField(carSpeedText, new GUIContent("Speed Text (UI): "));
        NitroSlider.objectReferenceValue = EditorGUILayout.ObjectField("Nitro Slider (UI): ", NitroSlider.objectReferenceValue, typeof(UnityEngine.UI.Slider), true);
        HealthSlider.objectReferenceValue = EditorGUILayout.ObjectField("Health Slider (UI): ", HealthSlider.objectReferenceValue, typeof(UnityEngine.UI.Slider), true);

    EditorGUILayout.EndToggleGroup();

    //
    //
    //SOUNDS
    //
    //

    GUILayout.Space(25);
    GUILayout.Label("SOUNDS", EditorStyles.boldLabel);
    GUILayout.Space(10);

    useSounds.boolValue = EditorGUILayout.BeginToggleGroup("Use sounds (car sounds)?", useSounds.boolValue);
    GUILayout.Space(10);

        EditorGUILayout.PropertyField(carEngineSound, new GUIContent("Car Engine Sound: "));
        EditorGUILayout.PropertyField(tireScreechSound, new GUIContent("Tire Screech Sound: "));
        initialCarEngineSoundPitch.floatValue = EditorGUILayout.Slider("Car Engine Sound Pitch:", initialCarEngineSoundPitch.floatValue, 0f, 5f);
        EditorGUILayout.PropertyField(ramSound, new GUIContent("Ram Sound: "));
        EditorGUILayout.PropertyField(ramHitSound, new GUIContent("Ram Hit Sound: "));

    EditorGUILayout.EndToggleGroup();

    //
    //
    //TOUCH CONTROLS
    //
    //

    GUILayout.Space(25);
    GUILayout.Label("TOUCH CONTROLS", EditorStyles.boldLabel);
    GUILayout.Space(10);

    useTouchControls.boolValue = EditorGUILayout.BeginToggleGroup("Use touch controls (mobile devices)?", useTouchControls.boolValue);
    GUILayout.Space(10);

        EditorGUILayout.PropertyField(throttleButton, new GUIContent("Throttle Button: "));
        EditorGUILayout.PropertyField(reverseButton, new GUIContent("Brakes/Reverse Button: "));
        EditorGUILayout.PropertyField(turnLeftButton, new GUIContent("Turn Left Button: "));
        EditorGUILayout.PropertyField(turnRightButton, new GUIContent("Turn Right Button: "));
        EditorGUILayout.PropertyField(handbrakeButton, new GUIContent("Handbrake Button: "));

    EditorGUILayout.EndToggleGroup();

    GUILayout.Space(25);
    GUILayout.Label("WAYPOINTS", EditorStyles.boldLabel);
    GUILayout.Space(10);
    // This section is for the TrackWaypoints script, which is not part of the PrometeoCarController but is included for completeness.

        EditorGUILayout.PropertyField(DriveController, new GUIContent("Drive Controller: "));
        EditorGUILayout.PropertyField(waypoints, new GUIContent("Track Waypoints: "));
        EditorGUILayout.PropertyField(nodes, new GUIContent("Waypoints Nodes: "));
        EditorGUILayout.PropertyField(distanceOffset, new GUIContent("Distance Offset: "));
        EditorGUILayout.PropertyField(AISteerSpeedMultiplier, new GUIContent("AI Steer Speed: "));

    //END

    GUILayout.Space(10);
    SO.ApplyModifiedProperties();

  }

}
