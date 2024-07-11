using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.IO;
using SFB;
using TMPro;

namespace Survival
{
    public class BrowseFiles : MonoBehaviour
    {

        public TextMeshProUGUI SelectedFile;
        public Image SelectedImage;


        public void BrowseImage()
        {
            BrowseLocalFile("Image", "png", "jpg", "jpeg");
        }

        public async void BrowseLocalFile(string FileType, params string[] extensions)
        {
            await Task.Yield();
            string fullpath = "";

            //Browse file, wait for files to be selected
            string[] files = await OpenFileBrowser("Select " + FileType, FileType + " Files", extensions);


            if (files.Length > 0)
                fullpath = files[0];

            SelectedFile.text = fullpath;

            if (!string.IsNullOrEmpty(fullpath))
                LoadTexture(fullpath);
        }

        public static async Task<string[]> OpenFileBrowser(string windowtitle, string filtername, params string[] filters)
        {
            await Task.Yield();
            ExtensionFilter[] extensions = new[] { new ExtensionFilter(filtername, filters) };
            return StandaloneFileBrowser.OpenFilePanel(windowtitle, "", extensions, true);
        }


        void LoadTexture(string url)
        {
            Texture2D _texture = new Texture2D(2, 2);
            //read Image locally
            _texture.LoadImage(File.ReadAllBytes(url));


            //Resize image to fit
            RectTransform rect = SelectedImage.GetComponent<RectTransform>();

            float final_width = _texture.width;
            float final_height = _texture.height;
            while (final_width > rect.sizeDelta.x || final_height > rect.sizeDelta.y)
            {
                final_width /= 1.1f;
                final_height /= 1.1f;
            }
            //Load texture into image sprite
            SelectedImage.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(.5f, .5f));
            rect.sizeDelta = new Vector2(final_width, final_height);

        }

    }
}