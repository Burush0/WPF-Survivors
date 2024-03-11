# WPF-Survivors
A simple "Vampire Survivors"-like game made in WPF in around a week.

## Screenshots:
Level Select Screen

<img src="images/img1.png?raw=true" width="600">

Loading Screen

<img src="images/img2.png?raw=true" width="600">

Gameplay Screen

<img src="images/img3.png?raw=true" width="600">

## Known bugs:
- After each round of gameplay, an error occurs due to the fact that no MySQL database is connected. It was intended to be used to track progress, which worked on the machine the game was created on.
- Tickspeed is tied to the monitor's refresh rate, so at 60Hz the gameplay is "as intended", anything higher will cause the game to run much faster.
