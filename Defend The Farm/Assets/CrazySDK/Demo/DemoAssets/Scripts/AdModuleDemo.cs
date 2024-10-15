using UnityEngine;
using UnityEngine.UI;

namespace CrazyGames
{
    [RequireComponent(typeof(Rigidbody))]
    public class AdModuleDemo : MonoBehaviour
    {
        [SerializeField] private CrazyAdType adType = CrazyAdType.Midgame;
        [SerializeField] private Text adTypeText;

        private readonly Vector3 _pushForce = Vector3.right * 2;
        private Rigidbody _rb;
        private Vector3 _startPos;


        private void Start()
        {
            CrazySDK.Init(() => { }); // ensure if starting this scene from editor it is initialized
            _rb = GetComponent<Rigidbody>();
            _startPos = transform.position;
            UpdateAdTypeText();
        }


        private void FixedUpdate()
        {
            transform.Translate(_pushForce * Time.fixedDeltaTime);

            if (!(transform.position.y < -20)) return;
            print("Player Died! Requesting ad " + adType);
            CrazySDK.Ad.RequestAd(adType, () => { print("Ad started"); }, (error) =>
            {
                enabled = false;
                print("Ad error, not respawning: " + error);
            }, () =>
            {
                print("Ad Finished! So respawning player!");
                transform.position = _startPos;
                _rb.velocity = Vector3.zero;
            });
        }


        public void SwitchAdType()
        {
            if (adType == CrazyAdType.Midgame)
            {
                adType = CrazyAdType.Rewarded;
            }
            else
            {
                adType = CrazyAdType.Midgame;
            }

            UpdateAdTypeText();
        }

        private void UpdateAdTypeText()
        {
            adTypeText.text = adType.ToString();
        }
    }
}