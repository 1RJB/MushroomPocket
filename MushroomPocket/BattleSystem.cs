using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MySql.Data.MySqlClient;

namespace MushroomPocket
{
    class BattleSystem
    {
        public enum Difficulty
        {
            Easy,
            Normal,
            Hard,
            Random
        }

        public static void BattleSystemFunction(List<Character> characters, Difficulty difficulty, MySqlConnection conn)
        {
            if (characters.Count == 0)
            {
                Console.WriteLine("You have no characters in your pocket! Please add at least 1 character first.");
                return;
            }

            Console.WriteLine("--- Mushroom Pocket Battle ---");
            Console.Write("Enter the Mushroom ID of the character you wish to battle with: ");
            string characterID = Console.ReadLine();
            Program.StopMushroomPocketAppOnQ(characterID);

            if (int.TryParse(characterID, out int id))
            {
                Character character = characters.FirstOrDefault(c => c.Id == id);
                if (character != null)
                {
                    Console.WriteLine("-------------------------");
                    Console.WriteLine($"You are battling with {character.Name}");
                    Console.WriteLine($"HP: {character.HP}");
                    Console.WriteLine($"EXP: {character.EXP}");
                    Console.WriteLine($"Skill: {character.Skill}");
                    Console.WriteLine("-------------------------");

                    GenerateBattle(characters, character, difficulty, conn);
                }
                else
                {
                    Console.WriteLine("Invalid Mushroom ID inputted. The ID must be of one of the characters in your pocket.\nGoing back to the main menu...");
                }
            }
            else
            {
                Console.WriteLine("Invalid Mushroom ID inputted. The ID must be a positive integer.\nGoing back to the main menu...");
            }
        }

        static void GenerateBattle(List<Character> characters, Character playerCharacter, Difficulty difficulty, MySqlConnection conn)
        {
            Character enemyCharacter = GenerateEnemyCharacter(difficulty);

            Console.WriteLine("-------------------------");
            Console.WriteLine($"You are battling against {enemyCharacter.Name}");
            Console.WriteLine($"HP: {enemyCharacter.HP}");
            Console.WriteLine($"EXP: {enemyCharacter.EXP}");
            Console.WriteLine($"Skill: {enemyCharacter.Skill}");
            Console.WriteLine("-------------------------");

            ExecuteBattle(characters, playerCharacter, enemyCharacter, conn);
        }

