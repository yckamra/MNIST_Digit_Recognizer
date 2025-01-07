using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DrawingManager : MonoBehaviour
{
    public RawImage drawingArea;  // The UI element that will serve as the drawing canvas
    public RawImage pixelatedArea;
    public RawImage upscaledArea; // TODO: for testing
    public RawImage upscaledAreaData; // TODO: for testing
    private Texture2D texture;    // The texture we will draw on
    private Texture2D textureForPixelated;
    private Texture2D textureForUpscaled; // TODO: for testing
    private Texture2D textureForUpscaledData; // TODO: for testing
    private Color drawColor = Color.white;  // Color to draw with
    private int brushSize = 15;  // Size of the brush
    public double[,] grayscaleArray = null;
    public AudioSource pop;
    public AudioSource backgroundMusic;

    public SceneObserverScript theSceneObserverScript;

    public TextMeshProUGUI text;

    private void Start()
    {


        backgroundMusic.Play();
        // Create a new Texture2D for the drawing
        texture = new Texture2D((int)drawingArea.rectTransform.rect.width, (int)drawingArea.rectTransform.rect.height);
        drawingArea.texture = texture;

        textureForPixelated = new Texture2D((int)pixelatedArea.rectTransform.rect.width, (int)pixelatedArea.rectTransform.rect.height);
        pixelatedArea.texture = textureForPixelated;


        textureForUpscaled = new Texture2D((int)upscaledArea.rectTransform.rect.width, (int)upscaledArea.rectTransform.rect.height); // TODO: For testing
        upscaledArea.texture = textureForUpscaled; // TODO: for testing

        textureForUpscaledData = new Texture2D((int)upscaledAreaData.rectTransform.rect.width, (int)upscaledAreaData.rectTransform.rect.height); // TODO: For testing
        upscaledAreaData.texture = textureForUpscaledData; // TODO: for testing

        // Clear the texture to white (background color)
        ClearTexture(texture);
        ClearTexture(textureForPixelated);
        ClearTexture(textureForUpscaled); // TODO: For testing
        ClearTexture(textureForUpscaledData); // TODO: For testing
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 localPoint;
            // Convert the screen coordinates to the drawing area's local coordinates
            RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingArea.rectTransform, Input.mousePosition, null, out localPoint);

            // Normalize the coordinates to the texture's space
            Vector2 normalizedPoint = new Vector2(localPoint.x + drawingArea.rectTransform.rect.width / 2, localPoint.y + drawingArea.rectTransform.rect.height / 2);

            // Draw the pixel or brush
            DrawOnTexture((int)normalizedPoint.x, (int)normalizedPoint.y);


            // Resize the original image to 28x28 pixels
            Texture2D resizedImage = ResizeTexture(texture, 28, 28);

            // Convert the resized image to a 1D grayscale array
            this.grayscaleArray = ConvertToGrayscaleArray(resizedImage);

            textureForPixelated = UpscaleTexture(resizedImage, 15);
            drawingArea.texture = textureForPixelated;
            //pixelatedArea.texture = textureForPixelated;


            //textureForUpscaled = FlattenedArrayToTexture2D(grayscaleArray, 28, 28); // TODO: for testing
            //textureForUpscaled = UpscaleTexture(textureForUpscaled, 15); // TODO: for testing
            //upscaledArea.texture = textureForUpscaled; // TODO: For testing

            /*if (theSceneObserverScript.inputData != null)
            {
                textureForUpscaledData = FlattenedArrayToTexture2D(theSceneObserverScript.inputData, 28, 28); // TODO: for testing
                textureForUpscaledData = UpscaleTexture(textureForUpscaledData, 15); // TODO: for testing
                upscaledAreaData.texture = textureForUpscaledData; // TODO: For testing
            }*/

        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            pop.Play();
            ClearTexture(texture);
            ClearTexture(textureForPixelated);
            ClearTexture(textureForUpscaled); // TODO: For testing
            ClearTexture(textureForUpscaledData); // TODO: For testing
        } else if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            Application.Quit();
        }

    }

    Texture2D UpscaleTexture(Texture2D source, int scaleFactor)
    {
        // Original width and height of the texture
        int originalWidth = source.width;
        int originalHeight = source.height;

        // New width and height after scaling
        int newWidth = originalWidth * scaleFactor;
        int newHeight = originalHeight * scaleFactor;

        // Create a new Texture2D with the scaled dimensions
        Texture2D upscaledTexture = new Texture2D(newWidth, newHeight);

        // Loop through each pixel in the original texture
        for (int y = 0; y < originalHeight; y++)
        {
            for (int x = 0; x < originalWidth; x++)
            {
                // Get the color of the current pixel in the original texture
                Color pixelColor = source.GetPixel(x, y);

                // Fill a block of pixels in the new texture with the same color
                for (int dy = 0; dy < scaleFactor; dy++)
                {
                    for (int dx = 0; dx < scaleFactor; dx++)
                    {
                        upscaledTexture.SetPixel(x * scaleFactor + dx, y * scaleFactor + dy, pixelColor);
                    }
                }
            }
        }

        // Apply changes to the texture
        upscaledTexture.Apply();

        return upscaledTexture;
    }

    public double[,] GetGrayscaleArray()
    {
        return this.grayscaleArray;
    }

    // Method to draw on the texture at the specified coordinates
    void DrawOnTexture(int x, int y)
    {
        for (int i = -brushSize; i <= brushSize; i++)
        {
            for (int j = -brushSize; j <= brushSize; j++)
            {
                // Ensure we're drawing within the texture bounds
                if (x + i >= 0 && x + i < texture.width && y + j >= 0 && y + j < texture.height)
                {
                    texture.SetPixel(x + i, y + j, drawColor);
                }
            }
        }
        texture.Apply();  // Apply the changes to the texture
    }

    // Clear the texture to a white background
    void ClearTexture(Texture2D texture)
    {
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, Color.black);
            }
        }
        texture.Apply();
    }
    // Function to resize a Texture2D
    Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        // Create a new Texture2D with the desired dimensions
        Texture2D resizedTexture = new Texture2D(newWidth, newHeight);

        // Resize the texture using Unity's built-in resizing function
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        Graphics.Blit(source, rt);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        resizedTexture.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        resizedTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return resizedTexture;
    }

    // Function to convert a Texture2D to a transposed and 90-degree rotated grayscale 1D double array
    double[,] ConvertToGrayscaleArray(Texture2D image)
    {
        int width = image.width;
        int height = image.height;
        double[,] grayscaleArray = new double[1, width * height];

        // Loop through each pixel
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Get the pixel color (assumes black-and-white, so we can just use the red channel)
                Color pixelColor = image.GetPixel(x, y);

                // Calculate grayscale value (0 to 1 range)
                double grayscaleValue = pixelColor.r;  // Red channel represents intensity in black and white images

                // Transpose the image: swap x and y
                int transposedX = y;
                int transposedY = x;

                // Then rotate 90 degrees clockwise
                // x' = transposedY, y' = (width - 1) - transposedX
                grayscaleArray[0, transposedY + (height * ((width - 1) - transposedX))] = grayscaleValue;
            }
        }

        return grayscaleArray;
    }

    Texture2D FlattenedArrayToTexture2D(double[,] normalizedArray, int width, int height)
    {
        // Create a new Texture2D with the specified width and height
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Loop through the flattened double array (assumed to be double[784, 1] for MNIST)
        int index = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Unnormalize the value (assuming values were normalized to [0, 1])
                double normalizedValue = normalizedArray[0, index];
                float greyscaleValue = (float)(normalizedValue * 255.0);  // Unnormalize back to [0, 255]

                // Set the pixel color in the texture (use the same value for R, G, B to create greyscale)
                Color color = new Color(greyscaleValue / 255.0f, greyscaleValue / 255.0f, greyscaleValue / 255.0f, 1.0f);
                texture.SetPixel(x, y, color);

                index++; // Move to the next element in the flattened array
            }
        }

        // Apply the changes to the texture
        texture.Apply();

        return texture;
    }
}
