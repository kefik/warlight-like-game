# Simulator

## Motivation
Suppose we want to examine how exactly the bot operates,
watch 2 bots fighting against each other. We would need
a *simulator* for that.

## Goal
The goal is to implement such simulator. Precisely, there has to
be an option to:
1. Let bots play against each other without stopping them.
2. Pause the playing bots.
3. Play one round, return the played round
4. Play one game action (one deployment, one attack,...) and reverse
it again
5. Play one player turn

There will be two main screens:
create new simulation screen and
the simulation screen.

### Create new simulation screen
<img src="simulator_create_screen.jpg" alt="Create new simulation screen"/>

This screen is very similar to
create new singleplayer game screen.
Their difference is that user cannot actively
play the game and the engine can generate restrictions
(restrictions are e.g. bot at the beginning can
choose only from specified regions) and user
instead of choosing bot's difficulty
will have an option to choose bot type
(MCTS bot,...).


After pressing the Start button user
will be moved to screen specified in
next article section.

### Simulation screen