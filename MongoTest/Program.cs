using MongoDB.Bson;
using MongoDB.Driver;
using MongoTest;

MongoProgram test = new MongoProgram();
test.Start();


namespace MongoTest
{
    public class MongoProgram : IMongoProgram
    {

        MongoClient dbClient = new MongoClient
            ("");
        int input;

        public void Start()
        {
            var database = dbClient.GetDatabase("Product");
            var gamesCollection = database.GetCollection<BsonDocument>("Games");

            while (true)
            {
                Console.WriteLine("= Main Menu =\n" +
                "1. List Games\n" +
                "2. Search Games\n" +
                "3. Add New Game\n" +
                "4. Edit Games\n" +
                "5. Delete\n" +
                "6. Exit");
                Console.Write("> ");

                if (Int32.TryParse(Console.ReadLine(), out input))
                {
                    switch (input)
                    {
                        case 1:
                            ReadAll();
                            break;
                        case 2:
                            SearchGame();
                            break;
                        case 3:
                            CreateGame();
                            break;
                        case 4:
                            UpdateGame();
                            break;
                        case 5:
                            DeleteGame();
                            break;
                        case 6:
                            Exit();
                            break;
                        default:
                            Console.WriteLine("Fel");
                            break;
                    }
                }
            }
        }

        public void ReadAll()
        {
            var database = dbClient.GetDatabase("Product");
            var gamesCollection = database.GetCollection<BsonDocument>("Games");

            Console.WriteLine("\n= List Games =");
            Console.WriteLine("1. All games\n2. Action games\n3. Roleplaying Games\n4. Strategy games\n5. Return");
            Console.Write("> ");

            try
            {
                if (Int32.TryParse(Console.ReadLine(), out input))
                {
                    switch (input)
                    {
                        case 1:
                            Console.WriteLine("\nListing all games: ");
                            var documents = gamesCollection.Find(new BsonDocument()).ToList();
                            foreach (var document in documents)
                                Console.WriteLine(document);
                            break;
                        case 2:
                            Console.WriteLine("\nListing all Action games: ");
                            var actiongamesFilter = Builders<BsonDocument>.Filter.Eq("genre", "Action");
                            var actiongames = gamesCollection.Find(actiongamesFilter).ToList();
                            foreach (var actiongame in actiongames)
                                Console.WriteLine(actiongame);
                            break;
                        case 3:
                            Console.WriteLine("\nListing all Roleplaying games: ");
                            var rpggamesFilter = Builders<BsonDocument>.Filter.Eq("genre", "RPG");
                            var rpggames = gamesCollection.Find(rpggamesFilter).ToList();
                            foreach (var rpggame in rpggames)
                                Console.WriteLine(rpggame);
                            break;
                        case 4:
                            Console.WriteLine("\nListing all Strategy games: ");
                            var stratgamesFilter = Builders<BsonDocument>.Filter.Eq("genre", "Strategy");
                            var stratgames = gamesCollection.Find(stratgamesFilter).ToList();
                            foreach (var stratgame in stratgames)
                                Console.WriteLine(stratgame);
                            break;
                        case 5:
                            Start();
                            break;
                        default:
                            Console.WriteLine("Error. Try again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Error. Try again.");
                    ReadAll();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Start();
            }

        }

        public void SearchGame()
        {
            var database = dbClient.GetDatabase("Product");
            var gamesCollection = database.GetCollection<BsonDocument>("Games");

            Console.WriteLine("= Search Game =");
            Console.WriteLine("Enter Title:");
            Console.Write("> ");

            string inp = Console.ReadLine();

            var filter = Builders<BsonDocument>.Filter.Regex("name", new BsonRegularExpression($"(?i).*{inp}.*"));
            Console.WriteLine("Results: ");
            try
            {
                if (filter != null)
                {
                    var doc = gamesCollection.Find(filter).FirstOrDefault();
                    if (doc != null)
                        Console.WriteLine(doc.ToString());
                    else
                        Console.WriteLine("Could not find game. Please try again.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void CreateGame()
        {
            var database = dbClient.GetDatabase("Product");
            var gamesCollection = database.GetCollection<BsonDocument>("Games");

            try
            {
                string gameName = "";
                string gameGenre = "";
                int genreChoice = 0;
                int gamePrice = 0;
                int gameQuantity = 0;

                Console.WriteLine("\n= Add Game =");
                Console.Write("Game Title:\n>");

                gameName = Console.ReadLine();
                if (string.IsNullOrEmpty(gameName))
                {
                    Console.WriteLine("Invalid input. Please try again.");
                    Start();
                }
                else
                {
                    Console.Write("Genre:\n>");


                    if (Int32.TryParse(Console.ReadLine(), out genreChoice))


                        //gameGenre = Console.ReadLine();

                        if (string.IsNullOrEmpty(gameGenre))
                        {
                            Console.WriteLine("Invalid input. Please try again.");
                            Start();
                        }

                    Console.Write("Price:\n>");

                    if (Int32.TryParse(Console.ReadLine(), out gamePrice))
                    {
                        Console.Write("Quantity:\n>");
                        if (Int32.TryParse(Console.ReadLine(), out gameQuantity))
                        {
                            var document = new BsonDocument
            {
                {"name", gameName},
                {"genre", gameGenre},
                {"price", gamePrice},
                {"quantity", gameQuantity }
            };
                            gamesCollection.InsertOne(document);
                            Console.WriteLine($"The game \"{gameName}\" has been added.\n");
                            Start();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error, could not add game.");
                        CreateGame();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Start();
            }
        }

        public void UpdateGame()
        {
            var database = dbClient.GetDatabase("Product");
            var gamesCollection = database.GetCollection<BsonDocument>("Games");

            Console.WriteLine("= Edit game =");
            Console.Write("Enter Game ID:\n>");

            try
            {
                string gameID = Console.ReadLine();
                var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(gameID));
                var doc = gamesCollection.Find(filter).FirstOrDefault();

                if (doc == null)
                {
                    Console.WriteLine("Could not find game with that ID.");
                    return;
                }
                else
                {
                    Console.WriteLine($"Updating game \n{doc["name"].AsString}\n.");
                    Console.WriteLine("What do you want to change?");
                    Console.Write("1. Title\n2. Genre\n3. Price\n4. Quantity\n>");

                    if (Int32.TryParse(Console.ReadLine(), out input))
                    {
                        switch (input)
                        {
                            case 1:
                                string oldName = doc["name"].ToString();
                                string newName = Console.ReadLine();
                                Console.WriteLine($"Current title: {oldName}");
                                Console.Write("Choose new title: \n>");

                                if (!string.IsNullOrWhiteSpace(newName))
                                {
                                    var update = Builders<BsonDocument>.Update.Set("name", newName);
                                    gamesCollection.FindOneAndUpdate(filter, update);
                                    Console.WriteLine($"Game title successfully changed from \"{oldName}\" to \"{newName}\"\n");
                                }
                                else if (newName == "")
                                {
                                    Console.WriteLine("Invalid input. Please try again.");
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input. Please try again.");
                                }
                                break;
                            case 2:
                                string oldGenre = doc["genre"].ToString();
                                string newGenre = Console.ReadLine();
                                Console.WriteLine("Current genre: ");
                                Console.Write("Choose new genre: \n>");

                                if (!string.IsNullOrWhiteSpace(newGenre))
                                {
                                    var update = Builders<BsonDocument>.Update.Set("genre", newGenre);
                                    gamesCollection.FindOneAndUpdate(filter, update);
                                    Console.WriteLine($"Game genre successfully changed from \"{oldGenre}\" to \"{newGenre}\".\n");
                                }
                                else if (newGenre == "")
                                {
                                    Console.WriteLine("Invalid input. Please try again.");
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input. Please try again.");
                                }
                                break;
                            case 3:
                                string oldPrice = doc["price"].ToString();
                                int newPrice = Int32.Parse(Console.ReadLine());
                                Console.WriteLine($"Current price: {oldPrice}");
                                Console.Write("Choose new price: \n>");

                                if (newPrice > 0)
                                {
                                    var update = Builders<BsonDocument>.Update.Set("price", newPrice);
                                    gamesCollection.FindOneAndUpdate(filter, update);
                                    Console.WriteLine($"Game price successfully changed from \"{oldPrice}\" to \"{newPrice}\"\n");
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input. Please try again.");
                                }
                                break;
                            case 4:
                                string oldQuantity = doc["quantity"].ToString();
                                int newQuantity = Int32.Parse(Console.ReadLine());
                                Console.WriteLine($"Current quantity: {oldQuantity}");
                                Console.Write("Choose new quantity: \n>");

                                if (newQuantity >= 0)
                                {
                                    var update = Builders<BsonDocument>.Update.Set("quantity", newQuantity);
                                    gamesCollection.FindOneAndUpdate(filter, update);
                                    Console.WriteLine($"Game quantity successfully changed from \"{oldQuantity}\" to \"{newQuantity}\"\n");
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input. Please try again.");
                                }
                                break;
                            default:
                                Console.WriteLine("Invalid option. Please try again.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Start();
            }

        }

        public void DeleteGame()
        {
            var database = dbClient.GetDatabase("Product");
            var gamesCollection = database.GetCollection<BsonDocument>("Games");
            string ID = "";
            Console.WriteLine("= Delete Game =");
            Console.Write("Enter game ID to delete:\n>");
            try
            {
                ID = Console.ReadLine();
                var DeleteID = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(ID));
                gamesCollection.DeleteOne(DeleteID);
                var doc = gamesCollection.Find(DeleteID).FirstOrDefault();
                Console.WriteLine($"Deleted {doc["name"].AsString} from database.");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not delete game with that ID. " + ex);
                Start();
            }

        }

        public void Exit()
        {
            Environment.Exit(0);
        }
    }
}

