using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace MushroomPocket
{
    class Program
    {
        static void Main(string[] args)
        {
            //MushroomMaster criteria list for checking character transformation availability.   
            /*************************************************************************
                PLEASE DO NOT CHANGE THE CODES FROM LINE 15-19
            *************************************************************************/
            List<MushroomMaster> mushroomMasters = new List<MushroomMaster>(){
            new MushroomMaster("Daisy", 2, "Peach"),
            new MushroomMaster("Wario", 3, "Mario"),
            new MushroomMaster("Waluigi", 1, "Luigi")
            };

            List<Character> characters = new List<Character>(); // Create a new list to store characters

            //Use "Environment.Exit(0);" if you want to implement an exit of the console program
            //Start your assignment 1 requirements below.

            // Setup connection to database
            MySqlConnection conn;
            string myConnectionString = "server=127.0.0.1;uid=it2154;" + "pwd=it2154password;database=it2154";

            try
            {
                conn = new(myConnectionString);
                conn.Open();
                characters = GetCharactersFromDatabase(conn);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error connecting to database: {ex.Message}");
                return; // Exit the program if connetion fails
            }

            // Mushroom Pocket App menu
            while (true)
            {
                Console.WriteLine("********************************");
                Console.WriteLine("Welcome to Mushroom Pocket App");
                Console.WriteLine("********************************");
                Console.WriteLine("(1). Add Mushroom's character to my pocket");
                Console.WriteLine("(2). List character(s) in my Pocket");
                Console.WriteLine("(3). Check if I can transform my characters");
                Console.WriteLine("(4). Transform character(s)");
                Console.WriteLine("(5). Search for a character in my pocket");
                Console.WriteLine("(6). Modify a character");
                Console.WriteLine("(7). Transform a character");
                Console.WriteLine("(8). Delete a character from  my pocket");
                Console.WriteLine("(9). Battle computer controlled characters");
                Console.WriteLine("Take note of the Mushroom ID of the character you wish to Modify/Transform/Delete/Use in Battle."); ;
                Console.Write("Please only enter [1,2,3,4,5,6,7,8,9] or Q to quit: ");

                string choice = Console.ReadLine();
                StopMushroomPocketAppOnQ(choice);

                switch (choice.ToLower())
                {
                    case "1":
                        AddCharacter(characters, conn);
                        break;
                    case "2":
                        ListCharacters(characters);
                        break;
                    case "3":
                        CheckTransformableCharacters(characters, mushroomMasters);
                        break;
                    case "4":
                        TransformCharacters(characters, mushroomMasters, conn);
                        break;
                    case "5":
                        SearchCharacter(characters, mushroomMasters, conn);
                        break;
                    case "6":
                        ModifyCharacterHP_EXP(characters, conn);
                        break;
                    case "7":
                        TransformSpecificCharacter(characters, mushroomMasters, conn);
                        break;
                    case "8":
                        DeleteCharacter(characters, conn);
                        break;
                    case "9":
                        Console.WriteLine("Choose difficulty level:");
                        Console.WriteLine("1. Easy");
                        Console.WriteLine("2. Normal");
                        Console.WriteLine("3. Hard");
                        Console.WriteLine("4. Random");
                        Console.Write("Enter the number corresponding to your choice: ");
                        string difficultyChoice = Console.ReadLine();

                        switch (difficultyChoice)
                        {
                            case "1":
                                // Call BattleSystemFunction with selected difficulty level
                                BattleSystem.BattleSystemFunction(characters, BattleSystem.Difficulty.Easy, conn);
                                break;
                            case "2":
                                // Call BattleSystemFunction with selected difficulty level
                                BattleSystem.BattleSystemFunction(characters, BattleSystem.Difficulty.Normal, conn);
                                break;
                            case "3":
                                // Call BattleSystemFunction with selected difficulty level
                                BattleSystem.BattleSystemFunction(characters, BattleSystem.Difficulty.Hard, conn);
                                break;
                            case "4":
                                // Call BattleSystemFunction with selected difficulty level
                                BattleSystem.BattleSystemFunction(characters, BattleSystem.Difficulty.Random, conn);
                                break;
                            default:
                                Console.WriteLine("Invalid input. Please enter a number between 1 and 4.");
                                break;
                        }
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please enter only 1,2,3,4,5,6,7,8 or Q.");
                        break;
                }

                Console.WriteLine();
            }

        }

        public static void StopMushroomPocketAppOnQ(string input) // Stop the console program if user enters Q 
        {
            if (input.ToLower() == "q")
            {
                Console.WriteLine("You entered Q, it is used to quit the program at any time.");
                Console.WriteLine("Thank you for using the Mushroom Pocket App!");
                Console.WriteLine("Stopping the Mushroom Pocket App Now...");
                Environment.Exit(0);
            }
        }

        // Add a character to the pocket        
        static void AddCharacter(List<Character> characters, MySqlConnection conn)
        {
            Console.Write("Enter Character's Name (Waluigi, Daisy, or Wario): ");
            string characterName = CapitalizeName(Console.ReadLine());

            while (characterName != "Waluigi" && characterName != "Daisy" && characterName != "Wario")
            {
                StopMushroomPocketAppOnQ(characterName);
                Console.WriteLine($"Invalid Character Name. No such character named '{characterName}' is available. Only Waluigi, Daisy, and Wario can be added. Please try again.");
                Console.Write("Re-enter Character's Name (Waluigi, Daisy, or Wario): ");
                characterName = CapitalizeName(Console.ReadLine());
            }

            int characterHP = GetValidatedInput(characterName, "HP");
            int characterEXP = GetValidatedInput(characterName, "EXP");

            var newCharacter = CreateCharacterInstance(characterName, characterHP, characterEXP);
            characters.Add(newCharacter);
            AddCharacterToDatabase(conn, newCharacter);
            Console.WriteLine($"{characterName} has been added.");
        }

        static string CapitalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "";
            return char.ToUpper(name[0]) + name[1..].ToLower();
        }

        static int GetValidatedInput(string characterName, string attributeName)
        {
            Console.Write($"Enter {characterName}'s {attributeName}: ");
            string input = Console.ReadLine();
            bool isValid = int.TryParse(input, out int result);

            while (!isValid || result < 0)
            {
                StopMushroomPocketAppOnQ(input);
                Console.WriteLine($"Invalid {attributeName} value. {attributeName} has to be an integer. Please try again.");
                Console.Write($"Re-enter {characterName}'s {attributeName}: ");
                input = Console.ReadLine();
                isValid = int.TryParse(input, out result);
            }
            return result;
        }

        // Create an instance of a character using the character name, character HP, and character EXP
        static Character CreateCharacterInstance(string characterName, int characterHP, int characterEXP)
        {
            return characterName switch
            {
                "Waluigi" => new Waluigi(characterHP, characterEXP),
                "Daisy" => new Daisy(characterHP, characterEXP),
                "Wario" => new Wario(characterHP, characterEXP),
                _ => null,
            };
        }

        // Add character to database
        static void AddCharacterToDatabase(MySqlConnection conn, Character character)
        {
            string query = "INSERT INTO Characters (Name, HP, EXP, Skill) VALUES (@Name, @HP, @EXP, @Skill)";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Name", character.Name);
            cmd.Parameters.AddWithValue("@HP", character.HP);
            cmd.Parameters.AddWithValue("@EXP", character.EXP);
            cmd.Parameters.AddWithValue("@Skill", character.Skill);
            cmd.ExecuteNonQuery();
            character.Id = (int)cmd.LastInsertedId;
        }

        // Retrieve the characters in the pocket from the database
        static List<Character> GetCharactersFromDatabase(MySqlConnection conn)
        {
            List<Character> characters = [];
            string query = "SELECT * FROM Characters";
            MySqlCommand cmd = new(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int id = reader.GetInt32("id");
                string name = reader.GetString("Name");
                int hp = reader.GetInt32("HP");
                int exp = reader.GetInt32("EXP");
                string skill = reader.GetString("Skill");
                Character character = new AnyMushroomCharacter(name, hp, exp, skill)
                {
                    Id = id
                };
                characters.Add(character);
            }

            reader.Close();
            return characters;
        }

        static void DeleteCharacterFromDatabase(MySqlConnection conn, int id)
        {
            string deleteQuery = "DELETE FROM Characters WHERE id = @Id";
            MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn);
            deleteCmd.Parameters.AddWithValue("@Id", id);
            deleteCmd.ExecuteNonQuery();
        }

        // List all characters added to the pocket with their HP, EXP, and Skill
        static void ListCharacters(List<Character> characters)
        {
            if (characters.Count == 0)
            {
                Console.WriteLine("You have no characters in your pocket!");
                return;
            }

            // Sort characters by HP in decending order
            var sortedCharacters = characters.OrderByDescending(character => character.HP);

            foreach (Character character in sortedCharacters)
            {
                Console.WriteLine("-------------------------");
                Console.WriteLine($"Mushroom ID: {character.Id}");
                Console.WriteLine($"Name: {character.Name}");
                Console.WriteLine($"HP: {character.HP}");
                Console.WriteLine($"EXP: {character.EXP}");
                Console.WriteLine($"Skill: {character.Skill}");
                Console.WriteLine("-------------------------");
            }
        }

        // Check for characters that can be transformed. Characters that meet the transformation requirement are outputted to the console.
        static void CheckTransformableCharacters(List<Character> characters, List<MushroomMaster> mushroomMasters)
        {
            if (characters.Count == 0)
            {
                Console.WriteLine("You have no characters in your pocket! Please add at least one character to your pocket before checking for transformable characters.");
                return;
            }

            int numWaluigi = 0;
            int numDaisy = 0;
            int numWario = 0;
            foreach (Character character in characters)
            {

                if (character.Name == "Waluigi")
                {
                    numWaluigi += 1;
                }
                else if (character.Name == "Daisy")
                {
                    numDaisy += 1;
                }
                else if (character.Name == "Wario")
                {
                    numWario += 1;
                }
            }

            bool canAnyTransform = false;
            foreach (Character character in characters)
            {
                foreach (MushroomMaster mushroomMaster in mushroomMasters)
                {
                    if (character.Name == "Waluigi" && numWaluigi >= mushroomMaster.NoToTransform && mushroomMaster.Name == "Waluigi")
                    {
                        Console.WriteLine("Waluigi --> Luigi");
                        numWaluigi -= mushroomMaster.NoToTransform;
                        canAnyTransform = true;
                    }
                    else if (character.Name == "Daisy" && numDaisy >= mushroomMaster.NoToTransform && mushroomMaster.Name == "Daisy")
                    {
                        Console.WriteLine("Daisy --> Peach");
                        numDaisy -= mushroomMaster.NoToTransform;
                        canAnyTransform = true;
                    }
                    else if (character.Name == "Wario" && numWario >= mushroomMaster.NoToTransform && mushroomMaster.Name == "Wario")
                    {
                        Console.WriteLine("Wario --> Mario");
                        numWario -= mushroomMaster.NoToTransform;
                        canAnyTransform = true;
                    }
                }
            }

            if (!canAnyTransform)
            {
                Console.WriteLine("You have no transformable characters in your pocket!");
            }
        }

        // Transform multiple tranformable Characters
        static void TransformCharacters(List<Character> characters, List<MushroomMaster> mushroomMasters, MySqlConnection conn)
        {
            if (characters.Count == 0)
            {
                Console.WriteLine("You have no characters in your pocket!");
                return;
            }

            int numWaluigi = 0;
            int numDaisy = 0;
            int numWario = 0;

            foreach (Character character in characters)
            {
                if (character.Name == "Waluigi")
                {
                    numWaluigi += 1;
                }
                else if (character.Name == "Daisy")
                {
                    numDaisy += 1;
                }
                else if (character.Name == "Wario")
                {
                    numWario += 1;
                }
            }

            bool anyTransformed = false;
            var charactersToRemove = new List<Character>();

            foreach (Character character in characters.ToList())
            {
                foreach (MushroomMaster mushroomMaster in mushroomMasters)
                {
                    if (character.Name == "Waluigi" && numWaluigi >= mushroomMaster.NoToTransform && mushroomMaster.Name == "Waluigi")
                    {
                        TransformCharacter(conn, character, mushroomMaster);
                        numWaluigi -= mushroomMaster.NoToTransform;
                        anyTransformed = true;
                        // Add Waluigis used for transformation to remove list
                        charactersToRemove.AddRange(characters.Where(c => c.Name == "Waluigi").Take(mushroomMaster.NoToTransform - 1));
                    }
                    else if (character.Name == "Daisy" && numDaisy >= mushroomMaster.NoToTransform && mushroomMaster.Name == "Daisy")
                    {
                        TransformCharacter(conn, character, mushroomMaster);
                        numDaisy -= mushroomMaster.NoToTransform;
                        anyTransformed = true;
                        // Add Daisies used for transformation to remove list
                        charactersToRemove.AddRange(characters.Where(c => c.Name == "Daisy").Take(mushroomMaster.NoToTransform - 1));
                    }
                    else if (character.Name == "Wario" && numWario >= mushroomMaster.NoToTransform && mushroomMaster.Name == "Wario")
                    {
                        TransformCharacter(conn, character, mushroomMaster);
                        numWario -= mushroomMaster.NoToTransform;
                        anyTransformed = true;
                        // Add Warios used for transformation to remove list
                        charactersToRemove.AddRange(characters.Where(c => c.Name == "Wario").Take(mushroomMaster.NoToTransform - 1));
                    }
                }
            }
            // Remove the characters that were used for transformation(s)
            foreach (var character in charactersToRemove)
            {
                DeleteCharacterFromDatabase(conn, character.Id);
                characters.Remove(character);
            }
            if (!anyTransformed)
            {
                Console.WriteLine("You have no transformable characters in your pocket!");
            }
        }

        // Transform a single character and update the character's name, hp, exp, and skill in the database
        static void TransformCharacter(MySqlConnection conn, Character character, MushroomMaster mushroomMaster)
        {
            string oldName = character.Name;
            character.Name = mushroomMaster.TransformTo;
            character.HP = 100;
            character.EXP = 0;

            switch (character.Name)
            {
                case "Luigi":
                    character.Skill = "Precision & Accuracy";
                    break;
                case "Peach":
                    character.Skill = "Magic Abilities";
                    break;
                case "Mario":
                    character.Skill = "Combat Skills";
                    break;
            }

            string updateQuery = "UPDATE Characters SET Name = @NewName, HP = @NewHP, EXP = @NewEXP, Skill = @NewSkill WHERE id = @Id";
            // Using the id as the primary key, update the character's name, hp, exp, and skill
            MySqlCommand updateCmd = new(updateQuery, conn);
            updateCmd.Parameters.AddWithValue("@NewName", character.Name);
            updateCmd.Parameters.AddWithValue("@NewHP", character.HP);
            updateCmd.Parameters.AddWithValue("@NewEXP", character.EXP);
            updateCmd.Parameters.AddWithValue("@NewSkill", character.Skill);
            updateCmd.Parameters.AddWithValue("@Id", character.Id);
            updateCmd.ExecuteNonQuery();

            Console.WriteLine($"{oldName} has been transformed into {character.Name}");
        }

        static void SearchCharacter(List<Character> characters, List<MushroomMaster> mushroomMasters, MySqlConnection conn)
        {
            Console.Write("Please enter the name of the character you are looking for or Q to quit: ");
            string characterNameToSearch = CapitalizeName(Console.ReadLine());
            StopMushroomPocketAppOnQ(characterNameToSearch);

            if (characters.Count == 0)
            {
                Console.WriteLine("You have no characters in your pocket! Please add at least 1 character first.");
                return; // Exit the method if there are no characters in the pocket
            }

            // Check if the character exists in the pocket, case insensitive, and add it to the list of found characters to display
            var foundCharacters = characters.Where(c => c.Name.Contains(characterNameToSearch, StringComparison.OrdinalIgnoreCase)).ToList();

            if (foundCharacters.Count == 0)
            {
                Console.WriteLine($"No characters found with the name containing '{characterNameToSearch}'.");
                Console.WriteLine("Going back to the main menu...");
                return; // Exit if no characters were found
            }

            // Sort the list of found characters based on the closeness of the match
            foundCharacters = foundCharacters.OrderByDescending(c => GetMatchScore(c.Name, characterNameToSearch)).ToList();

            Console.WriteLine($"Found {foundCharacters.Count} character(s): ");
            foreach (var character in foundCharacters)
            {
                Console.WriteLine("-------------------------");
                Console.WriteLine($"Mushroom ID: {character.Id}");
                Console.WriteLine($"Name: {character.Name}");
                Console.WriteLine($"HP: {character.HP}");
                Console.WriteLine($"EXP: {character.EXP}");
                Console.WriteLine($"Skill: {character.Skill}");
                Console.WriteLine("-------------------------");
            }

            Console.Write("Do you wish to do Modify, Transform, or Delete a character in your pocket? (Y/N): ");
            string wantToDoSomething = Console.ReadLine().ToLower();
            StopMushroomPocketAppOnQ(wantToDoSomething);

            if (wantToDoSomething == "y")
            {
                Console.WriteLine("--- What would you like to do with a character in your pocket? ---");
                Console.WriteLine("(1). Modify character(s) HP/EXP");
                Console.WriteLine("(2). Transform specific character(s)");
                Console.WriteLine("(3). Delete character(s)");
                Console.Write("Please only enter [1,2,3] or Q to quit: ");
                string characterActionOption = Console.ReadLine();
                StopMushroomPocketAppOnQ(characterActionOption);

                if (characterActionOption == "1")
                {
                    ModifyCharacterHP_EXP(characters, conn);
                }
                else if (characterActionOption == "2")
                {
                    TransformSpecificCharacter(characters, mushroomMasters, conn);
                }
                else if (characterActionOption == "3")
                {
                    DeleteCharacter(characters, conn);
                }
            }
            else if (wantToDoSomething == "n")
            {
                Console.WriteLine("Going back to the main menu...");
            }
            else
            {
                Console.WriteLine("Invalid option inputted, going back to the main menu...");
            }
        }

        // Helper method to get the match score between two strings
        static int GetMatchScore(string name, string searchQuery)
        {
            // If the name starts with the search query, return a high score
            if (name.StartsWith(searchQuery, StringComparison.OrdinalIgnoreCase))
            {
                return searchQuery.Length * 2; // Double the score for exact match
            }
            // If the name contains the search query, return a medium score
            else if (name.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) != -1)
            {
                return searchQuery.Length;
            }
            // Otherwise, return a low score
            else
            {
                return 0;
            }
        }

        static void ModifyCharacterHP_EXP(List<Character> characters, MySqlConnection conn)
        {
            Console.Write("Please enter the Mushroom ID of the character you wish to modify: ");
            string characterID = Console.ReadLine();
            StopMushroomPocketAppOnQ(characterID);

            if (int.TryParse(characterID, out int id))
            {
                Character character = characters.FirstOrDefault(c => c.Id == id);
                if (character != null)
                {
                    int newHP = GetValidatedInput(character.Name, "HP");
                    int newEXP = GetValidatedInput(character.Name, "EXP");

                    character.HP = newHP;
                    character.EXP = newEXP;

                    string updateQuery = "UPDATE Characters SET HP = @HP, EXP = @EXP WHERE Id = @Id";
                    MySqlCommand updateCmd = new(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@HP", newHP);
                    updateCmd.Parameters.AddWithValue("@EXP", newEXP);
                    updateCmd.Parameters.AddWithValue("@Id", id);
                    updateCmd.ExecuteNonQuery();

                    Console.WriteLine($"{character.Name} with Mushroom ID {id} has been updated!");
                }
                else
                {
                    Console.WriteLine("Invalid Mushroom ID inputted, the ID has to be of one of the characters in your pocket. \nGoing back to the main menu...");
                }
            }
            else
            {
                Console.WriteLine("Invalid Mushroom ID inputted, the ID has to be a positive integer. Going back to the main menu...");
            }
        }

        static void TransformSpecificCharacter(List<Character> characters, List<MushroomMaster> mushroomMasters, MySqlConnection conn)
        {
            Console.Write("Please enter the Mushroom ID of the character you wish to modify: ");
            string characterID = Console.ReadLine();
            StopMushroomPocketAppOnQ(characterID);

            if (int.TryParse(characterID, out int id))
            {
                Character character = characters.FirstOrDefault(c => c.Id == id);
                if (character != null)
                {
                    bool anyTransformed = false;
                    var charactersToRemove = new List<Character>();
                    foreach (var mushroomMaster in mushroomMasters)
                    {
                        var noOfSameCharacters = characters.Where(c => c.Name.Equals(character.Name)).ToList().Count;
                        if (character.Name == mushroomMaster.Name && noOfSameCharacters >= mushroomMaster.NoToTransform)
                        {
                            TransformCharacter(conn, character, mushroomMaster);
                            charactersToRemove.AddRange(characters.Where(c => c.Name == mushroomMaster.Name).Take(mushroomMaster.NoToTransform - 1));
                            anyTransformed = true;
                        }
                    }

                    foreach (var characterToRemove in charactersToRemove)
                    {
                        DeleteCharacterFromDatabase(conn, characterToRemove.Id);
                        characters.Remove(characterToRemove);
                    }

                    if (!anyTransformed)
                    {
                        Console.WriteLine("You have no transformable characters in your pocket! \nGoing back to the main menu...");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Mushroom ID inputted, the ID has to be of one of the characters in your pocket. \nGoing back to the main menu...");
                }
            }
            else
            {
                Console.WriteLine("Invalid Mushroom ID inputted, the ID has to be a positive integer. \nGoing back to the main menu...");
            }
        }

        static void DeleteCharacter(List<Character> characters, MySqlConnection conn)
        {
            Console.Write("Please enter the mushroom id of the character you wish to delete: ");
            string characterID = Console.ReadLine();
            StopMushroomPocketAppOnQ(characterID);

            if (int.TryParse(characterID, out int id))
            {
                Character character = characters.FirstOrDefault(c => c.Id == id);
                if (character != null)
                {
                    Console.Write($"Are you sure you want to delete {character.Name} with Musroom ID {id} from your pocket? (Y/N): ");
                    string deletionConfirmation = Console.ReadLine().ToLower();

                    if (deletionConfirmation == "y")
                    {
                        DeleteCharacterFromDatabase(conn, id);
                        characters.Remove(character);
                        Console.WriteLine($"{character.Name} with Mushroom ID {id} has been deleted from your pocket! \nGoing back to the main menu...");
                    }
                    else if (deletionConfirmation == "n")
                    {
                        Console.WriteLine("Since you entered N, the character has not been deleted from your pocket! \nGoing back to the main menu...");
                    }
                    else
                    {
                        Console.WriteLine("You have entered an invalid option, Y, N or Q are the only options, the character has not been deleted from your pocket! \nGoing back to the main menu...");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Mushroom ID inputted, the ID has to be of one of the characters in your pocket. \nGoing back to the main menu...");
                }
            }
            else
            {
                Console.WriteLine("Invalid Mushroom ID inputted, the ID has to be a positive integer. \nGoing back to the main menu...");
            }
        }

    }
}