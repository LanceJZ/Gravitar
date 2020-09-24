using System;
using System.Collections.Generic;
using System.Text;
using Gravitar.Entities;
using Microsoft.Xna.Framework;

namespace Gravitar.Managers
{
    public struct PlanetData
    {
        public uint number;
        public List<Bunker> bunkerList;
        public List<FuelDepot> fuelDepotList;
        public Vector3[] landModel;
        public bool gravityCenter;
        public bool gravityDown;
    }
}
