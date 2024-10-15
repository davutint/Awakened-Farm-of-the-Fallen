/// <summary>
/// This script belongs to cowsins™ as a part of the cowsins´ FPS Engine. All rights reserved. 
/// </summary>
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace cowsins
{
    public class WeaponSway : MonoBehaviour
    {
        #region shared
        [System.Serializable]
        public enum SwayMethod
        {
            Simple, PivotBased
        }
        public SwayMethod swayMethod;
        public delegate void Sway();

        public Sway sway;
        #endregion
        #region simple
        [Header("Position")]
        [SerializeField] private float amount = 0.02f;
        [SerializeField] private float maxAmount = 0.06f;

        [Header("Tilting")]
        [SerializeField] private float tiltAmount = 4f;
        [SerializeField] private float maxTiltAmount = 5f;

        [Header("Spring Settings")]
        [SerializeField, Range(2, 500)] private float springStiffness = 80f;
        [SerializeField, Range(1, 24)] private float springDamping = 8f;
        [SerializeField, ReadOnly] private AnimationCurve springCurvePosition;
        [SerializeField, ReadOnly] private AnimationCurve springCurveRotation;

        [SerializeField, Range(.1f, 1)] private float aimingMultiplier;
        private WeaponController player;

        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private Vector3 pivotVelocity;
        private Vector3 pivotAngularVelocity;


        private float InputX;
        private float InputY;
        private float playerMultiplier;

        private Vector3 velocity;
        private Vector3 angularVelocity;
        #endregion
        #region pivotBased
        [SerializeField] private Transform pivot;

        [SerializeField] private float swaySpeed;

        [SerializeField] private Vector2 swayMovementAmount;

        [SerializeField] private Vector2 swayRotationAmount;

        [SerializeField] private float swayTiltAmount;


        private Camera cam;
        #endregion

        private void Start()
        {
            cam = Camera.main;
            if (swayMethod == SwayMethod.Simple)
            {
                initialPosition = transform.localPosition;
                initialRotation = transform.localRotation;
                sway = SimpleSway;
            }
            else
            {
                sway = PivotSway;
            }
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponController>();
        }

        private void Update()
        {
            if (!PlayerStats.Controllable) return;
            sway?.Invoke();
        }

        private void SimpleSway()
        {
            CalculateSway();
            MoveSway();
            TiltSway();
        }

        private void CalculateSway()
        {
            InputX = -InputManager.mousex / 10 - 5 * InputManager.controllerx;
            InputY = -InputManager.mousey / 10 - 2 * InputManager.controllery;

            if (player.isAiming) playerMultiplier = 5f;
            else playerMultiplier = 1f;
        }

        private void MoveSway()
        {
            float moveX = Mathf.Clamp(InputX * amount, -maxAmount, maxAmount) / playerMultiplier;
            float moveY = Mathf.Clamp(InputY * amount, -1, 1) / playerMultiplier;

            Vector3 targetPosition = new Vector3(moveX, moveY, 0) + initialPosition;

            Vector3 displacement = targetPosition - transform.localPosition;
            Vector3 springForce = displacement * springStiffness;
            Vector3 dampingForce = velocity * springDamping;

            Vector3 force = springForce - dampingForce;
            velocity += force * Time.deltaTime;

            transform.localPosition += velocity * Time.deltaTime;
        }

        private void TiltSway()
        {
            float moveX = Mathf.Clamp(InputX * tiltAmount, -maxTiltAmount, maxTiltAmount) / playerMultiplier;

            Quaternion targetRotation = Quaternion.Euler(0, 0, moveX) * initialRotation;

            Quaternion deltaRotation = targetRotation * Quaternion.Inverse(transform.localRotation);
            deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
            angle = Mathf.Clamp(angle, -180, 180);
            Vector3 angularDisplacement = axis * (angle * Mathf.Deg2Rad);

            Vector3 springTorque = angularDisplacement * springStiffness;
            Vector3 dampingTorque = angularVelocity * springDamping;

            Vector3 torque = springTorque - dampingTorque;
            angularVelocity += torque * Time.deltaTime;

            transform.localRotation *= Quaternion.Euler(angularVelocity * Mathf.Rad2Deg * Time.deltaTime);
        }

        private void PivotSway()
        {
            transform.localRotation = HandleSwayRotation();
            transform.localPosition = HandleSwayLocation();
        }

        private Quaternion HandleSwayRotation()
        {
            Quaternion swayRot;

            var right = cam.transform.right;
            right.y = 0f;
            right.Normalize();

            float swayRotationAmountX = player.isAiming ? swayRotationAmount.x * aimingMultiplier : swayRotationAmount.x;
            float swayRotationAmountY = player.isAiming ? swayRotationAmount.y * aimingMultiplier : swayRotationAmount.y;

            // HANDLE HORIZONTAL ROTATION
            transform.RotateAround(pivot.position, new Vector3(0, 1, 0), Time.deltaTime * swayRotationAmountX * -InputManager.mousex);
            // HANDLE VERTICAL ROTATION
            transform.RotateAround(pivot.position, right, Time.deltaTime * swayRotationAmountY * InputManager.mousey);
            // HANDLE TILT ROTATION
            swayRot = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(Vector3.zero), Time.deltaTime * swaySpeed);

            // Apply spring dynamics to the rotation
            Quaternion targetRotation = Quaternion.Euler(0, 0, swayTiltAmount * -InputManager.mousex);
            Quaternion deltaRotation = targetRotation * Quaternion.Inverse(transform.localRotation);
            deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
            angle = Mathf.Clamp(angle, -180, 180);
            Vector3 angularDisplacement = axis * (angle * Mathf.Deg2Rad);

            Vector3 springTorque = angularDisplacement * springStiffness;
            Vector3 dampingTorque = pivotAngularVelocity * springDamping;

            Vector3 torque = springTorque - dampingTorque;
            pivotAngularVelocity += torque * Time.deltaTime;

            transform.localRotation *= Quaternion.Euler(pivotAngularVelocity * Mathf.Rad2Deg * Time.deltaTime);

            // Normalize the quaternion to ensure it's unit-length
            transform.localRotation = Quaternion.Normalize(transform.localRotation);

            return swayRot;
        }
        private Vector3 HandleSwayLocation()
        {
            Vector3 swayPos;

            Vector3 finalPosition = new Vector3(-InputManager.mousex * swayMovementAmount.x, InputManager.mousey * swayMovementAmount.y, 0) / 100;

            Vector3 displacement = finalPosition - transform.localPosition;
            Vector3 springForce = displacement * springStiffness;
            Vector3 dampingForce = pivotVelocity * springDamping;

            Vector3 force = springForce - dampingForce;
            pivotVelocity += force * Time.deltaTime;

            swayPos = transform.localPosition + pivotVelocity * Time.deltaTime;

            return swayPos;
        }

        private void OnValidate()
        {
            UpdateSpringCurves();
        }

        private void UpdateSpringCurves()
        {
            springCurvePosition = new AnimationCurve();
            springCurveRotation = new AnimationCurve();

            float amplitude = 1f; // Assume an initial displacement of 1 unit
            float dampingRatio = springDamping / (2 * Mathf.Sqrt(springStiffness)); // Critical damping ratio
            float angularFrequency = Mathf.Sqrt(springStiffness);

            for (float t = 0; t < 2f; t += 0.01f) // Simulate for 2 seconds
            {
                float dampedAmplitude = amplitude * Mathf.Exp(-dampingRatio * angularFrequency * t);
                float displacement = dampedAmplitude * Mathf.Cos(angularFrequency * Mathf.Sqrt(1 - dampingRatio * dampingRatio) * t);
                springCurvePosition.AddKey(t, displacement);
                springCurveRotation.AddKey(t, displacement * 30); // Scale rotation effect for visibility
            }
        }


    }
