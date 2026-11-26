# Week 1 evidence

Keep this short. Bullet points, a small table and one diagram or code trace are enough. This is your checkpoint record, not an extra report. Replace the prompts as you work.


Name: Hampus Carlström Svanberg

Exact Unity version: 6000.3.26f1

Checkpoint commit / repository: main

Current stage and anything unfinished: Stage 3 done, Stage 4 is next.

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

### Stage 2 follow-up: Hidden pickup (F4)

- Changed rule: In `DecisionPolicy.Infer`, a candidate must be available and either visible or remembered before its distance is compared. Hidden unknown pickups are rejected.
- Observed result: In hidden pickup scenario (F4), the rival ignored a hidden pickup, then selected it once it became visible and was closer. This matches the new rule because becoming visible made the pickup eligible for distance comparison.

## Scenario record

| Scenario | Predicted result | Observed result and short evidence / remaining issue |
|---|---|---|
| Normal loop and restart | | |
| Carrying cargo | | |
| Hidden / remembered pickup | Hidden unknown pickups should be ignored until visible. | In Scenario "hidden pickup" (F4), the rival ignored a hidden pickup, then selected it once it became visible and was closer. Remembered pickups outside sight were covered by the supplied Stage 2 test. |
| Target taken | Release the target when it is taken, then select another eligible pickup or Explore if none qualifies. | In "Target taken" (F5) scenario, the targeted pickup for the rival was instead picked up for the player so it got removed. The rival released that target and continued as expected. |
| Unreachable target | | |
| No usable pickup | | |

Supplied test result, with any added tests identified: After the Stage 2 edit, 13 passed and 4 failed, matching the expected Stage 2 total.

After the Stage 3 edit, tests showed 7 passed and 3 failed. Both Stage 3 tests, and all three Stage 2 tests and both regression tests still passed.

Live walkthrough checked by / recording link if used:

## One correction and one independent check

(Will probably switch what I wrote to something else when I have something better to show)

The original weakness and why it happened:

- The original condition checked only availability and distance, so a nearer hidden unknown pickup could win even though the rival did not know about it.

What I changed and why it addresses the cause:

- In `DecisionPolicy.Infer`, I added `(candidate.Visible || candidate.Remembered)` to the availability and distance checks. A pickup must now be available and either visible or remembered before distance ranks it. This rejects hidden unknown pickups while allowing remembered pickups outside sight.

My counterexample or experiment: prediction, observed result, and one limitation:

## Stage 2 - small extra notes

- A hidden remembered pickup can qualify because the rival previously selected it while it was visible and still remembers that target. Losing sight does not immediately erase that knowledge.
- A hidden unknown pickup cannot qualify because the rival has not seen or remembered it. Being nearby does not mean the rival knows it exists.
- The pickup must still be available, even if remembered. Distance only decides between candidates that pass the availability and knowledge checks.
- Memory holds one selected target, not every pickup ever seen. Stage 2 allows switching to a closer eligible pickup keeping a useful current target comes in Stage 3.

## Stage 3

- `DecisionPolicy.Infer` keeps the current target while it is eligible, even if another pickup is closer. It sets `RetainedTarget` to true. If the current target is missing or unavailable, the nearest eligible alternative is selected with `RetainedTarget` false.
- `DecisionPolicy.Choose` now gives different Collect reasons for retaining the current target and selecting a new one.
- In `RivalBrain.Update`, the supplied availability check clears the remembered target when its pickup has been taken. The scenario F5, observation above confirmed that the rival releases the target and continues.

State transitions:
- Explore -> Collect when an eligible pickup is selected.
- Collect -> Deliver after successful collection gives the rival cargo.
- Deliver -> Collect or Explore after delivery, depending on current available knowledge the rival has.
- Collect -> another Collect target or Explore when the current target becomes unusable.

## Collaboration and help

People I discussed or playtested with:
Code, sources or substantive tools/AI assistance I used, and how I verified them:
If unfinished: what works, exact reproduction steps, and the help I need:

- Did the "Check together" part (the group task) of stage 1 with Sebastian and Abbie
