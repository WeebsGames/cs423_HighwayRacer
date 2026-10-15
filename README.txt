STACKED DELIVERY

A single player car game where the objective is to deliver items to a designated area as fast as possible! However, the twist of the game is that the items are stacked on the roof of the vehicle, so the player must manage balancing the items without them flying off, as well as reaching the destination as quickly as possible!

The player needs to navigate through the levels while avoiding obstacles such as traffic and wildlife to get to the end of the level. Traffic forces the player to take different racing lines depending on how fast they are. Wildlife does the same, except is much more unpredictable and forces the player to be more cautious. The player must find the optimal balance between speed and safety to deliver the boxes on time. Streetlights guide the way through the level and show the player the recommended routing. Routes that aren't lit up are either suboptimal or are very difficult. 


Itch.io link to download the game: https://areebj.itch.io/stacked-delivery

Voice boost:
CarControl finds the active object tagged Listener and reads its VoiceInput boostActive state.
While boost is active, motor torque (acceleration) increases by 50% and top speed increases by 25%.
The existing drifting speed bonus stacks with boost. Turning boost off restores normal torque
and the normal top-speed target without abruptly clamping velocity. Braking and throttle input remain unchanged.
Each driving scene using voice boost needs an active Listener-tagged object with VoiceInput attached.


To show the player the urgency of the task, the time has been set to night to simulate a late night emergency delivery. Think of it this way: Its 11:50pm and you have to make a same day delivery!

Assignment 7 Details:
UI Design details:
2-3 instances of UI improvement based off of our current demo/prototype

(Anthony) Speedometer: Currently there is no feedback for how fast the vehicle is going, so it would be nice to add a speedometer for feedback to add to the visual of speed. The stability meter also needs to be more clear that it is for the stability of the boxes/items on top of the vehicle, so adding Text stating what the bar purpose on the screen, it more clear for the user/player what that UI element is for. 
Color: Color for the text matches the level of stability of the meter, where the color states matches the bar fill color
Green: Stability is fine
Orange: Stability is unstable/critical stability for the boxes
Red: Boxes flew out the car
These colors people would typically map and associate with levels of good and bad states, which is why the text also matches the stability fill color states to reflect the stability of the boxes
(Areeb): Menu update: Currently menu static in transitions between menu scenes
Fix: Making menu more dynamic and transitions flow more naturally between menu scenes
Binding of Issac menu example
Could also just have an animated background for certain menu scenes (ex level select)

Sound Effects added:
 - Tire Screech
 - Collision FX
 - Box falling FX
 - Alarm
The sound effects make the driving feel more immersive and give feedback to the player to let them know when they have crashed and when they are drifting. The alarm tells the player to be more careful or else their boxes will fall over. If the player's boxes fall over, the sound effect informs the player of their mistake.

UI Improvements:
Instead of a canvas for the main menu, a 3D environment is used for more immersion. The level select and controls screen are in the same scene to make the transistions feel smooth. A simple pause menu was added to the game to allow the player to restart or quit the game. The stability meter has a label and warnings for low stability to make it more clear to the player what it represents. A speedometer was added to show the player how fast they are going.

Playtesting questions specific for our game
Notes on what information/data we are trying to gather from play testers

 - UI
 - Procedures
 - Level Design: Objective of the game? 
 - Outcome: When players fail, what should happen
 - When players success (reach the finish zone, with boxes) what should happen?
 - Other elements questions
 - Is the need for a restart button necessary? 

Assignment 8 Details:

Based on playtester feedback, we added and made a few adjustments to level design, procedures, and UI/feedback elements.

Level design updates: Added more depth to design of tracks such as jumps and sharper turns, to increase challenge and “fun” design that was lacking in our previous levels. We added more obstacles such as objects that affect traction of the car. New Level 3 has “oil” object that causes vehicle to slip if the vehicle drives over “oil puddle”. These sorts of obstacles were noted by some of the playtesters, saying that adding some sort of obstacles relative to the theme of the level can add to the difficulty to the level (for example, Level 2 snow map, maybe adding ice on the road?). Playtesters noted that some levels were too easy without some sort of obstacles and such.

Updated UI: Increased the size of UI elements such as speedometer and stability bar. We also added a timer. Some playtesters noted that they didn’t really notice the UI elements during gameplay. So we adjusted the size and position of these elements to make them more visible to the player. We also added the timer that counts down. This was originally not implemented for the Alpha, and playtesters noted that there was no real rush to complete level. So by adding this element, we want players to feel pressured to complete a level in time.

Procedure updates: Added new feature to enforce drifting, where using drifting procedure, the vehicle’s top speed is increased. Playtesters noted that they didn’t need to use the drifting procedure, as they noted that there was no benefit. With this update, we want users to use drifting procedure now that they have the added pressure of completing a level before time runs out. 

Level Completion update: Now when crossing finish line, the Finish line/object now triggers where players are now shown a new menu panel. If the player crosses the finish line in time and with the items still on the vehicle, then they are shown the level complete text and are able to click the following buttons: Next level, Quit, and Restart. Next level is shown when they complete the level in time and they are able to make it to the finish line with the items still on the car. Quit takes you to the main menu, and restart button restarts the level. Previously there was no sort of event that occurred when crossing finish line, so playtesters were confused on the end of a level.

Gameplay update: When a player loses their items/cargo, there was no sequence of events that causes the level to pause. We added a panel that displays for the player to restart level or quit. Playtesters noted that they didn’t really see the point of the game if they were still able to drive and technically reach the finish without the items. So we wanted to enforce that they can not continue the level and reach the finish without the items on the car.


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