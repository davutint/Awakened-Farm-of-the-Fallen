using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
namespace cowsins
{
    public class Compass : MonoBehaviour
    {
        [SerializeField] private Transform cam;
        [SerializeField] private TextMeshProUGUI compassText;

        [SerializeField] private GameObject compassElementIcon;

        private List<CompassElement> compassElements = new List<CompassElement>();
        private RawImage compass;
        public static Compass Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(this);
            else Instance = this;

            compass = GetComponent<RawImage>();
        }

        private void Update()
        {
            compass.uvRect = new Rect(cam.localEulerAngles.y / 360, 0, 1, 1);
            compassText.text = cam.localEulerAngles.y.ToString("F0");

            foreach (CompassElement el in compassElements)
            {
                el.image.rectTransform.anchoredPosition = GetElementPositionInCompass(el);
            }
        }

        // We want to call this to add new compass elements.
        public void AddCompassElement(CompassElement element)
        {
            GameObject newElement = Instantiate(compassElementIcon, compass.transform);
            element.image = newElement.transform.GetChild(0).GetComponent<Image>();
            element.image.sprite = element.icon;
            compassElements.Add(element);
        }

        // We want to call this to remove new compass elements.
        public void RemoveCompassElement(CompassElement element)
        {
            compassElements.Remove(element);
            Destroy(element.image);
        }

        // Calculates the position of the image depending on where the compass element and the players are.
        private Vector2 GetElementPositionInCompass(CompassElement element)
        {
            Vector2 playerPosition = new Vector2(cam.position.x, cam.position.z);
            Vector2 playerForward = new Vector2(cam.forward.x, cam.forward.z);
            float angle = Vector2.SignedAngle(element.GetVector2Pos() - playerPosition, playerForward);

            return new Vector2(angle * compass.rectTransform.rect.width / 360, 0);
        }
    }
}