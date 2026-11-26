# Week 1 evidence

Keep this short. Bullet points, a small table and one diagram or code trace are enough. This is your checkpoint record, not an extra report. Replace the prompts as you work.


Name: Hampus Carlström Svanberg

Exact Unity version: 6000.3.26f1

Checkpoint commit / repository: main

Current stage and anything unfinished: stage 0

## Initial prediction

Three observations the rival needs, including one it might not have:

- The location of nearby scrap that it can collect (so it has something to choose from)
- Whether it is already carrying scrap (or if it can pick up more).
- Whether there is a valid route to the scrap (or back to a dropoff if it needs to drop the scrap off).

Seeing a pickup across a gap proves / does not prove reachability because:

- Seeing a pickup does not prove rachability because there might not be a valid path to it. The rival might be able to see the pickup but be unable to reach it because of obstacles, gaps or other enviromental factors that are in the way.

One question about NPC behaviour:

- Why do NPCs in Bethesda games seem to have no sense of self-preservation when being attacked? (Skyrim, Fallout 4 etc)

## One complete trace

Use one actual situation. Include the observation data, derived facts, selected task and reason, movement request/result, and how feedback affects a later decision. Identify the responsible code locations. A tiny state diagram can accompany this trace.

- Observation:
  - During play, we saw Task: Collect, Target: 10, Travel: Moving and Cargo: False.
- Derived facts:
  - From reading `DecisionPolicy.Infer`, this corresponds to `NeedsDelivery = false`, `SelectedTargetId = 10` and `HasUsableTarget = true`.
- Decision:
  - `DecisionPolicy.Choose` selects Collect with pickup target 10.
- Movement:
  - `RivalBrain.Update` calls `navigator.Request` with the selected pickup’s position. We observed the rival moving toward it.
- Gameplay feedback:
  - When `MatchController` confirms collection, the rival gains cargo. On the next decision, `Infer` should derive `NeedsDelivery = true`, and `Choose` should select Deliver with Target: -1. The rival should then navigate to the depot. Which it does.

## Scenario record

| Scenario | Predicted result | Observed result and short evidence / remaining issue |
|---|---|---|
| Normal loop and restart | | |
| Carrying cargo | | |
| Hidden / remembered pickup | | |
| Target taken | | |
| Unreachable target | | |
| No usable pickup | | |

Supplied test result, with any added tests identified:
Live walkthrough checked by / recording link if used:

## One correction and one independent check

The original weakness and why it happened:
What I changed and why it addresses the cause:
My counterexample or experiment: prediction, observed result, and one limitation:

## Collaboration and help

People I discussed or playtested with:
Code, sources or substantive tools/AI assistance I used, and how I verified them:
If unfinished: what works, exact reproduction steps, and the help I need:

- Did the "Check together" part of stage 1 with ...