using UnityEngine;

public static class SkyboxCreator
{
    // Create a 6-sided skybox from 6 textures
    public static Material Create6SidedSkybox(Texture frontTex, Texture backTex, Texture leftTex, 
                                           Texture rightTex, Texture upTex, Texture downTex)
    {
        // Create a material with the 6-sided skybox shader
        Material skyboxMat = new Material(Shader.Find("Skybox/6 Sided"));
        
        // Assign the textures
        skyboxMat.SetTexture("_FrontTex", frontTex);
        skyboxMat.SetTexture("_BackTex", backTex);
        skyboxMat.SetTexture("_LeftTex", leftTex);
        skyboxMat.SetTexture("_RightTex", rightTex);
        skyboxMat.SetTexture("_UpTex", upTex);
        skyboxMat.SetTexture("_DownTex", downTex);
        
        return skyboxMat;
    }
    
    // Create a procedural skybox
    public static Material CreateProceduralSkybox(Color skyTint, float atmosphereThickness, 
                                               Color groundColor, float exposure)
    {
        // Create a material with the procedural skybox shader
        Material skyboxMat = new Material(Shader.Find("Skybox/Procedural"));
        
        // Set properties
        skyboxMat.SetColor("_SkyTint", skyTint);
        skyboxMat.SetFloat("_AtmosphereThickness", atmosphereThickness);
        skyboxMat.SetColor("_GroundColor", groundColor);
        skyboxMat.SetFloat("_Exposure", exposure);
        
        return skyboxMat;
    }
    
    // Create a cubemap skybox from a single cubemap texture
    public static Material CreateCubemapSkybox(Cubemap cubemap, float exposure = 1.0f)
    {
        // Create a material with the cubemap skybox shader
        Material skyboxMat = new Material(Shader.Find("Skybox/Cubemap"));
        
        // Set properties
        skyboxMat.SetTexture("_Tex", cubemap);
        skyboxMat.SetFloat("_Exposure", exposure);
        
        return skyboxMat;
    }
    
    // Create a panoramic skybox from a single 360 texture
    public static Material CreatePanoramicSkybox(Texture2D panoramaTexture, float exposure = 1.0f)
    {
        // Create a material with the panoramic skybox shader
        Material skyboxMat = new Material(Shader.Find("Skybox/Panoramic"));
        
        // Set properties
        skyboxMat.SetTexture("_MainTex", panoramaTexture);
        skyboxMat.SetFloat("_Exposure", exposure);
        
        return skyboxMat;
    }
    
    // Create a simple gradient skybox (custom shader required)
    public static Material CreateGradientSkybox(Color topColor, Color bottomColor, float exponent = 1.0f)
    {
        // Check if the gradient skybox shader exists
        Shader gradientShader = Shader.Find("Skybox/Gradient");
        if (gradientShader == null)
        {
            Debug.LogError("Gradient skybox shader not found! You need to add a custom gradient skybox shader.");
            return null;
        }
        
        // Create material with the gradient shader
        Material skyboxMat = new Material(gradientShader);
        
        // Set properties (these property names might vary depending on the shader)
        skyboxMat.SetColor("_TopColor", topColor);
        skyboxMat.SetColor("_BottomColor", bottomColor);
        skyboxMat.SetFloat("_Exponent", exponent);
        
        return skyboxMat;
    }
} 