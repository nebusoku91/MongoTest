namespace MongoTest
{
    public interface IMongoProgram
    {
        public void Start();
        public void ReadAll();
        public void SearchGame();
        public void CreateGame();
        public void UpdateGame();
        public void DeleteGame();
        public void Exit();
    }
}
