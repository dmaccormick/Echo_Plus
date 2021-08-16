# Echo+

## Introduction
This repository contains all of the final source code for my master's thesis project, Echo. It was completed in 2021. This includes all of the changes that turn it into Echo+ as well.

Echo / Echo+ is a tool designed to help game user researchers evaluate their games by providing an efficient and effective method for analysis of player sessions. Echo  records data in the background while a user plays through a game. The data is taken from most objects in the game and it includes their transform and rendering information. The recorded data is output to a log file at the end of the session. After all of the gameplay sessions are complete, the researchers can use Echo's other main component, the visualization, and import as many gameplay sessions as they want. Echo uses the recorded data to reconstruct the sessions within an interactable 3D scene. The researcher can then control the replays by playing, reversing, slowing down, and so on. Since the replays are reconstructed with data instead of video footage, the researchers are able to interact with and view it from any angle. Also, they can analyze multiple sessions at the same time. 

Video demo for updated version, Echo+: https://www.youtube.com/watch?v=U-6vJyXIeoM
Video demo of original Echo: https://www.youtube.com/watch?v=YhAOt4rjYFU

There is another repo on my account that was specifically for the version submitted to CHI PLAY 2020. That repository contains the source code for the Echo version at that point in time, in addition to an executable where it can be easily tried out. You can find that here: https://github.com/dmaccormick/Echo_CHIPLAY2020. You can also find the published paper from that same submission here: https://dl.acm.org/doi/10.1145/3410404.3414254.

## Brief Overview of the Code
- The code can be found in ThesisV2/Assets/Thesis/MyAssets/Scripts
- Below, some of the primary modules will be discussed

### Recording 
- Contains the main drivers of the recording system
- This system runs in the background while the user is playing the game
- Any object provided with the Recording_Object component will be recognized by the Recording_Manager as something that can contain data 
- The Recording_Manager drives the actual recording process, and pulls the data from all Recording_Object 

### RecTrack
- Recording_Objects can be configured to record certain types of data, so the researcher can save on resources (ie: no need to record rendering info for cameras). This configuration is done by adding recording tracks
- This folder contains all of the different recording tracks that can be added to objects. They follow a specific interface, which means that new tracks can also be added by researchers as needded

### Visualization
- Just like the recording folder, this contains the drivers for the loading and visualization of the data

### VisTrack
- All RecTrack's have a sibling VisTrack that is specifically designed to visualize just that track's data

## How To Run It
NOTE: I recommend checking out the CHI PLAY repository mentioned earlier (link again: https://github.com/dmaccormick/Echo_CHIPLAY2020) as it has an executable version of Echo that would take much less space and be easier to run than this Unity project version

- This project contains the full source code for the Unity project, so Unity is required
- Download the full project
- Open up the Scenes folder by navigating to ThesisV2/Assets/Thesis/My Assets/Scenes
- Open the Recording_Test or Visualization_Test scene to try out the recording or visualization features, respectively

