# Splash Screen Setup Guide

## Step 1: Create the Splash Screen Scene
1. In Unity, go to **File > New Scene** and select the **Basic (URP)** template.
2. Save this scene as `SplashScreen` in the `Assets/Scenes` folder.

## Step 2: Set up the Canvas
1. Delete the default directional light if needed (the splash screen doesn't need 3D lighting).
2. In the Hierarchy, right-click and select **UI > Canvas** to create a new Canvas.
3. Make sure the Canvas Scaler is set to "Scale With Screen Size" for proper display on all devices.
4. Add a **CanvasGroup** component to the Canvas (this will be used for fading).

## Step 3: Create the Splash Screen UI
1. Add a **Panel** as a child of the Canvas:
   - Set its color to a dark background (e.g., dark blue or black)
   - Make it fill the entire screen

2. Add two **TextMeshPro - Text** objects as children of the Panel:
   - Title Text:
     - Position it in the upper half of the screen
     - Set text to "Maze Adventures"
     - Font size: 72, Bold
     - Color: White or another contrasting color
   
   - Studio Text:
     - Position it below the title
     - Set text to "Kings Gaming Studio"
     - Font size: 36
     - Color: Light gray or similar

3. (Optional) Add a logo image if you have one available.

## Step 4: Add the SplashScreen Script
1. Select the Canvas or create an empty GameObject as a child of the Canvas.
2. Add the **SplashScreen** component that was created.
3. Assign the references:
   - Canvas Group: drag the Canvas with the CanvasGroup component
   - Title Text: drag the "Maze Adventures" TextMeshPro object
   - Studio Text: drag the "Kings Gaming Studio" TextMeshPro object
4. Adjust timing settings if needed:
   - Display Time: How long to display the splash screen (default: 3 seconds)
   - Fade In Time: How long it takes to fade in (default: 1 second)
   - Fade Out Time: How long it takes to fade out (default: 1 second)

## Step 5: Update Build Settings
1. Go to **File > Build Settings**
2. Add the SplashScreen scene to the build
3. Make sure the SplashScreen scene is the FIRST scene in the list (drag it to the top)
4. The MainMenu scene should be the second scene

Now, when you run the game, the Splash Screen will appear first, display for the specified time, and then automatically transition to the Main Menu. 