using UnityEngine;

namespace CrazyGames
{
    public class BannerModuleDemo : MonoBehaviour
    {
        public CrazyBanner bannerPrefab;

        private void Start()
        {
            CrazySDK.Init(UpdateBannersDisplay);
        }

        public void UpdateBannersDisplay()
        {
            CrazySDK.Banner.RefreshBanners();
        }

        public void DisableLastBanner()
        {
            var banners = FindObjectsOfType<CrazyBanner>();
            foreach (var banner in banners)
            {
                if (!banner.IsVisible()) continue;
                banner.gameObject.SetActive(false);
                return;
            }
        }

        public void AddBanner()
        {
            var banner = Instantiate(bannerPrefab, new Vector3(), new Quaternion(),
                GameObject.Find("Banners").transform);
            banner.Size = (CrazyBanner.BannerSize)Random.Range(0, 3);
            banner.Position = new Vector2(Random.Range(-461, 461), Random.Range(-243, 243));
        }

        public void HideAllBanners()
        {
            // when leaving this scene in a browser build, hide all banners
            var banners = FindObjectsOfType<CrazyBanner>();
            foreach (var banner in banners)
            {
                banner.gameObject.SetActive(false);
            }

            CrazySDK.Banner.RefreshBanners();
        }
    }
}