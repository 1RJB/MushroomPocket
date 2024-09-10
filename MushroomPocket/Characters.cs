using System;
using System.Collections.Generic;
using System.Net.Http;

namespace MushroomPocket
{
    public class Waluigi : Character
    {
        public Waluigi(int hp, int exp)
        {
            Name = "Waluigi";
            HP = hp;
            EXP = exp;
            Skill = "Agility";
        }
    }

    public class Daisy : Character
    {
        public Daisy(int hp, int exp)
        {
            Name = "Daisy";
            HP = hp;
            EXP = exp;
            Skill = "Leadership";
        }
    }

    public class Wario : Character
    {
        public Wario(int hp, int exp)
        {
            Name = "Wario";
            HP = hp;
            EXP = exp;
            Skill = "Strength";
        }
    }

    public class AnyMushroomCharacter : Character
    {
        public AnyMushroomCharacter(string name, int hp, int exp, string skill)
        {
            Name = name;
            HP = hp;
            EXP = exp;
            Skill = skill;
        }
    }
}