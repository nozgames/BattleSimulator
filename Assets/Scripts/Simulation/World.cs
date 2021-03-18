using System.Collections.Generic;

namespace BattleSimulator.Simulation
{
    public class World
    {
        private List<Unit> _units;


        /// <summary>
        /// Create a unit using the given graph and return its index.  This index will be 
        /// valid until the unit is destroyed.
        /// </summary>
        /// <param name="graph">Graph to use to create the unit</param>
        /// <returns>Index of unit</returns>
        public int CreateUnit (Graph graph)
        {
            // TODO: If graph is only input parameter then the server needs to be able to load the configuration files. JSON?
            // TODO: This makes it a pain to link assets, but we can do it with databases if we need to, or events.  The client side
            //       representation can respond to specific events to play effects, etc, and we could take it out of the ability and effect.  It would
            //       be a contract between the ability definition and the client side prefab.  Anything missing and it that portion wouldnt work, this would 
            //       also sync up nicely with how we want to raise events.

            return 0;
        }

        // TODO: Cells that store the units that are in them for fast spatial look up

        // TODO: Method to enumerate all units in a radius

        // TODO: List of free unit spots
    }
}
