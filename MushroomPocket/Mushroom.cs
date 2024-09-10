using System;
using System.Collections.Generic;
using System.Net.Http;

namespace MushroomPocket
{
    public class MushroomMaster
    {
        public string Name { get; set; }
        public int NoToTransform { get; set; }
        public string TransformTo { get; set; }

        public MushroomMaster(string name, int noToTransform, string transformTo)
        {
            this.Name = name;
            this.NoToTransform = noToTransform;
            this.TransformTo = transformTo;
        }
    }

    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int HP { get; set; }
        public int EXP { get; set; }
        public string Skill { get; set; }
    }
}


