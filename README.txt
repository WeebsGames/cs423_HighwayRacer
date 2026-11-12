            >>>>HIGHWAY RACER<<<<

    The idea behind this was to create a classic highway racer where players had to speed for as long as possible on a highway while avoiding other cars on the road.

    Developers:
        Areeb Jamili: Lead Dev and Project Manager
        Pathik Sangani: Coding assistant

    We wanted to see how NUIs would help/detrement the gaming experience, so we decided to implement voice and gyro, (maybe gestures), controls.
        Voice is used to active the three powerups and navigate menus. 
        Gyro controls are used to control the movement of the car; steering and acceleration
        Tilting adjusts stored throttle from -0.5 to 1; neutral tilt holds the current value.
        Negative throttle applies up to 50% brake torque without driving in reverse.
        ThrottleFrontPositive shows acceleration, while ThrottleFrontNegative fills as braking increases
        (full at -0.5). Both UI Images should use the Filled image type.
        Shield and siren cooldown Images should also use the Filled image type.
        Their overlays are empty while ready or active, then fill completely when
        the effect ends and drain to empty as the cooldown finishes.

To play the game, you must have an android device. The current build of the game is labeled android_test.apk. Open the apk on your android device to play the game.



>AI<
AI was used to understand how to use the UnityAndroidSpeechRecognizer library. 
It was also used to generate a skeleton for some functions, specifically gyro controls and voice controls. The siren powerup was initially created using AI, but was refined later (AI wasn't able to produce what I wanted).

Below are where we found most of the assests for the game.

The game that we used to create the highway racer was Stacked Delivery
Link to original repo: https://github.com/WeebsGames/cs426_Stacked_Delivery 

We used Eric Batlle's speech recognizer library: https://github.com/EricBatlle/UnityAndroidSpeechRecognizer


Audio used:
https://youtu.be/sx2rNv-NJ-U?si=jrjkhxFGlLBXuBmc - main menu music
https://youtu.be/InGGr5LIshk?si=zS2MR-2VKbU5bmpj - main menu music
https://youtu.be/9Kq89qHGRK8?si=FK5LjVFEXvWnRl5o - main menu music

references
Terrain (grass, dirt) textures
https://kaffeeaffe.itch.io/terrain-textures

rock texture
https://www.cgtrader.com/free-3d-models/textures/architectural-textures/rock-wall-texture-material

stop light models
https://www.cgtrader.com/free-3d-models/industrial/industrial-machine/low-poly-traffic-light

nature objects asset
https://assetstore.unity.com/packages/3d/environments/landscapes/low-poly-simple-nature-pack-16215

additional references
https://assetstore.unity.com/packages/p/yughues-free-sand-materials-12964
https://assetstore.unity.com/packages/p/hq-pbr-old-retro-radio-free-180303
https://assetstore.unity.com/packages/3d/environments/landscapes/mytown-177012
https://assetstore.unity.com/packages/p/modular-storage-shelves-garage-organization-props-252198
https://assetstore.unity.com/packages/p/allsky-free-10-sky-skybox-set-146014
https://assetstore.unity.com/packages/p/road-system-192818
https://assetstore.unity.com/packages/2d/textures-materials/nature/terrain-textures-free-271990
https://assetstore.unity.com/packages/p/yughues-free-concrete-materials-12951
https://sketchfab.com/3d-models/1991-honda-civic-eg6-f81da34570b145deb463fa091604d251 - Honda Civic
