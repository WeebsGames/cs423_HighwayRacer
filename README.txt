The game that we used to create the highway racer was Stacked Delivery

A single player car game where the objective is to deliver items to a designated area as fast as possible! However, the twist of the game is that the items are stacked on the roof of the vehicle, so the player must manage balancing the items without them flying off, as well as reaching the destination as quickly as possible!

The player needs to navigate through the levels while avoiding obstacles such as traffic and wildlife to get to the end of the level. Traffic forces the player to take different racing lines depending on how fast they are. Wildlife does the same, except is much more unpredictable and forces the player to be more cautious. The player must find the optimal balance between speed and safety to deliver the boxes on time. Streetlights guide the way through the level and show the player the recommended routing. Routes that aren't lit up are either suboptimal or are very difficult. 


Itch.io link to download the game: https://areebj.itch.io/stacked-delivery

Link to original repo: https://github.com/WeebsGames/cs426_Stacked_Delivery 


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



>AI<
AI was used to understand how to use the UnityAndroidSpeechRecognizer library. 
It was also used to generate a skeleton for some functions, specifically gyro controls and voice controls. The siren powerup was initially created using AI, but was refined later (AI wasn't able to produce what I wanted).

Below are where we found most of the assests for the game.

Audio used:
https://opengameart.org/content/super-santa-claus - christmas themed music for level 2
https://opengameart.org/content/technomania101-2000s-europop-electronic-dance-music - unused
https://opengameart.org/content/fever-stadium - upbeat music for level 1
https://opengameart.org/content/alarm-1 - warn player of impending doom
https://youtu.be/mHJ3l18YqNM?si=dRvXFFFYrme8U0q6 - indicate win state to player
https://youtu.be/sx2rNv-NJ-U?si=jrjkhxFGlLBXuBmc - main menu music
https://youtu.be/InGGr5LIshk?si=zS2MR-2VKbU5bmpj - main menu music
https://youtu.be/9Kq89qHGRK8?si=FK5LjVFEXvWnRl5o - main menu music
https://youtu.be/lGCBPFx-hmM?si=mndhf8HcWAyk0Eni - audio feeback for boxes falling over
https://youtu.be/M1MUvFyOWkY?si=TyrMvSEhZb_7Qt5G - indicate failure to the player
https://www.youtube.com/watch?v=6pyhHlxVzj4 - alt menu audio
https://www.youtube.com/watch?v=ww1baEX3KdM - level 6 audio

references
Terrain (grass, dirt) textures
https://kaffeeaffe.itch.io/terrain-textures

rock texture
https://www.cgtrader.com/free-3d-models/textures/architectural-textures/rock-wall-texture-material

stop light models
https://www.cgtrader.com/free-3d-models/industrial/industrial-machine/low-poly-traffic-light

nature objects asset
https://assetstore.unity.com/packages/3d/environments/landscapes/low-poly-simple-nature-pack-162153

animal assets
https://assetstore.unity.com/packages/3d/characters/animals/animals-free-animated-low-poly-3d-models-260727#content


racer mixamo link
https://www.mixamo.com/#/?page=1&query=waving&type=Character

and animations
https://www.mixamo.com/#/?page=1&query=waving&type=Motion%2CMotionPack
https://www.mixamo.com/#/?page=1&query=cheer&type=Motion%2CMotionPack

additional references
https://opengameart.org/content/snow-flake
https://assetstore.unity.com/packages/2d/textures-materials/water/simple-water-shader-urp-191449
https://assetstore.unity.com/packages/p/yughues-free-sand-materials-12964
https://assetstore.unity.com/packages/p/hq-pbr-old-retro-radio-free-180303
https://assetstore.unity.com/packages/3d/environments/landscapes/mytown-177012
https://assetstore.unity.com/packages/p/modular-storage-shelves-garage-organization-props-252198
https://assetstore.unity.com/packages/p/allsky-free-10-sky-skybox-set-146014
https://assetstore.unity.com/packages/p/road-system-192818
https://assetstore.unity.com/packages/2d/textures-materials/nature/terrain-textures-free-271990
https://assetstore.unity.com/packages/p/yughues-free-concrete-materials-12951
https://sketchfab.com/3d-models/nissan-skyline-r32-gt-r-8810e677eca543a6b840cf67dd064aa4 - Nissan Skyline
https://sketchfab.com/3d-models/1991-honda-civic-eg6-f81da34570b145deb463fa091604d251 - Honda Civic
https://www.svgrepo.com/svg/344705/cup-straw - Cup for stability meter