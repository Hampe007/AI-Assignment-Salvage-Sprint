namespace SalvageSprint.Core
{
    // Week 1 assignment starter. Work through README stages in this same project.
    // STAGE 2: distinguish visible candidates from unknown hidden objects.
    // STAGE 3: retain a usable remembered target. STAGE 4: use failed-route feedback.
    // Delivery is supplied so the unmodified game has a complete scoring loop.
    public static class DecisionPolicy
    {
        public const string Label = "Part 1 starter: nearest candidate";


        private static bool IsEligable(Candidate candidate)
        {
            // Avaiable means the pickup has not been taken.
            // Known means currently visible or retained in memory.
            return candidate.Available && (candidate.Visible || candidate.Remembered);
        }
        
        // Infer the agent's facts from its observation. This is a simple, naive implementation that ignores visibility and memory.
        public static AgentFacts Infer(Observation observation)
        {
            if (observation.Carrying)
                return new AgentFacts(true, -1, false);

            foreach (Candidate candidate in observation.Candidates)
            {
                if (candidate.Id == observation.CurrentTargetId && IsEligable(candidate))
                {
                    // We have no cargo, this target selected, we shall keep existing target
                    return new AgentFacts(false, candidate.Id, true);
                }
            }
            
            int nearest = -1;
            float distance = float.PositiveInfinity;
            foreach (Candidate candidate in observation.Candidates)
            {
                // Still a bit naive when it comes to "Blocked", but now NPC takes Visible and/or Remembered into account.
                if (IsEligable(candidate) && candidate.DistanceSquared < distance)
                {
                    nearest = candidate.Id;
                    distance = candidate.DistanceSquared;
                }
            }
            return new AgentFacts(false, nearest, false);
        }

        public static Decision Choose(Observation observation) => Choose(Infer(observation));

        public static Decision Choose(AgentFacts facts)
        {
            // Deliberately naive: ignores RetainedTarget and any other facts.
            if (facts.NeedsDelivery)
                return new Decision(TaskKind.Deliver, -1, "Cargo held: return to depot");

            if (!facts.HasUsableTarget)
            {
                return new Decision(TaskKind.Explore, -1, "No known eligible target to pick-up, NPC will visit next survey point");    
            }
            
            string reason = facts.RetainedTarget
                ? "Retained the current eligible target"
                : "Nearest eligible known pickup";

            return new Decision(TaskKind.Collect, facts.SelectedTargetId, reason);
        }
    }
}
