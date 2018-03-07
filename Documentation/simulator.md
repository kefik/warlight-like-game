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
(restrictions are e.g. that bot at the beginning can
choose only from specified regions) and user
instead of choosing bot's difficulty
will have an option to choose bot type
(MCTS bot,...).


After pressing the Start button user
will be moved to screen specified in
next article section.

### Game simulation screen
<img src="simulator_game_screen.jpg" alt="Game simulation screen"/>

Looks similar to standard game screens, but controls on the left
are different.

- *>|* - starts or stops playing the bot
- *<* or *>* - skips to next action or returns the last played action if there's any
- *<<* or *>>* - skips to next turn or returns the last played turn if there's any
- *<<<* or *>>>* - skips to next round or returns the last played round if there's any
- *<<<<* or *>>>>* - skips to end of the so far played game or
returns the game to the beginning

*Time for bot* represents time each bot has for evaluation of its next turn.
It's time measurement unit will be a millisecond. Its default value
will be set for 4000 ms.