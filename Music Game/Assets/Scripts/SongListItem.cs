using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class SongListItem : MonoBehaviour, ISelectHandler {
        public MapJson MapJson { get; set; }
        // Use this for initialization
        private void Start()
        {
       
        }

        public void OnSelect(BaseEventData eventData)
        {
            var ob = GameObject.Find("SelectedMapPanel").transform.GetComponent<HandleSelections>();
            ob.Selected = this;
            ob.UpdateMapInfoPanel();
            GameObject.Find("Canvas").transform.GetComponent<Image>().sprite =
                LoadNewSprite(MapJson.filePath + @"/Background.jpg");
        }
        public void UpdateText ()
        {
            transform.GetChild(0).GetComponent<Text>().text = MapJson.artist + " - " + MapJson.title;
            transform.GetChild(1).GetComponent<Text>().text = "*" + MapJson.mapCreator;
            var txt = "BPM: " + MapJson.bpm + "  Complexity: " + MapJson.complexity;
            transform.GetChild(2).GetComponent<Text>().text = txt;
        }


        public Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
        {

            // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference


            Texture2D SpriteTexture = LoadTexture(FilePath);
            var newSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);

            return newSprite;
        }

        public Texture2D LoadTexture(string FilePath)
        {

            // Load a PNG or JPG file from disk to a Texture2D
            // Returns null if load fails

            Texture2D Tex2D;
            byte[] FileData;

            if (File.Exists(FilePath))
            {
                FileData = File.ReadAllBytes(FilePath);
                Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
                if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                    return Tex2D;                 // If data = readable -> return texture
            }
            return null;                     // Return null if load failed
        }
    }
}
