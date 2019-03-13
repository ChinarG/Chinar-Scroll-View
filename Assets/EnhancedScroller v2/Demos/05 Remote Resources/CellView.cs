using UnityEngine;
using UnityEngine.UI;
using ChinarUi.ScrollView;
using System.Collections;

namespace EnhancedCScrollViewDemos.RemoteResourcesDemo
{
    public class UnitView : CScrollUnitUi
    {
        public Image unitImage;
        public Sprite defaultSprite;

        public void SetData(Data data)
        {
            StartCoroutine(LoadRemoteImage(data));
        }

        public IEnumerator LoadRemoteImage(Data data)
        {
            string path = data.imageUrl;
            WWW www = new WWW(path);
            yield return www;

            unitImage.sprite = Sprite.Create(www.texture, new Rect(0, 0, data.imageDimensions.x, data.imageDimensions.y), new Vector2(0, 0), data.imageDimensions.x);
        }

        public void ClearImage()
        {
            unitImage.sprite = defaultSprite;
        }
    }
}