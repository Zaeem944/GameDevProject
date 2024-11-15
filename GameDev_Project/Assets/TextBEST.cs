using UnityEngine;
using TMPro;
using System.Collections;


public class ArcadeTextCustomizer : MonoBehaviour
{
    public TextMeshProUGUI livesText;

    void Start()
    {
        if (livesText != null)
        {
            // Set Font Size
            livesText.fontSize = 60;

            // Set Arcade-like Gradient Colors
            livesText.enableVertexGradient = true;
            livesText.colorGradient = new VertexGradient(
                new Color32(255, 0, 0, 255),   // Top Left - Bright Red
                new Color32(255, 255, 0, 255), // Top Right - Yellow
                new Color32(0, 255, 0, 255),   // Bottom Left - Bright Green
                new Color32(0, 128, 255, 255)  // Bottom Right - Bright Blue
            );

            // Enable Bold Style
            livesText.fontStyle = FontStyles.Bold;

            // Add Outline
            livesText.outlineWidth = 0.5f;
            livesText.outlineColor = new Color32(0, 0, 0, 255); // Black outline

            // Add Glow Effect
            livesText.fontMaterial.EnableKeyword("GLOW_ON");
            livesText.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, new Color32(255, 255, 255, 255)); // White glow for a neon look
            livesText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0.8f); // High intensity for an arcade neon effect

            // Start the arcade-style animations
            StartCoroutine(ArcadeAnimations());
        }
    }

    // Coroutine for adding arcade-style animations
    IEnumerator ArcadeAnimations()
    {
        Vector3 originalPosition = livesText.transform.localPosition;
        float originalFontSize = livesText.fontSize;

        while (true)
        {
            // Make the text pulse by changing its font size
            livesText.fontSize = originalFontSize + Mathf.Sin(Time.time * 1.01f) * 1.01f;

            // Make the color oscillate between different hues
            float colorOscillation = Mathf.PingPong(Time.time * 0.5f, 1f);
            livesText.color = Color.Lerp(new Color32(255, 0, 0, 255), new Color32(0, 255, 255, 255), colorOscillation);

            yield return null;
        }
    }
}