        static Character GenerateEnemyCharacter(Difficulty difficulty)
        {
            List<string> allPossibleCharacters = new List<string>
    {
        "Wario", "Daisy", "Waluigi", "Mario", "Luigi", "Yoshi", "Bowser", "Peach",
        "Koopa", "Toad", "Donkey Kong", "Toadette", "Rosalina", "Luma", "Bowser Jr.",
        "Wario Jr.", "Daisy Jr.", "Waluigi Jr.", "Mario Jr.", "Luigi Jr.", "Yoshi Jr.",
        "Peach Jr.", "Koopa Jr.", "Toad Jr.", "Donkey Kong Jr.", "Toadette Jr.", "Rosalina Jr.", "Luma Jr."
    };

            Random random = new Random();
            int randomCharacterID = random.Next(0, allPossibleCharacters.Count);
            string randomCharacterName = allPossibleCharacters[randomCharacterID];
            int randomCharacterHP = 0;
            int randomCharacterEXP = 0;

            switch (difficulty)
            {
                case Difficulty.Easy:
                    randomCharacterHP = random.Next(1, 50);
                    randomCharacterEXP = random.Next(1, 50);
                    break;
                case Difficulty.Normal:
                    randomCharacterHP = random.Next(50, 150);
                    randomCharacterEXP = random.Next(50, 100);
                    break;
                case Difficulty.Hard:
                    randomCharacterHP = random.Next(200, 300);
                    break;
                case Difficulty.Random:
                    randomCharacterHP = random.Next(1, 300);
                    randomCharacterEXP = random.Next(1, 200);
                    break;
            }

            return new AnyMushroomCharacter(randomCharacterName, randomCharacterHP, randomCharacterEXP, "Unknown");
        }
        static void ExecuteBattle(List<Character> characters, Character player, Character enemy, MySqlConnection conn)
        {
            while (player.HP > 0 && enemy.HP > 0)
            {
                DisplayAttackOptions(player);
                string attackOption = Console.ReadLine();

                if (attackOption == "Q")
                {

                    return;
                }

                PerformPlayerAttack(player, enemy, attackOption);

                if (enemy.HP > 0)
                {
                    Console.WriteLine($"{enemy.Name} attacks back!");
                    player.HP -= (int)(enemy.EXP * 0.9);
                    Console.WriteLine($"{player.Name} HP is now {player.HP}");
                }
            }

            if (player.HP > 0)
            {
                Console.WriteLine($"{player.Name} won the battle!");
                Console.Write("Do you want to catch the enemy character? (yes/no): ");
                string catchChoice = Console.ReadLine().ToLower();

                if (catchChoice == "yes")
                {
                    PlayCatchMiniGame(characters, enemy);
                }

                player.HP += 10; // Increase HP as a reward
                player.EXP += 20; // Increase EXP as a reward
                Console.WriteLine($"{player.Name}'s HP is now {player.HP} and EXP is now {player.EXP}");
                string updateQuery = "UPDATE Characters SET Name = @Name, HP = @NewHP, EXP = @NewEXP, Skill = @Skill WHERE id = @Id";
                // Using the id as the primary key, update the character's name, hp, exp, and skill
                MySqlCommand updateCmd = new(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@Name", player.Name);
                updateCmd.Parameters.AddWithValue("@NewHP", player.HP);
                updateCmd.Parameters.AddWithValue("@NewEXP", player.EXP);
                updateCmd.Parameters.AddWithValue("@Skill", player.Skill);
                updateCmd.Parameters.AddWithValue("@Id", player.Id);
                updateCmd.ExecuteNonQuery();

            }
            else
            {
                Console.WriteLine($"{player.Name} was defeated...");
            }
        }

        static void DisplayAttackOptions(Character player)
        {
            Console.WriteLine("Which attack would you like to use?");
            switch (player.Name)
            {
                case "Mario":
                    Console.WriteLine("(1). Fire Punch");
                    Console.WriteLine("(2). Chop");
                    Console.WriteLine("(3). Stomp");
                    break;

                case "Luigi":
                    Console.WriteLine("(1). Green Missile");
                    Console.WriteLine("(2). Fireball");
                    Console.WriteLine("(3). Poltergust");
                    break;

                case "Wario":
                    Console.WriteLine("(1). Garlic Breath");
                    Console.WriteLine("(2). Body Slam");
                    Console.WriteLine("(3). Wario Waft");
                    break;

                case "Waluigi":
                    Console.WriteLine("(1). Tennis Serve");
                    Console.WriteLine("(2). Purple Kick");
                    Console.WriteLine("(3). Waluigi Whirl");
                    break;

                case "Daisy":
                    Console.WriteLine("(1). Flower Power");
                    Console.WriteLine("(2). Royal Slam");
                    Console.WriteLine("(3). Daisy Dance");
                    break;

                case "Peach":
                    Console.WriteLine("(1). Heart Kiss");
                    Console.WriteLine("(2). Peach Bomber");
                    Console.WriteLine("(3). Parasol Drill");
                    break;

                default:
                    Console.WriteLine("(1). EXP Attack");
                    Console.WriteLine("(2). Skill Attack");
                    break;
            }
            Console.WriteLine("(4). Run Away");
            Console.Write("Please enter [1,2,3,4] or Q to quit: ");
        }

        static void PerformPlayerAttack(Character player, Character enemy, string attackOption)
        {
            switch (attackOption)
            {
                case "1":
                    if (player.Name == "Mario")
                    {
                        Console.WriteLine($"{player.Name} uses Fire Punch!");
                        enemy.HP -= (int)(player.EXP * 1.2);
                    }
                    else if (player.Name == "Luigi")
                    {
                        Console.WriteLine($"{player.Name} uses Green Missile!");
                        enemy.HP -= (int)(player.EXP * 1.1);
                    }
                    else if (player.Name == "Wario")
                    {
                        Console.WriteLine($"{player.Name} uses Garlic Breath!");
                        enemy.HP -= (int)(player.EXP * 1.3);
                    }
                    else if (player.Name == "Waluigi")
                    {
                        Console.WriteLine($"{player.Name} uses Tennis Serve!");
                        enemy.HP -= (int)(player.EXP * 1.1);
                    }
                    else if (player.Name == "Daisy")
                    {
                        Console.WriteLine($"{player.Name} uses Flower Power!");
                        enemy.HP -= (int)(player.EXP * 1.2);
                        Console.WriteLine($"{enemy.Name} HP is now {enemy.HP}");
                    }
                    else if (player.Name == "Peach")
                    {
                        Console.WriteLine($"{player.Name} uses Heart Kiss!");
                        enemy.HP -= (int)(player.EXP * 1.2);
                        Console.WriteLine($"{enemy.Name} HP is now {enemy.HP}");
                    }
                    break;
                case "2":
                    if (player.Name == "Mario")
                    {
                        Console.WriteLine($"{player.Name} uses Chop!");
                        enemy.HP -= (int)(player.EXP * 1.1);
                    }
                    else if (player.Name == "Luigi")
                    {
                        Console.WriteLine($"{player.Name} uses Fireball!");
                        enemy.HP -= (int)(player.EXP * 1.2);
                    }
                    else if (player.Name == "Wario")
                    {
                        Console.WriteLine($"{player.Name} uses Body Slam!");
                        enemy.HP -= (int)(player.EXP * 1.3);
                    }
                    else if (player.Name == "Waluigi")
                    {
                        Console.WriteLine($"{player.Name} uses Purple Kick!");
                        enemy.HP -= (int)(player.EXP * 1.1);
                    }
                    else if (player.Name == "Daisy")
                    {
                        Console.WriteLine($"{player.Name} uses Royal Slam!");
                        enemy.HP -= (int)(player.EXP * 1.2);
                        Console.WriteLine($"{enemy.Name} HP is now {enemy.HP}");
                    }
                    else if (player.Name == "Peach")
                    {
                        Console.WriteLine($"{player.Name} uses Peach Bomber!");
                        enemy.HP -= (int)(player.EXP * 1.2);
                        Console.WriteLine($"{enemy.Name} HP is now {enemy.HP}");
                    }
                    break;
                case "3":
                    if (player.Name == "Mario")
                    {
                        Console.WriteLine($"{player.Name} uses Stomp!");
                        enemy.HP -= (int)(player.EXP * 1.1);
                    }
                    else if (player.Name == "Luigi")
                    {
                        Console.WriteLine($"{player.Name} uses Poltergust!");
                        enemy.HP -= (int)(player.EXP * 1.1);
                    }
                    else if (player.Name == "Wario")
                    {
                        Console.WriteLine($"{player.Name} uses Wario Waft!");
                        enemy.HP -= (int)(player.EXP * 1.3);
                    }
                    else if (player.Name == "Waluigi")
                    {
                        Console.WriteLine($"{player.Name} uses Waluigi Whirl!");
                        enemy.HP -= (int)(player.EXP * 1.2);
                    }
                    else if (player.Name == "Daisy")
                    {
                        Console.WriteLine($"{player.Name} uses Daisy Dance!");
                        enemy.HP -= (int)(player.EXP * 1.1);
                        Console.WriteLine($"{enemy.Name} HP is now {enemy.HP}");
                    }
                    else if (player.Name == "Peach")
                    {
                        Console.WriteLine($"{player.Name} uses Parasol Drill!");
                        enemy.HP -= (int)(player.EXP * 1.1);
                        Console.WriteLine($"{enemy.Name} HP is now {enemy.HP}");
                    }
                    break;
                case "4":
                    Console.WriteLine($"{player.Name} runs away!");
                    // Implement running away logic if needed
                    Console.WriteLine("You successfully ran away from the battle.");
                    return;
                default:
                    Console.WriteLine("Invalid option!");
                    break;

            }
        }

        static void PlayCatchMiniGame(List<Character> characters, Character enemy)
        {
            Console.WriteLine("Press the 'C' key when the indicator is in the catch zone!");
            Random random = new Random();
            int catchZoneStart = random.Next(20, 50);
            int catchZoneEnd = catchZoneStart + 10;
            bool caught = false;

            for (int i = 0; i < 100; i++)
            {
                Console.Clear();
                Console.Write("Catch Zone: [");
                for (int j = 0; j < 100; j++)
                {
                    if (j >= catchZoneStart && j <= catchZoneEnd)
                    {
                        Console.Write("=");
                    }
                    else
                    {
                        Console.Write("-");
                    }
                }
                Console.WriteLine("]");
                Console.WriteLine("Press 'C' to catch!");

                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.C)
                    {
                        if (i >= catchZoneStart && i <= catchZoneEnd)
                        {
                            caught = true;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                Thread.Sleep(50);
            }

            if (caught)
            {
                Console.WriteLine($"You caught {enemy.Name}!");
                characters.Add(enemy);
            }
            else
            {
                Console.WriteLine($"Failed to catch {enemy.Name}. Better luck next time!");
            }
        }


    }
}