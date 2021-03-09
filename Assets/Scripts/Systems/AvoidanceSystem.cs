using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

namespace BattleSimulator
{
    class AvoidanceSystem
    {
        private struct Avoidance
        {
            public float weight;
            public float3 dir;            
        }

        public void OnUpdate ()
        {
            var units = Unit.GetUnits();

            NativeArray<Avoidance> avoidances = new NativeArray<Avoidance>(units.Length, Allocator.Temp);

            for (int i = 0; i < units.Length; i++)
                avoidances[i] = new Avoidance { dir = Vector3.zero, weight = 0.0001f };

            Avoidance[] avoidanceVectors = new Avoidance[8];

            for (int i = 0; i < units.Length; i++)
            {
                if (units[i].Target == null)
                    continue;

                var forward = (units[i].Target.transform.position - units[i].transform.position).normalized;
                avoidanceVectors[0].dir = forward;
                avoidanceVectors[0].weight = 1.0f;
                for (int j = 1; j < avoidanceVectors.Length; j++)
                {
                    avoidanceVectors[j].dir = Quaternion.Euler(0, j * 45, 0) * forward;
                    avoidanceVectors[j].weight = 0.9f;
                }

                for (int j = 0; j < units.Length; j++)
                {
                    if (i == j)
                        continue;

                    // Minimum desired distance the two units should be from each other 
                    var minDistance = units[i].size + units[j].size;

                    // Actual distance between the two units
                    var distance = units[i].DistanceTo(units[j]);

                    // Amount the two units overlap (negative means no overlap)
                    var overlap = minDistance - distance;
                    if (overlap <= 0.0f)
                        continue;

                    // Normalized amount of overlap of the two units (0 = no overlap, 1 = equal positions)
                    var normalizedOverlap = overlap / minDistance;

                    // TODO: what should this be named
                    var dir = (units[j].transform.position - units[i].transform.position).normalized;

                    for (int k = 0; k < avoidanceVectors.Length; k++)
                    {
                        var projected = Vector3.Dot(avoidanceVectors[k].dir, dir);
                        if (projected <= 0)
                            continue;

                        avoidanceVectors[k].weight -= (normalizedOverlap * 10 * projected);
                    }
                }

                var bestVec = avoidanceVectors[0].dir;
                var bestWeight = avoidanceVectors[0].weight;
                for (int k = 0; k < avoidanceVectors.Length; k++)
                {
                    if (avoidanceVectors[k].weight > bestWeight)
                    {
                        bestVec = avoidanceVectors[k].dir;
                        bestWeight = avoidanceVectors[k].weight;
                    }
                }

                units[i].avoidance = bestVec;
            }



            avoidances.Dispose();
        }
    }
}
