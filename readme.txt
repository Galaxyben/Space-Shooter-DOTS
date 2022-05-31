Things to take in count while playing:
- There is a chance for the asteroid spawning over the player, this will kill the player instantly.

Things that I consider are worth noticing:
- Nothing in the game uses Monobehaviours
- Using full DOTS (Jobs, ECS, Burst, Physics), not only ECS.

Extra notes:
- I started the project fully 2D, so at the beginning of the development I stumbled into an issue, the Physics library was not integrated eventhough I got the DOTS configuration by instaling the Hybrid Renderer.
This caused me a lot of wasted time trying to figure out how to solve manually 2d physics. You can see this on the player's spaceship which it moves without the physics library.
Afterwards I founded the url to install the Physics library from github and that solved all the rest of the issues.