#if UNITY_EDITOR
    [CustomEditor(typeof(WeaponSway))]
    public class WeaponSwayEditor : Editor
    {
        override public void OnInspectorGUI()
        {
            serializedObject.Update();
            var myScript = target as WeaponSway;

            EditorGUILayout.LabelField("WEAPON SWAY", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("swayMethod"));
            EditorGUILayout.Space(10f);

            if (myScript.swayMethod == WeaponSway.SwayMethod.Simple)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("POSITION");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("amount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxAmount"));
                EditorGUILayout.LabelField("ROTATION");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("tiltAmount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxTiltAmount"));
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("pivot"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("swaySpeed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("swayMovementAmount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("swayRotationAmount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("swayTiltAmount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("aimingMultiplier"));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space(5f);
            EditorGUILayout.LabelField("SPRING SETTINGS");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("springStiffness"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("springDamping"));

            EditorGUILayout.Space(5f);
            EditorGUILayout.LabelField("SPRING CURVES [DEBUG]");
            var springCurvePositionProperty = serializedObject.FindProperty("springCurvePosition");
            var springCurveRotationProperty = serializedObject.FindProperty("springCurveRotation");

            EditorGUILayout.CurveField(springCurvePositionProperty, Color.green, new Rect(0, -1, 1, 2), GUILayout.Height(100));
            EditorGUILayout.CurveField(springCurveRotationProperty, Color.green, new Rect(0, -1, 1, 2), GUILayout.Height(100));


            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

}
